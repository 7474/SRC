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

test('登録済シナリオを選んでプレイする', async ({ page }, testInfo) => {
  await open(page);
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
  await page.getByRole('button', { name: 'シナリオ選択へ戻る' }).first().click();
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
  await page.getByRole('button', { name: 'シナリオ選択へ戻る' }).first().click();
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
  await page.getByRole('button', { name: 'シナリオ選択へ戻る' }).first().click();
  await page.waitForSelector('#catalog button', { timeout: 180 * 1000 });
  await page.locator('#archives li', { hasText: name }).getByRole('button', { name: '削除' }).click();
});
