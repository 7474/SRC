// SRC#Studio のマップ作成ツールを、VS Code 拡張のカスタムエディタ（Webview）で動かす（7474/SRC#1641）。
// ファイルの読み書きは拡張（SRCSharpVSCode の src/mapEditor.ts）が行い、メッセージでやり取りする。
//   拡張 → 画面: { type: 'init', roots: [{ name, files }], mapPath, text, lang, focus? } / { type: 'update', text } / { type: 'eventsUpdated', file? } / { type: 'focus', focus } / { type: 'file', id, bytes } / { type: 'fileError', id, message }
//   画面 → 拡張: { type: 'ready' } / { type: 'loaded', terrains, width, height, placements } / { type: 'edit', text } / { type: 'editEvent', file, line, expectedLine, newLine } / { type: 'read', id, root, path } / { type: 'undo' } / { type: 'redo' } / { type: 'reopenAsText' } / { type: 'error', message }
import { loadWorkspace, findImage, findFile, imageNumbers, parseMap, serializeMap, autoConnectMap, decodeText, parsePlacements, replacePlacementCoordinates, computeMoveRange } from './studio-core.js';
import { MapEditor } from './map-editor.js';
import { t } from './i18n.js';

// vscode-boot.js が acquireVsCodeApi() の結果を置き、エラーを拡張に伝える仕組みを用意している
const vscode = window.srcsharpVsCode;
const pending = new Map();
let nextId = 1;
let roots = [];
let lastText = null;
let currentMapPath = '';
const eventCache = new Map();

function read(root, path) {
  const id = nextId++;
  vscode.postMessage({ type: 'read', id, root, path });
  return new Promise((resolve, reject) => pending.set(id, { resolve, reject }));
}

const normPath = p => String(p ?? '').replace(/\\/g, '/').replace(/^\.\//, '').toLowerCase();

const editor = new MapEditor(document.getElementById('editor'), {
  async loadImage(bitmap, no) {
    const found = await findImage(bitmap, no);
    if (!found) {
      return null;
    }
    return createImageBitmap(new Blob([await read(found.root, found.path)]));
  },
  async loadUnitImage(bitmap) {
    if (!bitmap || bitmap === '-.bmp') {
      return null;
    }
    const found = await findFile([`Bitmap/Unit/${bitmap}`, `Bitmap/${bitmap}`]);
    if (!found) {
      return null;
    }
    return createImageBitmap(new Blob([await read(found.root, found.path)]));
  },
  imageNumbers,
  serialize: serializeMap,
  autoConnect: autoConnectMap,
  computeMoveRange,
  async onEditPlacement(item, x, y) {
    const key = [...eventCache.keys()].find(k => normPath(k) === normPath(item.file));
    if (!key) {
      return;
    }
    const text = eventCache.get(key);
    const eol = text.includes('\r\n') ? '\r\n' : '\n';
    const lines = text.replace(/\r\n/g, '\n').split('\n');
    const origLine = lines[item.line - 1];
    if (origLine == null) {
      return;
    }
    const newLine = await replacePlacementCoordinates(origLine, x, y);
    if (newLine === origLine) {
      return;
    }
    lines[item.line - 1] = newLine;
    eventCache.set(key, lines.join(eol));
    vscode.postMessage({ type: 'editEvent', file: key, line: item.line, expectedLine: origLine, newLine });
    await refreshPlacements();
  },
  onEdit(text) {
    lastText = text;
    vscode.postMessage({ type: 'edit', text });
  },
  // 元に戻すは VS Code の文書の元に戻す（Ctrl+Z も VS Code が受ける）
  undo: { undo: () => vscode.postMessage({ type: 'undo' }), redo: () => vscode.postMessage({ type: 'redo' }) },
});

async function refreshPlacements() {
  if (!currentMapPath || !roots[0]) {
    editor.setPlacements([]);
    return;
  }
  const eveFiles = roots[0].files.filter(f => f.toLowerCase().endsWith('.eve'));
  const events = [];
  for (const f of eveFiles) {
    if (!eventCache.has(f)) {
      try {
        eventCache.set(f, decodeText(await read(0, f)));
      } catch {
        continue;
      }
    }
    events.push({ path: f, text: eventCache.get(f) });
  }
  editor.setPlacements(await parsePlacements(currentMapPath, events));
}

function showError(message) {
  const box = document.getElementById('message');
  box.hidden = !message;
  box.textContent = message ?? '';
  document.getElementById('reopen').hidden = !message;
}

async function showText(text, keepHistory) {
  const map = await parseMap(new TextEncoder().encode(text));
  if (map.error) {
    // SRC のマップでないもの（JavaScript のソースマップなど）は、テキストで開き直せるようにする
    showError(t('studio.mapError', map.error));
    return;
  }
  showError(null);
  editor.setMap(map, keepHistory);
}

document.getElementById('reopen').addEventListener('click', () => vscode.postMessage({ type: 'reopenAsText' }));

window.addEventListener('message', async ev => {
  const msg = ev.data;
  switch (msg.type) {
    case 'init': {
      roots = msg.roots.map((r, i) => ({ name: r.name, files: r.files, read: path => read(i, path) }));
      currentMapPath = msg.mapPath ?? '';
      eventCache.clear();
      const result = await loadWorkspace(roots);
      if (result.errors.length) {
        vscode.postMessage({ type: 'error', message: t('studio.terrainErrors', result.errors.join(' / ')) });
      }
      editor.setTerrains(result.terrains);
      lastText = msg.text;
      await showText(msg.text, false);
      await refreshPlacements();
      if (msg.focus) {
        await editor.focusPlacement(msg.focus);
      }
      document.body.dataset.ready = 'true';
      vscode.postMessage({ type: 'loaded', terrains: result.terrains.length, width: editor.map?.width ?? 0, height: editor.map?.height ?? 0, placements: editor.placements.length });
      break;
    }
    case 'update':
      // 自分の編集が返ってきたものは描き直さない（VS Code の元に戻す・外での変更だけ）
      if (msg.text !== lastText) {
        lastText = msg.text;
        await showText(msg.text, true);
      }
      break;
    case 'eventsUpdated':
      if (msg.file) {
        const target = normPath(msg.file);
        for (const k of [...eventCache.keys()]) {
          if (normPath(k) === target) {
            eventCache.delete(k);
          }
        }
        if (roots[0] && !roots[0].files.some(f => normPath(f) === target)) {
          roots[0].files.push(msg.file);
        }
      } else {
        eventCache.clear();
      }
      await refreshPlacements();
      break;
    case 'focus':
      await editor.focusPlacement(msg.focus);
      break;
    case 'file':
      pending.get(msg.id)?.resolve(new Uint8Array(msg.bytes));
      pending.delete(msg.id);
      break;
    case 'fileError':
      pending.get(msg.id)?.reject(new Error(msg.message));
      pending.delete(msg.id);
      break;
  }
});

vscode.postMessage({ type: 'ready' });
