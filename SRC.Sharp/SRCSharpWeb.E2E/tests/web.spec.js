// @ts-check
const { test, expect } = require('@playwright/test');
const fs = require('node:fs');
const path = require('node:path');
const { makeZip } = require('./zip');

const root = path.resolve(__dirname, '..', process.env.SRCWEB_ROOT ?? '../SRCSharpWeb/bin/web/wwwroot');
// SRC#Web を置いたフォルダ（サブパスに置く場合はサイトのルートの下）
const webRoot = path.join(root, process.env.SRCWEB_BASE ?? '/');

/** 初回は coi-serviceworker を有効にするために再読み込みされる。シナリオの一覧が出るまで待つ */
async function open(page) {
  page.on('pageerror', e => console.log('pageerror:', e.message));
  page.on('console', m => {
    if (m.type() === 'error') {
      console.log('console.error:', m.text());
    }
  });
  // baseURL（サブパスを含む）からの相対で開く
  await page.goto('./');
  await page.waitForSelector('#catalog button', { timeout: 180 * 1000 });
  expect(await page.evaluate(() => self.crossOriginIsolated)).toBe(true);
}

/** canvas へ書き込んだ回数 */
function frames(page) {
  return page.evaluate(() => globalThis.srcwebFrames ?? 0);
}

/** 画面とイベントの状態（WebInterop.DebugState の JSON 文字列） */
function debugState(page) {
  return page.evaluate(async () => {
    const runtime = await globalThis.getDotnetRuntime(0);
    const exports = await runtime.getAssemblyExports('SRCSharpWeb.dll');
    return exports.SRCSharpWeb.WebInterop.DebugState();
  });
}

async function screenState(page) {
  return JSON.parse(await debugState(page));
}

/** リストが出るまでメッセージを送り、項目を矢印キーで選んで決定する */
async function chooseListItem(page, caption, item) {
  await page.locator('#screen').focus();
  await expect.poll(async () => {
    const s = await screenState(page);
    if (s.list && (s.list.caption.includes(caption) || (s.list.info ?? '').includes(caption))) {
      return true;
    }
    if (s.message !== undefined) {
      await page.keyboard.press('Enter');
    }
    return false;
  }, { intervals: [500, 1000] }).toBe(true);
  await expect.poll(async () => {
    const s = await screenState(page);
    const target = s.list.items.findIndex(x => x.includes(item));
    if (target < 0) {
      throw new Error(`リストに「${item}」がありません: ${JSON.stringify(s.list)}`);
    }
    if (s.list.highlight === target) {
      return true;
    }
    await page.keyboard.press(s.list.highlight < target ? 'ArrowDown' : 'ArrowUp');
    return false;
  }, { intervals: [200, 500] }).toBe(true);
  await page.keyboard.press('Enter');
}

async function attachScreen(page, testInfo, name) {
  await testInfo.attach(name, { body: await page.locator('#screen').screenshot(), contentType: 'image/png' });
}

/** Enter でメッセージを送り、イベントが先へ進むことを確かめる */
async function expectAdvancesWithEnter(page) {
  await expect.poll(() => frames(page)).toBeGreaterThan(0);
  const before = await debugState(page);
  await page.locator('#screen').focus();
  await expect.poll(async () => {
    await page.keyboard.press('Enter');
    return debugState(page);
  }, { intervals: [500, 1000, 2000] }).not.toBe(before);
}

/** プレイ中に画面の右下のシステムメニューからシナリオ選択へ戻る（確認に「はい」と答える） */
async function backToMenu(page) {
  await page.locator('#system-menu-button').click();
  await page.getByRole('menuitem', { name: 'シナリオ選択へ戻る…' }).click();
  await page.getByRole('alertdialog').getByRole('button', { name: 'はい' }).click();
}

test('登録済シナリオを選んでプレイする', async ({ page }, testInfo) => {
  await open(page);
  await testInfo.attach('menu', { body: await page.screenshot({ fullPage: true }), contentType: 'image/png' });
  await page.getByRole('button', { name: 'battle-basic' }).click();
  await expect(page.locator('#screen')).toBeVisible();
  await expect.poll(() => debugState(page)).toContain('test.eve');
  await expectAdvancesWithEnter(page);
  await expect(page).toHaveTitle(/SRC#Web/);
  await attachScreen(page, testInfo, 'battle-basic');
});

test('アーカイブを追加し、再読み込み後もプレイできる', async ({ page }, testInfo) => {
  const name = 'テスト追加.zip';
  await open(page);
  await page.setInputFiles('#add-archive', {
    name,
    mimeType: 'application/zip',
    buffer: fs.readFileSync(path.join(webRoot, 'scenarios', 'battle-victory.zip')),
  });
  await expect(page.locator('#archives li')).toContainText(name);

  // ブラウザ（IndexedDB）に残る
  await page.reload();
  await page.waitForSelector('#catalog button', { timeout: 180 * 1000 });
  await expect(page.locator('#archives li')).toContainText(name);

  await page.locator('#archives li', { hasText: name }).getByRole('button', { name }).click();
  await expect(page.locator('#screen')).toBeVisible();
  await expect.poll(() => debugState(page)).toContain('archive-' + name);
  await expectAdvancesWithEnter(page);
  await attachScreen(page, testInfo, 'archive');

  // 削除する
  await backToMenu(page);
  await page.waitForSelector('#catalog button', { timeout: 180 * 1000 });
  await page.locator('#archives li', { hasText: name }).getByRole('button', { name: '削除' }).click();
  await expect(page.locator('#archives li')).toHaveCount(0);
});

test('インターミッションでセーブし、ブラウザに残ったセーブデータから再開する', async ({ page }, testInfo) => {
  const scenario = 'catalog-intermission-save';
  await open(page);
  await page.evaluate(id => window.srcwebStore.listSaves(id)
    .then(saves => Promise.all(saves.map(x => window.srcwebStore.removeSave(id, x.name)))), scenario);
  await page.getByRole('button', { name: 'intermission-save' }).click();
  await expect(page.locator('#screen')).toBeVisible();

  // データセーブ → ファイル名の入力 → OK
  await chooseListItem(page, 'コマンド', 'データセーブ');
  await expect(page.locator('#modal-input')).toBeVisible();
  await attachScreen(page, testInfo, 'save-input');
  await page.locator('#modal-input').press('Enter');

  // ゲームが書いたセーブデータがブラウザに保存される（ステージ開始時のリスタート用の .srcq も保存される）
  const saveNames = () => page.evaluate(id => window.srcwebStore.listSaves(id).then(x => x.map(s => s.name)), scenario);
  await expect.poll(async () => (await saveNames()).filter(x => x.endsWith('.srcs'))).toHaveLength(1);
  const saveName = (await saveNames()).find(x => x.endsWith('.srcs'));

  // 読み込み直して、セーブデータから再開する（インターミッションに戻る）
  await backToMenu(page);
  await page.waitForSelector('#catalog button', { timeout: 180 * 1000 });
  await page.getByRole('button', { name: 'intermission-save' }).click();
  await page.locator('#saves li', { hasText: saveName }).getByRole('button', { name: saveName }).click();
  await expect(page.locator('#screen')).toBeVisible();
  await expect.poll(async () => (await screenState(page)).list?.caption ?? '').toContain('コマンド');
  await attachScreen(page, testInfo, 'resumed');

  // 次のステージへ進んで終わる
  await chooseListItem(page, 'コマンド', '次のステージへ');
  await expect.poll(async () => (await screenState(page)).dialog ?? '').toContain('次のステージへ進みますか');
  await page.keyboard.press('Enter');
  await expect.poll(async () => {
    const s = await screenState(page);
    if (s.message !== undefined) {
      await page.keyboard.press('Enter');
    }
    return s.terminated;
  }, { intervals: [500, 1000] }).toBe(true);
});

const corpus = path.resolve(__dirname, '../../E2E');
const soundFont = process.env.SRC_SOUNDFONT ?? '/usr/share/sounds/sf2/TimGM6mb.sf2';

function soundCounts(page) {
  return page.evaluate(() => globalThis.srcwebSound);
}

test('効果音（WAV）を鳴らす。SoundFont が無ければ MIDI は鳴らさずに進む', async ({ page }) => {
  await open(page);
  await page.getByRole('button', { name: 'sound', exact: true }).click();
  await expect(page.locator('#screen')).toBeVisible();
  // ブラウザは操作があるまで音を鳴らさない
  await page.locator('#screen').click({ position: { x: 600, y: 300 } });
  await expect.poll(async () => (await soundCounts(page)).encoded).toBeGreaterThan(0);
  await expectAdvancesWithEnter(page);
  expect((await soundCounts(page)).pcm).toBe(0);
});

test('アーカイブの SoundFont で MIDI を合成して鳴らす', async ({ page }) => {
  test.skip(!fs.existsSync(soundFont), `SoundFont が無い: ${soundFont}`);
  const files = ['test.eve', 'test.map', 'Sound/se.wav', 'Midi/bgm.mid']
    .map(name => ({ name: 'サウンド/' + name, data: fs.readFileSync(path.join(corpus, 'sound', name)) }));
  files.push({ name: 'サウンド/Midi/test.sf2', data: fs.readFileSync(soundFont) });
  const name = 'MIDI のテスト.zip';
  await open(page);
  await page.setInputFiles('#add-archive', { name, mimeType: 'application/zip', buffer: makeZip(files) });
  await page.locator('#archives li', { hasText: name }).getByRole('button', { name }).click();
  await expect(page.locator('#screen')).toBeVisible();
  await page.locator('#screen').click({ position: { x: 600, y: 300 } });
  await expect.poll(async () => (await soundCounts(page)).pcm, { timeout: 120 * 1000 }).toBeGreaterThan(0);
  await backToMenu(page);
  await page.waitForSelector('#catalog button', { timeout: 180 * 1000 });
  await page.locator('#archives li', { hasText: name }).getByRole('button', { name: '削除' }).click();
});

// ── 外部ホスティング（docs/gui/web-hosting.md） ──

/**
 * page.route は Service Worker を通る要求を捕まえられないため、Service Worker（coi-serviceworker）を止め、
 * クロスオリジン分離のヘッダは応答に付ける。個別の route はこの後に登録する（後のものが優先される）
 */
async function isolateWithHeaders(page) {
  await page.route('**/*', async route => {
    const response = await route.fetch();
    await route.fulfill({
      response,
      headers: {
        ...response.headers(),
        'cross-origin-opener-policy': 'same-origin',
        'cross-origin-embedder-policy': 'require-corp',
      },
    });
  });
}

test('URL の play で登録済シナリオを直接始める', async ({ page }) => {
  page.on('pageerror', e => console.log('pageerror:', e.message));
  await page.goto('./?play=battle-basic');
  await expect(page.locator('#screen')).toBeVisible({ timeout: 180 * 1000 });
  await expect.poll(() => debugState(page)).toContain('catalog-battle-basic');
  await expectAdvancesWithEnter(page);
});

test.describe('サイトの設定とカタログ', () => {
test.use({ serviceWorkers: 'block' });

test('サイトの設定とカタログで配信し、シナリオが求める素材を重ねる', async ({ page }) => {
  // シナリオには効果音を入れず、素材（汎用データ）の zip に入れる
  const scenario = makeZip(['test.eve', 'test.map', 'Midi/bgm.mid']
    .map(name => ({ name: 'シナリオ/' + name, data: fs.readFileSync(path.join(corpus, 'sound', name)) })));
  const asset = makeZip([{ name: '汎用データ/Sound/se.wav', data: fs.readFileSync(path.join(corpus, 'sound', 'Sound/se.wav')) }]);
  const json = body => ({ status: 200, contentType: 'application/json', body: JSON.stringify(body) });
  await isolateWithHeaders(page);
  await page.route('**/srcweb.json', route => route.fulfill(json({
    title: 'テストの配信',
    catalogs: ['dist/catalog.json'],
    allowUserArchives: false,
    allowUserCatalogs: false,
  })));
  await page.route('**/dist/catalog.json', route => route.fulfill(json({
    format: 1,
    name: 'テストの配信元',
    entries: [
      { id: 'sound-with-asset', title: '素材を使うシナリオ', archive: 'scenario.zip', root: 'シナリオ', eve: 'test.eve', requires: ['common'] },
      { id: 'common', kind: 'asset', title: '汎用データ', archive: 'assets/common.zip', root: '汎用データ' },
    ],
  })));
  await page.route('**/dist/scenario.zip', route => route.fulfill({ status: 200, contentType: 'application/zip', body: scenario }));
  await page.route('**/dist/assets/common.zip', route => route.fulfill({ status: 200, contentType: 'application/zip', body: asset }));

  await open(page);
  await expect(page).toHaveTitle('テストの配信');
  await expect(page.locator('h2').first()).toHaveText('テストの配信元');
  // 素材は一覧に出さない。利用者の追加は許していない
  await expect(page.locator('#catalog button')).toHaveText(['素材を使うシナリオ']);
  await expect(page.locator('#add-archive')).toHaveCount(0);
  await expect(page.locator('#catalog-url')).toHaveCount(0);

  await page.getByRole('button', { name: '素材を使うシナリオ' }).click();
  await expect(page.locator('#screen')).toBeVisible();
  await page.locator('#screen').click({ position: { x: 600, y: 300 } });
  // 素材の効果音が鳴る
  await expect.poll(async () => (await soundCounts(page)).encoded).toBeGreaterThan(0);
});

test('URL の archive で任意の zip を直接始める', async ({ page }) => {
  const zip = fs.readFileSync(path.join(webRoot, 'scenarios', 'battle-victory.zip'));
  await isolateWithHeaders(page);
  await page.route('**/external/victory.zip', route => route.fulfill({ status: 200, contentType: 'application/zip', body: zip }));
  page.on('pageerror', e => console.log('pageerror:', e.message));
  await page.goto('./?archive=' + encodeURIComponent('external/victory.zip') + '&eve=test.eve');
  await expect(page.locator('#screen')).toBeVisible({ timeout: 180 * 1000 });
  await expect.poll(() => debugState(page)).toContain('url-');
  await expectAdvancesWithEnter(page);
});
});

test('システムメニューから戻るときは確認し、「いいえ」ならプレイを続ける', async ({ page }, testInfo) => {
  await open(page);
  await page.getByRole('button', { name: 'battle-basic' }).click();
  await expect(page.locator('#screen')).toBeVisible();
  // プレイ中に目立つ「戻る」ボタンは無い
  await expect(page.getByRole('button', { name: 'シナリオ選択へ戻る' })).toHaveCount(0);
  await page.locator('#system-menu-button').click();
  await testInfo.attach('system-menu', { body: await page.screenshot(), contentType: 'image/png' });
  await page.getByRole('menuitem', { name: 'シナリオ選択へ戻る…' }).click();
  await expect(page.getByRole('alertdialog')).toBeVisible();
  await testInfo.attach('confirm', { body: await page.screenshot(), contentType: 'image/png' });
  await page.getByRole('alertdialog').getByRole('button', { name: 'いいえ' }).click();
  await expect(page.getByRole('alertdialog')).toHaveCount(0);
  await expect(page.locator('#screen')).toBeVisible();
  await expectAdvancesWithEnter(page);
  // メニューの外を押すと閉じる
  await page.locator('#system-menu-button').click();
  await expect(page.getByRole('menu')).toBeVisible();
  await page.mouse.click(10, 10);
  await expect(page.getByRole('menu')).toHaveCount(0);
});
