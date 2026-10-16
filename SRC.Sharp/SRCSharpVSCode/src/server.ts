import * as fs from 'fs';
import * as os from 'os';
import * as path from 'path';
import { execFile, spawn } from 'child_process';
import * as vscode from 'vscode';

/** srcsharp の .NET tool のパッケージ */
export const TOOL_PACKAGE = 'SRCSharpTools';
const DEFAULT_COMMAND = 'srcsharp';
export const DOTNET_DOWNLOAD_URL = 'https://dotnet.microsoft.com/download/dotnet/10.0';

/** 動かす srcsharp */
export interface ServerInfo {
  command: string;
  /** サブコマンドの前に付ける引数（設定の serverArgs） */
  args: string[];
  /** 見つかったか */
  found: boolean;
  /** setting: 設定のパス、path: PATH、dotnet-tools: ~/.dotnet/tools、missing: 見つからない */
  source: 'setting' | 'path' | 'dotnet-tools' | 'missing';
  /** srcsharp に渡す環境変数（DOTNET_ROOT・SRCSHARP_LANG） */
  env: NodeJS.ProcessEnv;
}

function executableNames(name: string): string[] {
  return process.platform === 'win32' ? [`${name}.exe`, `${name}.cmd`, name] : [name];
}

function isFile(file: string): boolean {
  try {
    return fs.statSync(file).isFile();
  } catch {
    return false;
  }
}

/** PATH から実行ファイルを探す */
export function findOnPath(name: string): string | undefined {
  for (const dir of (process.env.PATH ?? '').split(path.delimiter)) {
    if (!dir) {
      continue;
    }
    for (const exe of executableNames(name)) {
      const file = path.join(dir, exe);
      if (isFile(file)) {
        return file;
      }
    }
  }
  return undefined;
}

/** グローバルの .NET tool の入る場所（https://learn.microsoft.com/dotnet/core/tools/global-tools） */
export function dotnetToolsDir(): string {
  return path.join(process.env.DOTNET_CLI_HOME || os.homedir(), '.dotnet', 'tools');
}

/** dotnet の実行ファイル。PATH・DOTNET_ROOT・~/.dotnet（dotnet-install の既定）の順に探す */
export function findDotnet(): string | undefined {
  const onPath = findOnPath('dotnet');
  if (onPath) {
    try {
      return fs.realpathSync(onPath);
    } catch {
      return onPath;
    }
  }
  const roots = [process.env.DOTNET_ROOT, path.join(os.homedir(), '.dotnet')];
  for (const root of roots) {
    if (!root) {
      continue;
    }
    for (const exe of executableNames('dotnet')) {
      const file = path.join(root, exe);
      if (isFile(file)) {
        return file;
      }
    }
  }
  return undefined;
}

/**
 * srcsharp を動かす時の環境変数。
 * dotnet が既定以外の場所（~/.dotnet など）にあると、tool の起動に DOTNET_ROOT が要る
 * （https://learn.microsoft.com/dotnet/core/tools/dotnet-environment-variables）
 */
function serverEnv(language: string): NodeJS.ProcessEnv {
  const env: NodeJS.ProcessEnv = { ...process.env };
  if (!env.DOTNET_ROOT) {
    const dotnet = findDotnet();
    if (dotnet) {
      env.DOTNET_ROOT = path.dirname(dotnet);
    }
  }
  if (language) {
    env.SRCSHARP_LANG = language;
  }
  return env;
}

/** 設定と PATH・~/.dotnet/tools から srcsharp を探す */
export function resolveServer(): ServerInfo {
  const config = vscode.workspace.getConfiguration('srcsharp');
  const serverPath = config.get<string>('serverPath', DEFAULT_COMMAND).trim() || DEFAULT_COMMAND;
  const args = config.get<string[]>('serverArgs', []);
  const env = serverEnv(config.get<string>('language', ''));
  if (serverPath !== DEFAULT_COMMAND) {
    const found = path.isAbsolute(serverPath) ? isFile(serverPath) : findOnPath(serverPath) !== undefined;
    return { command: serverPath, args, found, source: 'setting', env };
  }
  const onPath = findOnPath(DEFAULT_COMMAND);
  if (onPath) {
    return { command: onPath, args, found: true, source: 'path', env };
  }
  // tool を入れた直後の VS Code は、PATH に ~/.dotnet/tools が無いことがある
  for (const exe of executableNames(DEFAULT_COMMAND)) {
    const file = path.join(dotnetToolsDir(), exe);
    if (isFile(file)) {
      return { command: file, args, found: true, source: 'dotnet-tools', env };
    }
  }
  return { command: DEFAULT_COMMAND, args, found: false, source: 'missing', env };
}

/** サブコマンドを動かすコマンドと引数 */
export function serverCommand(info: ServerInfo, subcommand: string, args: string[] = []): { command: string; args: string[] } {
  return { command: info.command, args: [...info.args, subcommand, ...args] };
}

/** srcsharp の版（--version）。0.5.1 までの srcsharp は --version に対応していないため undefined */
export function serverVersion(info: ServerInfo): Promise<string | undefined> {
  if (!info.found) {
    return Promise.resolve(undefined);
  }
  return new Promise((resolve) => {
    execFile(info.command, [...info.args, '--version'], { env: info.env, timeout: 15000 }, (error, stdout) => {
      const version = stdout?.toString().trim();
      resolve(!error && /^\d+\.\d+/.test(version) ? version : undefined);
    });
  });
}

/**
 * dotnet tool update -g SRCSharpTools で srcsharp を入れるか更新する（入っていなければ入れる）。
 * https://learn.microsoft.com/dotnet/core/tools/dotnet-tool-update
 */
export async function installServer(output: vscode.OutputChannel): Promise<boolean> {
  const dotnet = findDotnet();
  if (!dotnet) {
    const open = '.NET のダウンロードを開く';
    const pick = await vscode.window.showErrorMessage(
      'srcsharp を入れるには .NET 10 SDK が要ります。.NET を入れてから VS Code を起動し直してください。',
      open,
    );
    if (pick === open) {
      await vscode.env.openExternal(vscode.Uri.parse(DOTNET_DOWNLOAD_URL));
    }
    return false;
  }
  const args = ['tool', 'update', '-g', TOOL_PACKAGE];
  output.show(true);
  output.appendLine(`> ${dotnet} ${args.join(' ')}`);
  const code = await vscode.window.withProgress(
    { location: vscode.ProgressLocation.Notification, title: 'srcsharp（SRCSharpTools）を入れています…' },
    () =>
      new Promise<number>((resolve) => {
        const child = spawn(dotnet, args, { env: { ...process.env, DOTNET_CLI_TELEMETRY_OPTOUT: '1' } });
        child.stdout.on('data', (d: Buffer) => output.append(d.toString()));
        child.stderr.on('data', (d: Buffer) => output.append(d.toString()));
        child.on('error', (e) => {
          output.appendLine(e.message);
          resolve(-1);
        });
        child.on('close', (c: number | null) => resolve(c ?? -1));
      }),
  );
  output.appendLine(`(終了コード ${code})`);
  if (code !== 0) {
    vscode.window.showErrorMessage('srcsharp を入れられませんでした。出力パネルの「SRC#」を確認してください。');
    return false;
  }
  return true;
}
