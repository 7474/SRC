import { ChildProcess, spawn } from 'child_process';
import * as path from 'path';
import * as vscode from 'vscode';
import { resolveServer, serverCommand } from './server';
import { webviewHtml } from './webview';

export interface TalkPreviewBlockSummary {
  index: number;
  command: string;
  startLine: number;
  endLine: number;
  endKind: string;
  label?: string;
  stepCount: number;
  previewText: string;
}

export interface TalkPreviewPage {
  pageIndex: number;
  stepIndex: number;
  line: number;
  command: string;
  speaker?: string;
  rawSpeaker?: string;
  imageOptions?: string;
  text: string;
  renderedLines?: string[];
  pngBase64?: string;
}

export interface TalkPreviewData {
  file: string;
  scenarioRoot?: string;
  appPath?: string;
  cursorLine: number;
  selectedBlockIndex: number;
  startPageIndex: number;
  blocks: TalkPreviewBlockSummary[];
  pages: TalkPreviewPage[];
  warnings: string[];
}

export interface TalkPreviewStatus {
  file?: string;
  blocks: number;
  pages: number;
  selectedBlockIndex: number;
  currentPage: number;
  rendered: boolean;
  error?: string;
}

type WebviewMessage =
  | { type: 'ready' }
  | { type: 'refresh' }
  | { type: 'selectBlock'; index: number }
  | { type: 'selectPage'; pageIndex: number }
  | { type: 'openLine'; line: number }
  | { type: 'setFollowCursor'; value: boolean }
  | { type: 'rendered'; blocks: number; pages: number; currentPage: number };

/**
 * 会話のプレビュー（Talk・AutoTalk）パネル（7474/SRC#1655）。
 * srcsharp talk-preview --stdin --format json を呼び出し、SRCRender が描画したメッセージウィンドウ（PNG）を
 * ページ送りしながら確認できる。
 */
export class TalkPreviewPanel implements vscode.Disposable {
  private panel: vscode.WebviewPanel | undefined;
  private ready = false;
  private targetUri: vscode.Uri | undefined;
  private cursorLine = 1;
  private pinnedBlockIndex: number | undefined;
  private followCursor = true;
  private data: TalkPreviewData | undefined;
  private currentPage = 0;
  private lastError: string | undefined;
  private rendered = false;
  private timer: NodeJS.Timeout | undefined;
  private runningChild: ChildProcess | undefined;
  private requestSeq = 0;
  private readonly disposables: vscode.Disposable[] = [];

  constructor() {
    this.disposables.push(
      vscode.window.onDidChangeTextEditorSelection((e) => {
        if (!this.panel || !this.followCursor || !this.targetUri) {
          return;
        }
        if (e.textEditor.document.uri.toString() === this.targetUri.toString()) {
          const nextLine = e.selections[0].active.line + 1;
          if (nextLine !== this.cursorLine || this.pinnedBlockIndex !== undefined) {
            this.cursorLine = nextLine;
            this.pinnedBlockIndex = undefined;
            this.scheduleRefresh(250);
          }
        }
      }),
      vscode.workspace.onDidChangeTextDocument((e) => {
        if (!this.panel || !this.targetUri) {
          return;
        }
        if (e.document.uri.toString() === this.targetUri.toString()) {
          this.scheduleRefresh(450);
        }
      }),
      vscode.workspace.onDidSaveTextDocument((doc) => {
        if (!this.panel || !this.targetUri) {
          return;
        }
        if (doc.uri.toString() === this.targetUri.toString()) {
          this.scheduleRefresh(150);
        }
      }),
    );
  }

  public getStatus(): TalkPreviewStatus {
    return {
      file: this.targetUri?.fsPath ?? this.data?.file,
      blocks: this.data?.blocks.length ?? 0,
      pages: this.data?.pages.length ?? 0,
      selectedBlockIndex: this.data?.selectedBlockIndex ?? -1,
      currentPage: this.currentPage,
      rendered: this.rendered,
      ...(this.lastError ? { error: this.lastError } : {}),
    };
  }

  public async show(explicitUri?: vscode.Uri, explicitLine?: number): Promise<void> {
    const editor = vscode.window.activeTextEditor;
    const uri = explicitUri
      ?? (editor && (editor.document.languageId === 'src-eve' || editor.document.fileName.toLowerCase().endsWith('.eve'))
        ? editor.document.uri
        : undefined);
    if (!uri || uri.scheme !== 'file') {
      void vscode.window.showErrorMessage('SRC#: 会話をプレビューするイベントファイル（*.eve）を開いてください。');
      return;
    }

    this.targetUri = uri;
    this.cursorLine = explicitLine ?? (editor && editor.document.uri.toString() === uri.toString() ? editor.selection.active.line + 1 : 1);
    this.pinnedBlockIndex = undefined;
    this.rendered = false;
    this.lastError = undefined;

    if (!this.panel) {
      this.panel = vscode.window.createWebviewPanel(
        'srcsharp.talkPreview',
        `SRC# 会話のプレビュー: ${path.basename(uri.fsPath)}`,
        vscode.ViewColumn.Beside,
        { enableScripts: true, retainContextWhenHidden: true },
      );
      this.panel.onDidDispose(() => {
        this.panel = undefined;
        this.ready = false;
        this.rendered = false;
        this.cancelRunning();
      });
      this.panel.webview.onDidReceiveMessage((msg: WebviewMessage) => void this.receive(msg));
      this.panel.webview.html = webviewHtml(this.panel.webview, 'SRC# 会話のプレビュー', STYLE, BODY, SCRIPT);
    } else {
      this.panel.title = `SRC# 会話のプレビュー: ${path.basename(uri.fsPath)}`;
      this.panel.reveal(vscode.ViewColumn.Beside, false);
    }
    await this.refresh();
  }

  /** テストや外部から JSON データを直接渡して表示する */
  public async showData(previewData: TalkPreviewData): Promise<void> {
    this.targetUri = previewData.file ? vscode.Uri.file(previewData.file) : undefined;
    this.data = previewData;
    this.currentPage = previewData.startPageIndex ?? 0;
    this.lastError = undefined;
    this.rendered = false;
    const title = previewData.file ? `SRC# 会話のプレビュー: ${path.basename(previewData.file)}` : 'SRC# 会話のプレビュー';
    if (!this.panel) {
      this.panel = vscode.window.createWebviewPanel(
        'srcsharp.talkPreview',
        title,
        vscode.ViewColumn.Beside,
        { enableScripts: true, retainContextWhenHidden: true },
      );
      this.panel.onDidDispose(() => {
        this.panel = undefined;
        this.ready = false;
        this.rendered = false;
        this.cancelRunning();
      });
      this.panel.webview.onDidReceiveMessage((msg: WebviewMessage) => void this.receive(msg));
      this.panel.webview.html = webviewHtml(this.panel.webview, 'SRC# 会話のプレビュー', STYLE, BODY, SCRIPT);
    } else {
      this.panel.title = title;
      this.panel.reveal(vscode.ViewColumn.Beside, false);
    }
    this.postState();
  }

  public async refresh(): Promise<void> {
    if (!this.targetUri) {
      return;
    }
    const server = resolveServer();
    if (!server.found) {
      this.lastError = 'srcsharp が見つかりません。';
      this.postState();
      return;
    }

    const seq = ++this.requestSeq;
    this.cancelRunning();

    let text: string | undefined;
    try {
      const doc = vscode.workspace.textDocuments.find((d) => d.uri.toString() === this.targetUri?.toString())
        ?? await vscode.workspace.openTextDocument(this.targetUri);
      text = doc.getText();
    } catch (e) {
      this.lastError = (e as Error)?.message ?? String(e);
      this.postState();
      return;
    }

    const appPath = vscode.workspace.getConfiguration('srcsharp').get<string>('appPath');
    const args = [
      this.targetUri.fsPath,
      '--line', String(this.cursorLine),
      '--stdin',
      '--format', 'json',
      ...(this.pinnedBlockIndex !== undefined ? ['--block', String(this.pinnedBlockIndex)] : []),
      ...(appPath ? ['--app-path', appPath] : []),
    ];
    const cmd = serverCommand(server, 'talk-preview', args);

    try {
      const jsonText = await new Promise<string>((resolve, reject) => {
        const child = spawn(cmd.command, cmd.args, { cwd: path.dirname(this.targetUri!.fsPath), env: server.env });
        this.runningChild = child;
        let stdout = '';
        let stderr = '';
        child.stdout.on('data', (d: Buffer) => { stdout += d.toString(); });
        child.stderr.on('data', (d: Buffer) => { stderr += d.toString(); });
        child.on('error', reject);
        child.on('close', (code) => {
          if (this.runningChild === child) {
            this.runningChild = undefined;
          }
          if (code === 0) {
            resolve(stdout);
          } else {
            reject(new Error(stderr.trim() || `srcsharp talk-preview が終了コード ${code} で失敗しました`));
          }
        });
        child.stdin.write(text, 'utf8');
        child.stdin.end();
      });

      if (seq !== this.requestSeq) {
        return;
      }
      this.data = JSON.parse(jsonText) as TalkPreviewData;
      this.currentPage = Math.max(0, Math.min(this.data.startPageIndex ?? 0, Math.max(0, (this.data.pages?.length ?? 1) - 1)));
      this.lastError = undefined;
    } catch (e) {
      if (seq !== this.requestSeq) {
        return;
      }
      this.lastError = (e as Error)?.message ?? String(e);
    }
    this.postState();
  }

  private cancelRunning(): void {
    if (this.runningChild && !this.runningChild.killed) {
      try {
        this.runningChild.kill();
      } catch {
        // ignore
      }
    }
    this.runningChild = undefined;
  }

  private scheduleRefresh(delay = 300): void {
    if (this.timer) {
      clearTimeout(this.timer);
    }
    this.timer = setTimeout(() => void this.refresh(), delay);
  }

  private postState(): void {
    if (!this.panel || !this.ready) {
      return;
    }
    void this.panel.webview.postMessage({
      type: 'talkPreview',
      data: this.data,
      currentPage: this.currentPage,
      followCursor: this.followCursor,
      error: this.lastError,
    });
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
      case 'selectBlock':
        this.pinnedBlockIndex = msg.index;
        await this.refresh();
        break;
      case 'selectPage':
        this.currentPage = msg.pageIndex;
        break;
      case 'setFollowCursor':
        this.followCursor = msg.value;
        break;
      case 'openLine':
        if (this.targetUri) {
          try {
            const doc = await vscode.workspace.openTextDocument(this.targetUri);
            const editor = await vscode.window.showTextDocument(doc, vscode.ViewColumn.One);
            const line = Math.max(0, (msg.line || 1) - 1);
            const pos = new vscode.Position(line, 0);
            editor.selection = new vscode.Selection(pos, pos);
            editor.revealRange(new vscode.Range(pos, pos), vscode.TextEditorRevealType.InCenterIfOutsideViewport);
          } catch {
            // ignore
          }
        }
        break;
      case 'rendered':
        this.rendered = true;
        this.currentPage = msg.currentPage;
        break;
    }
  }

  public dispose(): void {
    if (this.timer) {
      clearTimeout(this.timer);
    }
    this.cancelRunning();
    this.panel?.dispose();
    for (const d of this.disposables) {
      d.dispose();
    }
  }
}

const STYLE = `
body { margin: 0; padding: 12px 16px; display: flex; flex-direction: column; gap: 10px; }
.toolbar { display: flex; flex-wrap: wrap; align-items: center; justify-content: space-between; gap: 8px; border-bottom: 1px solid var(--vscode-panel-border); padding-bottom: 10px; }
.title-area h2 { margin: 0; font-size: 1.1em; }
.controls { display: flex; flex-wrap: wrap; align-items: center; gap: 8px; }
.controls select { max-width: 360px; }
.nav { display: flex; align-items: center; gap: 6px; }
.page-label { min-width: 80px; text-align: center; font-variant-numeric: tabular-nums; }
.follow { display: inline-flex; align-items: center; gap: 4px; cursor: pointer; font-size: 0.9em; }
.warnings { border: 1px solid var(--vscode-editorWarning-foreground); background: color-mix(in srgb, var(--vscode-editorWarning-foreground) 10%, transparent); border-radius: 4px; padding: 6px 10px; font-size: 0.9em; }
.stage { display: flex; flex-direction: column; align-items: flex-start; gap: 10px; }
.preview-img { max-width: 100%; border: 1px solid var(--vscode-panel-border); image-rendering: pixelated; cursor: pointer; display: block; user-select: none; }
.preview-img:focus { outline: 1px solid var(--vscode-focusBorder); }
.meta { width: 100%; border: 1px solid var(--vscode-panel-border); border-radius: 4px; padding: 8px 12px; background: var(--vscode-editorWidget-background); box-sizing: border-box; font-size: 0.9em; }
.meta-row { display: flex; flex-wrap: wrap; gap: 12px; margin-bottom: 4px; color: var(--vscode-descriptionForeground); }
.meta-text { white-space: pre-wrap; font-family: var(--vscode-editor-font-family); margin: 0; }
.empty { padding: 24px; color: var(--vscode-descriptionForeground); }
`;

const BODY = `
<div class="toolbar">
  <div class="title-area">
    <h2 id="title">会話のプレビュー</h2>
  </div>
  <div class="controls">
    <select id="block-select" title="Talk / AutoTalk のブロック"></select>
    <div class="nav">
      <button id="btn-prev" class="secondary">前へ</button>
      <span id="page-label" class="page-label">0 / 0</span>
      <button id="btn-next">次へ</button>
    </div>
    <button id="btn-jump" class="secondary">行へ飛ぶ</button>
    <button id="btn-refresh" class="secondary">再描画</button>
    <label class="follow"><input type="checkbox" id="chk-follow" checked> カーソルに追従</label>
  </div>
</div>
<div id="error" class="error" hidden></div>
<div id="warnings" class="warnings" hidden></div>
<div class="stage">
  <img id="preview-img" class="preview-img" tabindex="0" alt="メッセージウィンドウのプレビュー（クリックで次へ）" hidden>
  <div id="empty" class="empty">Talk または AutoTalk のブロックが見つかりません。</div>
  <div id="meta" class="meta" hidden>
    <div id="meta-row" class="meta-row"></div>
    <pre id="meta-text" class="meta-text"></pre>
  </div>
</div>
`;

const SCRIPT = `
(function () {
  const titleEl = document.getElementById('title');
  const blockSelect = document.getElementById('block-select');
  const btnPrev = document.getElementById('btn-prev');
  const btnNext = document.getElementById('btn-next');
  const btnJump = document.getElementById('btn-jump');
  const btnRefresh = document.getElementById('btn-refresh');
  const chkFollow = document.getElementById('chk-follow');
  const pageLabel = document.getElementById('page-label');
  const errorEl = document.getElementById('error');
  const warningsEl = document.getElementById('warnings');
  const imgEl = document.getElementById('preview-img');
  const emptyEl = document.getElementById('empty');
  const metaEl = document.getElementById('meta');
  const metaRow = document.getElementById('meta-row');
  const metaText = document.getElementById('meta-text');

  let data = null;
  let currentPage = 0;

  function renderPage() {
    const pages = (data && data.pages) || [];
    if (pages.length === 0) {
      imgEl.hidden = true;
      metaEl.hidden = true;
      emptyEl.hidden = false;
      pageLabel.textContent = '0 / 0';
      btnPrev.disabled = true;
      btnNext.disabled = true;
      btnJump.disabled = true;
      return;
    }

    currentPage = Math.max(0, Math.min(currentPage, pages.length - 1));
    const page = pages[currentPage];
    emptyEl.hidden = true;
    pageLabel.textContent = (currentPage + 1) + ' / ' + pages.length;
    btnPrev.disabled = currentPage <= 0;
    btnNext.disabled = currentPage >= pages.length - 1;
    btnJump.disabled = !page.line;

    if (page.pngBase64) {
      imgEl.src = 'data:image/png;base64,' + page.pngBase64;
      imgEl.hidden = false;
    } else {
      imgEl.hidden = true;
    }

    metaEl.hidden = false;
    const parts = [
      'コマンド: ' + (page.command || 'Talk'),
      '行: ' + (page.line || '-'),
      '話者: ' + (page.speaker || page.rawSpeaker || '(地の文)'),
    ];
    if (page.imageOptions) {
      parts.push('画像指定: ' + page.imageOptions);
    }
    metaRow.textContent = parts.join('　/　');
    metaText.textContent = (page.renderedLines && page.renderedLines.length > 0)
      ? page.renderedLines.join('\\n')
      : (page.text || '');

    vscode.postMessage({ type: 'selectPage', pageIndex: currentPage });
    vscode.postMessage({ type: 'rendered', blocks: (data.blocks || []).length, pages: pages.length, currentPage });
  }

  function step(delta) {
    const pages = (data && data.pages) || [];
    if (pages.length === 0) return;
    const next = currentPage + delta;
    if (next >= 0 && next < pages.length) {
      currentPage = next;
      renderPage();
    }
  }

  btnPrev.addEventListener('click', () => step(-1));
  btnNext.addEventListener('click', () => step(1));
  imgEl.addEventListener('click', () => {
    const pages = (data && data.pages) || [];
    if (pages.length === 0) return;
    currentPage = (currentPage + 1) < pages.length ? currentPage + 1 : 0;
    renderPage();
  });
  imgEl.addEventListener('keydown', (e) => {
    if (e.key === ' ' || e.key === 'Enter' || e.key === 'ArrowRight') {
      e.preventDefault();
      step(1);
    } else if (e.key === 'ArrowLeft') {
      e.preventDefault();
      step(-1);
    }
  });
  blockSelect.addEventListener('change', () => {
    const idx = Number(blockSelect.value);
    if (!Number.isNaN(idx)) {
      vscode.postMessage({ type: 'selectBlock', index: idx });
    }
  });
  btnJump.addEventListener('click', () => {
    const pages = (data && data.pages) || [];
    const page = pages[currentPage];
    if (page && page.line) {
      vscode.postMessage({ type: 'openLine', line: page.line });
    }
  });
  btnRefresh.addEventListener('click', () => vscode.postMessage({ type: 'refresh' }));
  chkFollow.addEventListener('change', () => vscode.postMessage({ type: 'setFollowCursor', value: chkFollow.checked }));

  window.addEventListener('message', (event) => {
    const msg = event.data;
    if (!msg || msg.type !== 'talkPreview') return;
    if (msg.error) {
      errorEl.textContent = msg.error;
      errorEl.hidden = false;
    } else {
      errorEl.hidden = true;
    }
    chkFollow.checked = !!msg.followCursor;
    data = msg.data;
    if (!data) {
      renderPage();
      return;
    }

    titleEl.textContent = '会話のプレビュー: ' + (data.file ? data.file.split(/[\\\\/]/).pop() : '');
    const blocks = data.blocks || [];
    blockSelect.innerHTML = '';
    for (const b of blocks) {
      const opt = document.createElement('option');
      opt.value = String(b.index);
      const labelPrefix = b.label ? b.label + ' / ' : '';
      opt.textContent = '#' + (b.index + 1) + ' ' + labelPrefix + b.command + ' (' + b.startLine + '行): ' + (b.previewText || '');
      if (b.index === data.selectedBlockIndex) {
        opt.selected = true;
      }
      blockSelect.appendChild(opt);
    }
    blockSelect.disabled = blocks.length === 0;

    const warnings = data.warnings || [];
    if (warnings.length > 0) {
      warningsEl.textContent = warnings.join(' / ');
      warningsEl.hidden = false;
    } else {
      warningsEl.hidden = true;
    }

    currentPage = msg.currentPage ?? data.startPageIndex ?? 0;
    renderPage();
    if ((data.pages || []).length === 0) {
      vscode.postMessage({ type: 'rendered', blocks: blocks.length, pages: 0, currentPage: 0 });
    }
  });

  vscode.postMessage({ type: 'ready' });
})();
`;
