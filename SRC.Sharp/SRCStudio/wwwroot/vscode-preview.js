// SRC#Studio のデータのプレビューを、VS Code の Webview（データのファイルの横）で動かす（7474/SRC#1643）。
// 拡張（SRCSharpVSCode の src/preview.ts）がデータのファイルの中身とカーソルの行を送り、ここで SRCCore の読み込み処理（WebAssembly）で読んで出す。
//   拡張 → 画面: { type: 'init', roots: [{ name, files }] } / { type: 'data', name, text, line, before: [{ name, text }] } / { type: 'file', id, bytes } / { type: 'fileError', id, message }
//   画面 → 拡張: { type: 'ready' } / { type: 'shown', kind, entries, selected } / { type: 'select', line } / { type: 'read', id, root, path } / { type: 'editLines', edits: [{ line, text }] }
import { loadWorkspace, previewData, findFile } from './studio-core.js';
import { DataPreview } from './preview.js';

const vscode = window.srcsharpVsCode;
const pending = new Map();
let nextId = 1;
let roots = [];
let latest = 0;

function read(root, path) {
  const id = nextId++;
  vscode.postMessage({ type: 'read', id, root, path });
  return new Promise((resolve, reject) => pending.set(id, { resolve, reject }));
}

let lastText = '';
const preview = new DataPreview(document.getElementById('preview'), {
  compare: true,
  lines: () => lastText.split(/\r?\n/),
  // 表で直した行は、拡張が文書に書き換える（保存・元に戻すは VS Code のもの）
  editLines: edits => vscode.postMessage({ type: 'editLines', edits }),
  async loadImage(kind, bitmap) {
    const folder = kind === 'pilot' ? 'Pilot' : 'Unit';
    const found = await findFile([`Bitmap/${folder}/${bitmap}`, `Bitmap/${bitmap}`]);
    return found ? createImageBitmap(new Blob([await read(found.root, found.path)])) : null;
  },
  // 一覧で選ぶと、エディタのその行へ
  onSelect: entry => vscode.postMessage({ type: 'select', line: entry.startLine }),
});

window.addEventListener('message', async ev => {
  const msg = ev.data;
  switch (msg.type) {
    case 'init':
      roots = msg.roots.map((r, i) => ({ name: r.name, files: r.files, read: path => read(i, path) }));
      await loadWorkspace(roots);
      break;
    case 'data': {
      // 打つたびに届くので、古いものの結果は捨てる
      const n = ++latest;
      lastText = msg.text;
      const result = await previewData(msg.name, msg.text, msg.before);
      if (n !== latest) {
        break;
      }
      preview.show(result, msg.line);
      vscode.postMessage({ type: 'shown', kind: result.kind, entries: result.entries.map(e => e.name), selected: preview.selected?.name ?? null, errors: result.errors });
      break;
    }
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
