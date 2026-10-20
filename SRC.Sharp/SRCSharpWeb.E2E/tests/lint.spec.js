// @ts-check
// ブラウザで動く lint（SRCLintWeb）。CI は発行物を配信のルートの /lint/ に置く（.github/workflows/CI.yml の test-web）
const { test, expect } = require('@playwright/test');
const { makeZip } = require('./zip');

const lintBase = process.env.SRCLINT_BASE ?? '/lint/';
const file = (name, text) => ({ name, data: Buffer.from(text.split('\n').join('\r\n'), 'utf8') });

const pilot = (sp) => file('シナリオ/Data/pilot.txt', [
  'テスト', 'テスト, てすと, 男性, 汎用, AAAA, 100', '特殊能力なし', '100, 100, 100, 100, 100, 100, 普通', `ＳＰ, 30, ${sp}, 1`, '-.bmp, -.mid', '',
].join('\n'));
const system = makeZip([file('Data/System/sp.txt', ['集中, しゅうちゅう', '集, 15, 自分, ターン, -, -, 集中', '命中強化Lv3 回避強化Lv3', '１ターンの間、命中＆回避+30', ''].join('\n'))]);

async function lint(page, zip) {
  return page.evaluate(async ([base, zipB64, systemB64]) => {
    const bytes = (b64) => new Blob([Uint8Array.from(atob(b64), c => c.charCodeAt(0))]);
    const { lintZip } = await import(base + 'srclint.js');
    return lintZip(bytes(zipB64), { systemArchives: [bytes(systemB64)] });
  }, [lintBase, zip.toString('base64'), system.toString('base64')]);
}

test.beforeEach(async ({ page }) => {
  // 配信のルートからの絶対パス（baseURL のサブパスに関わらず同じオリジンの /lint/）
  const res = await page.goto(lintBase + 'srclint.js');
  test.skip(!res || res.status() !== 200, 'SRCLintWeb が配信されていない（SRCLINT_BASE）');
});

test('正しいシナリオは問題なし（汎用データのスペシャルパワーを参照できる）', async ({ page }) => {
  const zip = makeZip([file('シナリオ/test.eve', 'スタート:\nExit\n'), pilot('集中')]);
  const result = await lint(page, zip);
  expect(result.root).toBe('シナリオ');
  expect(result.checkedFiles).toBe(2);
  expect(result.diagnostics).toEqual([]);
});

test('イベントとデータの問題をファイル・行つきで返す', async ({ page }) => {
  const zip = makeZip([file('シナリオ/test.eve', 'スタート:\nIf 1 = 1 Then\nExit\n'), pilot('存在しない')]);
  const result = await lint(page, zip);
  const messages = result.diagnostics.map(d => `${d.file}:${d.line} ${d.message}`);
  expect(messages).toContainEqual(expect.stringMatching(/^シナリオ\/test\.eve:2 .*EndIf/));
  expect(messages).toContainEqual(expect.stringMatching(/^シナリオ\/Data\/pilot\.txt:\d+ 存在しないというスペシャルパワーは存在しません/));
});
