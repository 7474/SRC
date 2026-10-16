import * as path from 'path';
import { spawn } from 'child_process';
import * as vscode from 'vscode';
import { LanguageClient, LanguageClientOptions, ServerOptions } from 'vscode-languageclient/node';
import { pickAppPath, suggestAppPath } from './appPath';
import { PanelProvider } from './panel';
import { ScreenView } from './screen';
import { installServer, resolveServer, serverCommand, ServerInfo, serverVersion } from './server';
import { SettingsView } from './settings';

/** srcsharp と Language Server の状態 */
export class Controller implements vscode.Disposable {
  server: ServerInfo = resolveServer();
  version: string | undefined;
  clientState: 'running' | 'starting' | 'stopped' | 'error' = 'stopped';
  private client: LanguageClient | undefined;
  private readonly changed = new vscode.EventEmitter<void>();
  readonly onDidChange = this.changed.event;

  constructor(readonly output: vscode.OutputChannel) {}

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
      documentSelector: [{ scheme: 'file', language: 'src-eve' }],
      initializationOptions: {
        appPath: vscode.workspace.getConfiguration('srcsharp').get<string>('appPath') || undefined,
      },
    };
    this.client = new LanguageClient('srcsharp', 'SRC#', serverOptions, clientOptions);
    this.setState('starting');
    try {
      await this.client.start();
      this.setState('running');
    } catch (e) {
      this.setState('error');
      this.output.appendLine(`Language Server を起動できませんでした: ${(e as Error)?.message ?? e}`);
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
  const output = vscode.window.createOutputChannel('SRC#');
  const controller = new Controller(output);
  current = controller;
  const screen = new ScreenView();
  const settings = new SettingsView(controller);
  context.subscriptions.push(output, controller, screen, settings);

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

  context.subscriptions.push(
    vscode.commands.registerCommand('srcsharp.lint', async () => {
      const dir = workspaceDir();
      if (dir) {
        await run('lint', [dir], dir);
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
    vscode.commands.registerCommand('srcsharp.runFile', () => startEvent(false)),
    vscode.commands.registerCommand('srcsharp.debugFile', () => startEvent(true)),
    vscode.commands.registerCommand('srcsharp.showScreen', () => screen.show()),
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
      if (!['srcsharp.serverPath', 'srcsharp.serverArgs', 'srcsharp.appPath', 'srcsharp.language'].some((k) => e.affectsConfiguration(k))) {
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
