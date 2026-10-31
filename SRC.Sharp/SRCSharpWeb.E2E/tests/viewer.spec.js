// @ts-check
// データビューア（SRCTestBlazor）。置いたサイトのアーカイブ（viewer-fixtures.mjs が作る）を読み、データ・画像・音と、
// データの BGM を鳴らす帯（画面を移っても鳴り続ける）を確かめる（#1361、docs/gui/dataviewer.md）。
// CI の test-viewer は、データビューアの発行物を配信のルートに置いて動かす。配信されていなければ飛ばす（VIEWER_REQUIRED なら落とす）
const { test, expect } = require('@playwright/test');

const archive = id => `archive?id=${encodeURIComponent(id)}`;

// 上のメニュー（Bulma の navbar）がたたまれない幅にする
test.use({ viewport: { width: 1280, height: 900 } });

test.beforeEach(async ({ page }) => {
  const res = await page.goto('viewer.json');
  const served = !!res && res.status() === 200 && (await res.text()).includes('archiveIndexes');
  if (process.env.VIEWER_REQUIRED) {
    expect(served, 'データビューアとアーカイブが配信されていない（viewer-fixtures.mjs）').toBe(true);
  }
  test.skip(!served, 'データビューアとアーカイブが配信されていない');
});

test('アーカイブの一覧をメニューから開き、置いたサイトのシナリオとパックが並ぶ', async ({ page }) => {
  await page.goto('./');
  await page.locator('a.navbar-item[href="archives"]').click();
  await expect(page.getByText('E2E のシナリオ').first()).toBeVisible({ timeout: 60 * 1000 });
  await expect(page.getByText('E2E の素材').first()).toBeVisible();
});

test('シナリオのデータを、重ねたパックの画像と一緒に見る', async ({ page }) => {
  await page.goto(archive('scenarios/test'));
  await expect(page.getByText('テスト機').first()).toBeVisible({ timeout: 60 * 1000 });
    await expect(page.locator('img[src*="Bitmap/Unit/"]').first()).toBeVisible();
  // パイロットの顔の絵はシナリオに無く、重ねたパック（packs/assets）の Bitmap/Pilot から読む
  await page.getByText('Pilot', { exact: true }).click();
  await expect(page.locator('img[src*="packs/assets/Bitmap/Pilot/"]').first()).toBeVisible();
});

test('データの BGM を鳴らし、画面を移っても鳴り続け、止められる', async ({ page }) => {
  test.skip(!process.env.SRC_SOUNDFONT_ARCHIVE, 'SoundFont のパックが無い（viewer-fixtures.mjs に .sf2 を渡す）');
  await page.goto(archive('scenarios/test'));
  // ユニットの特殊能力「ＢＧＭ」（unit_bgm.mid）のボタン。重ねたパックの Midi にあるので鳴らせる
  const bgm = page.getByRole('button', { name: 'unit_bgm.mid' });
  await expect(bgm.first()).toBeVisible({ timeout: 60 * 1000 });
  await bgm.first().click();
  const bar = page.locator('.src-bgm-bar');
  await expect(bar).toBeVisible({ timeout: 60 * 1000 });
  await expect(bar).toContainText('unit_bgm.mid');
  // 別の画面（アーカイブの一覧）へ移っても、帯は残る
  await page.locator('a.navbar-item[href="archives"]').click();
  await expect(page.getByText('E2E の素材').first()).toBeVisible();
  await expect(bar).toBeVisible();
  await bar.getByRole('button', { name: '止める' }).click();
  await expect(bar).toBeHidden();
});

test('画像のタブで、データで使っていない絵だけに絞れる', async ({ page }) => {
  await page.goto(archive('scenarios/test'));
  await page.locator('.tabs').getByText('画像').click();
  // 画像は 1 つずつ figure に並ぶ（title はアーカイブの中の道）
  const image = name => page.locator(`figure.src-archive-image[title$="${name}"]`);
  await expect(image('使っていない絵.bmp')).toHaveCount(1, { timeout: 60 * 1000 });
  await expect(image('テスト機.bmp')).toHaveCount(1);
  // データ（robot.txt の「テスト機.bmp」）で使っている絵にだけ印が付く
  await expect(image('テスト機.bmp')).toContainText('データで使用');
  await expect(image('使っていない絵.bmp')).not.toContainText('データで使用');
  await page.getByText('データで使っていないものだけ').click();
  await expect(image('使っていない絵.bmp')).toHaveCount(1);
  await expect(image('テスト機.bmp')).toHaveCount(0);
});

test('音のタブで、効果音と MIDI が並び、MIDI を合成して鳴らせる', async ({ page }) => {
  await page.goto(archive('packs/assets'));
  await page.locator('.tabs').getByText('音').click();
  await expect(page.getByText('se.wav')).toBeVisible({ timeout: 60 * 1000 });
  await expect(page.locator('audio').first()).toBeAttached();
  test.skip(!process.env.SRC_SOUNDFONT_ARCHIVE, 'SoundFont のパックが無い');
  await page.getByRole('button', { name: '合成して鳴らす' }).first().click();
  await expect(page.locator('.src-bgm-bar')).toBeVisible({ timeout: 60 * 1000 });
});
