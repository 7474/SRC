// VS Code の Webview で、SRC#Studio（vscode.js）を読み込む前に、エラーを拡張に伝える仕組みを用意する。
// モジュール（vscode.js とその先の .NET の実行環境）が読み込めないときも、理由を拡張に伝えられるよう、普通のスクリプトで動かす
(function () {
  const vscode = acquireVsCodeApi();
  window.srcsharpVsCode = vscode;
  window.addEventListener('error', function (ev) { vscode.postMessage({ type: 'error', message: String(ev.message || ev.error) }); });
  window.addEventListener('unhandledrejection', function (ev) { vscode.postMessage({ type: 'error', message: String((ev.reason && ev.reason.stack) || ev.reason) }); });
  import('./vscode.js').catch(function (e) { vscode.postMessage({ type: 'error', message: 'vscode.js: ' + String((e && e.stack) || e) }); });
})();
