import { spawn } from 'child_process';
import * as path from 'path';
import * as vscode from 'vscode';
import { scenarioRoot } from './hotReload';
import { resolveServer, serverCommand } from './server';
import { webviewHtml } from './webview';

export interface AssetUsage {
  file: string;
  line: number;
}

export interface AssetItem {
  name: string;
  kind: 'image' | 'sound' | 'map';
  missing: boolean;
  foundIn?: string;
  foundPath?: string;
  usages: AssetUsage[];
}

export interface AssetReportData {
  scenarioRoot?: string;
  roots: { kind: string; name: string; files: number }[];
  assets: AssetItem[];
  dynamic: { file: string; line: number; text: string }[];
  unused: string[];
  checkedFiles: number;
  missing: number;
}

export interface AssetsStatus {
  scenarioRoot?: string;
  assets: number;
  missing: number;
  dynamic: number;
  unused: number;
  rendered: boolean;
  error?: string;
}

type WebviewMessage =
  | { type: 'ready' }
  | { type: 'refresh' }
  | { type: 'copyReadme' }
  | { type: 'openFile'; file?: string; line?: number }
  | { type: 'rendered' };

/**
 * 素材の一覧パネル（7474/SRC#1654）。srcsharp assets --format json の結果を表にし、
 * 参照している場所をクリックするとそのファイル・行を開く。素材の入手・再配布は扱わない
 */
export class AssetsPanel implements vscode.Disposable {
  private panel: vscode.WebviewPanel | undefined;
  private ready = false;
  private root: string | undefined;
  private data: AssetReportData | undefined;
  private lastError: string | undefined;
  private rendered = false;
  private timer: NodeJS.Timeout | undefined;
  private readonly disposables: vscode.Disposable[] = [];

  constructor() {
    this.disposables.push(
      vscode.workspace.onDidSaveTextDocument((doc) => {
        if (this.panel && this.root && /\.(eve|txt)$/i.test(doc.fileName)) {
          if (this.timer) {
            clearTimeout(this.timer);
          }
          this.timer = setTimeout(() => void this.refresh(), 500);
        }
      }),
    );
  }

  public getStatus(): AssetsStatus {
    return {
      scenarioRoot: this.root,
      assets: this.data?.assets.length ?? 0,
      missing: this.data?.missing ?? 0,
      dynamic: this.data?.dynamic.length ?? 0,
      unused: this.data?.unused.length ?? 0,
      rendered: this.rendered,
      ...(this.lastError ? { error: this.lastError } : {}),
    };
  }

  public async show(explicitRoot?: string): Promise<void> {
    const root = explicitRoot ?? this.detectRoot();
    if (!root) {
      void vscode.window.showErrorMessage('SRC#: 素材を調べるシナリオのフォルダかイベントファイル（*.eve）を開いてください。');
      return;
    }
    this.root = root;
    this.ensurePanel();
    await this.refresh();
  }

  /** テスト用。JSON を直接渡して表示する */
  public async showData(data: AssetReportData): Promise<void> {
    this.root = data.scenarioRoot;
    this.data = data;
    this.lastError = undefined;
    this.ensurePanel();
    this.postState();
  }

  private ensurePanel(): void {
    this.rendered = false;
    if (this.panel) {
      this.panel.reveal(vscode.ViewColumn.Beside, false);
      return;
    }
    this.panel = vscode.window.createWebviewPanel('srcsharp.assets', 'SRC# 素材の一覧', vscode.ViewColumn.Beside, {
      enableScripts: true,
      retainContextWhenHidden: true,
    });
    this.panel.onDidDispose(() => {
      this.panel = undefined;
      this.ready = false;
      this.rendered = false;
    });
    this.panel.webview.onDidReceiveMessage((msg: WebviewMessage) => void this.receive(msg));
    this.panel.webview.html = webviewHtml(this.panel.webview, 'SRC# 素材の一覧', STYLE, BODY, SCRIPT);
  }

  private detectRoot(): string | undefined {
    const editor = vscode.window.activeTextEditor;
    if (editor && editor.document.uri.scheme === 'file') {
      return scenarioRoot(editor.document.uri.fsPath);
    }
    return vscode.workspace.workspaceFolders?.[0]?.uri.fsPath;
  }

  private runAssets(format: string): Promise<string> {
    const server = resolveServer();
    if (!server.found || !this.root) {
      return Promise.reject(new Error('srcsharp が見つかりません。'));
    }
    const appPath = vscode.workspace.getConfiguration('srcsharp').get<string>('appPath');
    const args = [this.root, '--format', format, ...(appPath ? ['--app-path', appPath] : [])];
    const cmd = serverCommand(server, 'assets', args);
    return new Promise<string>((resolve, reject) => {
      const child = spawn(cmd.command, cmd.args, { cwd: this.root, env: server.env });
      let stdout = '';
      let stderr = '';
      child.stdout.on('data', (d: Buffer) => { stdout += d.toString(); });
      child.stderr.on('data', (d: Buffer) => { stderr += d.toString(); });
      child.on('error', reject);
      child.on('close', (code) => {
        if (code === 0) {
          resolve(stdout);
        } else {
          reject(new Error(stderr.trim() || `srcsharp assets が終了コード ${code} で失敗しました`));
        }
      });
    });
  }

  public async refresh(): Promise<void> {
    if (!this.root) {
      return;
    }
    try {
      this.data = JSON.parse(await this.runAssets('json')) as AssetReportData;
      this.lastError = undefined;
    } catch (e) {
      this.lastError = (e as Error)?.message ?? String(e);
    }
    this.postState();
  }

  private postState(): void {
    if (!this.panel || !this.ready) {
      return;
    }
    void this.panel.webview.postMessage({ type: 'assets', data: this.data, error: this.lastError });
  }

  private async receive(msg: WebviewMessage): Promise<void> {
    switch (msg.type) {
      case 'ready':
        this.ready = true;
        this.postState();
        break;
      case 'refresh':
        await this.refresh();
        break;
      case 'copyReadme':
        try {
          await vscode.env.clipboard.writeText(await this.runAssets('readme'));
          void vscode.window.showInformationMessage('SRC#: README に書く「必要な素材」の下書きをクリップボードにコピーしました。');
        } catch (e) {
          void vscode.window.showErrorMessage(`SRC#: ${(e as Error)?.message ?? e}`);
        }
        break;
      case 'openFile':
        if (msg.file && this.root) {
          const full = path.isAbsolute(msg.file) ? msg.file : path.join(this.root, msg.file);
          try {
            const doc = await vscode.workspace.openTextDocument(vscode.Uri.file(full));
            const editor = await vscode.window.showTextDocument(doc, vscode.ViewColumn.One);
            const pos = new vscode.Position(Math.max(0, (msg.line ?? 1) - 1), 0);
            editor.selection = new vscode.Selection(pos, pos);
            editor.revealRange(new vscode.Range(pos, pos), vscode.TextEditorRevealType.InCenterIfOutsideViewport);
          } catch {
            void vscode.window.showWarningMessage(`SRC#: ファイルを開けませんでした: ${full}`);
          }
        }
        break;
      case 'rendered':
        this.rendered = true;
        break;
    }
  }

  public dispose(): void {
    if (this.timer) {
      clearTimeout(this.timer);
    }
    this.panel?.dispose();
    for (const d of this.disposables) {
      d.dispose();
    }
  }
}

const STYLE = `
.toolbar { display: flex; flex-wrap: wrap; align-items: center; justify-content: space-between; gap: 8px; border-bottom: 1px solid var(--vscode-panel-border); padding-bottom: 10px; margin-bottom: 10px; }
.toolbar h2 { margin: 0; font-size: 1.15em; }
.actions { display: flex; gap: 6px; flex-wrap: wrap; }
.summary { display: flex; flex-wrap: wrap; gap: 6px; margin: 8px 0; }
.chip { border: 1px solid var(--vscode-panel-border); border-radius: 99px; padding: 2px 10px; cursor: pointer; background: transparent; color: var(--vscode-foreground); }
.chip[aria-pressed="true"] { background: var(--vscode-button-secondaryBackground); color: var(--vscode-button-secondaryForeground); border-color: var(--vscode-focusBorder); }
.chip.bad { color: var(--vscode-errorForeground); }
.note { font-size: 0.9em; color: var(--vscode-descriptionForeground); margin: 4px 0 10px; }
table { width: 100%; border-collapse: collapse; font-size: 0.92em; }
th, td { border-bottom: 1px solid var(--vscode-panel-border); padding: 4px 8px; text-align: left; vertical-align: top; }
th { color: var(--vscode-descriptionForeground); font-weight: 600; }
td.name { font-family: var(--vscode-editor-font-family); word-break: break-all; }
.missing { color: var(--vscode-errorForeground); font-weight: 600; }
a.loc { color: var(--vscode-textLink-foreground); cursor: pointer; text-decoration: none; margin-right: 8px; white-space: nowrap; }
a.loc:hover { text-decoration: underline; }
section { margin-bottom: 18px; }
section h3 { font-size: 1em; margin: 12px 0 4px; }
.empty { color: var(--vscode-descriptionForeground); padding: 12px 0; }
`;

const BODY = `
<div class="toolbar">
  <h2>素材の一覧</h2>
  <div class="actions">
    <button id="btn-refresh" class="secondary">再読み込み</button>
    <button id="btn-readme" class="secondary" title="シナリオに含まれない素材の名前を、README に書く形でコピーします">README 用の一覧をコピー</button>
  </div>
</div>
<div id="error" class="error" style="display:none"></div>
<div id="roots" class="note"></div>
<div id="summary" class="summary" role="toolbar" aria-label="絞り込み"></div>
<div class="note">素材の入手・再配布はこの画面では扱いません。シナリオに必要な素材が手元に揃っているかだけを確かめます。</div>
<div id="content"></div>
`;

const SCRIPT = `
(function () {
  const errorEl = document.getElementById('error');
  const rootsEl = document.getElementById('roots');
  const summaryEl = document.getElementById('summary');
  const contentEl = document.getElementById('content');
  let data = null;
  let filter = 'all';
  const ROOT_LABEL = { scenario: 'シナリオ', extdata: '拡張データ', extdata2: '拡張データ2', app: 'SRC 本体' };
  const KIND_LABEL = { image: '画像', sound: '音', map: 'マップ' };

  document.getElementById('btn-refresh').addEventListener('click', () => vscode.postMessage({ type: 'refresh' }));
  document.getElementById('btn-readme').addEventListener('click', () => vscode.postMessage({ type: 'copyReadme' }));

  function el(tag, attrs, text) {
    const e = document.createElement(tag);
    if (attrs) for (const k of Object.keys(attrs)) e.setAttribute(k, attrs[k]);
    if (text !== undefined) e.textContent = text;
    return e;
  }

  function loc(file, line) {
    const a = el('a', { class: 'loc', href: '#' }, file + ':' + line);
    a.addEventListener('click', (ev) => { ev.preventDefault(); vscode.postMessage({ type: 'openFile', file, line }); });
    return a;
  }

  function chip(id, label, count, bad) {
    const b = el('button', { class: 'chip' + (bad && count > 0 ? ' bad' : ''), 'aria-pressed': String(filter === id), 'data-filter': id }, label + ' ' + count);
    b.addEventListener('click', () => { filter = id; render(); });
    return b;
  }

  function assetTable(list) {
    if (list.length === 0) return el('div', { class: 'empty' }, '該当する素材はありません。');
    const table = el('table');
    table.innerHTML = '<thead><tr><th>素材</th><th>種類</th><th>見つかった場所</th><th>参照している場所</th></tr></thead>';
    const tbody = el('tbody');
    for (const a of list) {
      const tr = el('tr', { 'data-asset': a.name });
      tr.appendChild(el('td', { class: 'name' }, a.name));
      tr.appendChild(el('td', null, KIND_LABEL[a.kind] || a.kind));
      const where = el('td');
      if (a.missing) {
        where.appendChild(el('span', { class: 'missing' }, '見つからない'));
      } else {
        where.textContent = (ROOT_LABEL[a.foundIn] || a.foundIn) + ': ' + a.foundPath;
      }
      tr.appendChild(where);
      const usages = el('td');
      for (const u of a.usages) usages.appendChild(loc(u.file, u.line));
      tr.appendChild(usages);
      tbody.appendChild(tr);
    }
    table.appendChild(tbody);
    return table;
  }

  function render() {
    contentEl.innerHTML = '';
    summaryEl.innerHTML = '';
    if (!data) return;
    const assets = data.assets || [];
    const dynamic = data.dynamic || [];
    const unused = data.unused || [];
    rootsEl.textContent = '探した順: ' + (data.roots || []).map((r) => (ROOT_LABEL[r.kind] || r.kind) + '（' + r.name + '）').join(' → ');
    summaryEl.appendChild(chip('all', 'すべて', assets.length));
    summaryEl.appendChild(chip('missing', '見つからない', data.missing || 0, true));
    summaryEl.appendChild(chip('external', 'シナリオの外', assets.filter((a) => !a.missing && a.foundIn !== 'scenario').length));
    summaryEl.appendChild(chip('dynamic', '確かめられない', dynamic.length));
    summaryEl.appendChild(chip('unused', '参照していない', unused.length));

    if (filter === 'all' || filter === 'missing' || filter === 'external') {
      const list = filter === 'missing' ? assets.filter((a) => a.missing)
        : filter === 'external' ? assets.filter((a) => !a.missing && a.foundIn !== 'scenario')
        : assets;
      const sec = el('section');
      sec.appendChild(el('h3', null, '参照している素材'));
      sec.appendChild(assetTable(list));
      contentEl.appendChild(sec);
    }
    if (filter === 'all' || filter === 'dynamic') {
      const sec = el('section');
      sec.appendChild(el('h3', null, '確かめられない参照（式で名前を組み立てている）'));
      if (dynamic.length === 0) sec.appendChild(el('div', { class: 'empty' }, 'ありません。'));
      for (const d of dynamic) {
        const div = el('div');
        div.appendChild(loc(d.file, d.line));
        div.appendChild(el('code', null, d.text));
        sec.appendChild(div);
      }
      contentEl.appendChild(sec);
    }
    if (filter === 'all' || filter === 'unused') {
      const sec = el('section');
      sec.appendChild(el('h3', null, '参照していない素材（シナリオのフォルダ）'));
      sec.appendChild(el('div', { class: 'note' }, '式で組み立てる名前で使っていることもあります。消すかは確かめてから決めてください。'));
      if (unused.length === 0) sec.appendChild(el('div', { class: 'empty' }, 'ありません。'));
      const ul = el('ul');
      for (const u of unused) ul.appendChild(el('li', { class: 'name' }, u));
      sec.appendChild(ul);
      contentEl.appendChild(sec);
    }
  }

  window.addEventListener('message', (event) => {
    const msg = event.data;
    if (!msg || msg.type !== 'assets') return;
    errorEl.style.display = msg.error ? 'block' : 'none';
    errorEl.textContent = msg.error || '';
    data = msg.data || null;
    render();
    vscode.postMessage({ type: 'rendered' });
  });

  vscode.postMessage({ type: 'ready' });
})();
`;
