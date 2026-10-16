import * as fs from 'fs';
import * as path from 'path';
import * as vscode from 'vscode';
import type { ScreenView } from './screen';

/** HotReloader（デバッグアダプタの読み直しのリクエストと、結果のイベント） */
const RELOAD = 'srcsharpReload';
/** 変更をまとめる時間（保存が続いた時に 1 度だけ読み直す） */
const DEBOUNCE_MS = 400;

interface Kinds {
  events?: boolean;
  data?: boolean;
  images?: boolean;
}

interface ReloadBody {
  status: 'waiting' | 'reloaded' | 'failed' | 'canceled';
  message: string;
  diagnostics?: { file?: string; line?: number; severity: string; message: string }[];
}

const IMAGE = /\.(bmp|png|jpe?g|gif)$/i;

/** ファイルの変化を読み直しの種類にする。関係の無いファイルは undefined */
export function kindOf(file: string): keyof Kinds | undefined {
  const lower = file.toLowerCase();
  if (lower.endsWith('.eve')) {
    return 'events';
  }
  if (lower.endsWith('.txt') && file.split(/[\\/]/).some((x) => x.toLowerCase() === 'data')) {
    return 'data';
  }
  if (IMAGE.test(file)) {
    return 'images';
  }
  return undefined;
}

/** シナリオのフォルダ（src.ini か Data のある、いちばん近い親。ScenarioAnalyzer.FindScenarioRoot と同じ） */
export function scenarioRoot(file: string): string {
  const dir = path.dirname(file);
  for (let d = dir; ; d = path.dirname(d)) {
    if (['src.ini', 'Src.ini', 'Data', 'data'].some((x) => fs.existsSync(path.join(d, x)))) {
      return d;
    }
    if (path.dirname(d) === d) {
      return dir;
    }
  }
}

/**
 * デバッグ中のシナリオのホットリロード。シナリオのフォルダと SRC 本体のフォルダのファイルの変化
 * （VS Code での保存も、外での変更も）を見て、デバッグアダプタに読み直しを頼む（srcsharp.hotReload）
 */
export class HotReload implements vscode.Disposable {
  private session: vscode.DebugSession | undefined;
  private watchers: vscode.Disposable[] = [];
  private pending: Kinds = {};
  private files = new Set<string>();
  private timer: NodeJS.Timeout | undefined;
  private unsupported = false;
  private readonly disposables: vscode.Disposable[] = [];

  constructor(
    private readonly output: vscode.OutputChannel,
    private readonly screen: ScreenView,
  ) {
    this.disposables.push(
      vscode.debug.onDidStartDebugSession((s) => {
        if (s.type === 'srcsharp' && s.configuration.program) {
          this.attach(s);
        }
      }),
      vscode.debug.onDidTerminateDebugSession((s) => {
        if (s === this.session) {
          this.detach();
        }
      }),
      vscode.debug.onDidReceiveDebugSessionCustomEvent((e) => {
        if (e.session === this.session && e.event === RELOAD) {
          this.report(e.body as ReloadBody);
        }
      }),
      vscode.workspace.onDidChangeConfiguration((e) => {
        if (e.affectsConfiguration('srcsharp.hotReload') && this.session) {
          this.attach(this.session);
        }
      }),
    );
  }

  private mode(): 'onChange' | 'manual' | 'off' {
    return vscode.workspace.getConfiguration('srcsharp').get<'onChange' | 'manual' | 'off'>('hotReload', 'onChange');
  }

  private attach(session: vscode.DebugSession): void {
    this.detach();
    this.session = session;
    this.unsupported = false;
    if (this.mode() !== 'onChange') {
      return;
    }
    const roots = new Set<string>([scenarioRoot(path.resolve(session.configuration.program as string))]);
    const appPath = (session.configuration.appPath as string | undefined) || vscode.workspace.getConfiguration('srcsharp').get<string>('appPath');
    if (appPath) {
      roots.add(path.resolve(appPath));
    }
    for (const root of roots) {
      const watcher = vscode.workspace.createFileSystemWatcher(new vscode.RelativePattern(vscode.Uri.file(root), '**/*.{eve,txt,bmp,png,jpg,jpeg,gif}'));
      const changed = (uri: vscode.Uri): void => this.changed(uri.fsPath);
      watcher.onDidChange(changed);
      watcher.onDidCreate(changed);
      this.watchers.push(watcher);
    }
  }

  private detach(): void {
    this.watchers.forEach((w) => w.dispose());
    this.watchers = [];
    this.session = undefined;
    this.pending = {};
    this.files.clear();
    if (this.timer) {
      clearTimeout(this.timer);
      this.timer = undefined;
    }
  }

  private changed(file: string): void {
    const kind = kindOf(file);
    // セーブデータ（_リスタート.srcq など）や SRC が書くファイルは対象外
    if (!kind || !this.session || this.unsupported) {
      return;
    }
    this.pending[kind] = true;
    this.files.add(file);
    if (this.timer) {
      clearTimeout(this.timer);
    }
    this.timer = setTimeout(() => {
      this.timer = undefined;
      const kinds = this.pending;
      const files = [...this.files];
      this.pending = {};
      this.files.clear();
      void this.request(kinds, files);
    }, DEBOUNCE_MS);
  }

  /** 読み直しを頼む。kinds が空ならすべて */
  async request(kinds: Kinds = {}, files: string[] = []): Promise<void> {
    const session = this.session;
    if (!session) {
      vscode.window.showInformationMessage('SRC#: 読み直すシナリオを実行していません。');
      return;
    }
    if (files.length) {
      this.output.appendLine(`[読み直し] ${files.map((f) => path.basename(f)).join(', ')}`);
    }
    try {
      await session.customRequest(RELOAD, kinds);
    } catch (e) {
      // 0.5.1 までの srcsharp は読み直しに対応していない
      if (String((e as Error)?.message ?? e).includes(RELOAD)) {
        this.unsupported = true;
        this.screen.setReloadStatus({ text: 'この srcsharp は読み直しに対応していません。更新してください', waiting: false, error: true });
      }
    }
  }

  async cancel(): Promise<void> {
    await this.session?.customRequest(RELOAD, { cancel: true }).then(undefined, () => undefined);
  }

  private report(body: ReloadBody): void {
    const time = new Date().toLocaleTimeString();
    for (const d of body.diagnostics ?? []) {
      const where = d.file ? `${d.file}${d.line ? `(${d.line})` : ''}: ` : '';
      this.output.appendLine(`  ${d.severity}: ${where}${d.message}`);
    }
    switch (body.status) {
      case 'waiting':
        this.screen.setReloadStatus({ text: `読み直し待ち: ${body.message}`, waiting: true, error: false });
        break;
      case 'reloaded': {
        const warnings = body.diagnostics?.length ? `（警告 ${body.diagnostics.length} 件。出力の「SRC#」を参照）` : '';
        this.screen.setReloadStatus({ text: `${time} ${body.message}${warnings}`, waiting: false, error: false });
        vscode.window.setStatusBarMessage(`$(flame) SRC#: ${body.message}`, 4000);
        break;
      }
      case 'failed': {
        this.screen.setReloadStatus({ text: `${time} ${body.message}`, waiting: false, error: true });
        const show = '出力を表示';
        void vscode.window.showWarningMessage(`SRC#: ${body.message}`, show).then((pick) => pick === show && this.output.show(true));
        break;
      }
      case 'canceled':
        this.screen.setReloadStatus(undefined);
        break;
    }
    this.output.appendLine(`[読み直し] ${body.message}`);
  }

  dispose(): void {
    this.detach();
    this.disposables.forEach((d) => d.dispose());
  }
}
