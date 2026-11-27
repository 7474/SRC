// SRC#Studio のシナリオの見通し（7474/SRC#1692）を、VS Code の Webview で動かす。
// 拡張（SRCSharpVSCode の src/outline.ts）が srcsharp outline --format json の結果を送り、ここで表にする（WebAssembly は使わない）。
//   拡張 → 画面: { type: 'outline', outline } / { type: 'busy', value }
//   画面 → 拡張: { type: 'ready' } / { type: 'shown', status } / { type: 'openLine', file, line } / { type: 'previewTalk', file, line } / { type: 'refresh' } / { type: 'export' }
import { OutlineView } from './outline.js';

const vscode = window.srcsharpVsCode;
const root = document.getElementById('outline-view');
const view = new OutlineView(root, {
  openLine: (file, line) => vscode.postMessage({ type: 'openLine', file, line }),
  // 台本の行を選ぶと、その行を開いて会話のプレビュー（SRCRender で描いた画）を出す
  previewTalk: line => {
    vscode.postMessage({ type: 'previewTalk', file: line.file, line: line.line });
    report();
  },
  onRefresh: () => vscode.postMessage({ type: 'refresh' }),
  onExport: () => vscode.postMessage({ type: 'export' }),
});

function report() {
  vscode.postMessage({ type: 'shown', status: view.status() });
}

// 表の切り替え・絞り込みのたびに、拡張に今の状態を伝える（テストの srcsharp._outlineStatus が読む）
new MutationObserver(() => report()).observe(root, { childList: true });

window.addEventListener('message', ev => {
  const msg = ev.data;
  switch (msg.type) {
    case 'outline':
      view.setData(msg.outline);
      report();
      break;
    case 'select': {
      // テスト用: 表と行を選ぶ
      if (msg.section) {
        view.section = msg.section;
        view.render();
      }
      if (msg.index !== undefined) {
        root.querySelectorAll('.ol-script tbody tr')[msg.index]?.click();
      }
      report();
      break;
    }
  }
});

vscode.postMessage({ type: 'ready' });
