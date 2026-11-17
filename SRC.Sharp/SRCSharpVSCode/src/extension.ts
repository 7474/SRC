import * as path from 'path';
import { spawn } from 'child_process';
import * as vscode from 'vscode';
import { LanguageClient, LanguageClientOptions, ServerOptions } from 'vscode-languageclient/node';
import { pickAppPath, suggestAppPath } from './appPath';
import { MapEditorProvider } from './mapEditor';
import { DataPreviewPanel } from './preview';
import { HotReload, scenarioRoot } from './hotReload';
import { PanelProvider } from './panel';
import { ScreenView } from './screen';
import { installServer, resolveServer, serverCommand, ServerInfo, serverVersion } from './server';
import { SettingsView } from './settings';

/** データに動かす規則のグループ（設定 srcsharp.rules）。空なら none */
export function ruleGroups(): string[] {
  const rules = vscode.workspace.getConfiguration('srcsharp').get<string[]>('rules', ['src']);
  return rules.length ? rules : ['none'];
}

/** srcsharp と Language Server の状態 */
export class Controller implements vscode.Disposable {
  server: ServerInfo = resolveServer();
  version: string | undefined;
  clientState: 'running' | 'starting' | 'stopped' | 'error' = 'stopped';
  private client: LanguageClient | undefined;
  private readonly changed = new vscode.EventEmitter<void>();
  readonly onDidChange = this.changed.event;

  constructor(readonly output: vscode.LogOutputChannel) {}

  /** srcsharp を探し直し、版を調べる */
  async refresh(): Promise<void> {
    this.server = resolveServer();
    this.version = undefined;
    await vscode.commands.executeCommand('setContext', 'srcsharp.serverMissing', !this.server.found);
    this.changed.fire();
    this.version = await serverVersion(this.server);
    this.changed.fire();
  }

  /** Language Server（srcsharp lsp）を（起動し直して）起動する */
  async startClient(): Promise<void> {
    await this.stopClient();
    await this.refresh();
    if (!this.server.found) {
      return;
    }
    const lsp = serverCommand(this.server, 'lsp');
    const serverOptions: ServerOptions = { command: lsp.command, args: lsp.args, options: { env: this.server.env } };
    const clientOptions: LanguageClientOptions = {
      documentSelector: [
        { scheme: 'file', language: 'src-eve' },
        { scheme: 'file', language: 'src-data' },
      ],
      initializationOptions: {
        appPath: vscode.workspace.getConfiguration('srcsharp').get<string>('appPath') || undefined,
        // データに動かす規則のグループ（空なら none。srcsharp lint --rules と同じ）
        rules: ruleGroups(),
        logLevel: vscode.workspace.getConfiguration('srcsharp').get<string>('logLevel') || undefined,
      },
      // Language Server のログ（window/logMessage）も同じ出力パネルに出す
      outputChannel: this.output,
    };
    this.client = new LanguageClient('srcsharp', 'SRC#', serverOptions, clientOptions);
    this.setState('starting');
    try {
      await this.client.start();
      this.setState('running');
    } catch (e) {
      this.setState('error');
      this.output.error(`Language Server を起動できませんでした: ${(e as Error)?.message ?? e}`);
    }
  }

  async stopClient(): Promise<void> {
    const client = this.client;
    this.client = undefined;
    if (client) {
      try {
        await client.dispose();
      } catch {
        // 起動に失敗したクライアント
      }
    }
    this.setState('stopped');
  }

  async sendRequest<T = unknown>(method: string, params: unknown): Promise<T | undefined> {
    if (!this.client || this.clientState !== 'running') {
      return undefined;
    }
    return await this.client.sendRequest<T>(method, params);
  }

  private setState(state: Controller['clientState']): void {
    this.clientState = state;
    this.changed.fire();
  }

  dispose(): void {
    void this.stopClient();
    this.changed.dispose();
  }
}

let current: Controller | undefined;

/** srcsharp が見つからない時の案内 */
async function promptInstall(): Promise<void> {
  const install = '入れる';
  const settings = '設定を開く';
  const pick = await vscode.window.showWarningMessage(
    'SRC#: srcsharp が見つかりません。dotnet tool（SRCSharpTools）として入れますか？',
    install,
    settings,
  );
  if (pick === install) {
    await vscode.commands.executeCommand('srcsharp.installServer');
  } else if (pick === settings) {
    await vscode.commands.executeCommand('srcsharp.openSettings');
  }
}

/** 開いているイベントファイル */
function activeEventFile(): vscode.Uri | undefined {
  const doc = vscode.window.activeTextEditor?.document;
  return doc && (doc.languageId === 'src-eve' || doc.fileName.toLowerCase().endsWith('.eve')) ? doc.uri : undefined;
}

export async function activate(context: vscode.ExtensionContext): Promise<void> {
  // レベル付きの出力パネル（Developer: Set Log Level で絞れる）
  const output = vscode.window.createOutputChannel('SRC#', { log: true });
  const controller = new Controller(output);
  current = controller;
  const screen = new ScreenView();
  const settings = new SettingsView(controller);
  const hotReload = new HotReload(output, screen);
  context.subscriptions.push(output, controller, screen, settings, hotReload);

  // 状態バー
  const status = vscode.window.createStatusBarItem(vscode.StatusBarAlignment.Left, 0);
  status.name = 'SRC#';
  const updateStatus = (): void => {
    if (!controller.server.found) {
      status.text = '$(warning) SRC#';
      status.tooltip = 'srcsharp が見つかりません。クリックで設定';
    } else if (controller.clientState === 'error') {
      status.text = '$(error) SRC#';
      status.tooltip = 'Language Server を起動できませんでした。クリックで設定';
    } else {
      status.text = controller.clientState === 'starting' ? '$(loading~spin) SRC#' : '$(check) SRC#';
      status.tooltip = `srcsharp ${controller.version ?? ''}\n${controller.server.command}\nクリックで設定`;
    }
    status.command = 'srcsharp.openSettings';
  };
  context.subscriptions.push(status, controller.onDidChange(updateStatus));
  updateStatus();
  const showStatus = (): void => {
    const editor = vscode.window.activeTextEditor;
    if (editor && ['src-eve', 'src-data'].includes(editor.document.languageId)) {
      status.show();
    } else {
      status.hide();
    }
  };
  context.subscriptions.push(vscode.window.onDidChangeActiveTextEditor(showStatus));
  showStatus();

  // マップ（.map）のカスタムエディタ（SRC#Studio のマップ作成ツール。7474/SRC#1641）
  const mapEditor = new MapEditorProvider(context);
  interface PlacementAtResult {
    mapPath: string;
    fullMapPath: string;
    x: number;
    y: number;
    command: string;
    unit: string;
    file: string;
    line: number;
  }
  context.subscriptions.push(
    mapEditor,
    vscode.window.registerCustomEditorProvider(MapEditorProvider.viewType, mapEditor, { webviewOptions: { retainContextWhenHidden: true } }),
    // テスト用（画面の WebAssembly が動き、地形とマップを読めたか）。package.json には出さない
    vscode.commands.registerCommand('srcsharp._mapEditorStatus', (uri: vscode.Uri) => mapEditor.loaded.get(uri.toString())),
    // イベント行の座標をマップ作成ツール上で開いて確認する（7474/SRC#1653）
    vscode.commands.registerCommand('srcsharp.showOnMap', async (args?: { uri?: string; line?: number }) => {
      const editor = vscode.window.activeTextEditor;
      const targetUri = args?.uri ?? (editor?.document.languageId === 'src-eve' ? editor.document.uri.toString() : undefined);
      const targetLine = args?.line ?? editor?.selection.active.line;
      if (!targetUri || targetLine === undefined) {
        vscode.window.showInformationMessage('SRC#: イベントファイル（*.eve）の座標がある行を選んでください。');
        return;
      }
      if (controller.clientState !== 'running') {
        if (!controller.server.found) {
          void promptInstall();
        } else {
          vscode.window.showInformationMessage('SRC#: Language Server の起動を待ってからもう一度お試しください。');
        }
        return;
      }
      const res = await controller.sendRequest<PlacementAtResult | null>('srcsharp/placementAt', {
        textDocument: { uri: targetUri },
        position: { line: targetLine, character: 0 },
      });
      if (!res) {
        vscode.window.showInformationMessage('SRC#: この行にはマップ上の座標がありません。');
        return;
      }
      if (!res.fullMapPath) {
        vscode.window.showInformationMessage(`SRC#: マップファイル（${res.mapPath || '未指定'}）が見つかりません。`);
        return;
      }
      await mapEditor.openMapAt(res.fullMapPath, {
        file: res.file,
        line: res.line,
        x: res.x,
        y: res.y,
        command: res.command,
        unit: res.unit,
      });
    }),
    vscode.languages.registerHoverProvider({ scheme: 'file', language: 'src-eve' }, {
      async provideHover(document, position) {
        if (controller.clientState !== 'running') {
          return undefined;
        }
        const res = await controller.sendRequest<PlacementAtResult | null>('srcsharp/placementAt', {
          textDocument: { uri: document.uri.toString() },
          position: { line: position.line, character: position.character },
        });
        if (!res?.fullMapPath) {
          return undefined;
        }
        const q = encodeURIComponent(JSON.stringify([{ uri: document.uri.toString(), line: position.line }]));
        const md = new vscode.MarkdownString(`[マップの上で見る（${res.x}, ${res.y}）](command:srcsharp.showOnMap?${q})`);
        md.isTrusted = true;
        return new vscode.Hover(md);
      },
    }),
  );

  // データのプレビュー（SRC#Studio。7474/SRC#1643）
  const dataPreview = new DataPreviewPanel(context);
  context.subscriptions.push(
    dataPreview,
    vscode.commands.registerCommand('srcsharp.showPreview', () => dataPreview.show()),
    // テスト用（プレビューに出したもの）。package.json には出さない
    vscode.commands.registerCommand('srcsharp._previewStatus', () => dataPreview.lastShown),
  );

  // 操作パネル
  context.subscriptions.push(vscode.window.registerTreeDataProvider('srcsharp.panel', new PanelProvider(controller)));

  // デバッガ（srcsharp dap）
  context.subscriptions.push(
    vscode.debug.registerDebugAdapterDescriptorFactory('srcsharp', {
      createDebugAdapterDescriptor(): vscode.ProviderResult<vscode.DebugAdapterDescriptor> {
        const server = resolveServer();
        if (!server.found) {
          void promptInstall();
          return undefined;
        }
        const dap = serverCommand(server, 'dap');
        return new vscode.DebugAdapterExecutable(dap.command, dap.args, { env: server.env as { [key: string]: string } });
      },
    }),
    // launch.json が無くても F5 で開いているイベントを実行する
    vscode.debug.registerDebugConfigurationProvider('srcsharp', {
      resolveDebugConfiguration(_folder, config): vscode.ProviderResult<vscode.DebugConfiguration> {
        if (!config.type && !config.request && !config.name) {
          if (!activeEventFile()) {
            return undefined;
          }
          config.type = 'srcsharp';
          config.request = 'launch';
          config.name = 'SRC# イベントを実行';
          config.program = '${file}';
          config.interactive = true;
          config.stopOnEntry = true;
        }
        if (!config.program) {
          const file = activeEventFile();
          if (!file) {
            vscode.window.showErrorMessage('SRC#: 実行するイベントファイル（program）を指定してください。');
            return undefined;
          }
          config.program = file.fsPath;
        }
        // SRC 本体のフォルダ（共通の素材・Lib）は設定から渡す
        const appPath = vscode.workspace.getConfiguration('srcsharp').get<string>('appPath');
        if (!config.appPath && appPath) {
          config.appPath = appPath;
        }
        // ログのレベル（launch.json に無ければ設定から）
        const logLevel = vscode.workspace.getConfiguration('srcsharp').get<string>('logLevel');
        if (!config.logLevel && logLevel) {
          config.logLevel = logLevel;
        }
        return config;
      },
    }),
  );

  /** srcsharp を動かして出力を表示する。終わったら標準出力を返す */
  const run = (subcommand: string, args: string[], cwd: string): Promise<string | undefined> => {
    const server = resolveServer();
    if (!server.found) {
      void promptInstall();
      return Promise.resolve(undefined);
    }
    return new Promise((resolve, reject) => {
      const c = serverCommand(server, subcommand, args);
      output.show(true);
      output.appendLine(`> ${c.command} ${c.args.join(' ')}`);
      const child = spawn(c.command, c.args, { cwd, env: server.env });
      let stdout = '';
      child.stdout.on('data', (d: Buffer) => {
        stdout += d.toString();
        output.append(d.toString());
      });
      child.stderr.on('data', (d: Buffer) => output.append(d.toString()));
      child.on('error', reject);
      child.on('close', (code: number) => {
        output.appendLine(`(終了コード ${code})`);
        resolve(stdout);
      });
    });
  };

  const workspaceDir = (): string | undefined => vscode.workspace.workspaceFolders?.[0]?.uri.fsPath;

  const startEvent = async (stopOnEntry: boolean): Promise<void> => {
    const file = activeEventFile();
    if (!file) {
      vscode.window.showErrorMessage('SRC#: イベントファイル（*.eve）を開いてください。');
      return;
    }
    await vscode.debug.startDebugging(vscode.workspace.getWorkspaceFolder(file), {
      type: 'srcsharp',
      request: 'launch',
      name: stopOnEntry ? 'SRC# イベントをデバッグ' : 'SRC# イベントを実行',
      program: file.fsPath,
      interactive: true,
      stopOnEntry,
    });
  };

  // 開いているイベントを画面付き・早送りで自動プレイする。.play スクリプトは選ばなくてもよい
  const autoPlay = async (): Promise<void> => {
    const file = activeEventFile();
    if (!file) {
      vscode.window.showErrorMessage('SRC#: イベントファイル（*.eve）を開いてください。');
      return;
    }
    const folder = vscode.workspace.getWorkspaceFolder(file);
    const dir = path.dirname(file.fsPath);
    const scripts = (await vscode.workspace.findFiles(new vscode.RelativePattern(folder ?? dir, '**/*.play'), '**/node_modules/**', 200))
      .map((u) => u.fsPath)
      // 同じフォルダのものを先に、名前の順に
      .sort((a, b) => Number(path.dirname(b) === dir) - Number(path.dirname(a) === dir) || a.localeCompare(b));
    type Pick = vscode.QuickPickItem & { script?: string };
    const items: Pick[] = [
      { label: '$(run-all) スクリプト無し', description: '選択肢は先頭、味方フェイズは自動' },
      ...scripts.map((s) => ({ label: path.basename(s), description: path.relative(folder?.uri.fsPath ?? dir, path.dirname(s)) || undefined, script: s })),
    ];
    const picked = scripts.length === 0
      ? items[0]
      : await vscode.window.showQuickPick(items, { title: `${path.basename(file.fsPath)} を自動プレイ`, placeHolder: '入力を与える .play スクリプト' });
    if (!picked) {
      return;
    }
    await vscode.debug.startDebugging(folder, {
      type: 'srcsharp',
      request: 'launch',
      name: `SRC# 自動プレイ（${picked.script ? path.basename(picked.script) : 'スクリプト無し'}）`,
      program: file.fsPath,
      ...(picked.script ? { script: picked.script } : {}),
      skipWait: true,
    });
  };

  context.subscriptions.push(
    vscode.commands.registerCommand('srcsharp.lint', async () => {
      const dir = workspaceDir();
      if (dir) {
        await run('lint', [dir], dir);
      }
    }),
    // 新しいシナリオのひな形（srcsharp new。7474/SRC#1640）。作ったフォルダを開く
    vscode.commands.registerCommand('srcsharp.newScenario', async () => {
      const picked = await vscode.window.showOpenDialog({
        canSelectFolders: true,
        canSelectFiles: false,
        canSelectMany: false,
        openLabel: 'ここにシナリオを作る',
        title: 'SRC#: 新しいシナリオを作るフォルダ（空のフォルダを選ぶか作る）',
      });
      if (!picked?.[0]) {
        return;
      }
      const folder = picked[0].fsPath;
      const title = await vscode.window.showInputBox({ title: 'SRC#: シナリオの題名', value: path.basename(folder) });
      if (title === undefined) {
        return;
      }
      const appPath = vscode.workspace.getConfiguration('srcsharp').get<string>('appPath');
      const args = [folder, '--title', title, ...(appPath ? ['--app-path', appPath] : [])];
      if ((await run('new', args, folder)) === undefined) {
        return;
      }
      const open = await vscode.window.showInformationMessage(`SRC#: ${folder} にシナリオのひな形を作りました。`, 'フォルダを開く');
      if (open) {
        await vscode.commands.executeCommand('vscode.openFolder', vscode.Uri.file(folder));
      }
    }),
    // 試し戦闘（srcsharp trial。7474/SRC#1643）。データのユニット・パイロットを選び、敵と戦うイベントを作って画面付きで動かす
    vscode.commands.registerCommand('srcsharp.trialBattle', async () => {
      const editor = vscode.window.activeTextEditor;
      const file = editor?.document.uri.scheme === 'file' ? editor.document.uri.fsPath : vscode.workspace.workspaceFolders?.[0]?.uri.fsPath;
      if (!file) {
        vscode.window.showErrorMessage('SRC#: シナリオのデータかイベントを開いてください。');
        return;
      }
      const root = scenarioRoot(editor ? file : path.join(file, 'x'));
      const appPath = vscode.workspace.getConfiguration('srcsharp').get<string>('appPath');
      const appArgs = appPath ? ['--app-path', appPath] : [];
      const listed = await run('trial', [root, '--list', ...appArgs], root);
      if (listed === undefined) {
        return;
      }
      let data: { units: string[]; pilots: string[] };
      try {
        data = JSON.parse(listed.trim().split('\n').pop() ?? '{}');
      } catch {
        vscode.window.showErrorMessage('SRC#: シナリオのデータを読めませんでした。');
        return;
      }
      if (data.units.length === 0 || data.pilots.length === 0) {
        vscode.window.showErrorMessage('SRC#: シナリオのデータにユニットとパイロットが要ります。');
        return;
      }
      // カーソルのあるユニット（データのプレビューと同じく、その行より前のいちばん近い名前）を先頭に
      const near = (names: string[]): string[] => {
        if (!editor) {
          return names;
        }
        const lines = editor.document.getText().split(/\r?\n/).slice(0, editor.selection.active.line + 1).reverse().map((x) => x.trim());
        const hit = lines.find((x) => names.includes(x));
        return hit ? [hit, ...names.filter((x) => x !== hit)] : names;
      };
      const pick = (items: string[], title: string) => vscode.window.showQuickPick(items, { title: `SRC# 試し戦闘: ${title}` });
      const unit = await pick(near(data.units), '味方のユニット');
      const pilot = unit && (await pick(near(data.pilots), '味方のパイロット'));
      const enemy = pilot && (await pick([unit, ...data.units.filter((x) => x !== unit)], '敵のユニット'));
      const generic = data.pilots.filter((x) => x !== pilot);
      const enemyPilot = enemy && (await pick([...generic.filter((x) => /\((ザコ|汎用)\)/.test(x)), ...generic.filter((x) => !/\((ザコ|汎用)\)/.test(x))], '敵のパイロット（ザコか汎用）'));
      if (!enemyPilot) {
        return;
      }
      const created = await run('trial', [root, '--unit', unit, '--pilot', pilot, '--enemy', enemy, '--enemy-pilot', enemyPilot, ...appArgs], root);
      const eve = created?.trim().split('\n').pop();
      if (!eve) {
        return;
      }
      await vscode.debug.startDebugging(vscode.workspace.getWorkspaceFolder(vscode.Uri.file(root)), {
        type: 'srcsharp',
        request: 'launch',
        name: 'SRC# 試し戦闘',
        program: eve,
        interactive: true,
      });
    }),
    vscode.commands.registerCommand('srcsharp.balance', async () => {
      const dir = workspaceDir();
      if (!dir) {
        return;
      }
      const report = path.join(dir, 'balance.md');
      if ((await run('balance', [dir, '--output', report], dir)) !== undefined) {
        await vscode.window.showTextDocument(vscode.Uri.file(report));
      }
    }),
    vscode.commands.registerCommand('srcsharp.play', async () => {
      const file = activeEventFile()?.fsPath;
      if (file) {
        await run('play', [file], path.dirname(file));
      }
    }),
    vscode.commands.registerCommand('srcsharp.autoPlay', () => autoPlay()),
    vscode.commands.registerCommand('srcsharp.runFile', () => startEvent(false)),
    vscode.commands.registerCommand('srcsharp.debugFile', () => startEvent(true)),
    vscode.commands.registerCommand('srcsharp.showScreen', () => screen.show()),
    vscode.commands.registerCommand('srcsharp.reload', () => hotReload.request()),
    vscode.commands.registerCommand('srcsharp.cancelReload', () => hotReload.cancel()),
    vscode.commands.registerCommand('srcsharp.openSettings', () => settings.show()),
    vscode.commands.registerCommand('srcsharp.pickAppPath', () => pickAppPath()),
    vscode.commands.registerCommand('srcsharp.restartServer', () => controller.startClient()),
    vscode.commands.registerCommand('srcsharp.installServer', async () => {
      if (await installServer(output)) {
        await controller.startClient();
        vscode.window.showInformationMessage(`SRC#: srcsharp ${controller.version ?? ''} を入れました。`);
      }
    }),
  );

  // 設定を変えたら Language Server を起動し直す（ウィンドウの再読み込みは要らない）
  let restartTimer: NodeJS.Timeout | undefined;
  context.subscriptions.push(
    vscode.workspace.onDidChangeConfiguration((e) => {
      if (!['srcsharp.serverPath', 'srcsharp.serverArgs', 'srcsharp.appPath', 'srcsharp.rules', 'srcsharp.language', 'srcsharp.logLevel'].some((k) => e.affectsConfiguration(k))) {
        return;
      }
      // 設定画面の保存は複数の値を続けて書くため、まとめて 1 度だけ起動し直す
      if (restartTimer) {
        clearTimeout(restartTimer);
      }
      restartTimer = setTimeout(() => void controller.startClient(), 300);
    }),
    { dispose: () => restartTimer && clearTimeout(restartTimer) },
    vscode.workspace.onDidChangeWorkspaceFolders(() => void controller.refresh()),
  );

  await controller.startClient();
  if (!controller.server.found) {
    void promptInstall();
  }
  void suggestAppPath(context);
}

export async function deactivate(): Promise<void> {
  await current?.stopClient();
}
