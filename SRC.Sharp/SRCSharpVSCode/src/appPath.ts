import * as fs from 'fs';
import * as path from 'path';
import * as vscode from 'vscode';

/** Lib か Data/System のあるフォルダを SRC 本体のフォルダとみなす（ScenarioAnalyzer.FindAppPath と同じ） */
export function isAppFolder(dir: string): boolean {
  const isDir = (p: string): boolean => {
    try {
      return fs.statSync(p).isDirectory();
    } catch {
      return false;
    }
  };
  return isDir(path.join(dir, 'Lib')) || isDir(path.join(dir, 'Data', 'System'));
}

function ancestors(dir: string): string[] {
  const result: string[] = [];
  let d = path.resolve(dir);
  for (;;) {
    result.push(d);
    const parent = path.dirname(d);
    if (parent === d) {
      return result;
    }
    d = parent;
  }
}

function descendants(dir: string, depth: number): string[] {
  if (depth <= 0) {
    return [];
  }
  let entries: fs.Dirent[];
  try {
    entries = fs.readdirSync(dir, { withFileTypes: true });
  } catch {
    return [];
  }
  const result: string[] = [];
  for (const e of entries) {
    if (!e.isDirectory() || e.name.startsWith('.') || e.name === 'node_modules') {
      continue;
    }
    const child = path.join(dir, e.name);
    result.push(child, ...descendants(child, depth - 1));
  }
  return result;
}

/** ワークスペースのフォルダの親が SRC 本体のフォルダか（それなら Language Server が自分で見つける） */
export function appPathInferable(): boolean {
  return (vscode.workspace.workspaceFolders ?? []).some((f) => ancestors(f.uri.fsPath).some(isAppFolder));
}

/** SRC 本体のフォルダの候補（ワークスペースのフォルダとその親、2 階層下まで） */
export function appPathCandidates(): string[] {
  const result = new Set<string>();
  for (const folder of vscode.workspace.workspaceFolders ?? []) {
    const root = folder.uri.fsPath;
    for (const dir of [...ancestors(root), ...descendants(root, 2)]) {
      if (isAppFolder(dir)) {
        result.add(dir);
      }
    }
  }
  return [...result];
}

/** 設定を書く先。ワークスペースを開いていればワークスペース、無ければユーザー */
export function defaultTarget(): vscode.ConfigurationTarget {
  return vscode.workspace.workspaceFolders?.length ? vscode.ConfigurationTarget.Workspace : vscode.ConfigurationTarget.Global;
}

export async function setAppPath(dir: string): Promise<void> {
  await vscode.workspace.getConfiguration('srcsharp').update('appPath', dir, defaultTarget());
}

/** フォルダの選択ダイアログで選ぶ。SRC 本体のフォルダに見えなければ確かめる */
export async function browseAppPath(): Promise<string | undefined> {
  const picked = await vscode.window.showOpenDialog({
    canSelectFiles: false,
    canSelectFolders: true,
    canSelectMany: false,
    openLabel: 'SRC 本体のフォルダにする',
    defaultUri: vscode.workspace.workspaceFolders?.[0]?.uri,
  });
  const dir = picked?.[0]?.fsPath;
  if (!dir) {
    return undefined;
  }
  if (!isAppFolder(dir)) {
    const use = 'このフォルダを使う';
    const pick = await vscode.window.showWarningMessage(`${dir} に Lib も Data/System もありません。`, { modal: true }, use);
    if (pick !== use) {
      return undefined;
    }
  }
  return dir;
}

/** 候補とフォルダの選択から SRC 本体のフォルダを選んで設定する */
export async function pickAppPath(): Promise<void> {
  const current = vscode.workspace.getConfiguration('srcsharp').get<string>('appPath', '');
  type Item = vscode.QuickPickItem & { value?: string; browse?: boolean };
  const items: Item[] = appPathCandidates().map((dir) => ({
    label: path.basename(dir) || dir,
    description: dir,
    detail: dir === current ? '現在の設定' : undefined,
    value: dir,
  }));
  items.push(
    { label: '$(folder-opened) フォルダを選ぶ…', browse: true },
    { label: '$(clear-all) 設定しない（シナリオのフォルダの親から探す）', value: '' },
  );
  const item = await vscode.window.showQuickPick(items, {
    title: 'SRC 本体のフォルダ（Lib や共通の Data のあるフォルダ）',
    placeHolder: items.length > 2 ? 'ワークスペースで見つかったフォルダ' : '見つかりませんでした。フォルダを選んでください',
  });
  if (!item) {
    return;
  }
  const dir = item.browse ? await browseAppPath() : item.value;
  if (dir !== undefined) {
    await setAppPath(dir);
  }
}

const SUPPRESS_KEY = 'srcsharp.appPathSuggestionDismissed';

/**
 * appPath が空で、Language Server が親から見つけられず、ワークスペースの中に候補がある時に設定を勧める
 */
export async function suggestAppPath(context: vscode.ExtensionContext): Promise<void> {
  if (vscode.workspace.getConfiguration('srcsharp').get<string>('appPath') || context.workspaceState.get(SUPPRESS_KEY)) {
    return;
  }
  if (appPathInferable()) {
    return;
  }
  const candidates = appPathCandidates();
  if (candidates.length === 0) {
    return;
  }
  const use = '使う';
  const other = 'ほかを選ぶ';
  const never = '今後表示しない';
  const pick = await vscode.window.showInformationMessage(
    `SRC 本体のフォルダ（システムのラベルなどを読む）に ${candidates[0]} を使いますか？`,
    use,
    other,
    never,
  );
  if (pick === use) {
    await setAppPath(candidates[0]);
  } else if (pick === other) {
    await pickAppPath();
  } else if (pick === never) {
    await context.workspaceState.update(SUPPRESS_KEY, true);
  }
}
