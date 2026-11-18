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
  core.ClearPlacementDataFiles();
  core.ClearImageRoots();
  // 地形データは SRC 本体 → シナリオの順に重ねる（後のものが同じ番号を上書きする）。SRCCore と同じく Data/System/terrain.txt
  for (const root of [...roots].reverse()) {
    const terrain = root.files.find(f => f.replace(/\\/g, '/').toLowerCase() === 'data/system/terrain.txt');
    if (terrain) {
      core.AddTerrainFile(`${root.name}/${terrain}`, await root.read(terrain));
    }
    for (const dataFile of root.files.filter(f => /(^|\/)(robot|unit|pilot)\.txt$/i.test(f) && /^data\//i.test(f.replace(/\\/g, '/')))) {
      core.AddPlacementDataFile(dataFile, await root.read(dataFile));
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

/** バイト列を文字列にする（UTF-8 として読めなければ Shift_JIS。SRC のデータは両方ある） */
export function decodeText(bytes) {
  try {
    return new TextDecoder('utf-8', { fatal: true }).decode(bytes).replace(/^﻿/, '');
  } catch {
    return new TextDecoder('shift_jis').decode(bytes);
  }
}

/**
 * データのプレビュー（7474/SRC#1643）。name・text はデータのファイル、before は先に読むデータ（[{ name, text }]。汎用データの sp.txt など）。
 * { kind: 'unit'|'pilot'|'', entries: [...], errors: [...] }
 */
export async function previewData(name, text, before = []) {
  const core = await studioCore();
  return JSON.parse(core.PreviewData(name, text, before.map(b => b.name), before.map(b => b.text)));
}

/** 候補のファイルのうち、フォルダ（loadWorkspace の順）にある最初のもの。{ root, path } か null */
export async function findFile(candidates) {
  const core = await studioCore();
  const found = core.FindFile(candidates);
  if (!found) {
    return null;
  }
  const i = found.indexOf(':');
  return { root: Number(found.slice(0, i)), path: found.slice(i + 1) };
}

/** 指定したマップに対応するイベントの配置（Create・Launch・Move・Organize）を解析する（7474/SRC#1653） */
export async function parsePlacements(targetMapPath, events) {
  const core = await studioCore();
  const paths = events.map(e => e.path);
  const texts = events.map(e => e.text);
  return JSON.parse(core.ParsePlacements(targetMapPath, paths, texts));
}

/** イベント 1 行の座標だけを書き換える */
export async function replacePlacementCoordinates(line, x, y) {
  const core = await studioCore();
  return core.ReplacePlacementCoordinates(line, x, y);
}

/** イベントテキスト全体の指定行（1 始まり）の座標だけを書き換える */
export async function replacePlacementFileCoordinates(text, lineNumber, x, y) {
  const core = await studioCore();
  return core.ReplacePlacementFileCoordinates(text, lineNumber, x, y);
}

/** 移動力の範囲の目安を計算する（[{ x, y }]） */
export async function computeMoveRange(map, placement) {
  const core = await studioCore();
  const flat = Array.from(core.ComputeMoveRange(
    map.width, map.height, map.tiles, map.layer ?? [],
    placement.x, placement.y, placement.speed ?? 0,
    placement.transportation ?? '', placement.adaption ?? ''));
  const cells = [];
  for (let i = 0; i + 1 < flat.length; i += 2) {
    cells.push({ x: flat[i], y: flat[i + 1] });
  }
  return cells;
}

/** かんたん作成（7474/SRC#1659）。場面の JSON から { errors: [...], files: { 相対パス: テキスト } } */
export async function buildQuickBattle(json) {
  const core = await studioCore();
  return JSON.parse(core.BuildQuickBattle(json));
}

/** 素材の参照を調べる対象ファイル（*.eve と素材を参照する Data/*.txt）か */
export function isAssetTarget(path) {
  const name = path.replace(/\\/g, '/').split('/').pop() ?? '';
  return /\.eve$/i.test(name) || /^(robot|unit|pilot|non_pilot|animation|effect|item|terrain)\.txt$/i.test(name);
}

/**
 * 素材の一覧（7474/SRC#1654）。
 * @param {{ kind: string, name: string, files: string[], read: (path: string) => Promise<Uint8Array> }[]} roots 探す順（シナリオ → SRC 本体）
 * @param {'json'|'md'|'readme'} format
 */
export async function buildAssetReport(roots, format = 'json') {
  const core = await studioCore();
  core.ClearAssetReportInputs();
  for (const root of roots) {
    core.AddAssetRoot(root.kind, root.name, root.files);
  }
  const scenario = roots.find(r => r.kind === 'scenario');
  if (scenario) {
    for (const file of scenario.files.filter(isAssetTarget)) {
      core.AddAssetTargetFile(file, await scenario.read(file));
    }
  }
  const out = core.BuildAssetReport(format);
  return format === 'json' ? JSON.parse(out) : out;
}

