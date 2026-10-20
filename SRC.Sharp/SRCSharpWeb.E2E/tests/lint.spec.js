// @ts-check
// ブラウザで動く lint（SRCLintWeb）。CI は発行物を配信のルートの /lint/ に置く（.github/workflows/CI.yml の test-web）
const { test, expect } = require('@playwright/test');
const { makeZip } = require('./zip');

const lintBase = process.env.SRCLINT_BASE ?? '/lint/';
const file = (name, text) => ({ name, data: Buffer.from(text.split('\n').join('\r\n'), 'utf8') });

const pilot = (sp) => file('シナリオ/Data/pilot.txt', [
  'テスト', 'テスト, てすと, 男性, 汎用, AAAA, 100', '特殊能力なし', '100, 100, 100, 100, 100, 100, 普通', `ＳＰ, 30, ${sp}, 1`, '-.bmp, -.mid', '',
].join('\n'));
const system = makeZip([
  file('Data/System/sp.txt', ['集中, しゅうちゅう', '集, 15, 自分, ターン, -, -, 集中', '命中強化Lv3 回避強化Lv3', '１ターンの間、命中＆回避+30', ''].join('\n')),
  // 汎用戦闘アニメ（本体の Lib）のラベル。静的解析はここにあるラベルも見る
  file('Lib/汎用戦闘アニメ/include.eve', 'AttackDemo:\nReturn\n'),
  // 汎用データの素材（中身は検査しない）
  { name: 'Bitmap/Event/共通の背景.bmp', data: Buffer.from('BM') },
]);

async function lint(page, zip, options = {}) {
  return page.evaluate(async ([base, zipB64, systemB64, options]) => {
    const bytes = (b64) => new Blob([Uint8Array.from(atob(b64), c => c.charCodeAt(0))]);
    const { lintZip } = await import(base + 'srclint.js');
    return lintZip(bytes(zipB64), { ...options, systemArchives: [bytes(systemB64)] });
  }, [lintBase, zip.toString('base64'), system.toString('base64'), options]);
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

test('静的解析: Call の飛び先のラベルと include するファイルが無ければ知らせる（汎用データの Lib のラベルは見つかる）', async ({ page }) => {
  const zip = makeZip([
    file('シナリオ/test.eve', '<部品.eve>\n<無いファイル.eve>\nスタート:\nCall AttackDemo\nCall 部品のラベル\nCall 無いラベル\nExit\n\n呼び出し元のラベル:\nReturn\n'),
    // include される側は単独では解析しない（呼び出し元のラベルを「見つからない」としないため）
    file('シナリオ/部品.eve', '部品のラベル:\nCall 呼び出し元のラベル\nReturn\n'),
  ]);
  const result = await lint(page, zip);
  const messages = result.diagnostics.map(d => `${d.file}:${d.line} ${d.message}`);
  expect(messages).toContainEqual(expect.stringMatching(/^シナリオ\/test\.eve:\d+ ラベル「無いラベル」が見つかりません/));
  expect(messages).toContainEqual(expect.stringMatching(/^シナリオ\/test\.eve:2 include するファイル「無いファイル\.eve」が見つかりません/));
  expect(messages.filter(m => /AttackDemo|部品のラベル|呼び出し元のラベル/.test(m))).toEqual([]);
});

test('素材の参照: シナリオと汎用データの zip に無い画像・音を info で知らせる（assets を指定したときだけ）', async ({ page }) => {
  const zip = makeZip([
    file('シナリオ/test.eve', 'スタート:\nPaintPicture Event\\共通の背景.bmp 0 0\nPaintPicture 同梱.png 0 0\nPaintPicture 無い画像.bmp 0 0\nStartBGM 無い曲.mid\nPaintPicture $(画像) 0 0\nExit\n'),
    { name: 'シナリオ/Bitmap/同梱.png', data: Buffer.from('PNG') },
    pilot('集中'),
  ]);
  const result = await lint(page, zip, { assets: true });
  const messages = result.diagnostics.map(d => `${d.severity} ${d.file}:${d.line} ${d.message}`);
  expect(messages).toEqual([
    'info シナリオ/test.eve:4 素材が見つかりません: 無い画像.bmp',
    'info シナリオ/test.eve:5 素材が見つかりません: 無い曲.mid',
  ]);
  expect((await lint(page, zip)).diagnostics).toEqual([]);
});
