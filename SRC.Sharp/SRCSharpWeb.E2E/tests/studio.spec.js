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
function scenarioFolder() {
  const dir = fs.mkdtempSync(path.join(os.tmpdir(), 'studio-'));
  const scenario = path.join(dir, 'lantern');
  fs.mkdirSync(path.join(scenario, 'stage'), { recursive: true });
  fs.copyFileSync(path.join(lantern, '01_灯の港.map'), path.join(scenario, 'harbor.map'));
  fs.copyFileSync(path.join(lantern, 'ステージ', '02_霧の渓谷.map'), path.join(scenario, 'stage', 'valley.map'));
  fs.mkdirSync(path.join(scenario, 'Data', 'team'), { recursive: true });
  for (const name of ['unit.txt', 'pilot.txt']) {
    fs.copyFileSync(path.join(lantern, 'Data', 'ランタン', name), path.join(scenario, 'Data', 'team', name));
  }
  return scenario;
}

/** フォルダの読み書き（File System Access API）を使わず、<input webkitdirectory> とダウンロードで動かす */
async function open(page) {
  page.on('pageerror', e => console.log('pageerror:', e.message));
  const res = await page.goto(base + 'index.html?fs=input&lang=ja');
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
  await expect(page.locator('.dp-card .dp-stats')).toContainText('HP');
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

test('比べる: 書いているユニットとパイロットを、データパックの中の上位何 % かで見る（7474/SRC#1644）', async ({ page }) => {
  // 7474/SRC-Data の Pages の評価の JSON の代わり
  const analysis = {
    format: 1,
    files: [{ section: 'ロボ', file: 'pairs-ロボ.json' }, { section: 'Sharp', file: 'pairs-Sharp.json' }],
  };
  const units = Array.from({ length: 10 }, (_, i) => ({ name: `u${i}`, hp: (i + 1) * 1000, en: 100, armor: 1000, mobility: 100, speed: 5, values: { 最大攻撃力: (i + 1) * 300 } }));
  const pilots = Array.from({ length: 4 }, (_, i) => ({ name: `p${i}`, band: '経験値120-159', sp: 50 + i * 10, stats: { 格闘: 120 + i * 20, 射撃: 120, 命中: 120, 回避: 120, 技量: 120, 反応: 120 } }));
  await page.route('https://7474.github.io/SRC-Data/analysis/**', route => {
    const url = route.request().url();
    const body = url.endsWith('index.json') ? analysis : { format: 2, pairs: [], units, pilots };
    return route.fulfill({ status: 200, contentType: 'application/json', headers: { 'access-control-allow-origin': '*' }, body: JSON.stringify(body) });
  });
  await open(page);
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
