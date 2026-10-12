// @ts-check
const { test, expect } = require('@playwright/test');
const fs = require('node:fs');
const path = require('node:path');

const root = path.resolve(__dirname, '..', process.env.SRCWEB_ROOT ?? '../SRCSharpWeb/bin/web/wwwroot');

/** 初回は coi-serviceworker を有効にするために再読み込みされる。シナリオの一覧が出るまで待つ */
async function open(page) {
  page.on('pageerror', e => console.log('pageerror:', e.message));
  await page.goto('/');
  await page.waitForSelector('#catalog button', { timeout: 180 * 1000 });
  expect(await page.evaluate(() => self.crossOriginIsolated)).toBe(true);
}

/** canvas へ書き込んだ回数 */
function frames(page) {
  return page.evaluate(() => globalThis.srcwebFrames ?? 0);
}

/** 実行中のイベントの行など（WebInterop.DebugState） */
function debugState(page) {
  return page.evaluate(async () => {
    const runtime = await globalThis.getDotnetRuntime(0);
    const exports = await runtime.getAssemblyExports('SRCSharpWeb.dll');
    return exports.SRCSharpWeb.WebInterop.DebugState();
  });
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
    buffer: fs.readFileSync(path.join(root, 'scenarios', 'battle-victory.zip')),
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
