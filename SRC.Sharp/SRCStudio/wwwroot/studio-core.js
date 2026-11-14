// SRC#Studio の計算（Program.cs の StudioExports）を呼ぶ。スタンドアロン（index.html）と VS Code の Webview（vscode.html）で共通
import { dotnet } from './_framework/dotnet.js';

let exportsPromise;

/** .NET の実行環境を読み込み、StudioExports を返す（一度だけ） */
export function studioCore() {
  exportsPromise ??= (async () => {
    const { getAssemblyExports, getConfig } = await dotnet.create();
    const exports = await getAssemblyExports(getConfig().mainAssemblyName);
    return exports.SRCStudio.StudioExports;
  })();
  return exportsPromise;
}

/**
 * 地形データと画像を探すフォルダを設定する。
 * @param {{ name: string, files: string[], read: (path: string) => Promise<Uint8Array> }[]} roots 画像を探す順（シナリオ → SRC 本体）
 * @returns {Promise<{ terrains: Terrain[], errors: string[] }>}
 */
export async function loadWorkspace(roots) {
  const core = await studioCore();
  core.ClearTerrainFiles();
  core.ClearImageRoots();
  // 地形データは SRC 本体 → シナリオの順に重ねる（後のものが同じ番号を上書きする）。SRCCore と同じく Data/System/terrain.txt
  for (const root of [...roots].reverse()) {
    const terrain = root.files.find(f => f.replace(/\\/g, '/').toLowerCase() === 'data/system/terrain.txt');
    if (terrain) {
      core.AddTerrainFile(`${root.name}/${terrain}`, await root.read(terrain));
    }
  }
  for (const root of roots) {
    core.AddImageRoot(root.files);
  }
  return JSON.parse(core.LoadTerrains());
}

/** 画像のファイル（SRCCore と同じ順で探す）。{ root: 番号, path } か null */
export async function findImage(bitmap, no) {
  const core = await studioCore();
  const found = core.FindImage(bitmap, no);
  if (!found) {
    return null;
  }
  const i = found.indexOf(':');
  return { root: Number(found.slice(0, i)), path: found.slice(i + 1) };
}

/** この画像の名前で、手元にある画像の番号 */
export async function imageNumbers(bitmap) {
  const core = await studioCore();
  return Array.from(core.ImageNumbers(bitmap));
}

/** マップを読む。{ width, height, tiles, layer, template, legacy } か { error } */
export async function parseMap(bytes) {
  const core = await studioCore();
  return JSON.parse(core.ParseMap(bytes));
}

/** マップを書き出す（テキスト） */
export async function serializeMap(map) {
  const core = await studioCore();
  return core.SerializeMap(map.width, map.height, map.tiles, map.layer ?? [], map.template ?? 0);
}
