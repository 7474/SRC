// SRC#Studio のスタンドアロン（index.html）が開くフォルダ。
// { name, files: 相対パス（/ 区切り）の一覧, read(path) => Uint8Array, write?(path, bytes) }
// - File System Access API（Chrome・Edge）: 読み書きできる
// - <input type="file" webkitdirectory>（ほかのブラウザ）: 読むだけ（書き出しはダウンロード）
// - 同梱の最小の汎用データ（basic-data/）: 読むだけ

/** フォルダを直接読み書きできるか（File System Access API）。?fs=input で使わない（テスト用） */
export function canWriteFolders() {
  return typeof globalThis.showDirectoryPicker === 'function' && new URLSearchParams(location.search).get('fs') !== 'input';
}

/**
 * フォルダの中で読むファイルを絞る。SRC 本体のフォルダは大きい（Bitmap に数万のファイル）ので、
 * 地形データ（Data/System/terrain.txt）・スペシャルパワー（sp.txt）・データ（robot.txt・unit.txt・pilot.txt）・
 * 画像（Bitmap/Map・Bitmap/Unit・Bitmap/Pilot）・マップ（*.map）だけを見る
 */
function wanted(path, isDir) {
  const p = path.toLowerCase();
  if (isDir) {
    const top = p.split('/')[0];
    if (top === 'bitmap') {
      return p === 'bitmap' || /^bitmap\/(map|unit|pilot)(\/|$)/.test(p);
    }
    // 音・セーブなどは見ない
    return !['sound', 'midi', 'music', 'save', '.git', 'node_modules'].includes(top);
  }
  return p === 'data/system/terrain.txt' || p === 'data/system/sp.txt'
    // データのファイル（プレビュー）と、ユニット・パイロットの画像・イベント
    || (p.startsWith('data/') && /(^|\/)(robot|unit|pilot|sp)\.txt$/.test(p))
    || (/^bitmap\/(map|unit|pilot)\//.test(p) && /\.(bmp|png|jpg|gif)$/.test(p))
    || p.endsWith('.map')
    || p.endsWith('.eve');
}

/** showDirectoryPicker で選んだフォルダ */
export async function pickFolder(id) {
  const handle = await globalThis.showDirectoryPicker({ id, mode: 'readwrite' });
  return fromDirectoryHandle(handle);
}

export async function fromDirectoryHandle(handle) {
  const entries = new Map();
  async function walk(dir, prefix) {
    for await (const [name, entry] of dir.entries()) {
      const path = prefix ? `${prefix}/${name}` : name;
      if (entry.kind === 'directory') {
        if (wanted(path, true)) {
          await walk(entry, path);
        }
      } else if (wanted(path, false)) {
        entries.set(path, entry);
      }
    }
  }
  await walk(handle, '');
  return {
    name: handle.name,
    files: [...entries.keys()].sort(),
    async read(path) {
      const file = await entries.get(path).getFile();
      return new Uint8Array(await file.arrayBuffer());
    },
    async write(path, bytes) {
      let dir = handle;
      const parts = path.split('/');
      for (const part of parts.slice(0, -1)) {
        dir = await dir.getDirectoryHandle(part, { create: true });
      }
      const file = await dir.getFileHandle(parts.at(-1), { create: true });
      const w = await file.createWritable();
      await w.write(bytes);
      await w.close();
      entries.set(path, file);
      if (!this.files.includes(path)) {
        this.files.push(path);
        this.files.sort();
      }
    },
  };
}

/** <input type="file" webkitdirectory> で選んだフォルダ（読むだけ） */
export function fromFileList(fileList) {
  const entries = new Map();
  let name = '';
  for (const file of fileList) {
    const parts = (file.webkitRelativePath || file.name).split('/');
    name ||= parts.length > 1 ? parts[0] : '';
    const path = parts.length > 1 ? parts.slice(1).join('/') : parts[0];
    if (wanted(path, false)) {
      entries.set(path, file);
    }
  }
  return {
    name,
    files: [...entries.keys()].sort(),
    async read(path) {
      return new Uint8Array(await entries.get(path).arrayBuffer());
    },
  };
}

/** 同梱の最小の汎用データ（tools/studio_bundle.sh が置く basic-data/ と目次 files.json） */
export async function basicData(base = new URL('./basic-data/', import.meta.url)) {
  const res = await fetch(new URL('files.json', base));
  if (!res.ok) {
    return null;
  }
  const files = await res.json();
  return {
    name: 'basic-data',
    files,
    async read(path) {
      const r = await fetch(new URL(path.split('/').map(encodeURIComponent).join('/'), base));
      if (!r.ok) {
        throw new Error(`${path}: ${r.status}`);
      }
      return new Uint8Array(await r.arrayBuffer());
    },
  };
}

/** バイト列をファイルとしてダウンロードさせる */
export function download(name, bytes) {
  const url = URL.createObjectURL(new Blob([bytes], { type: 'application/octet-stream' }));
  const a = document.createElement('a');
  a.href = url;
  a.download = name.split('/').at(-1);
  document.body.append(a);
  a.click();
  a.remove();
  setTimeout(() => URL.revokeObjectURL(url), 10000);
}
