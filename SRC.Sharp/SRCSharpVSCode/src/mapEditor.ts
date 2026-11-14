import * as fs from 'fs';
import * as path from 'path';
import * as vscode from 'vscode';
import { isAppFolder } from './appPath';
import { scenarioRoot } from './hotReload';

/**
 * マップ（.map）のカスタムエディタ。画面は SRC#Studio（SRC.Sharp/SRCStudio の wwwroot。.vsix の studio/ に入れる）を
 * Webview で動かし、ファイルの読み書きはここで行う（7474/SRC#1641）。
 * 文書はテキストのまま扱う（CustomTextEditor）。保存・元に戻す・未保存の印は VS Code のものを使う。
 */
export class MapEditorProvider implements vscode.CustomTextEditorProvider {
  public static readonly viewType = 'srcsharp.mapEditor';

  /** 画面が読み込みを終えた文書（テスト用の srcsharp._mapEditorStatus が返す） */
  public readonly loaded = new Map<string, { terrains?: number; width?: number; height?: number; errors?: string[]; messages?: string[] }>();

  constructor(private readonly context: vscode.ExtensionContext) {}

  private get studioRoot(): vscode.Uri {
    return vscode.Uri.joinPath(this.context.extensionUri, 'studio');
  }

  async resolveCustomTextEditor(document: vscode.TextDocument, panel: vscode.WebviewPanel): Promise<void> {
    const webview = panel.webview;
    this.loaded.set(document.uri.toString(), { messages: ['resolve'] });
    const roots = folderRoots(document.uri.fsPath, path.join(this.studioRoot.fsPath, 'basic-data'));
    webview.options = {
      enableScripts: true,
      localResourceRoots: [this.studioRoot],
    };
    const html = path.join(this.studioRoot.fsPath, 'vscode.html');
    if (!fs.existsSync(html)) {
      webview.html = `<p>SRC#Studio が拡張に入っていません（tools/studio_bundle.sh で studio/ に置いてから拡張を作ります）。</p>`;
      return;
    }
    const csp = [
      "default-src 'none'",
      `img-src ${webview.cspSource} blob: data:`,
      `style-src ${webview.cspSource} 'unsafe-inline'`,
      // .NET の WebAssembly の実行環境
      `script-src ${webview.cspSource} 'wasm-unsafe-eval'`,
      `connect-src ${webview.cspSource}`,
    ].join('; ');
    webview.html = fs.readFileSync(html, 'utf8')
      .replace(/\{\{csp\}\}/g, csp)
      .replace(/\{\{base\}\}/g, webview.asWebviewUri(this.studioRoot).toString() + '/');

    let applying = false;
    const files = new Map<number, Promise<string[]>>();
    const listFiles = (i: number): Promise<string[]> => {
      if (!files.has(i)) {
        files.set(i, listWanted(roots[i].dir));
      }
      return files.get(i)!;
    };
    const subscriptions: vscode.Disposable[] = [];
    subscriptions.push(webview.onDidReceiveMessage(async (msg) => {
      const log = this.loaded.get(document.uri.toString()) ?? {};
      log.messages = [...(log.messages ?? []), msg.type];
      this.loaded.set(document.uri.toString(), log);
      switch (msg.type) {
        case 'ready':
          await webview.postMessage({
            type: 'init',
            roots: await Promise.all(roots.map(async (r, i) => ({ name: r.name, files: await listFiles(i) }))),
            text: document.getText(),
          });
          break;
        case 'read': {
          const root = roots[msg.root];
          const full = root && path.resolve(root.dir, msg.path);
          if (!root || !full.startsWith(path.resolve(root.dir) + path.sep)) {
            await webview.postMessage({ type: 'fileError', id: msg.id, message: 'bad path' });
            break;
          }
          try {
            const bytes = await vscode.workspace.fs.readFile(vscode.Uri.file(full));
            await webview.postMessage({ type: 'file', id: msg.id, bytes: Array.from(bytes) });
          } catch (e) {
            await webview.postMessage({ type: 'fileError', id: msg.id, message: String(e) });
          }
          break;
        }
        case 'edit': {
          if (msg.text === document.getText()) {
            break;
          }
          const edit = new vscode.WorkspaceEdit();
          edit.replace(document.uri, new vscode.Range(0, 0, document.lineCount, 0), msg.text);
          applying = true;
          try {
            await vscode.workspace.applyEdit(edit);
          } finally {
            applying = false;
          }
          break;
        }
        case 'loaded':
          this.loaded.set(document.uri.toString(), { ...this.loaded.get(document.uri.toString()), terrains: msg.terrains, width: msg.width, height: msg.height });
          break;
        case 'undo':
          await vscode.commands.executeCommand('undo');
          break;
        case 'redo':
          await vscode.commands.executeCommand('redo');
          break;
        case 'reopenAsText':
          await vscode.commands.executeCommand('vscode.openWith', document.uri, 'default');
          break;
        case 'error': {
          const state = this.loaded.get(document.uri.toString()) ?? {};
          state.errors = [...(state.errors ?? []), msg.message];
          this.loaded.set(document.uri.toString(), state);
          void vscode.window.showWarningMessage(`SRC#Studio: ${msg.message}`);
          break;
        }
      }
    }));
    // 元に戻す・外での変更を画面に伝える（自分の編集は画面が無視する）
    subscriptions.push(vscode.workspace.onDidChangeTextDocument((e) => {
      if (e.document.uri.toString() === document.uri.toString() && e.contentChanges.length > 0 && !applying) {
        void webview.postMessage({ type: 'update', text: document.getText() });
      }
    }));
    panel.onDidDispose(() => subscriptions.forEach((s) => s.dispose()));
  }
}

interface FolderRoot {
  name: string;
  dir: string;
}

/**
 * 画像を探すフォルダ（SRCCore と同じく、シナリオ → SRC 本体の順）。SRC 本体は設定（srcsharp.appPath）か、シナリオの親のうち
 * Lib か Data/System のあるもの。無ければ拡張に入れた最小の汎用データ
 */
export function folderRoots(mapFile: string, basicData: string): FolderRoot[] {
  const scenario = scenarioRoot(mapFile);
  const roots: FolderRoot[] = [{ name: path.basename(scenario), dir: scenario }];
  let app = vscode.workspace.getConfiguration('srcsharp').get<string>('appPath') || undefined;
  if (!app) {
    for (let d = path.dirname(scenario); ; d = path.dirname(d)) {
      if (isAppFolder(d)) {
        app = d;
        break;
      }
      if (path.dirname(d) === d) {
        break;
      }
    }
  }
  if (app && path.resolve(app) !== path.resolve(scenario)) {
    roots.push({ name: path.basename(app), dir: app });
  } else if (!app) {
    roots.push({ name: 'basic-data', dir: basicData });
  }
  return roots;
}

/** 地形データ・マップの画像・マップだけを一覧にする（SRC#Studio の folders.js の wanted と同じ） */
export async function listWanted(dir: string): Promise<string[]> {
  const result: string[] = [];
  const skip = ['sound', 'midi', 'music', 'save', '.git', 'node_modules'];
  async function walk(rel: string): Promise<void> {
    let entries: [string, vscode.FileType][];
    try {
      entries = await vscode.workspace.fs.readDirectory(vscode.Uri.file(path.join(dir, rel)));
    } catch {
      return;
    }
    for (const [name, type] of entries) {
      const p = rel ? `${rel}/${name}` : name;
      const lower = p.toLowerCase();
      if (type & vscode.FileType.Directory) {
        const top = lower.split('/')[0];
        if (top === 'bitmap' ? lower === 'bitmap' || lower === 'bitmap/map' || lower.startsWith('bitmap/map/') : !skip.includes(top)) {
          await walk(p);
        }
      } else if (lower === 'data/system/terrain.txt' || (lower.startsWith('bitmap/map/') && /\.(bmp|png)$/.test(lower)) || lower.endsWith('.map')) {
        result.push(p);
      }
    }
  }
  await walk('');
  return result.sort();
}
