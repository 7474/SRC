import * as fs from 'fs';
import * as vscode from 'vscode';
import { webviewHtml } from './webview';

/** DapServer.ScreenCommand（画面の独自のリクエストとイベント） */
const SCREEN_COMMAND = 'srcsharpScreen';
/** 実行中に画面を取り直す間隔 */
const POLL_INTERVAL_MS = 500;

interface ScreenBody {
  running?: boolean;
  paused?: boolean;
  png?: string;
}

/**
 * デバッグ中のシナリオの画面を出す Webview。
 * 止まった時と終わった時に取り直し、実行中は一定の間隔で取り直す（表示している間だけ）
 */
export class ScreenView implements vscode.Disposable {
  private panel: vscode.WebviewPanel | undefined;
  private session: vscode.DebugSession | undefined;
  private timer: NodeJS.Timeout | undefined;
  private requesting = false;
  private unsupported = false;
  private lastPng: string | undefined;
  private status: 'idle' | 'running' | 'paused' | 'ended' = 'idle';
  private readonly disposables: vscode.Disposable[] = [];

  constructor() {
    this.disposables.push(
      vscode.debug.onDidStartDebugSession((s) => {
        if (s.type !== 'srcsharp') {
          return;
        }
        this.session = s;
        this.unsupported = false;
        this.status = 'running';
        if (vscode.workspace.getConfiguration('srcsharp').get<boolean>('screen.autoOpen', true)) {
          this.show(true);
        }
        this.post();
        this.updateTimer();
      }),
      vscode.debug.onDidTerminateDebugSession((s) => {
        if (s === this.session) {
          this.session = undefined;
          this.status = 'ended';
          this.post();
          this.updateTimer();
        }
      }),
      vscode.debug.onDidReceiveDebugSessionCustomEvent((e) => {
        if (e.session === this.session && e.event === SCREEN_COMMAND) {
          // 終わる時に送られる最後の画面
          this.apply(e.body as ScreenBody);
          this.status = 'ended';
          this.post();
          this.updateTimer();
        }
      }),
      vscode.debug.registerDebugAdapterTrackerFactory('srcsharp', {
        createDebugAdapterTracker: (s) => ({
          onDidSendMessage: (m: { type?: string; event?: string }) => {
            if (s !== this.session || m.type !== 'event') {
              return;
            }
            if (m.event === 'stopped') {
              this.status = 'paused';
              void this.refresh();
            } else if (m.event === 'continued') {
              this.status = 'running';
            }
            this.updateTimer();
          },
          onWillReceiveMessage: (m: { type?: string; command?: string }) => {
            // 再開のリクエスト（continue・next など）を送ったら実行中に戻す
            if (s === this.session && m.type === 'request' && ['continue', 'next', 'stepIn', 'stepOut'].includes(m.command ?? '')) {
              this.status = 'running';
              this.updateTimer();
            }
          },
        }),
      }),
    );
  }

  /** 画面のビューを開く */
  show(preserveFocus = false): void {
    if (this.panel) {
      this.panel.reveal(undefined, preserveFocus);
      return;
    }
    this.panel = vscode.window.createWebviewPanel(
      'srcsharp.screen',
      'SRC# 画面',
      { viewColumn: vscode.ViewColumn.Beside, preserveFocus },
      { enableScripts: true },
    );
    this.panel.webview.html = this.html(this.panel.webview);
    this.panel.webview.onDidReceiveMessage((m: { type: string }) => {
      if (m.type === 'ready') {
        this.post();
        void this.refresh();
      } else if (m.type === 'refresh') {
        void this.refresh();
      } else if (m.type === 'save') {
        void this.save();
      } else if (m.type === 'start') {
        void vscode.commands.executeCommand('srcsharp.runFile');
      }
    });
    this.panel.onDidChangeViewState(() => this.updateTimer());
    this.panel.onDidDispose(() => {
      this.panel = undefined;
      this.updateTimer();
    });
    this.updateTimer();
  }

  private updateTimer(): void {
    const poll = !!this.panel?.visible && !!this.session && this.status === 'running' && !this.unsupported;
    if (poll && !this.timer) {
      this.timer = setInterval(() => void this.refresh(), POLL_INTERVAL_MS);
    } else if (!poll && this.timer) {
      clearInterval(this.timer);
      this.timer = undefined;
    }
  }

  private apply(body: ScreenBody | undefined): void {
    if (body?.png) {
      this.lastPng = body.png;
    }
  }

  /** 画面を取り直す */
  async refresh(): Promise<void> {
    const session = this.session;
    if (!session || !this.panel || this.requesting || this.unsupported) {
      return;
    }
    this.requesting = true;
    try {
      const body = (await session.customRequest(SCREEN_COMMAND)) as ScreenBody;
      this.apply(body);
      if (body.paused) {
        this.status = 'paused';
      }
    } catch (e) {
      // 0.5.1 までの srcsharp は画面のリクエストに対応していない（NotSupportedException の文面はコマンド名）
      if (session === this.session && String((e as Error)?.message ?? e).includes(SCREEN_COMMAND)) {
        this.unsupported = true;
      }
    } finally {
      this.requesting = false;
    }
    this.post();
    this.updateTimer();
  }

  private post(): void {
    void this.panel?.webview.postMessage({ type: 'state', status: this.status, png: this.lastPng, unsupported: this.unsupported });
  }

  private async save(): Promise<void> {
    if (!this.lastPng) {
      return;
    }
    const uri = await vscode.window.showSaveDialog({
      filters: { PNG: ['png'] },
      defaultUri: vscode.workspace.workspaceFolders?.[0]
        ? vscode.Uri.joinPath(vscode.workspace.workspaceFolders[0].uri, 'screen.png')
        : undefined,
    });
    if (uri) {
      await fs.promises.writeFile(uri.fsPath, Buffer.from(this.lastPng, 'base64'));
    }
  }

  private html(webview: vscode.Webview): string {
    return webviewHtml(
      webview,
      'SRC# 画面',
      `
.bar { display: flex; gap: 8px; align-items: center; margin-bottom: 8px; flex-wrap: wrap; }
.bar .status { flex: 1; }
.screen { max-width: 100%; image-rendering: pixelated; border: 1px solid var(--vscode-panel-border); display: block; }
#empty { padding: 24px 0; }
`,
      `
<div class="bar">
  <span class="status" id="status"></span>
  <button class="secondary" id="refresh">取り直す</button>
  <button class="secondary" id="save" disabled>PNG で保存</button>
</div>
<div id="unsupported" class="error" hidden>この srcsharp は画面の取得に対応していません。操作パネルの「srcsharp を入れる／更新する」で更新してください。</div>
<img class="screen" id="screen" alt="SRC の画面" hidden>
<div id="empty" class="muted">
  <p>SRC# のイベントをデバッグ（実行）すると、ここに画面を出します。止まった時と終わった時に取り直し、実行中は 0.5 秒ごとに取り直します。</p>
  <button id="start">開いているイベントを実行</button>
</div>
`,
      `
const labels = { idle: 'デバッグしていません', running: '実行中', paused: '止まっています', ended: '終わりました' };
document.getElementById('refresh').addEventListener('click', () => vscode.postMessage({ type: 'refresh' }));
document.getElementById('save').addEventListener('click', () => vscode.postMessage({ type: 'save' }));
document.getElementById('start').addEventListener('click', () => vscode.postMessage({ type: 'start' }));
window.addEventListener('message', (e) => {
  const m = e.data;
  if (m.type !== 'state') return;
  document.getElementById('status').textContent = labels[m.status] || '';
  document.getElementById('unsupported').hidden = !m.unsupported;
  const img = document.getElementById('screen');
  if (m.png) {
    img.src = 'data:image/png;base64,' + m.png;
    img.hidden = false;
  }
  document.getElementById('empty').hidden = !!m.png;
  document.getElementById('save').disabled = !m.png;
  document.getElementById('refresh').disabled = m.status === 'idle' || m.status === 'ended';
});
vscode.postMessage({ type: 'ready' });
`,
    );
  }

  dispose(): void {
    if (this.timer) {
      clearInterval(this.timer);
    }
    this.panel?.dispose();
    this.disposables.forEach((d) => d.dispose());
  }
}
