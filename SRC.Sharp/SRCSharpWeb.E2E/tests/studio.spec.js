// @ts-check
// SRC#Studio のマップ作成ツール（スタンドアロン。7474/SRC#1641）。CI は tools/studio_bundle.sh の出力を配信のルートの /studio/ に置く（.github/workflows/CI.yml の test-lint）
const { test, expect } = require('@playwright/test');
const fs = require('node:fs');
const path = require('node:path');

const base = process.env.STUDIO_BASE ?? '/studio/';
const os = require('node:os');
const lantern = path.resolve(__dirname, '..', '..', 'Content', 'lantern');

/**
 * ランタン号のマップを写したシナリオのフォルダ。Playwright でフォルダを選ぶ（setInputFiles）と、日本語の名前のファイルが読めないため、
 * ASCII の名前にする（ブラウザで使うときは日本語の名前も読める）
 */
function scenarioFolder(extraData = []) {
  const dir = fs.mkdtempSync(path.join(os.tmpdir(), 'studio-'));
  const scenario = path.join(dir, 'lantern');
  fs.mkdirSync(path.join(scenario, 'stage'), { recursive: true });
  fs.copyFileSync(path.join(lantern, '01_灯の港.map'), path.join(scenario, 'harbor.map'));
  fs.copyFileSync(path.join(lantern, '01_灯の港.eve'), path.join(scenario, 'harbor.eve'));
  fs.copyFileSync(path.join(lantern, 'ステージ', '02_霧の渓谷.map'), path.join(scenario, 'stage', 'valley.map'));
  fs.copyFileSync(path.join(lantern, 'ステージ', '02_霧の渓谷.eve'), path.join(scenario, 'stage', 'valley.eve'));
  fs.mkdirSync(path.join(scenario, 'Data', 'team'), { recursive: true });
  for (const name of ['unit.txt', 'pilot.txt', ...extraData]) {
    fs.copyFileSync(path.join(lantern, 'Data', 'ランタン', name), path.join(scenario, 'Data', 'team', name));
  }
  return scenario;
}

/** フォルダの読み書き（File System Access API）を使わず、<input webkitdirectory> とダウンロードで動かす */
async function open(page, tab = 'map') {
  page.on('pageerror', e => console.log('pageerror:', e.message));
  // 「比べる」が読む 7474/SRC-Data の評価には行かない（比べるのテストは、後から足す route で代わりを返す）
  await page.route('https://7474.github.io/SRC-Data/**', route => route.abort());
  const res = await page.goto(base + `index.html?fs=input&lang=ja&tab=${tab}`);
  test.skip(res?.status() === 404 && !process.env.STUDIO_REQUIRED, 'SRC#Studio が置かれていない（tools/studio_bundle.sh）');
  await expect(page.locator('body[data-ready="true"]')).toHaveCount(1, { timeout: 60 * 1000 });
}

async function downloaded(page) {
  const [download] = await Promise.all([page.waitForEvent('download'), page.locator('#download').click()]);
  return { name: download.suggestedFilename(), text: fs.readFileSync(await download.path(), 'utf8') };
}

/** マップのテキストの (x, y) のマス（1 始まり） */
function tileAt(text, x, y) {
  const lines = text.split(/\r?\n/);
  const [w, h] = lines[2].split(',').map(Number);
  return lines[3 + (x - 1) * h + (y - 1)];
}

async function clickCell(page, x, y, options = {}) {
  const box = await page.locator('.me-canvas').boundingBox();
  await page.mouse.click(box.x + (x - 0.5) * 32, box.y + (y - 0.5) * 32, options);
}

test('最小の汎用データの地形で新しいマップを作り、描く・四角・塗りつぶし・元に戻す・大きさを変えて書き出す', async ({ page }) => {
  await open(page);
  // 同梱の最小の汎用データの地形がパレットに並ぶ
  await expect(page.locator('#app-folder')).toContainText('最小の汎用データ');
  await expect(page.locator('.me-terrains li').first()).toHaveText('0 平地');
  page.once('dialog', d => d.accept('stage.map'));
  await page.locator('#new-map').click();
  await expect(page.locator('.me-canvas')).toBeVisible();

  // 描く: (2,1) に 2 番（道路）
  await page.locator('.me-terrains li', { hasText: '2 道路' }).click();
  await clickCell(page, 2, 1);
  // 四角: (1,3)-(3,4) に 17 番（海）
  await page.locator('.me-terrains li', { hasText: '17 海' }).click();
  await page.locator('.me-tools button[data-tool="rect"]').click();
  const box = await page.locator('.me-canvas').boundingBox();
  await page.mouse.move(box.x + 16, box.y + 2 * 32 + 16);
  await page.mouse.down();
  await page.mouse.move(box.x + 2 * 32 + 16, box.y + 3 * 32 + 16, { steps: 4 });
  await page.mouse.up();
  let { name, text } = await downloaded(page);
  expect(name).toBe('stage.map');
  expect(text.startsWith('MapData\r\n\r\n20,20\r\n')).toBe(true);
  expect(tileAt(text, 2, 1)).toBe('2,0');
  expect(tileAt(text, 3, 4)).toBe('17,0');
  expect(tileAt(text, 4, 4)).toBe('0,0');

  // 塗りつぶし: 海の四角を 11 番（林）に。つながっていない所は変わらない
  await page.locator('.me-terrains li', { hasText: '11 林' }).click();
  await page.locator('.me-tools button[data-tool="fill"]').click();
  await clickCell(page, 1, 3);
  ({ text } = await downloaded(page));
  expect(tileAt(text, 3, 4)).toBe('11,0');
  expect(tileAt(text, 2, 1)).toBe('2,0');

  // 元に戻す（ボタンと Ctrl+Z）・やり直す
  await page.locator('.me-undo').click();
  ({ text } = await downloaded(page));
  expect(tileAt(text, 3, 4)).toBe('17,0');
  await page.locator('.me-redo').click();
  ({ text } = await downloaded(page));
  expect(tileAt(text, 3, 4)).toBe('11,0');

  // スポイト（右クリック）で道路を選び、大きさを 15×16 に（増えた行は選んでいる地形）
  await clickCell(page, 2, 1, { button: 'right' });
  await expect(page.locator('.me-current span')).toContainText('2 道路');
  await page.locator('.me-resize').click();
  await page.locator('.me-resize-dialog input[name="width"]').fill('15');
  await page.locator('.me-resize-dialog input[name="height"]').fill('21');
  await page.locator('.me-resize-dialog button[value="ok"]').click();
  ({ text } = await downloaded(page));
  expect(text.split('\r\n')[2]).toBe('15,21');
  expect(tileAt(text, 1, 21)).toBe('2,0');
  expect(tileAt(text, 3, 4)).toBe('11,0');
});

test('シナリオのフォルダ（ランタン号）を開き、マップを変えずに書き出すと元と同じ。レイヤーに描ける', async ({ page }) => {
  await open(page);
  const scenario = scenarioFolder();
  await page.locator('#scenario-input').setInputFiles(scenario);
  await expect(page.locator('#scenario-folder')).toContainText('lantern');
  await expect(page.locator('#maps a')).toHaveText(['harbor.map', 'stage/valley.map']);
  await page.locator('#maps a', { hasText: 'harbor.map' }).click();
  await expect(page.locator('.me-canvas')).toBeVisible();
  const original = fs.readFileSync(path.join(lantern, '01_灯の港.map'), 'utf8');
  // 保存はダウンロード（このブラウザの設定ではフォルダに書けない）
  let { name, text } = await downloaded(page);
  expect(name).toBe('harbor.map');
  expect(text).toBe(original);

  // レイヤーに 21 番（街）を描くと、Layer の段ができる
  await page.locator('input[name="me-target"][value="layer"]').check();
  await page.locator('.me-terrains li', { hasText: '21 街' }).click();
  await clickCell(page, 5, 5);
  ({ text } = await downloaded(page));
  const lines = text.split('\n').map(l => l.trim());
  const layer = lines.indexOf('Layer');
  expect(layer).toBeGreaterThan(0);
  expect(lines[layer + 1 + 4 * 20 + 4]).toBe('21,0');
  expect(lines[layer + 1]).toBe('10000,10000');
  // マウスを置いたマスの座標と地形名
  const box = await page.locator('.me-canvas').boundingBox();
  await page.mouse.move(box.x + 4.5 * 32, box.y + 4.5 * 32);
  await expect(page.locator('.me-status')).toContainText('(5,5)');
  await expect(page.locator('.me-status')).toContainText('レイヤー: 21 街');
});

test('データのプレビュー: シナリオのユニット・パイロットを、汎用データのスペシャルパワーと一緒に見る（7474/SRC#1643）', async ({ page }) => {
  await open(page);
  await page.locator('#scenario-input').setInputFiles(scenarioFolder());
  await page.locator('#tab-data').click();
  await expect(page.locator('#data-files option')).toHaveText(['Data/team/pilot.txt', 'Data/team/unit.txt']);
  // パイロット: 最小の汎用データの sp.txt でスペシャルパワーを解く
  await expect(page.locator('.dp-list li').first()).toHaveText('ソラ');
  await expect(page.locator('.dp-card h2')).toHaveText('ソラ');
  await expect(page.locator('.dp-card')).toContainText('加速（Lv1）');
  await expect(page.locator('.dp-errors')).toBeHidden();
  // ユニット: 能力値と武器
  await page.locator('#data-files').selectOption('Data/team/unit.txt');
  await expect(page.locator('.dp-card .dp-weapons')).toBeVisible();
  await page.locator('.dp-list li').nth(1).click();
  await expect(page.locator('.dp-card h2')).toHaveText(await page.locator('.dp-list li').nth(1).textContent());
  await expect(page.locator('.dp-card > .dp-stats')).toContainText('HP');
});

test('表で直す: 武器の欄を直すと、その欄だけが変わり、並べ替えもできる（7474/SRC#1644）', async ({ page }) => {
  await open(page);
  // 欄の分け方と書き換え（括弧の中のカンマ・空白・必要条件を保つ）
  const result = await page.evaluate(async base => {
    const m = await import(base + 'table-edit.js');
    return {
      fields: m.splitLine('ビーム, 1500, 1, 4, +10, -, 10, -, AABA, +10, Ｂ <(a, b)> (技能)').fields,
      replaced: m.replaceField('ビーム,  1500 , 1, 4', 1, '1800'),
    };
  }, base);
  expect(result.fields).toEqual(['ビーム', '1500', '1', '4', '+10', '-', '10', '-', 'AABA', '+10', 'Ｂ <(a, b)> (技能)']);
  expect(result.replaced).toBe('ビーム,  1800 , 1, 4');

  await page.locator('#scenario-input').setInputFiles(scenarioFolder());
  await page.locator('#tab-data').click();
  await page.locator('#data-files').selectOption('Data/team/unit.txt');
  await expect(page.locator('.dp-card .dp-weapons')).toBeVisible();
  const before = fs.readFileSync(path.join(lantern, 'Data', 'ランタン', 'unit.txt'), 'utf8');
  const firstWeapon = await page.locator('.dp-weapons tbody tr').first().locator('td').first().textContent();
  await page.locator('.dp-table-toggle').click();
  const power = page.locator('.dp-edit').first().locator('tbody tr').first().locator('input[data-field="power"]');
  await power.fill('9999');
  await power.press('Enter');
  await expect(page.locator('.dp-weapons tbody tr').first()).toContainText('9999');
  await expect(page.locator('#save-data')).toBeEnabled();
  const [download] = await Promise.all([page.waitForEvent('download'), page.locator('#save-data').click()]);
  const after = fs.readFileSync(await download.path(), 'utf8');
  const a = before.split(/\r?\n/);
  const b = after.split(/\r?\n/);
  const changed = a.map((line, i) => [line, b[i]]).filter(([x, y]) => x !== y);
  // 変わったのは 1 行の攻撃力の欄だけ
  expect(changed.length).toBe(1);
  expect(changed[0][0].startsWith(firstWeapon)).toBe(true);
  expect(changed[0][1]).toBe(changed[0][0].replace(/^([^,]*,\s*)\d+/, '$19999'));

  // 並べ替え: 1 つ目の武器を下の行と入れ替えると、プレビューの武器の順も変わる
  const second = await page.locator('.dp-weapons tbody tr').nth(1).locator('td').first().textContent();
  await page.locator('.dp-edit').first().locator('tbody tr').first().locator('.dp-move button').nth(1).click();
  await expect(page.locator('.dp-weapons tbody tr').first().locator('td').first()).toHaveText(second);
});

test('表で直す: ユニットとパイロットの能力値・地形適応を直すと、その欄だけが変わる（7474/SRC#1656）', async ({ page }) => {
  await open(page);
  const helper = await page.evaluate(async base => {
    const m = await import(base + 'table-edit.js');
    return {
      commentPreserved: m.replaceField('3000, 100, 600, 60 // コメント', 3, '75'),
      pilot4: m.replaceField('愛称, 汎用, AAAA, 120', m.pilotAdaptionIndex('愛称, 汎用, AAAA, 120'), 'SAAA'),
      pilot5: m.replaceField('愛称, 男性, 汎用, AAAA, 120', m.pilotAdaptionIndex('愛称, 男性, 汎用, AAAA, 120'), 'SAAB'),
      pilot6: m.replaceField('愛称, あいしょう, 男性, 汎用, AAAA, 120 // c', m.pilotAdaptionIndex('愛称, あいしょう, 男性, 汎用, AAAA, 120 // c'), 'SSAA'),
    };
  }, base);
  expect(helper.commentPreserved).toBe('3000, 100, 600, 75 // コメント');
  expect(helper.pilot4).toBe('愛称, 汎用, SAAA, 120');
  expect(helper.pilot5).toBe('愛称, 男性, 汎用, SAAB, 120');
  expect(helper.pilot6).toBe('愛称, あいしょう, 男性, 汎用, SSAA, 120 // c');

  await page.locator('#scenario-input').setInputFiles(scenarioFolder());
  await page.locator('#tab-data').click();

  // パイロット: 格闘・反応・地形適応を表で直す
  await expect(page.locator('.dp-card h2')).toHaveText('ソラ');
  const pilotBefore = fs.readFileSync(path.join(lantern, 'Data', 'ランタン', 'pilot.txt'), 'utf8');
  await page.locator('.dp-table-toggle').click();
  const pilotStats = page.locator('.dp-edit-stats');
  await expect(pilotStats).toBeVisible();
  await pilotStats.locator('input[data-field="infight"]').fill('177');
  await pilotStats.locator('input[data-field="infight"]').press('Enter');
  await pilotStats.locator('input[data-field="intuition"]').fill('188');
  await pilotStats.locator('input[data-field="intuition"]').press('Enter');
  await pilotStats.locator('input[data-field="adaption"]').fill('SAAA');
  await pilotStats.locator('input[data-field="adaption"]').press('Enter');
  await expect(page.locator('.dp-card > .dp-stats')).toContainText('177');
  await expect(page.locator('.dp-card > .dp-stats')).toContainText('188');
  await expect(page.locator('.dp-card > .dp-stats')).toContainText('SAAA');

  const [pilotDl] = await Promise.all([page.waitForEvent('download'), page.locator('#save-data').click()]);
  const pilotAfter = fs.readFileSync(await pilotDl.path(), 'utf8');
  const pChanged = pilotBefore.split(/\r?\n/).map((line, i) => [line, pilotAfter.split(/\r?\n/)[i]]).filter(([x, y]) => x !== y);
  expect(pChanged).toEqual([
    ['主人公, そら, 男性, 灯機, AAAA, 150', '主人公, そら, 男性, 灯機, SAAA, 150'],
    ['150, 145, 150, 150, 150, 155, 強気', '177, 145, 150, 150, 150, 188, 強気'],
  ]);

  // ユニット: HP・移動力・地形適応を表で直す
  await page.locator('#data-files').selectOption('Data/team/unit.txt');
  await expect(page.locator('.dp-card .dp-weapons')).toBeVisible();
  const unitBefore = fs.readFileSync(path.join(lantern, 'Data', 'ランタン', 'unit.txt'), 'utf8');
  await page.locator('.dp-table-toggle').click();
  const unitStats = page.locator('.dp-edit-stats');
  await expect(unitStats).toBeVisible();
  await unitStats.locator('input[data-field="hp"]').fill('4200');
  await unitStats.locator('input[data-field="hp"]').press('Enter');
  await unitStats.locator('input[data-field="speed"]').fill('8');
  await unitStats.locator('input[data-field="speed"]').press('Enter');
  await unitStats.locator('input[data-field="adaption"]').fill('SSAA');
  await unitStats.locator('input[data-field="adaption"]').press('Enter');
  await expect(page.locator('.dp-card > .dp-stats')).toContainText('4200');
  await expect(page.locator('.dp-card > .dp-stats')).toContainText('SSAA');

  const [unitDl] = await Promise.all([page.waitForEvent('download'), page.locator('#save-data').click()]);
  const unitAfter = fs.readFileSync(await unitDl.path(), 'utf8');
  const uChanged = unitBefore.split(/\r?\n/).map((line, i) => [line, unitAfter.split(/\r?\n/)[i]]).filter(([x, y]) => x !== y);
  expect(uChanged).toEqual([
    ['陸, 5, M, 3000, 120', '陸, 8, M, 3000, 120'],
    ['3600, 140, 900, 100', '4200, 140, 900, 100'],
    ['AA-A, Lantern_Hotaru.bmp', 'SSAA, Lantern_Hotaru.bmp'],
  ]);
});

test('比べる: 書いているユニットとパイロットを、データパックの中の上位何 % かで見る（7474/SRC#1644）', async ({ page }) => {
  // 7474/SRC-Data の Pages の評価の JSON の代わり
  const analysis = {
    format: 1,
    files: [{ section: 'ロボ', file: 'pairs-ロボ.json' }, { section: 'Sharp', file: 'pairs-Sharp.json' }],
  };
  const units = Array.from({ length: 10 }, (_, i) => ({ name: `u${i}`, hp: (i + 1) * 1000, en: 100, armor: 1000, mobility: 100, speed: 5, values: { 最大攻撃力: (i + 1) * 300 } }));
  const pilots = Array.from({ length: 4 }, (_, i) => ({ name: `p${i}`, band: '経験値120-159', sp: 50 + i * 10, stats: { 格闘: 120 + i * 20, 射撃: 120, 命中: 120, 回避: 120, 技量: 120, 反応: 120 } }));
  await open(page);
  await page.route('https://7474.github.io/SRC-Data/analysis/**', route => {
    const url = route.request().url();
    const body = url.endsWith('index.json') ? analysis : { format: 2, pairs: [], units, pilots };
    return route.fulfill({ status: 200, contentType: 'application/json', headers: { 'access-control-allow-origin': '*' }, body: JSON.stringify(body) });
  });
  await page.locator('#scenario-input').setInputFiles(scenarioFolder());
  await page.locator('#tab-data').click();
  const compare = page.locator('.dp-compare');
  await expect(compare.locator('select option')).toHaveText(['ロボ']);
  await expect(compare.locator('.dp-compare-body')).toContainText('同じ帯');
  await expect(compare.locator('.dp-compare-body tr').first()).toContainText('上位');
  await page.locator('#data-files').selectOption('Data/team/unit.txt');
  await expect(page.locator('.dp-compare-body')).toContainText('全ユニット 10 体');
  await expect(page.locator('.dp-compare-body tr', { hasText: 'HP' })).toContainText('上位');
});

/** かんたん作成が読む 7474/SRC-Data の代わり（ランタン号のユニット・パイロットを 1 つの作品として返す） */
async function routeSrcData(page, scenes = null) {
  const index = {
    titles: [
      { title: 'ランタン', base: 'GSCデータパック/ロボ', path: 'GSCデータパック/ロボ/ランタン', tags: ['GSC', 'ロボ'], files: ['Unit.txt', 'pilot.txt', '更新履歴.txt'] },
      { title: 'System', base: 'GSCデータパック/ロボ', path: 'GSCデータパック/ロボ/System', tags: ['GSC', 'ロボ'], files: ['alias.txt'] },
    ],
  };
  const files = {
    'GSCデータパック/ロボ/ランタン/Unit.txt': fs.readFileSync(path.join(lantern, 'Data', 'ランタン', 'unit.txt')),
    'GSCデータパック/ロボ/ランタン/pilot.txt': fs.readFileSync(path.join(lantern, 'Data', 'ランタン', 'pilot.txt')),
    'GSCデータパック/ロボ/System/alias.txt': Buffer.from('# 別名\r\n'),
    'scenes/maps/harbor.map': fs.readFileSync(path.join(lantern, '01_灯の港.map')),
  };
  const requested = [];
  await page.route('https://raw.githubusercontent.com/7474/SRC-Data/master/**', route => {
    const p = decodeURIComponent(new URL(route.request().url()).pathname.replace('/7474/SRC-Data/master/', ''));
    requested.push(p);
    if (p === 'index.json') {
      return route.fulfill({ json: index });
    }
    if (p === 'scenes/index.json' && scenes) {
      return route.fulfill({ json: scenes });
    }
    return files[p] ? route.fulfill({ body: files[p] }) : route.fulfill({ status: 404 });
  });
  return requested;
}

test('かんたん作成: 作品を選び、味方と敵を置き、セリフを書いて zip に保存し、SRC#Web に渡す（7474/SRC#1659）', async ({ page, context }) => {
  const requested = await routeSrcData(page);
  await open(page, 'quick');
  await expect(page.locator('#tab-quick')).toHaveAttribute('aria-selected', 'true');

  // 1. 作品を探して選ぶ。同じデータパックの System も一緒に読み、更新履歴は読まない
  await page.locator('.q-search').fill('ランタン');
  await page.locator('.q-results button', { hasText: 'ランタン' }).click();
  await expect(page.locator('.q-chosen li')).toHaveCount(1);
  await expect(page.locator('.q-chosen li')).toContainText('ユニット');
  expect(requested).toContain('GSCデータパック/ロボ/System/alias.txt');
  expect(requested.some(p => p.includes('更新履歴'))).toBe(false);
  await expect(page.locator('#q-units option[value="灯機ホタル"]')).toHaveCount(1);

  // 2. 味方と敵
  await page.locator('.q-add[data-side="allies"]').click();
  await page.locator('tr[data-side="allies"] .q-unit').fill('灯機ホタル');
  await page.locator('tr[data-side="allies"] .q-unit').dispatchEvent('change');
  await page.locator('tr[data-side="allies"] .q-pilot').fill('ソラ');
  await page.locator('tr[data-side="allies"] .q-pilot').dispatchEvent('change');
  await page.locator('.q-add[data-side="enemies"]').click();
  await page.locator('tr[data-side="enemies"] .q-unit').fill('灯機ホタル');
  await page.locator('tr[data-side="enemies"] .q-unit').dispatchEvent('change');
  await page.locator('tr[data-side="enemies"] .q-pilot').fill('リオ');
  await page.locator('tr[data-side="enemies"] .q-pilot').dispatchEvent('change');
  // 敵はランク 2・ボスランク 3 にする
  await page.locator('tr[data-side="enemies"] .q-rank').fill('2');
  await page.locator('tr[data-side="enemies"] .q-rank').dispatchEvent('change');
  await page.locator('tr[data-side="enemies"] .q-boss').selectOption('3');

  // 3. 場面: 味方を選んで (3,4) に動かす
  // 題名はファイル名になる。日本語でもよいが、テストのブラウザ（headless）は日本語のダウンロードの名前を「download」にするので ASCII にする
  await page.locator('.q-title').fill('first-battle');
  await page.locator('.q-title').dispatchEvent('change');
  await page.locator('tr[data-side="allies"] .q-pos').click();
  await page.locator('.q-canvas').scrollIntoViewIfNeeded();
  const board = await page.locator('.q-canvas').boundingBox();
  await page.mouse.click(board.x + 2.5 * 26, board.y + 3.5 * 26);
  await expect(page.locator('tr[data-side="allies"] .q-pos')).toHaveText('3,4');

  // 4. セリフ
  await page.locator('.q-add-line[data-scene="opening"]').click();
  await page.locator('.q-line[data-scene="opening"] .q-speaker').selectOption('ソラ');
  await page.locator('.q-line[data-scene="opening"] .q-text').fill('行くぞ！');

  // 5. zip で保存: イベント・マップと、作品のデータ（名前は小文字）。圧縮しないので中身を文字で探せる
  const [download] = await Promise.all([page.waitForEvent('download'), page.locator('.q-save').click()]);
  expect(download.suggestedFilename()).toBe('first-battle.zip');
  const zip = fs.readFileSync(await download.path());
  const text = zip.toString('utf8');
  for (const name of ['第1話.eve', '第1話.map', '第1話.play', 'README.md', '.vscode/launch.json', '.github/workflows/src-check.yml', 'Data/ランタン/unit.txt', 'Data/ランタン/pilot.txt', 'Data/ロボ_System/alias.txt']) {
    expect(text).toContain(name);
  }
  expect(text).toContain('@ロボ_System\r\n@ランタン');
  expect(text).toContain('Create 味方 灯機ホタル 0 ソラ 10 3 4');
  expect(text).toMatch(/Create 敵 灯機ホタル 2 リオ 5 \d+ \d+\r\nBossRank 3\r\n/);
  expect(text.match(/BossRank/g)).toHaveLength(1);
  expect(text).toContain('Talk ソラ\r\n行くぞ！\r\nEnd');

  // SRC#Web で遊ぶ: 同じサイトの SRC#Web の「追加したアーカイブ」に入れ、?stored= で開く（ここでは SRC#Web の代わりの頁を返す）
  await context.route('**/web/**', route => route.fulfill({ contentType: 'text/html', body: '<p>SRC#Web</p>' }));
  const [popup] = await Promise.all([context.waitForEvent('page'), page.locator('.q-play').click()]);
  await expect.poll(() => popup.url()).toContain('stored=');
  expect(decodeURIComponent(popup.url())).toContain('/web/?stored=first-battle.zip');
  const stored = await page.evaluate(() => new Promise((resolve, reject) => {
    const request = indexedDB.open('srcweb');
    request.onsuccess = () => {
      const get = request.result.transaction('archives').objectStore('archives').get('first-battle.zip');
      get.onsuccess = () => resolve({ kind: get.result.kind, requires: get.result.requires, size: get.result.size });
      get.onerror = () => reject(get.error);
    };
    request.onerror = () => reject(request.error);
  }));
  expect(stored.kind).toBe('scenario');
  expect(stored.requires).toEqual(['basic-data']);
  expect(stored.size).toBe(zip.length);

  // 下書きはこのブラウザに残り、開き直すと作品を読み直す
  await page.reload();
  await expect(page.locator('body[data-ready="true"]')).toHaveCount(1, { timeout: 60 * 1000 });
  await expect(page.locator('.q-chosen li')).toHaveCount(1);
  await expect(page.locator('tr[data-side="allies"] .q-pilot')).toHaveValue('ソラ');
  await expect(page.locator('.q-line[data-scene="opening"] .q-text')).toHaveValue('行くぞ！');
  await expect(page.locator('tr[data-side="enemies"] .q-rank')).toHaveValue('2');
  await expect(page.locator('tr[data-side="enemies"] .q-boss')).toHaveValue('3');
});

test('かんたん作成の出来事: 勝ち負けの条件・援軍・戦闘中の会話・撤退を足すと、普通のイベントになる（7474/SRC#1689）', async ({ page }) => {
  await routeSrcData(page);
  await open(page, 'quick');
  await page.locator('.q-search').fill('ランタン');
  await page.locator('.q-results button', { hasText: 'ランタン' }).click();
  await expect(page.locator('.q-chosen li')).toHaveCount(1);
  const setUnit = async (row, unit, pilot) => {
    await row.locator('.q-unit').fill(unit);
    await row.locator('.q-unit').dispatchEvent('change');
    await row.locator('.q-pilot').fill(pilot);
    await row.locator('.q-pilot').dispatchEvent('change');
  };
  await page.locator('.q-add[data-side="allies"]').click();
  await setUnit(page.locator('tr[data-side="allies"]'), '灯機ホタル', 'ソラ');
  await page.locator('.q-add[data-side="enemies"]').click();
  await setUnit(page.locator('tr[data-side="enemies"]'), '灯機ツバメ', 'リオ');
  await page.locator('tr[data-side="enemies"] .q-boss').selectOption('2');
  await page.locator('.q-title').fill('events');
  await page.locator('.q-title').dispatchEvent('change');

  // 勝利の条件: リオを倒す。敗北の条件: 8 ターンを過ぎる
  await page.locator('.q-goal-kind').selectOption('boss');
  await page.locator('.q-goal-target').selectOption('リオ');
  await page.locator('.q-loss-kind').selectOption('turns');
  await page.locator('.q-loss-turns').fill('8');
  await page.locator('.q-loss-turns').dispatchEvent('change');

  // 援軍: 3 ターン目の敵のフェイズに、ミュウを (2,2) に置く
  await page.locator('.q-add-event[data-kind="reinforce"]').click();
  const reinforce = page.locator('.q-event[data-event="0"]');
  await reinforce.locator('.q-ev-turn').fill('3');
  await reinforce.locator('.q-ev-turn').dispatchEvent('change');
  await setUnit(reinforce.locator('tr[data-side="ev0.enemies"]'), '灯機ホタル', 'ミュウ');
  await reinforce.locator('tr[data-side="ev0.enemies"] .q-pos').click();
  await page.locator('.q-canvas').scrollIntoViewIfNeeded();
  const box = await page.locator('.q-canvas').boundingBox();
  await page.mouse.click(box.x + 1.5 * 26, box.y + 1.5 * 26);
  await expect(page.locator('tr[data-side="ev0.enemies"] .q-pos')).toHaveText('2,2');

  // 戦闘中の会話: ソラとリオが戦うとき
  await page.locator('.q-add-event[data-kind="talk"]').click();
  const talk = page.locator('.q-event[data-event="1"]');
  await expect(talk.locator('.q-ev-unit')).toHaveValue('ソラ');
  await expect(talk.locator('.q-ev-target')).toHaveValue('リオ');
  await talk.locator('.q-text').fill('リオ、目を覚まして！');

  // 撤退: リオの損傷率 60% で退く（倒す相手が退いたら勝ち）
  await page.locator('.q-add-event[data-kind="retreat"]').click();
  const retreat = page.locator('.q-event[data-event="2"]');
  await expect(retreat.locator('.q-ev-unit')).toHaveValue('リオ');
  await retreat.locator('.q-ev-ratio').fill('60');
  await retreat.locator('.q-ev-ratio').dispatchEvent('change');
  await retreat.locator('.q-text').fill('覚えていろ！');

  const [download] = await Promise.all([page.waitForEvent('download'), page.locator('.q-save').click()]);
  const text = fs.readFileSync(await download.path()).toString('utf8');
  expect(text).toContain('勝利条件：リオの撃破');
  expect(text).toMatch(/ターン 3 敵:\r\nCreate 敵 灯機ホタル 0 ミュウ 5 2 2\r\nExit/);
  expect(text).toMatch(/攻撃 ソラ リオ:\r\nTalk ソラ\r\nリオ、目を覚まして！\r\nEnd\r\nExit/);
  expect(text).toMatch(/損傷率 リオ 60:\r\nTalk リオ\r\n覚えていろ！\r\nEnd\r\nEscape リオ\r\n/);
  expect(text).toMatch(/破壊 リオ:\r\nTalk システム\r\n勝利！\r\nEnd\r\nGameClear/);
  expect(text).toMatch(/ターン 9 味方:\r\nGameOver/);

  // 下書きに残り、開き直しても出来事はそのまま
  await page.reload();
  await expect(page.locator('body[data-ready="true"]')).toHaveCount(1, { timeout: 60 * 1000 });
  await expect(page.locator('.q-event')).toHaveCount(3);
  await expect(page.locator('.q-goal-kind')).toHaveValue('boss');
  await expect(page.locator('.q-loss-turns')).toHaveValue('8');
});

test('かんたん作成の話を足す: 前の話の味方が続けて出て、勝つと次の話に進むシナリオになる（7474/SRC#1691）', async ({ page, context }) => {
  await routeSrcData(page);
  await open(page, 'quick');
  await page.locator('.q-search').fill('ランタン');
  await page.locator('.q-results button', { hasText: 'ランタン' }).click();
  await expect(page.locator('.q-chosen li')).toHaveCount(1);
  const setUnit = async (row, unit, pilot) => {
    await row.locator('.q-unit').fill(unit);
    await row.locator('.q-unit').dispatchEvent('change');
    await row.locator('.q-pilot').fill(pilot);
    await row.locator('.q-pilot').dispatchEvent('change');
  };
  await page.locator('.q-add[data-side="allies"]').click();
  await setUnit(page.locator('tr[data-side="allies"]'), '灯機ホタル', 'ソラ');
  await page.locator('.q-add[data-side="enemies"]').click();
  await setUnit(page.locator('tr[data-side="enemies"]'), '灯機ツバメ', 'リオ');
  await page.locator('.q-title').fill('campaign');
  await page.locator('.q-title').dispatchEvent('input');

  // 第2話を足す: 味方（ソラ）を写し、前の話から続けて出ることを示す
  await page.locator('.q-add-episode').click();
  await expect(page.locator('.q-episode')).toHaveCount(2);
  await expect(page.locator('.q-episode[aria-current="page"]')).toHaveText('第2話');
  await expect(page.locator('tr[data-side="allies"] .q-pilot')).toHaveValue('ソラ');
  await expect(page.locator('.q-episode-hint')).toContainText('ソラ');
  // 乗り換える（灯機ホタル → ランタン号）
  await setUnit(page.locator('tr[data-side="allies"]'), 'ランタン号', 'ソラ');
  await page.locator('.q-add[data-side="enemies"]').click();
  await setUnit(page.locator('tr[data-side="enemies"]'), '灯機ホタル', 'ミュウ');
  // 題名は 1 つの話ではなくシナリオのもの
  await expect(page.locator('.q-title')).toHaveValue('campaign');

  // 第1話に戻ると、第1話の敵が見える
  await page.locator('.q-episode[data-index="0"]').click();
  await expect(page.locator('tr[data-side="enemies"] .q-pilot')).toHaveValue('リオ');

  const [download] = await Promise.all([page.waitForEvent('download'), page.locator('.q-save').click()]);
  expect(download.suggestedFilename()).toBe('campaign.zip');
  const text = fs.readFileSync(await download.path()).toString('utf8');
  for (const name of ['第1話.eve', '第2話.eve', '第1話.map', '第2話.map', '全話.play']) {
    expect(text).toContain(name);
  }
  expect(text).toContain('Continue 第2話.eve');
  expect(text).toMatch(/If IsDefined\(ソラ, パイロット\) = 0 Then\r\n  Create 味方 ランタン号 0 ソラ /);
  expect(text).toMatch(/Else\r\n  # 灯機ホタルからランタン号に乗り換える\r\n  Unit ランタン号 0\r\n  Ride ソラ\r\n  Launch ソラ /);

  // SRC#Web で遊ぶ: 第1話から始める
  await context.route('**/web/**', route => route.fulfill({ contentType: 'text/html', body: '<p>SRC#Web</p>' }));
  const [popup] = await Promise.all([context.waitForEvent('page'), page.locator('.q-play').click()]);
  await expect.poll(() => popup.url()).toContain('stored=');
  expect(decodeURIComponent(popup.url())).toContain('/web/?stored=campaign.zip&eve=第1話.eve');

  // 下書きに 2 つの話が残る
  await page.reload();
  await expect(page.locator('body[data-ready="true"]')).toHaveCount(1, { timeout: 60 * 1000 });
  await expect(page.locator('.q-episode')).toHaveCount(2);
  await page.locator('.q-episode[data-index="1"]').click();
  await expect(page.locator('tr[data-side="enemies"] .q-pilot')).toHaveValue('ミュウ');

  // 話を消す
  page.once('dialog', d => d.accept());
  await page.locator('.q-remove-episode').click();
  await expect(page.locator('.q-episode')).toHaveCount(1);
  await expect(page.locator('.q-remove-episode')).toHaveCount(0);
});

test('かんたん作成: 足りないものは作らずに知らせる', async ({ page }) => {
  await routeSrcData(page);
  await open(page, 'quick');
  await page.locator('.q-save').click();
  await expect(page.locator('.quick-status')).toContainText('味方のユニットがありません');
  await expect(page.locator('.quick-status')).toContainText('敵のユニットがありません');
});

test('かんたん作成: 場面のカタログのサンプル（マップ・置く位置・作品・ユニット・セリフ）を使う', async ({ page }) => {
  await routeSrcData(page, {
    scenes: [{
      id: 'harbor', title: '灯の港の戦い', description: '港での小競り合い', map: 'maps/harbor.map',
      allies: [{ x: 3, y: 10 }, { x: 4, y: 10 }], enemies: [{ x: 15, y: 4 }],
      titles: ['GSCデータパック/ロボ/ランタン'],
      units: { allies: [{ unit: '灯機ホタル', pilot: 'ソラ', level: 12, x: 3, y: 10 }], enemies: [{ unit: '灯機ホタル', pilot: 'リオ', level: 8, bossRank: 2, x: 15, y: 4 }] },
      talk: { opening: [{ speaker: 'ソラ', text: '港を守るぞ！' }] },
      // 出来事（7474/SRC#1689）
      goal: { kind: 'boss', target: 'リオ' },
      events: [{ trigger: 'destroyed', unit: 'リオ', talk: [{ speaker: 'リオ', text: 'まだ終わらない……' }],
        enemies: [{ unit: '灯機ツバメ', pilot: 'グスタフ', level: 9, x: 16, y: 4 }] }],
    }],
  });
  await open(page, 'quick');
  // サンプルの場面の一覧は、押して開く（既定は閉じる）。マップの欄でも選べる
  await expect(page.locator('.q-gallery')).not.toHaveAttribute('open', '');
  await page.locator('.q-gallery summary').click();
  await expect(page.locator('.q-gallery')).toHaveAttribute('open', '');
  await expect(page.locator('.q-map option', { hasText: '灯の港の戦い' })).toHaveCount(1);
  const card = page.locator('.q-scene-card', { hasText: '灯の港の戦い' });
  await expect(card).toContainText('港での小競り合い');
  await card.click();
  // 入れたら一覧を閉じ、マップの欄はその場面になる
  await expect(page.locator('.q-gallery')).not.toHaveAttribute('open', '');
  await expect(page.locator('.q-map option:checked')).toHaveText('灯の港の戦い');
  await expect(page.locator('.q-scene-about')).toHaveText('港での小競り合い');

  // 何も選んでいなければ尋ねずに入れる
  await expect(page.locator('.q-chosen li')).toContainText('ランタン');
  await expect(page.locator('tr[data-side="allies"] .q-unit')).toHaveValue('灯機ホタル');
  await expect(page.locator('tr[data-side="allies"] .q-pos')).toHaveText('3,10');
  await expect(page.locator('tr[data-side="enemies"] .q-pilot')).toHaveValue('リオ');
  await expect(page.locator('tr[data-side="enemies"] .q-boss')).toHaveValue('2');
  await expect(page.locator('tr[data-side="allies"] .q-boss')).toHaveValue('-1');
  await expect(page.locator('.q-line[data-scene="opening"] .q-text')).toHaveValue('港を守るぞ！');
  await expect(page.locator('.q-title')).toHaveValue('灯の港の戦い');
  await expect(page.locator('.q-goal-kind')).toHaveValue('boss');
  await expect(page.locator('.q-goal-target')).toHaveValue('リオ');
  await expect(page.locator('.q-event .q-ev-trigger')).toHaveValue('destroyed');
  await expect(page.locator('tr[data-side="ev0.enemies"] .q-pilot')).toHaveValue('グスタフ');
  // 足した味方は、空いているおすすめの位置に置く
  await page.locator('.q-add[data-side="allies"]').click();
  await expect(page.locator('tr[data-side="allies"] .q-pos').nth(1)).toHaveText('4,10');
  await page.locator('tr[data-side="allies"] .q-remove').nth(1).click();

  // zip のマップはサンプルのマップのまま（題名は ASCII にする。前のテストを参照）
  await page.locator('.q-title').fill('harbor-battle');
  await page.locator('.q-title').dispatchEvent('change');
  const [download] = await Promise.all([page.waitForEvent('download'), page.locator('.q-save').click()]);
  const text = fs.readFileSync(await download.path()).toString('utf8');
  const source = fs.readFileSync(path.join(lantern, '01_灯の港.map'), 'utf8').replace(/^\uFEFF/, '');
  expect(text).toContain(source);
  expect(text).toContain('ChangeMap 第1話.map');
  expect(text).toMatch(/破壊 リオ:\r\nTalk リオ\r\nまだ終わらない……\r\nEnd\r\nCreate 敵 灯機ツバメ 0 グスタフ 9 16 4\r\n/);
});

test('かんたん作成: 内容をリセットすると、選んだ作品・マップ・ユニット・セリフ・題名と下書きが消える', async ({ page }) => {
  await routeSrcData(page, {
    scenes: [{
      id: 'harbor', title: '灯の港の戦い', description: '港での小競り合い', map: 'maps/harbor.map',
      allies: [{ x: 3, y: 10 }], enemies: [{ x: 15, y: 4 }],
      titles: ['GSCデータパック/ロボ/ランタン'],
      units: { allies: [{ unit: '灯機ホタル', pilot: 'ソラ', level: 12, x: 3, y: 10 }], enemies: [{ unit: '灯機ホタル', pilot: 'リオ', level: 8, x: 15, y: 4 }] },
      talk: { opening: [{ speaker: 'ソラ', text: '港を守るぞ！' }] },
    }],
  });
  await open(page, 'quick');
  await expect(page.locator('.q-reset')).toHaveCount(2);
  await expect(page.locator('.q-toolbar .q-reset')).toBeDisabled();
  await expect(page.locator('.q-actions .q-reset')).toBeDisabled();

  // サンプルの場面を入れ、下書きが保存された状態にする
  await page.locator('.q-gallery summary').click();
  await page.locator('.q-scene-card', { hasText: '灯の港の戦い' }).click();
  await expect(page.locator('.q-chosen li')).toHaveCount(1);
  await expect(page.locator('.q-title')).toHaveValue('灯の港の戦い');
  await expect(page.locator('.q-toolbar .q-reset')).toBeEnabled();
  await expect(page.locator('.q-actions .q-reset')).toBeEnabled();
  await expect.poll(() => page.evaluate(() => localStorage.getItem('srcstudio.quick'))).not.toBeNull();

  // 確認をキャンセルしたら内容は消えない
  page.once('dialog', d => d.dismiss());
  await page.locator('.q-toolbar .q-reset').click();
  await expect(page.locator('.q-chosen li')).toHaveCount(1);
  await expect(page.locator('.q-title')).toHaveValue('灯の港の戦い');

  // 確認で OK するとすべて初期状態に戻り、下書きも消える
  page.once('dialog', d => d.accept());
  await page.locator('.q-toolbar .q-reset').click();
  await expect(page.locator('.q-chosen li')).toHaveCount(0);
  await expect(page.locator('tr[data-side="allies"]')).toHaveCount(0);
  await expect(page.locator('tr[data-side="enemies"]')).toHaveCount(0);
  await expect(page.locator('.q-line')).toHaveCount(0);
  await expect(page.locator('.q-title')).toHaveValue('');
  await expect(page.locator('.q-map')).toHaveValue('');
  await expect(page.locator('.q-toolbar .q-reset')).toBeDisabled();
  await expect(page.locator('.quick-status')).toContainText('リセットしました');
  expect(await page.evaluate(() => localStorage.getItem('srcstudio.quick'))).toBeNull();

  // 開き直しても初期状態のまま
  await page.reload();
  await expect(page.locator('body[data-ready="true"]')).toHaveCount(1, { timeout: 60 * 1000 });
  await expect(page.locator('.q-chosen li')).toHaveCount(0);
  await expect(page.locator('.q-title')).toHaveValue('');
  await expect(page.locator('.q-toolbar .q-reset')).toBeDisabled();

  // 題名を 1 文字入れただけでも有効になり、5 側のリセットボタンでも同様に消せる
  await page.locator('.q-title').fill('temp');
  await expect(page.locator('.q-actions .q-reset')).toBeEnabled();
  page.once('dialog', d => d.accept());
  await page.locator('.q-actions .q-reset').click();
  await expect(page.locator('.q-title')).toHaveValue('');
  await expect(page.locator('.q-actions .q-reset')).toBeDisabled();
  expect(await page.evaluate(() => localStorage.getItem('srcstudio.quick'))).toBeNull();
});

test('マップの配置: .eve からユニットを読み込み、ドラッグで座標の欄だけを書き換える（7474/SRC#1653）', async ({ page }) => {
  await open(page, 'map');
  await page.locator('#scenario-input').setInputFiles(scenarioFolder());
  await page.locator('#maps a', { hasText: 'harbor.map' }).click();
  await expect(page.locator('.me-canvas')).toBeVisible();

  await page.locator('input[name="me-target"][value="placement"]').check();
  await expect(page.locator('.me-placements li').first()).toBeVisible();
  await expect(page.locator('.me-placements')).toContainText('ランタン号');
  await expect(page.locator('.me-placements')).toContainText('祈り手');
  // 出来事（ターン 2 味方）で出る祈り手は、破線の印とラベルを添える（7474/SRC#1692）
  const prayer = page.locator('.me-placements li', { hasText: '祈り手' }).first();
  await expect(prayer).toContainText('ターン 2 味方');
  await expect(prayer.locator('.me-dot-event')).toHaveCount(1);
  await expect(page.locator('.me-placements li', { hasText: 'ランタン号' }).first().locator('.me-dot-event')).toHaveCount(0);

  // ランタン号を選んで移動力の目安が出ることを確かめ、(3,8) から (5,10) へドラッグする
  await page.locator('.me-placements li', { hasText: 'ランタン号' }).click();
  await expect(page.locator('.me-placement-detail')).toContainText('移動力の目安');
  await expect(page.locator('.me-pl-x')).toHaveValue('3');
  await expect(page.locator('.me-pl-y')).toHaveValue('8');

  const box = await page.locator('.me-canvas').boundingBox();
  await page.mouse.move(box.x + (3 - 0.5) * 32, box.y + (8 - 0.5) * 32);
  await page.mouse.down();
  await page.mouse.move(box.x + (5 - 0.5) * 32, box.y + (10 - 0.5) * 32, { steps: 4 });
  await page.mouse.up();

  await expect(page.locator('.me-pl-x')).toHaveValue('5');
  await expect(page.locator('.me-pl-y')).toHaveValue('10');
  await expect(page.locator('#save-events')).toBeVisible();
  await expect(page.locator('#events-status')).toContainText('harbor.eve');

  const [download] = await Promise.all([page.waitForEvent('download'), page.locator('#save-events').click()]);
  expect(download.suggestedFilename()).toBe('harbor.eve');
  const before = fs.readFileSync(path.join(lantern, '01_灯の港.eve'), 'utf8').split(/\r?\n/);
  const after = fs.readFileSync(await download.path(), 'utf8').split(/\r?\n/);
  const changed = before.map((line, i) => [line, after[i]]).filter(([a, b]) => a !== b);
  expect(changed).toEqual([
    ['Create 味方 ランタン号 0 グスタフ 10 3 8', 'Create 味方 ランタン号 0 グスタフ 10 5 10'],
  ]);
});

test('人形劇: 場面を一手ずつ見て、会話の途中でユニットを動かすと Move を書き足す（7474/SRC#1694）', async ({ page }) => {
  await open(page, 'map');
  await page.locator('#scenario-input').setInputFiles(scenarioFolder());
  await page.locator('#maps a', { hasText: 'harbor.map' }).click();
  await expect(page.locator('.me-canvas')).toBeVisible();
  await page.locator('input[name="me-target"][value="placement"]').check();

  const select = page.locator('.me-puppet-block');
  await expect(select.locator('option', { hasText: 'harbor.eve: スタート' })).toHaveCount(1);
  const value = await select.locator('option', { hasText: 'harbor.eve: スタート' }).getAttribute('value');
  await select.selectOption(value);
  await expect(page.locator('.me-puppet-body')).toBeVisible();
  // 人形劇の間は、静的な配置の一覧を隠す
  await expect(page.locator('.me-placements')).toBeHidden();

  // 一手ずつ進め、「すごい……！」（ソラの台詞）まで行く
  const step = page.locator('.me-puppet-step');
  for (let i = 0; i < 40 && !(await step.textContent()).includes('すごい'); i++) {
    await page.locator('.me-puppet-next').click();
  }
  await expect(step).toContainText('ソラ');
  await expect(step).toContainText('すごい……！');

  // ソラ（灯機ホタル、(8,6)）を (10,6) へドラッグすると、会話を切って Move を書き足す
  const box = await page.locator('.me-canvas').boundingBox();
  await page.mouse.move(box.x + (8 - 0.5) * 32, box.y + (6 - 0.5) * 32);
  await page.mouse.down();
  await page.mouse.move(box.x + (10 - 0.5) * 32, box.y + (6 - 0.5) * 32, { steps: 4 });
  await page.mouse.up();
  await expect(step).toContainText('Move ソラ 10 6');
  await expect(page.locator('#save-events')).toBeVisible();

  // Move の手でもう一度動かすと、行き先を書き換える
  await page.mouse.move(box.x + (10 - 0.5) * 32, box.y + (6 - 0.5) * 32);
  await page.mouse.down();
  await page.mouse.move(box.x + (11 - 0.5) * 32, box.y + (7 - 0.5) * 32, { steps: 4 });
  await page.mouse.up();
  await expect(step).toContainText('Move ソラ 11 7');

  // 次の手は切った会話の続き
  await page.locator('.me-puppet-next').click();
  await expect(step).toContainText('リオ');

  const [download] = await Promise.all([page.waitForEvent('download'), page.locator('#save-events').click()]);
  const after = fs.readFileSync(await download.path(), 'utf8');
  // 会話を End で切り、Move を挟む。続きは元から Talk リオ なので話す人を書き直さない
  expect(after).toMatch(/すごい……！ でも、まだ来る！\r?\nEnd\r?\nMove ソラ 11 7\r?\nTalk リオ\r?\n/);

  // この場面を再生: シナリオ（書きかけのイベントを含む）と、その場面だけを流す 場面の再生.eve を SRC#Web に渡す（ここでは SRC#Web の代わりの頁を返す）
  await page.context().route('**/web/**', route => route.fulfill({ contentType: 'text/html', body: '<p>SRC#Web</p>' }));
  const [popup] = await Promise.all([page.context().waitForEvent('page'), page.locator('.me-puppet-play').click()]);
  await expect.poll(() => popup.url()).toContain('stored=');
  expect(decodeURIComponent(popup.url())).toContain('/web/?stored=lantern（場面の再生）.zip&eve=場面の再生.eve');
  const zipText = await page.evaluate(() => new Promise((resolve, reject) => {
    const request = indexedDB.open('srcweb');
    request.onsuccess = () => {
      const get = request.result.transaction('archives').objectStore('archives').get('lantern（場面の再生）.zip');
      get.onsuccess = async () => resolve(await get.result.data.text());
      get.onerror = () => reject(get.error);
    };
    request.onerror = () => reject(request.error);
  }));
  expect(zipText).toContain('場面の再生.eve');
  expect(zipText).toMatch(/スタート:\r?\nChangeMap harbor\.map\r?\n/);
  expect(zipText).toContain('Move ソラ 11 7');
  expect(zipText).toMatch(/場面の再生の終わり:\r?\nTelop 場面の再生はここまで\r?\nGameClear/);
});

test('戦闘メッセージ: メッセージウィンドウのプレビュー・充実度チップからの追記・全パイロット一覧表（7474/SRC#1651）', async ({ page }) => {
  await open(page);
  // メッセージウィンドウの整形・区切り・改行ロジック
  const mwCheck = await page.evaluate(async base => {
    const m = await import(base + 'message-window.js');
    return {
      stripped: m.stripMessageTags('<B>太字</B> <LT>a<GT>'),
      formatted: m.formatSrcMessageText('ああ――いいーーうう'),
      pages: m.splitMessagePages('行くぞ！$(武器).覚悟しろ;$(相手)め', { '$(武器)': '灯銃', '$(相手)': '敵' }),
      lines: m.layoutMessageLines('行くぞ！灯銃.覚悟しろ', true),
    };
  }, base);
  expect(mwCheck.stripped).toBe('太字 <a>');
  expect(mwCheck.formatted).toBe('ああ──いい──うう');
  expect(mwCheck.pages.map(p => p.text)).toEqual(['行くぞ！灯銃.覚悟しろ', '敵め']);
  expect(mwCheck.lines).toEqual(['「行くぞ！灯銃', '  覚悟しろ」']);

  await page.locator('#scenario-input').setInputFiles(scenarioFolder(['pilot_message.txt']));
  await page.locator('#tab-data').click();
  await page.locator('#data-files').selectOption('Data/team/pilot_message.txt');

  // ソラの最初のセリフがメッセージウィンドウに表示される
  await expect(page.locator('.dp-list li').first()).toHaveText('ソラ');
  await expect(page.locator('.dp-msg-window-section .mw-speaker')).toHaveText('【主人公愛称】');
  await expect(page.locator('.dp-msg-window-section .mw-lines')).toContainText('いくぞ、ホタル！');

  // 足りないシチュエーションのチップ（例: 発進）を押すと末尾へ追記され、保存できる
  const missingChip = page.locator('.dp-cov-chip.missing').first();
  const missingLabel = await missingChip.locator('span').first().textContent();
  await missingChip.click();
  await expect(page.locator('.dp-msg-table')).toContainText(missingLabel);
  await expect(page.locator('#save-data')).toBeEnabled();

  // 一覧表タブで全パイロットの表と未記入パイロットが見える
  await page.locator('.dp-msg-tabs button').nth(1).click();
  await expect(page.locator('.dp-msg-matrix')).toContainText('ソラ');
  await expect(page.locator('.dp-missing-pilots')).toContainText('教官ドーラ');
});

test('素材の一覧: シナリオが参照する素材の不足・参照場所・README 用一覧を表示できる（7474/SRC#1654）', async ({ page }) => {
  await open(page, 'assets');
  const scenario = scenarioFolder();
  // harbor.eve は Lantern_Deck.bmp や Midi 等を参照しており、コピーした最小フォルダには画像・音が同梱されていない
  await page.locator('#scenario-input').setInputFiles(scenario);
  await expect(page.locator('#assets-view')).toBeVisible();
  await expect(page.locator('#assets-view .assets-table')).toBeVisible();
  // 見つからない素材・参照ファイルと行が出る
  await expect(page.locator('#assets-view .assets-chip[data-filter="missing"]')).toContainText('見つからない');
  await expect(page.locator('#assets-view .assets-table')).toContainText('harbor.eve:');
  // 見つからない絞り込み・README 用一覧の下書き
  await page.locator('#assets-view .assets-chip[data-filter="missing"]').click();
  await expect(page.locator('#assets-view .assets-missing').first()).toBeVisible();
  await page.locator('#assets-readme-btn').click();
  await expect(page.locator('#assets-readme-text')).toHaveValue(/## 必要な素材/);
});

test('マップの自動接続: 描いた道・川・橋・海を自動でつなぎ、つなぎ直しを元に戻せる（7474/SRC#1663）', async ({ page }) => {
  await open(page);
  page.once('dialog', d => d.accept('auto.map'));
  await page.locator('#new-map').click();
  await expect(page.locator('.me-canvas')).toBeVisible();

  const autoConnect = page.locator('.me-auto-connect');
  await expect(autoConnect).not.toBeChecked();
  await autoConnect.check();

  // 四角で縦の川 (5,1)-(5,5)
  await page.locator('.me-terrains li', { hasText: '19 川' }).click();
  await page.locator('.me-tools button[data-tool="rect"]').click();
  const box = await page.locator('.me-canvas').boundingBox();
  await page.mouse.move(box.x + 4.5 * 32, box.y + 0.5 * 32);
  await page.mouse.down();
  await page.mouse.move(box.x + 4.5 * 32, box.y + 4.5 * 32, { steps: 4 });
  await page.mouse.up();
  await expect(page.locator('.me-undo')).toBeEnabled();

  // 四角で横の道 (3,3)-(4,3)、描くで (3,4)・(6,3)、橋を (5,3)
  await page.locator('.me-terrains li', { hasText: '2 道路' }).click();
  await page.mouse.move(box.x + 2.5 * 32, box.y + 2.5 * 32);
  await page.mouse.down();
  await page.mouse.move(box.x + 3.5 * 32, box.y + 2.5 * 32, { steps: 2 });
  await page.mouse.up();

  await page.locator('.me-tools button[data-tool="pen"]').click();
  await clickCell(page, 3, 4);
  await clickCell(page, 6, 3);
  await page.locator('.me-terrains li', { hasText: '25 橋' }).click();
  await clickCell(page, 5, 3);

  // 四角で 3×3 の海 (10,2)-(12,4)
  await page.locator('.me-terrains li', { hasText: '17 海' }).click();
  await page.locator('.me-tools button[data-tool="rect"]').click();
  await page.mouse.move(box.x + 9.5 * 32, box.y + 1.5 * 32);
  await page.mouse.down();
  await page.mouse.move(box.x + 11.5 * 32, box.y + 3.5 * 32, { steps: 4 });
  await page.mouse.up();

  await expect.poll(async () => tileAt((await downloaded(page)).text, 11, 2)).toBe('17,3');
  let { text } = await downloaded(page);
  expect(tileAt(text, 3, 3)).toBe('2,5');
  expect(tileAt(text, 5, 3)).toBe('25,501');
  expect(tileAt(text, 5, 2)).toBe('19,11');
  expect(tileAt(text, 10, 2)).toBe('17,35');
  expect(tileAt(text, 11, 2)).toBe('17,3');
  expect(tileAt(text, 12, 3)).toBe('17,4');

  // 自動接続を切って 2 番（道路）を描いたあと、「つなぎ直し」で接続し「元に戻す」で戻す
  await autoConnect.uncheck();
  await page.locator('.me-terrains li', { hasText: '2 道路' }).click();
  await page.mouse.move(box.x + 14.5 * 32, box.y + 9.5 * 32);
  await page.mouse.down();
  await page.mouse.move(box.x + 16.5 * 32, box.y + 9.5 * 32, { steps: 2 });
  await page.mouse.up();
  await page.locator('.me-tools button[data-tool="pen"]').click();
  await clickCell(page, 15, 11);
  await expect.poll(async () => tileAt((await downloaded(page)).text, 15, 10)).toBe('2,0');

  await page.locator('.me-reconnect').click();
  await expect.poll(async () => tileAt((await downloaded(page)).text, 15, 10)).toBe('2,5');

  await page.locator('.me-undo').click();
  await expect.poll(async () => tileAt((await downloaded(page)).text, 15, 10)).toBe('2,0');
});



