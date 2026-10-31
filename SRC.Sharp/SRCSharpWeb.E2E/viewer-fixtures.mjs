// データビューアの E2E（tests/viewer.spec.js）が読む、置いたサイトのアーカイブ（docs/gui/dataviewer.md）を作る（#1361）。
// 使い方: node viewer-fixtures.mjs <データビューアを置いたフォルダ> [SoundFont の .sf2]
// - viewer.json: アーカイブの一覧の URL
// - e2e/index.json: シナリオ 1 つ・素材のパック 1 つ・SoundFont のパック 1 つ（.sf2 を渡したとき）
// - e2e/<アーカイブ>/files.json と中のファイル
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const [root, soundFont] = process.argv.slice(2);
if (!root) {
  throw new Error('使い方: node viewer-fixtures.mjs <root> [soundfont.sf2]');
}
const here = path.dirname(fileURLToPath(import.meta.url));
const corpus = path.join(here, '..', 'E2E');
const crlf = lines => Buffer.from(lines.join('\r\n') + '\r\n', 'utf8');

// 24 bit の BMP（単純な図形。VRT・E2E 用）
function bmp(w, h, pixel) {
  const row = Math.ceil(w * 3 / 4) * 4;
  const data = Buffer.alloc(row * h);
  for (let y = 0; y < h; y++) {
    for (let x = 0; x < w; x++) {
      const [r, g, b] = pixel(x, h - 1 - y);
      data.writeUInt8(b, y * row + x * 3);
      data.writeUInt8(g, y * row + x * 3 + 1);
      data.writeUInt8(r, y * row + x * 3 + 2);
    }
  }
  const header = Buffer.alloc(54);
  header.write('BM', 0);
  header.writeUInt32LE(54 + data.length, 2);
  header.writeUInt32LE(54, 10);
  header.writeUInt32LE(40, 14);
  header.writeInt32LE(w, 18);
  header.writeInt32LE(h, 22);
  header.writeUInt16LE(1, 26);
  header.writeUInt16LE(24, 28);
  header.writeUInt32LE(data.length, 34);
  return Buffer.concat([header, data]);
}

const archives = {
  // シナリオ: データだけを持ち、画像・音は素材のパックから重ねる
  'scenarios/test': {
    kind: 'scenario', title: 'E2E のシナリオ', root: 'test', requires: ['packs/assets'],
    files: {
      'test/Data/robot.txt': crlf([
        'テスト機', 'テスト機, てすとき, 汎用, 1, 2', '陸, 4, M, 1000, 100', '特殊能力', 'ＢＧＭ=unit_bgm.mid',
        '4000, 120, 1200, 90', 'AAAA, テスト機.bmp',
        'テストビーム, 1800, 1, 3, +0, -, 10, -, AAAA, +10, Ｂ', '===',
      ]),
      'test/Data/pilot.txt': crlf([
        'テストパイロット', 'テスト, てすと, 男性, 汎用, AAAA, 100', '特殊能力なし',
        '180, 170, 180, 160, 170, 150, 強気', 'ＳＰ, 50, 必中, 1', 'テスト.bmp, pilot_bgm.mid',
      ]),
      // 画像のタブはそのアーカイブの中の画像を並べる。データで使う絵と使っていない絵を 1 つずつ
      'test/Bitmap/Unit/テスト機.bmp': bmp(32, 32, (x, y) => (x >= 8 && x < 24 && y >= 4 && y < 28 ? [40, 60, 160] : [255, 255, 255])),
      'test/Bitmap/Event/使っていない絵.bmp': bmp(32, 32, (x, y) => ((x + y) % 8 < 4 ? [200, 40, 40] : [250, 250, 250])),
    },
  },
  // 素材のパック: 顔の絵（シナリオに無く、このパックから重ねて読む）、BGM（MIDI）、効果音（WAV）
  'packs/assets': {
    kind: 'pack', title: 'E2E の素材',
    files: {
      'Bitmap/Pilot/テスト.bmp': bmp(64, 64, (x, y) => ((x - 32) ** 2 + (y - 34) ** 2 < 600 ? [240, 200, 170] : [210, 220, 230])),
      'Midi/pilot_bgm.mid': fs.readFileSync(path.join(corpus, 'sound', 'Midi', 'bgm.mid')),
      'Midi/unit_bgm.mid': fs.readFileSync(path.join(corpus, 'sound', 'Midi', 'bgm.mid')),
      'Sound/se.wav': fs.readFileSync(path.join(corpus, 'sound', 'Sound', 'se.wav')),
    },
  },
};
if (soundFont) {
  archives['packs/soundfont'] = { kind: 'pack', title: 'E2E の SoundFont', files: { 'SoundFont/TimGM6mb.sf2': fs.readFileSync(soundFont) } };
}

const list = [];
for (const [id, a] of Object.entries(archives)) {
  const dir = path.join(root, 'e2e', ...id.split('/'));
  for (const [file, data] of Object.entries(a.files)) {
    const p = path.join(dir, ...file.split('/'));
    fs.mkdirSync(path.dirname(p), { recursive: true });
    fs.writeFileSync(p, data);
  }
  const files = Object.entries(a.files).map(([p, data]) => ({ path: p, size: data.length }));
  fs.writeFileSync(path.join(dir, 'files.json'), JSON.stringify({ files }));
  // URL は一覧（e2e/index.json）を基準に解決する。中の道は区切りごとに URL エンコードする
  list.push({
    id, kind: a.kind, title: a.title, page: `/pages/${id}`,
    files: `${id}/files.json`, base: `${id}/`,
    ...(a.root ? { root: a.root } : {}),
    ...(a.requires ? { requires: a.requires } : {}),
  });
}
fs.writeFileSync(path.join(root, 'e2e', 'index.json'), JSON.stringify({
  format: 1, name: 'E2E', ...(soundFont ? { soundFont: 'packs/soundfont' } : {}), archives: list,
}, null, 1));
fs.writeFileSync(path.join(root, 'viewer.json'), JSON.stringify({ archiveIndexes: ['e2e/index.json'] }));
console.log(`データビューアのアーカイブを ${root} に置いた（${list.length} 個）`);
