// SRC#Studio のスタンドアロン（index.html）。フォルダを開き、マップを選んでマップ作成ツールで編集し、保存する
import { loadWorkspace, findImage, imageNumbers, parseMap, serializeMap, studioCore, decodeText, previewData, findFile } from './studio-core.js';
import { DataPreview } from './preview.js';
import { MapEditor, newMap } from './map-editor.js';
import { basicData, canWriteFolders, download, fromFileList, pickFolder } from './folders.js';
import { language, t } from './i18n.js';

const $ = s => document.querySelector(s);
const state = { app: null, scenario: null, roots: [], terrains: [], path: null, text: null, dirty: false };

document.documentElement.lang = language;
document.querySelectorAll('[data-t]').forEach(e => { e.textContent = t(e.dataset.t); });
document.title = t('title');

const editor = new MapEditor($('#editor'), {
  loadImage,
  imageNumbers,
  serialize: serializeMap,
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

const preview = new DataPreview($('#preview'), { loadImage: loadDataImage });

/** ユニット・パイロットの画像（SRC と同じく Bitmap/Unit・Bitmap/Pilot、次に Bitmap の直下） */
async function loadDataImage(kind, bitmap) {
  const folder = kind === 'pilot' ? 'Pilot' : 'Unit';
  const found = await findFile([`Bitmap/${folder}/${bitmap}`, `Bitmap/${bitmap}`]);
  if (!found) {
    return null;
  }
  return createImageBitmap(new Blob([await state.roots[found.root].read(found.path)]));
}

function dataFiles() {
  return (state.scenario?.files ?? []).filter(f => /(^|\/)(robot|unit|pilot)\.txt$/i.test(f) && /^data\//i.test(f));
}

function renderDataFiles() {
  const select = $('#data-files');
  select.innerHTML = dataFiles().map(f => `<option>${f.replace(/[&<>]/g, '')}</option>`).join('');
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

async function showData(path) {
  const text = decodeText(await state.scenario.read(path));
  preview.show(await previewData(path.split('/').at(-1), text, await systemTexts()));
}

$('#data-files').addEventListener('change', ev => showData(ev.target.value));

function selectTab(tab) {
  $('#tab-map').setAttribute('aria-selected', String(tab === 'map'));
  $('#tab-data').setAttribute('aria-selected', String(tab === 'data'));
  $('#map-section').hidden = tab !== 'map';
  $('#data-section').hidden = tab !== 'data';
  $('.files').hidden = tab !== 'map';
  if (tab === 'data') {
    renderDataFiles();
  }
}

$('#tab-map').addEventListener('click', () => selectTab('map'));
$('#tab-data').addEventListener('click', () => selectTab('data'));

function setDirty(dirty) {
  state.dirty = dirty;
  $('#save').disabled = !dirty || !state.path;
  $('#file-name').textContent = (state.path ?? '') + (dirty ? ' *' : '');
}

window.addEventListener('beforeunload', ev => {
  if (state.dirty) {
    ev.preventDefault();
  }
});

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
  editor.setTerrains(result.terrains);
  renderMapList();
  if (!$('#data-section').hidden) {
    renderDataFiles();
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
  $('#editor').hidden = false;
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
  $('#editor').hidden = false;
});

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
  $('#loading').hidden = true;
  document.body.dataset.ready = 'true';
})();
