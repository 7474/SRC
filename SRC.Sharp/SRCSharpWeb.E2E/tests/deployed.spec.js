// @ts-check
// 配信したデータビューア（Static Web Apps）を開き、7474/SRC-Data のデータを読めることを確かめる。
// CD-swa-dataviewer.yml の e2e が、配信した先の URL を DEPLOYED_URL に渡して動かす。マージの可否には使わない（docs/testing.md）。
// 配信の前のデータビューアは viewer.spec.js（CI の test-viewer）が確かめる。ここは配信した環境（ルーティング・配信の設定）と、
// GitHub に置いたデータを URL で読む経路（/titles/）を受け持つ
const { test, expect } = require('@playwright/test');

test.skip(!process.env.DEPLOYED_URL, 'DEPLOYED_URL が無い（配信したサイトを確かめるときだけ動かす）');

// 上のメニュー（Bulma の navbar）がたたまれない幅にする
test.use({ viewport: { width: 1280, height: 900 } });

test('データビューアが開き、フォントとメニューが読み込まれる', async ({ page }) => {
  await page.goto('./');
  await expect(page.locator('input[type="file"]')).toBeAttached({ timeout: 60 * 1000 });
  await expect.poll(() => page.evaluate(() => document.fonts.status), { timeout: 60 * 1000 }).toBe('loaded');
  await expect(page.locator('.navbar .navbar-menu .fa-folder').first()).toBeAttached();
});

test('GitHub（7474/SRC-Data）に置いたデータを URL で読み、ユニット・パイロット・アイテムを見る', async ({ page }) => {
  const dir = 'https://raw.githubusercontent.com/7474/SRC-Data/master/GSCデータパック/ロボ/∀ガンダム';
  const files = ['alias.txt', 'animation.txt', 'item.txt', 'non_pilot.txt', 'pilot.txt', 'pilot_dialog.txt', 'pilot_message.txt', 'robot.txt', '更新履歴.txt', '機体解説.txt'];
  await page.goto(`titles/${encodeURIComponent(dir)}/${files.map(encodeURIComponent).join('&')}`);
  await expect(page.getByText('∀ガンダム(前期)').first()).toBeVisible({ timeout: 60 * 1000 });
  await page.getByText('Pilot', { exact: true }).click();
  await expect(page.getByText('ロラン＝セアック').first()).toBeVisible();
  await page.getByText('Item', { exact: true }).click();
  await expect(page.getByText('∀ガンダム用核弾頭').first()).toBeVisible();
});
