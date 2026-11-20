// SRC#Studio のスタンドアロン（index.html）。フォルダを開き、マップを選んでマップ作成ツールで編集し、保存する
import { loadWorkspace, findImage, imageNumbers, parseMap, serializeMap, autoConnectMap, studioCore, decodeText, previewData, findFile, parsePlacements, replacePlacementFileCoordinates, computeMoveRange, buildAssetReport, timelineBlocks, buildTimeline, insertTimelineMove, timelineScene } from './studio-core.js';
import { AssetsView } from './assets.js';
import { DataPreview } from './preview.js';
import { MapEditor, newMap } from './map-editor.js';
import { QuickBuilder } from './quick.js';
import { makeZip } from './zip.js';
import { storeForWeb, studioConfig, webPlayUrl } from './web-handoff.js';
import { basicData, canWriteFolders, download, fromFileList, pickFolder } from './folders.js';
import { language, t } from './i18n.js';

const $ = s => document.querySelector(s);
const state = { app: null, scenario: null, roots: [], terrains: [], path: null, text: null, dirty: false, eventTexts: new Map(), dirtyEvents: new Map() };

document.documentElement.lang = language;
document.querySelectorAll('[data-t]').forEach(e => { e.textContent = t(e.dataset.t); });
document.title = t('title');

const editor = new MapEditor($('#editor'), {
  loadImage,
  loadUnitImage: bitmap => loadDataImage('unit', bitmap),
  imageNumbers,
  serialize: serializeMap,
  autoConnect: autoConnectMap,
  computeMoveRange,
  onEditPlacement: (item, x, y) => editPlacement(item, x, y),
  timeline: {
    blocks: () => puppetBlocks(),
    build: (file, line) => buildTimeline(state.eventTexts.get(file) ?? '', line, editor.map),
    insertMove: async (file, after, name, x, y) => {
      const result = await insertTimelineMove(state.eventTexts.get(file) ?? '', after, name, x, y);
      await setEventText(file, result.text);
      return result;
    },
    editMove: async (file, line, x, y) => setEventText(file, await replacePlacementFileCoordinates(state.eventTexts.get(file) ?? '', line, x, y)),
    play: (file, line) => playScene(file, line),
  },
  onEdit: text => {
    state.text = text;
    setDirty(true);
  },
});

async function loadImage(bitmap, no) {
  const found = await findImage(bitmap, no);
  if (!found) {
    return null;
  }
  const bytes = await state.roots[found.root].read(found.path);
  return createImageBitmap(new Blob([bytes]));
}

// ---- データのプレビュー（7474/SRC#1643） ----

const data = { path: null, text: null, dirty: false };
const preview = new DataPreview($('#preview'), {
  loadImage: loadDataImage,
  compare: true,
  lines: () => (data.text ?? '').split(/\r?\n/),
  // 表で直した行を書き換え、プレビューを作り直す（保存は「データを保存」）
  editLines: async edits => {
    const eol = data.text.includes('\r\n') ? '\r\n' : '\n';
    const lines = data.text.split(/\r?\n/);
    for (const e of edits) {
      lines[e.line - 1] = e.text;
    }
    data.text = lines.join(eol);
    data.dirty = true;
    $('#save-data').disabled = false;
    await renderData();
  },
  insertLines: async (afterLine, text) => {
    const eol = data.text.includes('\r\n') ? '\r\n' : '\n';
    const inserted = text.split('\n');
    if (!data.text) {
      data.text = inserted.join(eol);
    } else {
      const lines = data.text.split(/\r?\n/);
      const at = afterLine > 0 ? Math.min(afterLine, lines.length) : lines.length;
      lines.splice(at, 0, ...inserted);
      data.text = lines.join(eol);
    }
    data.dirty = true;
    $('#save-data').disabled = false;
    await renderData();
  },
});

/** ユニット・パイロットの画像（SRC と同じく Bitmap/Unit・Bitmap/Pilot、次に Bitmap の直下） */
async function loadDataImage(kind, bitmap) {
  if (!bitmap || bitmap === '-.bmp') {
    return null;
  }
  const folder = kind === 'pilot' || kind === 'message' || kind === 'dialog' ? 'Pilot' : 'Unit';
  const found = await findFile([`Bitmap/${folder}/${bitmap}`, `Bitmap/${bitmap}`]);
  if (!found) {
    return null;
  }
  return createImageBitmap(new Blob([await state.roots[found.root].read(found.path)]));
}

function dataFiles() {
  return (state.scenario?.files ?? []).filter(f => /(^|\/)(robot|unit|pilot|pilot_message|pilot_dialog)\.txt$/i.test(f) && /^data\//i.test(f));
}

function renderDataFiles() {
  const select = $('#data-files');
  const files = dataFiles();
  select.innerHTML = files.map(f => `<option>${f.replace(/[&<>]/g, '')}</option>`).join('');
  // シナリオのフォルダを開くまでは、何をすればよいかだけを出す
  $('#data-empty').hidden = files.length > 0;
  $('#data-chooser').hidden = files.length === 0;
  $('#preview').hidden = files.length === 0;
  if (select.value) {
    showData(select.value);
  } else {
    preview.show({ kind: '', entries: [], errors: [] });
  }
}

/** 汎用データとシナリオの sp.txt を先に読む（スペシャルパワーの参照を解く） */
async function systemTexts() {
  const result = [];
  for (const root of [...state.roots].reverse()) {
    for (const f of root.files.filter(x => /(^|\/)sp\.txt$/i.test(x))) {
      result.push({ name: `${root.name}/${f}`, text: decodeText(await root.read(f)) });
    }
  }
  return result;
}

/** メッセージ・ダイアログデータのプレビュー用に、パイロット・ユニット・イベントも一緒に渡す */
async function dataPreviewBefore(path) {
  const result = await systemTexts();
  if (!/(^|\/)(pilot_message|pilot_dialog)\.txt$/i.test(path)) {
    return result;
  }
  for (const root of [...state.roots].reverse()) {
    for (const f of root.files.filter(x => /^data\/.*(^|\/)(pilot|non_pilot|robot|unit)\.txt$/i.test(x))) {
      result.push({ name: `${root.name}/${f}`, text: decodeText(await root.read(f)) });
    }
  }
  for (const [evePath, eveText] of state.eventTexts) {
    result.push({ name: evePath, text: eveText });
  }
  return result;
}

async function showData(path) {
  if (data.dirty && !confirm(t('studio.unsaved'))) {
    $('#data-files').value = data.path;
    return;
  }
  data.path = path;
  data.text = decodeText(await state.scenario.read(path));
  data.dirty = false;
  $('#save-data').disabled = true;
  await renderData();
}

let renderDataSeq = 0;

/** プレビューを作り直す。ファイルを続けて切り替えたときなどに、前に始めた分が後から終わって画面を戻さないよう、最後に始めた分だけを出す */
async function renderData() {
  const seq = ++renderDataSeq;
  const result = await previewData(data.path.split('/').at(-1), data.text, await dataPreviewBefore(data.path));
  if (seq === renderDataSeq) {
    preview.show(result);
  }
}

$('#save-data').addEventListener('click', async () => {
  const bytes = new TextEncoder().encode(data.text);
  if (state.scenario?.write) {
    await state.scenario.write(data.path, bytes);
    showMessage(t('studio.saved', `${state.scenario.name}/${data.path}`), 'info');
  } else {
    download(data.path, bytes);
  }
  data.dirty = false;
  $('#save-data').disabled = true;
});

$('#data-files').addEventListener('change', ev => showData(ev.target.value));

// ---- 素材の一覧（7474/SRC#1654） ----

const assetsView = new AssetsView($('#assets-view'), {
  onRefresh: () => renderAssets(),
  onMessage: (text, kind) => showMessage(text, kind),
});

async function renderAssets() {
  const hasScenario = Boolean(state.scenario);
  $('#assets-empty').hidden = hasScenario;
  $('#assets-view').hidden = !hasScenario;
  if (!hasScenario) {
    assetsView.setData(null, '');
    return;
  }
  const roots = [
    { kind: 'scenario', name: state.scenario.name, files: state.scenario.files, read: p => state.scenario.read(p) },
    ...(state.app ? [{ kind: 'app', name: state.app.name === 'basic-data' ? t('studio.basicData') : state.app.name, files: state.app.files, read: p => state.app.read(p) }] : []),
  ];
  const report = await buildAssetReport(roots, 'json');
  const readme = await buildAssetReport(roots, 'readme');
  assetsView.setData(report, readme);
}

function selectTab(tab) {
  for (const name of ['quick', 'map', 'data', 'assets']) {
    $(`#tab-${name}`).setAttribute('aria-selected', String(tab === name));
    $(`#tab-${name}`).tabIndex = tab === name ? 0 : -1;
  }
  $('#map-section').hidden = tab !== 'map';
  $('#data-section').hidden = tab !== 'data';
  $('#assets-section').hidden = tab !== 'assets';
  $('#quick-section').hidden = tab !== 'quick';
  $('#workspace').hidden = tab === 'quick';
  $('.files').hidden = tab !== 'map';
  if (tab === 'data') {
    renderDataFiles();
  }
  if (tab === 'assets') {
    void renderAssets();
  }
  if (tab === 'quick') {
    quick.render();
  }
  try {
    localStorage.setItem('srcstudio.tab', tab);
  } catch {
    // 残せなくても切り替える
  }
}

$('#tab-quick').addEventListener('click', () => selectTab('quick'));
$('#tab-map').addEventListener('click', () => selectTab('map'));
$('#tab-data').addEventListener('click', () => selectTab('data'));
$('#tab-assets').addEventListener('click', () => selectTab('assets'));
// タブは左右の矢印キーでも移る
$('.studio-tabs').addEventListener('keydown', ev => {
  const tabs = ['quick', 'map', 'data', 'assets'];
  const current = tabs.findIndex(name => $(`#tab-${name}`).getAttribute('aria-selected') === 'true');
  const step = ev.key === 'ArrowRight' ? 1 : ev.key === 'ArrowLeft' ? -1 : 0;
  if (step) {
    const next = tabs[(current + step + tabs.length) % tabs.length];
    selectTab(next);
    $(`#tab-${next}`).focus();
  }
});
// 空の画面のボタン（フォルダを開く・新しいマップ）は、上のボタンと同じ
document.querySelectorAll('[data-open="scenario"]').forEach(b => b.addEventListener('click', () => $('#open-scenario').click()));
document.querySelectorAll('[data-new-map]').forEach(b => b.addEventListener('click', () => $('#new-map').click()));

// ---- かんたん作成（7474/SRC#1659） ----

const quick = new QuickBuilder($('#quick-section'), {
  systemTexts,
  // 開いたシナリオのフォルダのマップ（マップ作成ツールで描いたもの）を場面に使える
  maps: () => state.scenario?.files.filter(f => f.toLowerCase().endsWith('.map')) ?? [],
  readMap: path => state.scenario.read(path),
  parseMap,
  terrains: () => state.terrains,
  loadImage,
  download,
});

function setDirty(dirty) {
  state.dirty = dirty;
  $('#save').disabled = !dirty || !state.path;
  $('#file-name').textContent = (state.path ?? '') + (dirty ? ' *' : '');
}

window.addEventListener('beforeunload', ev => {
  if (state.dirty || data.dirty || state.dirtyEvents.size > 0) {
    ev.preventDefault();
  }
});

async function loadScenarioEvents() {
  state.eventTexts.clear();
  state.dirtyEvents.clear();
  updateEventsUi();
  for (const path of state.scenario?.files.filter(f => f.toLowerCase().endsWith('.eve')) ?? []) {
    state.eventTexts.set(path, decodeText(await state.scenario.read(path)));
  }
}

async function refreshPlacements() {
  if (!state.path || state.eventTexts.size === 0) {
    editor.setPlacements([]);
    return;
  }
  const events = [...state.eventTexts.entries()].map(([path, text]) => ({ path, text }));
  editor.setPlacements(await parsePlacements(state.path, events));
}

/** 人形劇（7474/SRC#1694）で選べる塊。このマップの配置があるイベントファイルの塊（無ければ全部） */
async function puppetBlocks() {
  const norm = p => String(p).replace(/\\/g, '/').replace(/^\.\//, '').toLowerCase();
  const files = new Set(editor.placements.map(p => norm(p.file)));
  const paths = [...state.eventTexts.keys()];
  const targets = paths.some(k => files.has(norm(k))) ? paths.filter(k => files.has(norm(k))) : paths;
  const blocks = [];
  for (const file of targets) {
    for (const b of await timelineBlocks(state.eventTexts.get(file))) {
      blocks.push({ ...b, file });
    }
  }
  return blocks;
}

/**
 * 場面の再生（7474/SRC#1694）。シナリオのフォルダ（書きかけのイベントとマップを含む）と、その場面だけを流す 場面の再生.eve を zip にして、
 * 同じサイトの SRC#Web に渡す（かんたん作成と同じ受け渡し）
 */
async function playScene(file, line) {
  // ポップアップを止められないよう、押したときに窓を開いてから中身を作る
  const win = window.open('', '_blank');
  try {
    const scene = await timelineScene(state.eventTexts.get(file) ?? '', line, state.path);
    if (!scene) {
      throw new Error(`${file}:${line}`);
    }
    const sceneName = '場面の再生.eve';
    const overrides = new Map([...state.dirtyEvents.entries()]);
    if (state.path && state.text != null) {
      overrides.set(state.path, state.text);
    }
    const files = [];
    for (const path of state.scenario.files) {
      if (/^\.srcsharp\//.test(path) || path === sceneName) {
        continue;
      }
      files.push({ path, data: overrides.has(path) ? overrides.get(path) : await state.scenario.read(path) });
    }
    files.push({ path: sceneName, data: scene });
    const config = await studioConfig();
    const name = `${state.scenario.name}（場面の再生）.zip`;
    await storeForWeb(name, makeZip(files), config.requires);
    const url = new URL(await webPlayUrl(name));
    url.searchParams.set('eve', sceneName);
    if (win) {
      win.location.href = url.toString();
    } else {
      location.href = url.toString();
    }
    const block = editor.puppetBlocks.find(b => b.file === file && b.line === line);
    showMessage(t('map.puppet.playing', block?.label ?? `${file}:${line}`), 'info');
  } catch (e) {
    win?.close();
    showMessage(t('map.puppet.playFailed', e?.message ?? e));
  }
}

async function setEventText(key, after) {
  if (after === state.eventTexts.get(key)) {
    return;
  }
  state.eventTexts.set(key, after);
  state.dirtyEvents.set(key, after);
  updateEventsUi();
  await refreshPlacements();
}

async function editPlacement(item, x, y) {
  const norm = p => String(p).replace(/\\/g, '/').replace(/^\.\//, '').toLowerCase();
  const target = norm(item.file);
  const key = [...state.eventTexts.keys()].find(k => norm(k) === target);
  if (!key) {
    return;
  }
  const before = state.eventTexts.get(key);
  const after = await replacePlacementFileCoordinates(before, item.line, x, y);
  if (after === before) {
    return;
  }
  state.eventTexts.set(key, after);
  state.dirtyEvents.set(key, after);
  updateEventsUi();
  await refreshPlacements();
}

function updateEventsUi() {
  const n = state.dirtyEvents.size;
  const btn = $('#save-events');
  const status = $('#events-status');
  btn.hidden = n === 0;
  btn.textContent = state.scenario?.write ? t('studio.saveEvents') : t('studio.downloadEvents');
  status.hidden = n === 0;
  status.textContent = n > 0 ? t('studio.eventsDirty', [...state.dirtyEvents.keys()].join(', ')) : '';
}

/** 開いているフォルダから地形と画像を読み直す（画像を探す順はシナリオ → SRC 本体） */
async function reloadWorkspace() {
  state.roots = [state.scenario, state.app].filter(Boolean);
  $('#app-folder').textContent = t('studio.appFolder', state.app?.name === 'basic-data' ? t('studio.basicData') : state.app?.name ?? t('studio.none'));
  $('#scenario-folder').textContent = t('studio.scenarioFolder', state.scenario?.name ?? t('studio.none'));
  const result = await loadWorkspace(state.roots);
  state.terrains = result.terrains;
  const messages = [];
  if (result.errors.length) {
    messages.push(t('studio.terrainErrors', result.errors.join(' / ')));
  }
  if (result.terrains.length === 0) {
    messages.push(t('studio.noTerrain'));
  }
  showMessage(messages.join('\n'));
  editor.images.clear();
  editor.unitImages.clear();
  quick.images.clear();
  editor.setTerrains(result.terrains);
  await loadScenarioEvents();
  await refreshPlacements();
  renderMapList();
  if (!$('#data-section').hidden) {
    renderDataFiles();
  }
  if (!$('#assets-section').hidden) {
    await renderAssets();
  }
}

function showMessage(text, kind = 'warning') {
  const box = $('#message');
  box.textContent = text;
  box.className = text ? kind : '';
  box.hidden = !text;
}

function renderMapList() {
  const list = $('#maps');
  list.innerHTML = '';
  for (const path of state.scenario?.files.filter(f => f.toLowerCase().endsWith('.map')) ?? []) {
    const li = document.createElement('li');
    const a = document.createElement('a');
    a.href = '#';
    a.textContent = path;
    a.addEventListener('click', ev => {
      ev.preventDefault();
      openMap(path);
    });
    li.append(a);
    list.append(li);
  }
  $('#maps-section').hidden = !state.scenario;
}

async function confirmDiscard() {
  return !state.dirty || confirm(t('studio.unsaved'));
}

async function openMap(path) {
  if (!(await confirmDiscard())) {
    return;
  }
  const map = await parseMap(await state.scenario.read(path));
  if (map.error) {
    showMessage(t('studio.mapError', map.error), 'error');
    return;
  }
  state.path = path;
  state.text = null;
  editor.setMap(map);
  setDirty(false);
  showEditor();
  await refreshPlacements();
}

$('#new-map').addEventListener('click', async () => {
  if (!(await confirmDiscard())) {
    return;
  }
  const name = prompt(t('studio.newMapName'), 'new.map');
  if (!name) {
    return;
  }
  const first = state.terrains[0];
  const map = newMap(20, 20, { terrain: first?.id ?? 0, bitmap: 0 });
  state.path = name.endsWith('.map') ? name : `${name}.map`;
  editor.setMap(map);
  state.text = await serializeMap(map);
  setDirty(true);
  showEditor();
  await refreshPlacements();
});

function showEditor() {
  $('#editor').hidden = false;
  $('#map-empty').hidden = true;
}

$('#save').addEventListener('click', async () => {
  const bytes = new TextEncoder().encode(state.text);
  if (state.scenario?.write) {
    await state.scenario.write(state.path, bytes);
    setDirty(false);
    renderMapList();
    showMessage(t('studio.saved', `${state.scenario.name}/${state.path}`), 'info');
  } else {
    download(state.path, bytes);
    setDirty(false);
  }
});

$('#save-events').addEventListener('click', async () => {
  const saved = [];
  for (const [path, text] of state.dirtyEvents) {
    const bytes = new TextEncoder().encode(text);
    if (state.scenario?.write) {
      await state.scenario.write(path, bytes);
    } else {
      download(path, bytes);
    }
    saved.push(path);
  }
  state.dirtyEvents.clear();
  updateEventsUi();
  if (saved.length > 0 && state.scenario?.write) {
    showMessage(t('studio.eventsSaved', saved.join(', ')), 'info');
  }
});

$('#download').addEventListener('click', async () => {
  if (state.text == null && editor.map) {
    state.text = await serializeMap(editor.map);
  }
  if (state.text != null) {
    download(state.path ?? 'new.map', new TextEncoder().encode(state.text));
  }
});

// フォルダを開く。File System Access API があれば読み書きでき、無ければ <input webkitdirectory> で読むだけ
function setupFolderButton(button, input, id, assign) {
  button.addEventListener('click', async () => {
    if (canWriteFolders()) {
      try {
        assign(await pickFolder(id));
        await reloadWorkspace();
      } catch (e) {
        if (e.name !== 'AbortError') {
          showMessage(String(e), 'error');
        }
      }
    } else {
      input.click();
    }
  });
  input.addEventListener('change', async () => {
    if (input.files.length) {
      assign(fromFileList(input.files));
      await reloadWorkspace();
    }
  });
}

setupFolderButton($('#open-app'), $('#app-input'), 'srcsharp-app', f => { state.app = f; });
setupFolderButton($('#open-scenario'), $('#scenario-input'), 'srcsharp-scenario', f => { state.scenario = f; });

if (!canWriteFolders()) {
  showMessage(t('studio.readOnly'), 'info');
}
$('#save').hidden = !canWriteFolders();

(async () => {
  $('#loading').hidden = false;
  await studioCore();
  state.app = await basicData();
  await reloadWorkspace();
  // はじめて開いた人は、かんたん作成から（?tab= か、前に開いていたタブ）
  let tab = new URLSearchParams(location.search).get('tab');
  try {
    tab ??= localStorage.getItem('srcstudio.tab');
  } catch {
    // localStorage を使えない
  }
  selectTab(['quick', 'map', 'data', 'assets'].includes(tab) ? tab : 'quick');
  await quick.restoreDraft();
  $('#loading').hidden = true;
  document.body.dataset.ready = 'true';
})();
