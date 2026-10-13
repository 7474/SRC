import * as path from 'path';
import { spawn } from 'child_process';
import * as vscode from 'vscode';
import { LanguageClient, LanguageClientOptions, ServerOptions } from 'vscode-languageclient/node';

let client: LanguageClient | undefined;

/** srcsharp を動かすコマンドと引数（設定から） */
function serverCommand(subcommand: string, args: string[] = []): { command: string; args: string[] } {
  const config = vscode.workspace.getConfiguration('srcsharp');
  return {
    command: config.get<string>('serverPath', 'srcsharp'),
    args: [...config.get<string[]>('serverArgs', []), subcommand, ...args],
  };
}

export async function activate(context: vscode.ExtensionContext): Promise<void> {
  // Language Server（srcsharp lsp）
  const lsp = serverCommand('lsp');
  const serverOptions: ServerOptions = { command: lsp.command, args: lsp.args };
  const clientOptions: LanguageClientOptions = {
    documentSelector: [{ scheme: 'file', language: 'src-eve' }],
    initializationOptions: {
      appPath: vscode.workspace.getConfiguration('srcsharp').get<string>('appPath') || undefined,
    },
  };
  client = new LanguageClient('srcsharp', 'SRC#', serverOptions, clientOptions);
  await client.start();
  context.subscriptions.push(client);

  // デバッガ（srcsharp dap）
  context.subscriptions.push(
    vscode.debug.registerDebugAdapterDescriptorFactory('srcsharp', {
      createDebugAdapterDescriptor(): vscode.ProviderResult<vscode.DebugAdapterDescriptor> {
        const dap = serverCommand('dap');
        return new vscode.DebugAdapterExecutable(dap.command, dap.args);
      },
    }),
  );

  const output = vscode.window.createOutputChannel('SRC#');
  context.subscriptions.push(output);

  /** srcsharp を動かして出力を表示する。終わったら標準出力を返す */
  const run = (subcommand: string, args: string[], cwd: string): Promise<string> =>
    new Promise((resolve, reject) => {
      const c = serverCommand(subcommand, args);
      output.show(true);
      output.appendLine(`> ${c.command} ${c.args.join(' ')}`);
      const child = spawn(c.command, c.args, { cwd });
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

  const workspaceDir = (): string | undefined => vscode.workspace.workspaceFolders?.[0]?.uri.fsPath;

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
      await run('balance', [dir, '--output', report], dir);
      await vscode.window.showTextDocument(vscode.Uri.file(report));
    }),
    vscode.commands.registerCommand('srcsharp.play', async () => {
      const file = vscode.window.activeTextEditor?.document.uri.fsPath;
      if (file) {
        await run('play', [file], path.dirname(file));
      }
    }),
  );
}

export async function deactivate(): Promise<void> {
  await client?.stop();
}
