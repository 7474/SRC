import * as fs from 'fs';
import * as path from 'path';
import * as vscode from 'vscode';
import { webviewHtml } from './webview';

/** DapServer.ScreenCommand（画面の独自のリクエストと、終わる時のイベント） */
const SCREEN_COMMAND = 'srcsharpScreen';
/** InteractiveScreen（interactive で起動した時の独自のイベントとリクエスト） */
const FRAME_EVENT = 'srcsharpFrame';
const INPUT_COMMAND = 'srcsharpInput';
const PROMPT_EVENT = 'srcsharpPrompt';
const PROMPT_RESULT_COMMAND = 'srcsharpPromptResult';
/** interactive でない時に、実行中の画面を取り直す間隔 */
const POLL_INTERVAL_MS = 500;

interface ScreenBody {
  running?: boolean;
  paused?: boolean;
  png?: string;
}

interface FrameBody {
  format: string;
  data: string;
}

interface PromptBody {
  id: number;
  kind: 'input' | 'open' | 'save';
  title?: string;
  message?: string;
  value?: string;
  initialDirectory?: string;
  fileType?: string;
  extension?: string;
}

interface Image {
  mime: string;
  data: string;
}

/** interactive（タブで操作する）で起動したか。.play スクリプトがあればスクリプトで動かす */
export function isInteractive(session: vscode.DebugSession): boolean {
  return !!session.configuration.interactive && !session.configuration.script;
}

/**
 * デバッグ中のシナリオの画面のタブ。
 * interactive なら、送られてくるフレームを出し、クリック・キーをシナリオに渡す。
 * そうでなければ、止まった時と終わった時に取り直し、実行中は一定の間隔で取り直す（表示している間だけ）。
 * 自動で進めるときもフレームを送る srcsharp なら、フレームを出し、間隔での取り直しはしない
 */
export class ScreenView implements vscode.Disposable {
  private panel: vscode.WebviewPanel | undefined;
  private session: vscode.DebugSession | undefined;
  private interactive = false;
  /** このセッションでフレームが送られてきたか */
  private framed = false;
  private timer: NodeJS.Timeout | undefined;
  private requesting = false;
  private unsupported = false;
  private image: Image | undefined;
  private status: 'idle' | 'running' | 'paused' | 'ended' = 'idle';
  private reload: { text: string; waiting: boolean; error: boolean } | undefined;
  private readonly disposables: vscode.Disposable[] = [];

  constructor() {
    this.disposables.push(
      vscode.debug.onDidStartDebugSession((s) => {
        if (s.type !== 'srcsharp') {
          return;
        }
        this.session = s;
        this.reload = undefined;
        this.interactive = isInteractive(s);
        this.framed = false;
        this.unsupported = false;
        this.status = 'running';
        if (this.interactive || vscode.workspace.getConfiguration('srcsharp').get<boolean>('screen.autoOpen', true)) {
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
        if (e.session !== this.session) {
          return;
        }
        if (e.event === FRAME_EVENT) {
          const body = e.body as FrameBody;
          this.image = { mime: `image/${body.format}`, data: body.data };
          void this.panel?.webview.postMessage({ type: 'frame', src: this.dataUrl() });
          if (!this.framed) {
            this.framed = true;
            this.updateTimer();
          }
        } else if (e.event === PROMPT_EVENT) {
          void this.prompt(e.session, e.body as PromptBody);
        } else if (e.event === SCREEN_COMMAND) {
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
              if (!this.interactive) {
                void this.refresh();
              }
              this.post();
            } else if (m.event === 'continued') {
              this.status = 'running';
              this.post();
            }
            this.updateTimer();
          },
          onWillReceiveMessage: (m: { type?: string; command?: string }) => {
            // 再開のリクエスト（continue・next など）を送ったら実行中に戻す
            if (s === this.session && m.type === 'request' && ['continue', 'next', 'stepIn', 'stepOut'].includes(m.command ?? '')) {
              this.status = 'running';
              this.post();
              this.updateTimer();
            }
          },
        }),
      }),
    );
  }

  /** 画面のタブを開く */
  show(preserveFocus = false): void {
    if (this.panel) {
      this.panel.reveal(undefined, preserveFocus);
      return;
    }
    this.panel = vscode.window.createWebviewPanel(
      'srcsharp.screen',
      'SRC# 画面',
      { viewColumn: vscode.ViewColumn.Beside, preserveFocus },
      { enableScripts: true, retainContextWhenHidden: true },
    );
    this.panel.webview.html = this.html(this.panel.webview);
    this.panel.webview.onDidReceiveMessage((m: { type: string; input?: object }) => {
      if (m.type === 'ready') {
        this.post();
        void this.refresh();
      } else if (m.type === 'input' && m.input) {
        this.input(m.input);
      } else if (m.type === 'refresh') {
        void this.refresh();
      } else if (m.type === 'save') {
        void this.save();
      } else if (m.type === 'reload') {
        void vscode.commands.executeCommand('srcsharp.reload');
      } else if (m.type === 'cancelReload') {
        void vscode.commands.executeCommand('srcsharp.cancelReload');
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

  private input(args: object): void {
    const session = this.session;
    if (!session || !this.interactive) {
      return;
    }
    // 終わった直後などの失敗は捨てる
    session.customRequest(INPUT_COMMAND, args).then(undefined, () => undefined);
  }

  /** シナリオの文字入力・ファイル選択を VS Code の入力欄・ダイアログで尋ねる */
  private async prompt(session: vscode.DebugSession, body: PromptBody): Promise<void> {
    let value: string | undefined;
    try {
      if (body.kind === 'input') {
        value = await vscode.window.showInputBox({
          title: body.title || 'SRC',
          prompt: body.message,
          value: body.value ?? '',
          ignoreFocusOut: true,
        });
      } else {
        const dir = body.initialDirectory || path.dirname(session.configuration.program ?? '');
        const defaultUri = dir ? vscode.Uri.file(body.value ? path.join(dir, body.value) : dir) : undefined;
        const filters = body.extension ? { [body.fileType || body.extension]: [body.extension] } : undefined;
        if (body.kind === 'open') {
          const picked = await vscode.window.showOpenDialog({ title: body.title, defaultUri, filters, canSelectMany: false });
          value = picked?.[0]?.fsPath;
        } else {
          value = (await vscode.window.showSaveDialog({ title: body.title, defaultUri, filters }))?.fsPath;
        }
      }
    } finally {
      await session.customRequest(PROMPT_RESULT_COMMAND, { id: body.id, value: value ?? null }).then(undefined, () => undefined);
    }
  }

  private updateTimer(): void {
    const poll = !!this.panel?.visible && !!this.session && !this.interactive && !this.framed && this.status === 'running' && !this.unsupported;
    if (poll && !this.timer) {
      this.timer = setInterval(() => void this.refresh(), POLL_INTERVAL_MS);
    } else if (!poll && this.timer) {
      clearInterval(this.timer);
      this.timer = undefined;
    }
  }

  private apply(body: ScreenBody | undefined): void {
    if (body?.png) {
      this.image = { mime: 'image/png', data: body.png };
    }
  }

  private dataUrl(): string | undefined {
    return this.image ? `data:${this.image.mime};base64,${this.image.data}` : undefined;
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
    void this.panel?.webview.postMessage({
      type: 'state',
      status: this.status,
      src: this.dataUrl(),
      interactive: this.interactive && this.status !== 'ended',
      unsupported: this.unsupported,
      reload: this.status === 'ended' ? undefined : this.reload,
    });
  }

  /** ホットリロードの状態を出す（undefined で消す） */
  setReloadStatus(reload: { text: string; waiting: boolean; error: boolean } | undefined): void {
    this.reload = reload;
    this.post();
  }

  private async save(): Promise<void> {
    // 実行中なら PNG で撮り直す（送られてくるフレームは WebP）
    if (this.session && !this.unsupported) {
      try {
        this.apply((await this.session.customRequest(SCREEN_COMMAND)) as ScreenBody);
      } catch {
        // 終わった直後は手元の画面を保存する
      }
    }
    const image = this.image;
    if (!image) {
      return;
    }
    const ext = image.mime === 'image/png' ? 'png' : image.mime.replace('image/', '');
    const uri = await vscode.window.showSaveDialog({
      filters: { [ext.toUpperCase()]: [ext] },
      defaultUri: vscode.workspace.workspaceFolders?.[0]
        ? vscode.Uri.joinPath(vscode.workspace.workspaceFolders[0].uri, `screen.${ext}`)
        : undefined,
    });
    if (uri) {
      await fs.promises.writeFile(uri.fsPath, Buffer.from(image.data, 'base64'));
    }
  }

  private html(webview: vscode.Webview): string {
    return webviewHtml(
      webview,
      'SRC# 画面',
      `
.bar { display: flex; gap: 8px; align-items: center; margin-bottom: 8px; flex-wrap: wrap; }
.bar .status { flex: 1; }
.screen { max-width: 100%; image-rendering: pixelated; border: 1px solid var(--vscode-panel-border); display: block; user-select: none; -webkit-user-drag: none; touch-action: none; }
.screen.interactive { cursor: pointer; }
.screen:focus { outline: 1px solid var(--vscode-focusBorder); }
#empty { padding: 24px 0; }
`,
      `
<div class="bar">
  <span class="status" id="status"></span>
  <button class="secondary" id="reload" title="イベント・データ・画像を、プレイの状態を保ったまま読み直す">読み直す</button>
  <button class="secondary" id="refresh">取り直す</button>
  <button class="secondary" id="save" disabled>画像を保存</button>
</div>
<div class="bar" id="reloadBar" hidden>
  <span class="status" id="reloadStatus"></span>
  <button class="secondary" id="cancelReload" hidden>取りやめる</button>
</div>
<div id="unsupported" class="error" hidden>この srcsharp は画面の取得に対応していません。操作パネルの「srcsharp を入れる／更新する」で更新してください。</div>
<img class="screen" id="screen" alt="SRC の画面" tabindex="0" draggable="false" hidden>
<div id="empty" class="muted">
  <p>SRC# のイベントを実行すると、ここに画面を出します。画面をクリック・キーで操作できます（Enter・Space で決定、Esc で取り消し、矢印で選択）。</p>
  <button id="start">開いているイベントを実行</button>
</div>
`,
      `
const labels = { idle: 'デバッグしていません', running: '実行中', paused: '止まっています', ended: '終わりました' };
const img = document.getElementById('screen');
let interactive = false;
document.getElementById('refresh').addEventListener('click', () => vscode.postMessage({ type: 'refresh' }));
document.getElementById('save').addEventListener('click', () => vscode.postMessage({ type: 'save' }));
document.getElementById('start').addEventListener('click', () => vscode.postMessage({ type: 'start' }));
document.getElementById('reload').addEventListener('click', () => vscode.postMessage({ type: 'reload' }));
document.getElementById('cancelReload').addEventListener('click', () => vscode.postMessage({ type: 'cancelReload' }));

function show(src) {
  if (!src) return;
  img.src = src;
  img.hidden = false;
  document.getElementById('empty').hidden = true;
  document.getElementById('save').disabled = false;
}

window.addEventListener('message', (e) => {
  const m = e.data;
  if (m.type === 'frame') {
    show(m.src);
    return;
  }
  if (m.type !== 'state') return;
  interactive = m.interactive;
  img.classList.toggle('interactive', interactive);
  let status = labels[m.status] || '';
  if (interactive && m.status === 'running') status += '（クリック・キーで操作）';
  document.getElementById('status').textContent = status;
  document.getElementById('unsupported').hidden = !m.unsupported;
  show(m.src);
  document.getElementById('refresh').disabled = interactive || m.status === 'idle' || m.status === 'ended';
  document.getElementById('reload').disabled = m.status === 'idle' || m.status === 'ended';
  document.getElementById('reloadBar').hidden = !m.reload;
  if (m.reload) {
    const r = document.getElementById('reloadStatus');
    r.textContent = m.reload.text;
    r.className = 'status ' + (m.reload.error ? 'error' : 'muted');
    document.getElementById('cancelReload').hidden = !m.reload.waiting;
  }
});

// 画面の座標（拡大・縮小を戻す）
function point(e) {
  const rect = img.getBoundingClientRect();
  return { x: (e.clientX - rect.left) * img.naturalWidth / rect.width, y: (e.clientY - rect.top) * img.naturalHeight / rect.height };
}
function send(input) {
  if (interactive) vscode.postMessage({ type: 'input', input });
}
img.addEventListener('pointerdown', (e) => {
  if (!interactive) return;
  img.focus();
  img.setPointerCapture(e.pointerId);
  const p = point(e);
  send({ type: 'pointer', kind: 'move', x: p.x, y: p.y });
  send({ type: 'pointer', kind: 'down', x: p.x, y: p.y, button: e.button, clicks: e.detail || 1 });
  e.preventDefault();
});
img.addEventListener('pointerup', (e) => {
  if (!interactive) return;
  const p = point(e);
  send({ type: 'pointer', kind: 'up', x: p.x, y: p.y, button: e.button });
  e.preventDefault();
});
// 移動はまとめて送る（最後の位置だけ、約 30 回／秒）
let moveTimer = null;
let lastMove = null;
img.addEventListener('pointermove', (e) => {
  if (!interactive) return;
  lastMove = point(e);
  if (moveTimer) return;
  moveTimer = setTimeout(() => {
    moveTimer = null;
    send({ type: 'pointer', kind: 'move', x: lastMove.x, y: lastMove.y });
  }, 33);
});
img.addEventListener('wheel', (e) => {
  if (!interactive) return;
  const p = point(e);
  const delta = -Math.sign(e.deltaY);
  if (delta !== 0) send({ type: 'wheel', x: p.x, y: p.y, delta });
  e.preventDefault();
}, { passive: false });
img.addEventListener('contextmenu', (e) => e.preventDefault());
// 画面で使うキーは VS Code に渡さない（ページのスクロールなど）
const used = new Set([8, 13, 27, 32, 37, 38, 39, 40, 46]);
img.addEventListener('keydown', (e) => {
  if (!interactive || e.ctrlKey || e.metaKey || e.altKey) return;
  send({ type: 'key', kind: 'down', keyCode: e.keyCode });
  if (used.has(e.keyCode)) e.preventDefault();
});
img.addEventListener('keyup', (e) => {
  if (!interactive) return;
  send({ type: 'key', kind: 'up', keyCode: e.keyCode });
});
img.addEventListener('blur', () => send({ type: 'key', kind: 'blur' }));
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
