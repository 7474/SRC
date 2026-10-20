// srcsharp lint（SRCAuthoring の DataLinter）をブラウザで動かす。使い方は README.md
//
//   import { lintZip } from '<置き場所>/srclint.js';
//   const result = await lintZip(file, { systemArchives: ['/files/packs/src-basic/xxxx.zip'] });
//   // result.diagnostics: [{ file, line, severity: 'error' | 'warning', message, source }]
//
// zip の目次を読み、*.eve と *.txt だけを取り出して検査する（画像や音は読まない）。
// systemArchives の zip（URL か Blob）からは汎用データとして Data/System/*.txt（sp.txt など）と Lib/**/*.eve（汎用戦闘アニメなど）を読み込む（検査はしない）。
// 静的解析（Call・Goto の飛び先のラベル、include するファイル）は、汎用データの Lib にあるラベルも見る。
import { dotnet } from './_framework/dotnet.js';

const MAX_TEXT_TOTAL = 128 * 1024 * 1024;

let exportsPromise;
function lintExports() {
  exportsPromise ??= (async () => {
    const { getAssemblyExports, getConfig } = await dotnet.create();
    const exports = await getAssemblyExports(getConfig().mainAssemblyName);
    return exports.SRCLintWeb.LintExports;
  })();
  return exportsPromise;
}

/** 実行環境を先に読み込む（検査の待ちを短くする） */
export function preload() {
  return lintExports();
}

/**
 * zip（Blob・File）を検査する。
 * @param {Blob} zip
 * @param {{ systemArchives?: (string | Blob)[] }} options systemArchives: 汎用データの zip。URL（Range 要求に応えること）か Blob
 * @returns {Promise<{ diagnostics: { file: string, line: number, severity: 'error' | 'warning', message: string, source?: string }[], checkedFiles: number, root: string, systemErrors: string[] }>}
 */
export async function lintZip(zip, options = {}) {
  const lint = await lintExports();
  lint.Reset();
  const reader = blobReader(zip);
  const entries = (await readEntries(reader)).filter(e => !e.directory && /\.(eve|txt)$/i.test(e.name));
  const root = commonRoot(entries.map(e => e.name));
  let total = 0;
  for (const e of entries) {
    total += e.size;
    if (total > MAX_TEXT_TOTAL) {
      throw new Error('テキストのファイルが大きすぎます');
    }
    lint.AddFile('scenario/' + e.name, await extract(reader, e));
  }
  // 汎用データは読めなければ飛ばす（読めなかったものは systemErrors に入れる）
  const systemErrors = [];
  let n = 0;
  for (const archive of options.systemArchives ?? []) {
    n++;
    try {
      const r = typeof archive === 'string' ? await httpReader(archive) : blobReader(archive);
      for (const e of await readEntries(r)) {
        const m = !e.directory && e.name.match(/(?:^|\/)((?:Data\/System\/[^/]+\.txt)|(?:Lib\/.+\.eve))$/i);
        if (m) {
          const data = await extract(r, e);
          // Lib のイベントは、静的解析に要るラベルと include の行だけにする（汎用戦闘アニメは 8 万行あり、読み込みに時間がかかる）
          lint.AddFile(`app/${m[1]}`, /\.eve$/i.test(m[1]) ? labelsOnly(data) : data);
        }
      }
    } catch (err) {
      systemErrors.push(`${typeof archive === 'string' ? archive : `#${n}`}: ${err.message ?? err}`);
    }
  }
  const result = JSON.parse(await lint.LintAsync(root));
  return { ...result, root, systemErrors };
}

const encoder = new TextEncoder();
function labelsOnly(bytes) {
  let text;
  try {
    text = strictUtf8.decode(bytes);
  } catch {
    text = sjis.decode(bytes);
  }
  const kept = text.split(/\r?\n/).filter(line => /^\s*[^#\s].*:\s*$/.test(line) || /^\s*<[^>]+>\s*$/.test(line));
  return encoder.encode(kept.join('\n') + '\n');
}

// すべての名前が同じフォルダの下にあれば、そのフォルダ（SRC#Web と SRC-Site の root と同じ考え方）
function commonRoot(names) {
  const first = names[0]?.split('/');
  if (!first || first.length < 2) {
    return '';
  }
  const top = first[0];
  return names.every(n => n.startsWith(top + '/')) ? top : '';
}

// ---- zip の読み取り（目次とローカルヘッダだけを読み、必要なファイルだけ展開する） ----
// 形式: https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT

function blobReader(blob) {
  return {
    size: blob.size,
    async read(offset, length) {
      return new Uint8Array(await blob.slice(offset, offset + length).arrayBuffer());
    },
  };
}

async function httpReader(url) {
  const head = await fetch(url, { method: 'HEAD' });
  const size = Number(head.headers.get('Content-Length'));
  if (!head.ok || !size) {
    throw new Error(`汎用データを読めません（${url}）`);
  }
  return {
    size,
    async read(offset, length) {
      const res = await fetch(url, { headers: { Range: `bytes=${offset}-${offset + length - 1}` } });
      if (res.status !== 206 && res.status !== 200) {
        throw new Error(`汎用データを読めません（${url}: ${res.status}）`);
      }
      const bytes = new Uint8Array(await res.arrayBuffer());
      return res.status === 206 ? bytes : bytes.subarray(offset, offset + length);
    },
  };
}

const u16 = (b, o) => b[o] | (b[o + 1] << 8);
const u32 = (b, o) => (b[o] | (b[o + 1] << 8) | (b[o + 2] << 16)) + b[o + 3] * 0x1000000;
const u64 = (b, o) => u32(b, o) + u32(b, o + 4) * 0x100000000;

const strictUtf8 = new TextDecoder('utf-8', { fatal: true });
const sjis = new TextDecoder('shift_jis');
// UTF-8 の印があれば UTF-8。無くても UTF-8 として正しく読めれば UTF-8、だめなら Shift_JIS（SRC#Web と同じ規則）
function decodeName(bytes, utf8) {
  if (utf8) {
    return new TextDecoder('utf-8').decode(bytes);
  }
  try {
    return strictUtf8.decode(bytes);
  } catch {
    return sjis.decode(bytes);
  }
}

async function readEntries(reader) {
  if (reader.size < 22) {
    throw new Error('zip ではありません');
  }
  const tailLength = Math.min(reader.size, 22 + 0xffff);
  const tail = await reader.read(reader.size - tailLength, tailLength);
  let eocd = -1;
  for (let i = tail.length - 22; i >= 0; i--) {
    if (u32(tail, i) === 0x06054b50) {
      eocd = i;
      break;
    }
  }
  if (eocd < 0) {
    throw new Error('zip ではありません');
  }
  let count = u16(tail, eocd + 10);
  let cdSize = u32(tail, eocd + 12);
  let cdOffset = u32(tail, eocd + 16);
  if (count === 0xffff || cdSize === 0xffffffff || cdOffset === 0xffffffff) {
    const locator = eocd - 20;
    if (locator < 0 || u32(tail, locator) !== 0x07064b50) {
      throw new Error('Zip64 の終端が見つかりません');
    }
    const z = await reader.read(u64(tail, locator + 8), 56);
    if (u32(z, 0) !== 0x06064b50) {
      throw new Error('Zip64 の終端が壊れています');
    }
    count = u64(z, 32);
    cdSize = u64(z, 40);
    cdOffset = u64(z, 48);
  }
  const cd = await reader.read(cdOffset, cdSize);
  const entries = [];
  let p = 0;
  for (let i = 0; i < count; i++) {
    if (p + 46 > cd.length || u32(cd, p) !== 0x02014b50) {
      throw new Error('zip の目次が壊れています');
    }
    const flags = u16(cd, p + 8);
    const method = u16(cd, p + 10);
    let compressedSize = u32(cd, p + 20);
    let size = u32(cd, p + 24);
    const nameLength = u16(cd, p + 28);
    const extraLength = u16(cd, p + 30);
    const commentLength = u16(cd, p + 32);
    let offset = u32(cd, p + 42);
    const extra = cd.subarray(p + 46 + nameLength, p + 46 + nameLength + extraLength);
    if (size === 0xffffffff || compressedSize === 0xffffffff || offset === 0xffffffff) {
      for (let e = 0; e + 4 <= extra.length;) {
        const id = u16(extra, e);
        const len = u16(extra, e + 2);
        if (id === 0x0001) {
          let q = e + 4;
          if (size === 0xffffffff) { size = u64(extra, q); q += 8; }
          if (compressedSize === 0xffffffff) { compressedSize = u64(extra, q); q += 8; }
          if (offset === 0xffffffff) { offset = u64(extra, q); }
          break;
        }
        e += 4 + len;
      }
    }
    const name = decodeName(cd.subarray(p + 46, p + 46 + nameLength), (flags & 0x0800) !== 0).replace(/\\/g, '/');
    entries.push({
      name: name.split('/').filter(x => x && x !== '.' && x !== '..').join('/'),
      directory: name.endsWith('/'),
      encrypted: (flags & 0x0001) !== 0,
      method,
      compressedSize,
      size,
      offset,
    });
    p += 46 + nameLength + extraLength + commentLength;
  }
  return entries;
}

async function extract(reader, e) {
  if (e.encrypted) {
    throw new Error(`暗号化されたファイルは読めません: ${e.name}`);
  }
  const local = await reader.read(e.offset, 30);
  if (u32(local, 0) !== 0x04034b50) {
    throw new Error(`zip が壊れています: ${e.name}`);
  }
  const start = e.offset + 30 + u16(local, 26) + u16(local, 28);
  const data = await reader.read(start, e.compressedSize);
  if (e.method === 0) {
    return data;
  }
  if (e.method === 8) {
    const stream = new Blob([data]).stream().pipeThrough(new DecompressionStream('deflate-raw'));
    return new Uint8Array(await new Response(stream).arrayBuffer());
  }
  throw new Error(`対応していない圧縮方式です（${e.method}）: ${e.name}`);
}
