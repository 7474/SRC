import * as fs from 'fs';
import * as os from 'os';
import * as path from 'path';
import { spawn } from 'child_process';
import * as vscode from 'vscode';
import { LanguageClient, LanguageClientOptions, ServerOptions } from 'vscode-languageclient/node';
import { pickAppPath, suggestAppPath } from './appPath';
import { AssetReportData, AssetsPanel } from './assets';
import { EventFlowGraphData, EventFlowPanel } from './eventFlow';
import { EpisodeFlowPanel, ScenarioFlowData } from './flow';
import { MapEditorProvider } from './mapEditor';
import { DataPreviewPanel } from './preview';
import { HotReload, scenarioRoot } from './hotReload';
import { PanelProvider } from './panel';
import { ScreenView } from './screen';
import { installServer, resolveServer, serverCommand, ServerInfo, serverVersion } from './server';
import { SettingsView } from './settings';
import { TalkPreviewData, TalkPreviewPanel } from './talkPreview';
import { SrcUriHandler } from './uriHandler';

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
  // 話のつながりの図（7474/SRC#1652）
  const episodeFlow = new EpisodeFlowPanel();
  // イベントの流れの図・会話のプレビュー（7474/SRC#1655）
  const eventFlow = new EventFlowPanel(controller);
  const talkPreview = new TalkPreviewPanel();
  context.subscriptions.push(
    dataPreview,
    episodeFlow,
    eventFlow,
    talkPreview,
    vscode.commands.registerCommand('srcsharp.showPreview', () => dataPreview.show()),
    // テスト用（プレビューに出したもの）。package.json には出さない
    vscode.commands.registerCommand('srcsharp._previewStatus', () => dataPreview.getStatus()),
    vscode.commands.registerCommand('srcsharp.showEpisodeFlow', (target?: string) => episodeFlow.show(target)),
    // テスト用（話のつながりの図）。package.json には出さない
    vscode.commands.registerCommand('srcsharp._flowStatus', () => episodeFlow.getStatus()),
    vscode.commands.registerCommand('srcsharp._showFlowData', (data: ScenarioFlowData) => episodeFlow.showData(data)),
    vscode.commands.registerCommand('srcsharp.showEventFlow', (target?: string) => eventFlow.show(target)),
    vscode.commands.registerCommand('srcsharp._eventFlowStatus', () => eventFlow.getStatus()),
    vscode.commands.registerCommand('srcsharp._showEventFlowData', (data: EventFlowGraphData) => eventFlow.showData(data)),
    vscode.commands.registerCommand('srcsharp.previewTalk', (uri?: vscode.Uri, line?: number) => talkPreview.show(uri, line)),
    vscode.commands.registerCommand('srcsharp._talkPreviewStatus', () => talkPreview.getStatus()),
    vscode.commands.registerCommand('srcsharp._showTalkPreviewData', (data: TalkPreviewData) => talkPreview.showData(data)),
  );

  // 素材の一覧（揃っているか。7474/SRC#1654）
  const assets = new AssetsPanel();
  // データビューアからデータを受け取る（7474/SRC#1656）
  const uriHandler = new SrcUriHandler();
  context.subscriptions.push(
    assets,
    vscode.window.registerUriHandler(uriHandler),
    vscode.commands.registerCommand('srcsharp.showAssets', (target?: string) => assets.show(target)),
    // テスト用。package.json には出さない
    vscode.commands.registerCommand('srcsharp._assetsStatus', () => assets.getStatus()),
    vscode.commands.registerCommand('srcsharp._showAssetsData', (data: AssetReportData) => assets.showData(data)),
    vscode.commands.registerCommand('srcsharp._handleUri', (uri: vscode.Uri | string, target?: 'new' | vscode.Uri) =>
      uriHandler.handle(typeof uri === 'string' ? vscode.Uri.parse(uri) : uri, target)),
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
    // 新しいシナリオのひな形（srcsharp new。7474/SRC#1640・#1652）。作ったフォルダを開く
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
      const episodes = await vscode.window.showInputBox({
        title: 'SRC#: 話数（1 以上）',
        value: '1',
        prompt: '1 は単発、2 以上で各話・インターミッション・通しプレイ付きの複数話を作成',
        validateInput: (v) => (/^[1-9]\d*$/.test(v.trim()) ? undefined : '1 以上の整数を入力してください'),
      });
      if (episodes === undefined) {
        return;
      }
      const epCount = episodes.trim();
      const appPath = vscode.workspace.getConfiguration('srcsharp').get<string>('appPath');
      const args = [folder, '--title', title, '--episodes', epCount, ...(appPath ? ['--app-path', appPath] : [])];
      if ((await run('new', args, folder)) === undefined) {
        return;
      }
      const open = await vscode.window.showInformationMessage(`SRC#: ${folder} にシナリオのひな形（${epCount} 話）を作りました。`, 'フォルダを開く');
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
      let data: { units: string[]; pilots: string[]; messageTypes?: Record<string, string> };
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
      // カーソルのあるユニット・パイロット（pilot_message/dialog のメッセージタイプや複数人見出しも含む）を先頭に
      const near = (names: string[], map?: Record<string, string>): string[] => {
        if (!editor) {
          return names;
        }
        const lines = editor.document.getText().split(/\r?\n/).slice(0, editor.selection.active.line + 1).reverse().map((x) => x.trim());
        for (const line of lines) {
          if (names.includes(line)) {
            return [line, ...names.filter((x) => x !== line)];
          }
          const firstToken = line.split(/\s+/)[0];
          const mapped = map?.[line] ?? map?.[firstToken] ?? (names.includes(firstToken) ? firstToken : undefined);
          if (mapped && names.includes(mapped)) {
            return [mapped, ...names.filter((x) => x !== mapped)];
          }
        }
        return names;
      };
      const pick = (items: string[], title: string) => vscode.window.showQuickPick(items, { title: `SRC# 試し戦闘: ${title}` });
      const unit = await pick(near(data.units), '味方のユニット');
      const pilot = unit && (await pick(near(data.pilots, data.messageTypes), '味方のパイロット'));
      const enemy = pilot && (await pick([unit, ...data.units.filter((x) => x !== unit)], '敵のユニット'));
      const generic = data.pilots.filter((x) => x !== pilot);
      const enemyPilot = enemy && (await pick([...generic.filter((x) => /\((ザコ|汎用)\)/.test(x)), ...generic.filter((x) => !/\((ザコ|汎用)\)/.test(x))], '敵のパイロット（ザコか汎用）'));
      if (!enemyPilot) {
        return;
      }
      const focusPick = await vscode.window.showQuickPick(
        [
          { label: '標準', description: '通常の配置で戦う', value: 'normal' },
          { label: '攻撃・とどめ', description: '隣に HP 10% の敵を置く（攻撃・武器・とどめのセリフ）', value: 'attack' },
          { label: '回避', description: '低レベルの敵を隣に置く（回避のセリフ）', value: 'evade' },
          { label: '被弾', description: '味方 HP 70% で格上の敵を隣に置く（ダメージのセリフ）', value: 'damage' },
          { label: '破壊', description: '味方 HP 1% で強敵を隣に置く（破壊・脱出のセリフ）', value: 'destroy' },
        ],
        { title: 'SRC# 試し戦闘: 試したい場面' },
      );
      if (!focusPick) {
        return;
      }
      const created = await run('trial', [root, '--unit', unit, '--pilot', pilot, '--enemy', enemy, '--enemy-pilot', enemyPilot, '--focus', focusPick.value, ...appArgs], root);
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
    // 強さの確かめ（srcsharp playtest。7474/SRC#1690）。開いている話を乱数を変えて自動で何回か遊び、結果を Markdown で横に出す
    vscode.commands.registerCommand('srcsharp.playtest', async () => {
      const file = activeEventFile()?.fsPath;
      if (!file) {
        vscode.window.showErrorMessage('SRC#: 強さを確かめる話（*.eve）を開いてください。');
        return;
      }
      const runs = await vscode.window.showQuickPick(['3', '5', '10'], { title: 'SRC#: 何回遊ぶか（乱数を変えて自動で遊ぶ）', placeHolder: '5' });
      if (!runs) {
        return;
      }
      const appPath = vscode.workspace.getConfiguration('srcsharp').get<string>('appPath');
      const report = path.join(os.tmpdir(), `srcsharp-playtest-${path.basename(file, path.extname(file))}.md`);
      const done = await vscode.window.withProgress(
        { location: vscode.ProgressLocation.Notification, title: `SRC#: 「${path.basename(file)}」を ${runs} 回自動で遊んでいます…` },
        () => run('playtest', [file, '--runs', runs, '--output', report, ...(appPath ? ['--app-path', appPath] : [])], path.dirname(file)));
      if (done !== undefined && fs.existsSync(report)) {
        await vscode.commands.executeCommand('markdown.showPreviewToSide', vscode.Uri.file(report));
      }
    }),
    // この話から遊ぶ（7474/SRC#1692）。通しの自動プレイ（全話.play の saveepisodes）が残した、この話の始めのセーブデータ（.srcsharp/episodes/話.srcs）から、
    // 画面付きのデバッガで始める。無ければ、通しの自動プレイを動かして作る
    vscode.commands.registerCommand('srcsharp.playFromEpisode', async () => {
      const file = activeEventFile()?.fsPath;
      if (!file) {
        vscode.window.showErrorMessage('SRC#: 遊びたい話（*.eve）を開いてください。');
        return;
      }
      const root = scenarioRoot(file);
      const episode = path.basename(file, path.extname(file));
      const save = path.join(root, '.srcsharp', 'episodes', `${episode}.srcs`);
      if (!fs.existsSync(save)) {
        const through = path.join(root, '全話.play');
        const first = path.join(root, '第1話.eve');
        if (!fs.existsSync(through) || !fs.existsSync(first)) {
          vscode.window.showInformationMessage(`SRC#: 「${episode}」の始めのセーブデータがありません。通しの自動プレイの手順（全話.play）に「saveepisodes .srcsharp/episodes」を書いて遊ぶと、インターミッションのたびに作られます。`);
          return;
        }
        const pick = await vscode.window.showInformationMessage(
          `SRC#: 「${episode}」の始めのセーブデータがありません。第1話から通しで自動で遊んで作りますか？`, '作る');
        if (pick !== '作る') {
          return;
        }
        const appPath = vscode.workspace.getConfiguration('srcsharp').get<string>('appPath');
        await vscode.window.withProgress(
          { location: vscode.ProgressLocation.Notification, title: 'SRC#: 第1話から通しで自動で遊んでいます…' },
          () => run('play', [first, '--script', through, '--skip-wait', '--seed', '1', ...(appPath ? ['--app-path', appPath] : [])], root));
        if (!fs.existsSync(save)) {
          vscode.window.showWarningMessage(`SRC#: 通しの自動プレイで「${episode}」まで進めませんでした（出力を見てください）。`);
          return;
        }
      }
      await vscode.debug.startDebugging(vscode.workspace.getWorkspaceFolder(vscode.Uri.file(root)), {
        type: 'srcsharp',
        request: 'launch',
        name: `SRC# ${episode}から遊ぶ`,
        program: save,
        scenarioRoot: root,
        interactive: true,
      });
    }),
    // 出来事を足す（srcsharp event-text。7474/SRC#1692）。きっかけ・会話・撤退をフォームで選び、かんたん作成と同じ書き方のイベントを開いている .eve の最後に足す
    vscode.commands.registerCommand('srcsharp.addEvent', async () => {
      const editor = vscode.window.activeTextEditor;
      const file = activeEventFile()?.fsPath;
      if (!editor || !file || editor.document.uri.fsPath !== file) {
        vscode.window.showErrorMessage('SRC#: 出来事を足す話（*.eve）を開いてください。');
        return;
      }
      const title = (s: string) => `SRC# 出来事を足す: ${s}`;
      // 出てくる人（Create・Launch・Talk・Unit のパイロット）
      const cast: string[] = [];
      for (const raw of editor.document.getText().split(/\r?\n/)) {
        const t = raw.trim().split(/[\s　]+/);
        const name = /^(Talk|AutoTalk|Launch|Ride)$/i.test(t[0]) ? t[1] : /^Create$/i.test(t[0]) ? t[5] : undefined;
        if (name && !/^[-\d"$]/.test(name) && !cast.includes(name)) {
          cast.push(name);
        }
      }
      const other = '（名前を入力する）';
      const person = async (s: string): Promise<string | undefined> => {
        const picked = await vscode.window.showQuickPick([...cast, other], { title: title(s) });
        if (picked !== other) {
          return picked;
        }
        const typed = (await vscode.window.showInputBox({ title: title(s), prompt: 'パイロットの名前' }))?.trim();
        if (typed && !cast.includes(typed)) {
          cast.push(typed);
        }
        return typed || undefined;
      };
      const trigger = await vscode.window.showQuickPick(
        [
          { label: 'ターン', description: 'N ターン目の味方・敵のフェイズの始め', value: 'turn' },
          { label: '戦うとき', description: '2 人が戦う前（どちらから仕掛けても）', value: 'attack' },
          { label: '損傷率', description: 'ユニットの損傷率が N% 以上になったとき', value: 'damaged' },
          { label: '落ちたとき', description: 'パイロットのユニットが破壊されたとき', value: 'destroyed' },
        ],
        { title: title('きっかけ') },
      );
      if (!trigger) {
        return;
      }
      const ev: Record<string, unknown> = { trigger: trigger.value };
      if (trigger.value === 'turn') {
        const turn = await vscode.window.showInputBox({ title: title('何ターン目'), value: '2', validateInput: (v) => (/^\d+$/.test(v.trim()) && +v > 0 ? undefined : '1 以上の数') });
        const phase = turn && (await vscode.window.showQuickPick(['敵', '味方'], { title: title('どちらのフェイズの始め') }));
        if (!phase) {
          return;
        }
        ev.turn = +turn;
        ev.phase = phase;
      } else {
        const unit = await person(trigger.value === 'attack' ? '戦う 1 人目' : 'だれの');
        if (!unit) {
          return;
        }
        ev.unit = unit;
        if (trigger.value === 'attack') {
          const target = await person('戦う相手');
          if (!target) {
            return;
          }
          ev.target = target;
        }
        if (trigger.value === 'damaged') {
          const ratio = await vscode.window.showInputBox({ title: title('損傷率（%）'), value: '50', validateInput: (v) => (/^\d+$/.test(v.trim()) && +v > 0 && +v < 100 ? undefined : '1〜99') });
          if (!ratio) {
            return;
          }
          ev.ratio = +ratio;
        }
      }
      const talk: { speaker: string; text: string }[] = [];
      const done = '（会話はここまで）';
      for (;;) {
        const picked = await vscode.window.showQuickPick([done, ...cast, other], { title: title(`会話 ${talk.length + 1} 行目を話す人`) });
        if (picked === undefined) {
          return;
        }
        if (picked === done) {
          break;
        }
        const speaker = picked === other ? (await vscode.window.showInputBox({ title: title('話す人'), prompt: 'パイロットの名前' }))?.trim() : picked;
        const text = speaker && (await vscode.window.showInputBox({ title: title(`${speaker} のセリフ`) }));
        if (speaker && text) {
          if (!cast.includes(speaker)) {
            cast.push(speaker);
          }
          talk.push({ speaker, text });
        }
      }
      ev.talk = talk;
      if (trigger.value === 'attack' || trigger.value === 'damaged') {
        const retreat = await vscode.window.showQuickPick(['撤退させない', '会話のあとで撤退させる'], { title: title(`${ev.unit} を撤退させるか`) });
        if (!retreat) {
          return;
        }
        ev.retreat = retreat !== '撤退させない';
      }
      const appPath = vscode.workspace.getConfiguration('srcsharp').get<string>('appPath');
      const text = await run('event-text', [file, '--json', JSON.stringify(ev), ...(appPath ? ['--app-path', appPath] : [])], path.dirname(file));
      if (!text?.trim()) {
        return;
      }
      const doc = editor.document;
      const end = doc.lineAt(doc.lineCount - 1).range.end;
      const eol = doc.eol === vscode.EndOfLine.CRLF ? '\r\n' : '\n';
      const lead = doc.getText().length === 0 || /\n\s*$/.test(doc.getText()) ? eol : eol + eol;
      await editor.edit((b) => b.insert(end, lead + text.replace(/\r?\n/g, eol)));
      const last = doc.lineCount - 1;
      editor.revealRange(new vscode.Range(last, 0, last, 0));
    }),
    // シナリオの見通し（srcsharp outline。7474/SRC#1692）。話ごとの部隊と登場人物・出来事・変数・台本を Markdown で横に出す（行へのリンク付き）
    vscode.commands.registerCommand('srcsharp.showOutline', async () => {
      const editor = vscode.window.activeTextEditor;
      const file = editor?.document.uri.scheme === 'file' ? editor.document.uri.fsPath : undefined;
      const dir = file ? scenarioRoot(file) : workspaceDir();
      if (!dir) {
        vscode.window.showErrorMessage('SRC#: シナリオのフォルダを開いてください。');
        return;
      }
      const appPath = vscode.workspace.getConfiguration('srcsharp').get<string>('appPath');
      const report = path.join(dir, '.srcsharp', '見通し.md');
      const done = await vscode.window.withProgress(
        { location: vscode.ProgressLocation.Notification, title: 'SRC#: シナリオの見通しを作っています…' },
        () => run('outline', [dir, '--output', report, ...(appPath ? ['--app-path', appPath] : [])], dir));
      if (done !== undefined && fs.existsSync(report)) {
        await vscode.commands.executeCommand('markdown.showPreviewToSide', vscode.Uri.file(report));
      }
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
    vscode.commands.registerCommand('srcsharp.previewAnimation', async () => {
      const editor = vscode.window.activeTextEditor;
      const doc = editor?.document;
      if (!doc || doc.uri.scheme !== 'file') {
        vscode.window.showErrorMessage('SRC#: animation.txt・ext_animation.txt・effect.txt またはイベントファイル（*.eve）を開いてください。');
        return;
      }
      const file = doc.uri.fsPath;
      const lowerName = path.basename(file).toLowerCase();
      const isAnimData = lowerName === 'animation.txt' || lowerName === 'ext_animation.txt' || lowerName === 'effect.txt';
      const isEve = doc.languageId === 'src-eve' || lowerName.endsWith('.eve');
      if (!isAnimData && !isEve) {
        vscode.window.showInformationMessage('SRC#: animation.txt・ext_animation.txt・effect.txt またはイベントファイル（*.eve）で使えます。');
        return;
      }
      if (doc.isDirty) {
        await doc.save();
      }
      const server = resolveServer();
      if (!server.found) {
        void promptInstall();
        return;
      }
      const runAnimeCli = (args: string[]): Promise<string> => new Promise((resolve, reject) => {
        const c = serverCommand(server, 'anime-preview', args);
        const child = spawn(c.command, c.args, { cwd: path.dirname(file), env: server.env });
        let stdout = '';
        let stderr = '';
        child.stdout.on('data', (d: Buffer) => { stdout += d.toString(); });
        child.stderr.on('data', (d: Buffer) => { stderr += d.toString(); });
        child.on('error', reject);
        child.on('close', (code) => {
          if (code === 0) {
            resolve(stdout);
          } else {
            reject(new Error(stderr.trim() || `srcsharp anime-preview が終了コード ${code} で失敗しました`));
          }
        });
      });

      const appPath = vscode.workspace.getConfiguration('srcsharp').get<string>('appPath');
      const appArgs = appPath ? ['--app-path', appPath] : [];
      const line1 = editor.selection.active.line + 1;
      interface AnimeListPayload {
        file: string;
        scenarioRoot: string;
        appPath: string;
        defaultSetup: { attackerUnit: string; attackerPilot: string; defenderUnit: string; defenderPilot: string };
        selectedIndex: number;
        entries: { index: number; kind: string; file: string; line: number; target: string; situation: string; spec: string }[];
      }
      let listData: AnimeListPayload;
      try {
        const raw = await runAnimeCli([file, '--line', String(line1), '--list', ...appArgs]);
        listData = JSON.parse(raw) as AnimeListPayload;
      } catch (e) {
        vscode.window.showErrorMessage(`SRC#: 戦闘アニメ・特殊効果を読めませんでした: ${(e as Error)?.message ?? e}`);
        return;
      }
      if (!listData.entries || listData.entries.length === 0) {
        vscode.window.showInformationMessage('SRC#: このファイルに再生できる戦闘アニメ・特殊効果・汎用戦闘アニメの定義が見つかりませんでした。');
        return;
      }
      const selected = listData.entries[listData.selectedIndex >= 0 ? listData.selectedIndex : 0];
      if (screen.isAnimationPreviewActive(listData.scenarioRoot)) {
        try {
          await screen.replayAnimationPreview({
            kind: selected.kind,
            file: selected.file,
            line: selected.line,
            target: selected.target,
            situation: selected.situation,
            spec: selected.spec,
            ...listData.defaultSetup,
          });
          screen.show(true);
          return;
        } catch {
          // 既に終わったセッションなら新しく起動する
        }
      }
      interface AnimePrepPayload {
        program: string;
        scenarioRoot: string;
        appPath: string;
        animationPreview: Record<string, unknown>;
      }
      let prep: AnimePrepPayload;
      try {
        const raw = await runAnimeCli([
          file,
          '--line', String(selected.line),
          '--kind', selected.kind,
          '--scenario-root', listData.scenarioRoot,
          ...appArgs,
        ]);
        prep = JSON.parse(raw) as AnimePrepPayload;
      } catch (e) {
        vscode.window.showErrorMessage(`SRC#: 演出プレビューの準備に失敗しました: ${(e as Error)?.message ?? e}`);
        return;
      }
      await vscode.debug.startDebugging(vscode.workspace.getWorkspaceFolder(vscode.Uri.file(listData.scenarioRoot)), {
        type: 'srcsharp',
        request: 'launch',
        name: `SRC# 演出プレビュー: ${selected.target} - ${selected.situation}`,
        program: prep.program,
        scenarioRoot: prep.scenarioRoot,
        appPath: prep.appPath,
        interactive: true,
        animationPreview: prep.animationPreview,
      });
    }),
    vscode.commands.registerCommand('srcsharp.replayAnimationPreview', async () => {
      if (!screen.isAnimationPreviewActive()) {
        vscode.window.showInformationMessage('SRC#: 戦闘アニメ・特殊効果のプレビューを実行していません。');
        return;
      }
      const activePreviewFile = vscode.debug.activeDebugSession?.configuration?.animationPreview?.file as string | undefined;
      if (activePreviewFile) {
        const openDoc = vscode.workspace.textDocuments.find((d) => path.resolve(d.uri.fsPath) === path.resolve(activePreviewFile));
        if (openDoc?.isDirty) {
          await openDoc.save();
        }
      }
      try {
        await screen.replayAnimationPreview();
      } catch (e) {
        vscode.window.showWarningMessage(`SRC#: もう一度再生できませんでした: ${(e as Error)?.message ?? e}`);
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
