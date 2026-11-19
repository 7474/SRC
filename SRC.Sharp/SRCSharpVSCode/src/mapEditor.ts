import * as fs from 'fs';
import * as path from 'path';
import * as vscode from 'vscode';
import { isAppFolder } from './appPath';
import { scenarioRoot } from './hotReload';

export interface PlacementFocus {
  file?: string;
  line?: number;
  x?: number;
  y?: number;
  command?: string;
  unit?: string;
}

interface ActivePanel {
  panel: vscode.WebviewPanel;
  scenarioDir: string;
  mapPath: string;
  isLoaded: boolean;
  pendingFocus?: PlacementFocus;
}

/**
 * マップ（.map）のカスタムエディタ。画面は SRC#Studio（SRC.Sharp/SRCStudio の wwwroot。.vsix の studio/ に入れる）を
 * Webview で動かし、ファイルの読み書きはここで行う（7474/SRC#1641）。
 * 文書はテキストのまま扱う（CustomTextEditor）。保存・元に戻す・未保存の印は VS Code のものを使う。
 */
export class MapEditorProvider implements vscode.CustomTextEditorProvider, vscode.Disposable {
  public static readonly viewType = 'srcsharp.mapEditor';

  /** 画面が読み込みを終えた文書（テスト用の srcsharp._mapEditorStatus が返す） */
  public readonly loaded = new Map<string, { terrains?: number; width?: number; height?: number; placements?: number; errors?: string[]; messages?: string[] }>();

  private readonly panels = new Map<string, ActivePanel>();
  private readonly pendingFocusByUri = new Map<string, PlacementFocus>();
  private readonly applyingEventFiles = new Set<string>();
  private readonly eventTimers = new Map<string, NodeJS.Timeout>();
  private readonly watcherDisposable: vscode.Disposable;

  constructor(private readonly context: vscode.ExtensionContext) {
    this.watcherDisposable = vscode.workspace.onDidChangeTextDocument((e) => {
      if (e.document.uri.scheme !== 'file' || !e.document.fileName.toLowerCase().endsWith('.eve') || e.contentChanges.length === 0) {
        return;
      }
      const key = e.document.uri.toString();
      if (this.applyingEventFiles.has(key)) {
        return;
      }
      const existing = this.eventTimers.get(key);
      if (existing) {
        clearTimeout(existing);
      }
      this.eventTimers.set(key, setTimeout(() => {
        this.eventTimers.delete(key);
        const fsPath = path.resolve(e.document.uri.fsPath);
        for (const entry of this.panels.values()) {
          const base = path.resolve(entry.scenarioDir);
          if (fsPath.startsWith(base + path.sep)) {
            const rel = path.relative(base, fsPath).replace(/\\/g, '/');
            void entry.panel.webview.postMessage({ type: 'eventsUpdated', file: rel });
          }
        }
      }, 250));
    });
  }

  dispose(): void {
    this.watcherDisposable.dispose();
    for (const timer of this.eventTimers.values()) {
      clearTimeout(timer);
    }
    this.eventTimers.clear();
  }

  private get studioRoot(): vscode.Uri {
    return vscode.Uri.joinPath(this.context.extensionUri, 'studio');
  }

  /** 指定したマップ（フルパス）を横に開き、配置タブで該当行・座標を出す（7474/SRC#1653） */
  async openMapAt(fullMapPath: string, focus: PlacementFocus): Promise<void> {
    const resolved = path.resolve(fullMapPath);
    for (const [uriStr, entry] of this.panels) {
      if (path.resolve(vscode.Uri.parse(uriStr).fsPath) === resolved) {
        entry.panel.reveal(vscode.ViewColumn.Beside, false);
        if (entry.isLoaded) {
          await entry.panel.webview.postMessage({ type: 'focus', focus });
        } else {
          entry.pendingFocus = focus;
        }
        return;
      }
    }
    const uri = vscode.Uri.file(resolved);
    this.pendingFocusByUri.set(uri.toString(), focus);
    await vscode.commands.executeCommand('vscode.openWith', uri, MapEditorProvider.viewType, vscode.ViewColumn.Beside);
  }

  async resolveCustomTextEditor(document: vscode.TextDocument, panel: vscode.WebviewPanel): Promise<void> {
    const webview = panel.webview;
    const docKey = document.uri.toString();
    this.loaded.set(docKey, { messages: ['resolve'] });
    const roots = folderRoots(document.uri.fsPath, path.join(this.studioRoot.fsPath, 'basic-data'));
    const scenarioDir = roots[0].dir;
    const mapPath = path.relative(scenarioDir, document.uri.fsPath).replace(/\\/g, '/');
    const panelEntry: ActivePanel = {
      panel,
      scenarioDir,
      mapPath,
      isLoaded: false,
      pendingFocus: this.pendingFocusByUri.get(docKey),
    };
    this.pendingFocusByUri.delete(docKey);
    this.panels.set(docKey, panelEntry);

    webview.options = {
      enableScripts: true,
      localResourceRoots: [this.studioRoot],
    };
    if (!setStudioHtml(webview, this.studioRoot, 'vscode.html')) {
      return;
    }

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
      const log = this.loaded.get(docKey) ?? {};
      log.messages = [...(log.messages ?? []), msg.type];
      this.loaded.set(docKey, log);
      switch (msg.type) {
        case 'ready': {
          const focus = panelEntry.pendingFocus;
          panelEntry.pendingFocus = undefined;
          await webview.postMessage({
            type: 'init',
            roots: await Promise.all(roots.map(async (r, i) => ({ name: r.name, files: await listFiles(i) }))),
            mapPath,
            text: document.getText(),
            focus,
          });
          break;
        }
        case 'read':
          await answerRead(webview, roots, msg);
          break;
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
        case 'editEvent': {
          const full = path.resolve(scenarioDir, msg.file);
          if (!full.startsWith(path.resolve(scenarioDir) + path.sep)) {
            break;
          }
          try {
            const eveUri = vscode.Uri.file(full);
            const eveDoc = await vscode.workspace.openTextDocument(eveUri);
            if (msg.line >= 1 && msg.line <= eveDoc.lineCount) {
              const line = eveDoc.lineAt(msg.line - 1);
              if (msg.expectedLine !== undefined && line.text !== msg.expectedLine) {
                await webview.postMessage({ type: 'eventsUpdated', file: msg.file });
                break;
              }
              if (line.text !== msg.newLine) {
                const edit = new vscode.WorkspaceEdit();
                edit.replace(eveUri, line.range, msg.newLine);
                const eveKey = eveUri.toString();
                this.applyingEventFiles.add(eveKey);
                try {
                  await vscode.workspace.applyEdit(edit);
                } finally {
                  this.applyingEventFiles.delete(eveKey);
                }
              }
            }
          } catch (e) {
            void vscode.window.showWarningMessage(`SRC#: イベントファイルの座標を書き換えられませんでした: ${(e as Error)?.message ?? e}`);
          }
          break;
        }
        case 'insertEvent': {
          // 人形劇（7474/SRC#1694）: at 行目（0 始まり）の前に lines を差し込む
          const full = path.resolve(scenarioDir, msg.file);
          if (!full.startsWith(path.resolve(scenarioDir) + path.sep) || !Array.isArray(msg.lines) || msg.lines.length === 0) {
            break;
          }
          try {
            const eveUri = vscode.Uri.file(full);
            const eveDoc = await vscode.workspace.openTextDocument(eveUri);
            const at = Number(msg.at);
            if (!(at >= 0 && at <= eveDoc.lineCount) || (at > 0 && msg.expectedLine !== undefined && eveDoc.lineAt(at - 1).text !== msg.expectedLine)) {
              await webview.postMessage({ type: 'eventsUpdated', file: msg.file });
              break;
            }
            const eol = eveDoc.eol === vscode.EndOfLine.CRLF ? '\r\n' : '\n';
            const edit = new vscode.WorkspaceEdit();
            if (at < eveDoc.lineCount) {
              edit.insert(eveUri, new vscode.Position(at, 0), msg.lines.join(eol) + eol);
            } else {
              edit.insert(eveUri, eveDoc.lineAt(eveDoc.lineCount - 1).range.end, eol + msg.lines.join(eol));
            }
            const eveKey = eveUri.toString();
            this.applyingEventFiles.add(eveKey);
            try {
              await vscode.workspace.applyEdit(edit);
            } finally {
              this.applyingEventFiles.delete(eveKey);
            }
          } catch (e) {
            void vscode.window.showWarningMessage(`SRC#: イベントファイルに Move を書き足せませんでした: ${(e as Error)?.message ?? e}`);
          }
          break;
        }
        case 'loaded':
          panelEntry.isLoaded = true;
          this.loaded.set(docKey, {
            ...this.loaded.get(docKey),
            terrains: msg.terrains,
            width: msg.width,
            height: msg.height,
            placements: msg.placements,
          });
          if (panelEntry.pendingFocus) {
            const focus = panelEntry.pendingFocus;
            panelEntry.pendingFocus = undefined;
            void webview.postMessage({ type: 'focus', focus });
          }
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
          const state = this.loaded.get(docKey) ?? {};
          state.errors = [...(state.errors ?? []), msg.message];
          this.loaded.set(docKey, state);
          void vscode.window.showWarningMessage(`SRC#Studio: ${msg.message}`);
          break;
        }
      }
    }));
    // 元に戻す・外での変更を画面に伝える（自分の編集は画面が無視する）
    subscriptions.push(vscode.workspace.onDidChangeTextDocument((e) => {
      if (e.document.uri.toString() === docKey && e.contentChanges.length > 0 && !applying) {
        void webview.postMessage({ type: 'update', text: document.getText() });
      }
    }));
    panel.onDidDispose(() => {
      this.panels.delete(docKey);
      subscriptions.forEach((s) => s.dispose());
    });
  }
}

/**
 * SRC#Studio の頁（studio/ の vscode.html など）を Webview に出す。無ければ（拡張に入っていなければ）その旨を出して false。
 * 内容のセキュリティポリシーは、.NET の WebAssembly の実行環境に要るもの（wasm-unsafe-eval）だけを足す
 */
export function setStudioHtml(webview: vscode.Webview, studioRoot: vscode.Uri, page: string, connect: string[] = []): boolean {
  const html = path.join(studioRoot.fsPath, page);
  if (!fs.existsSync(html)) {
    webview.html = `<p>SRC#Studio が拡張に入っていません（tools/studio_bundle.sh で studio/ に置いてから拡張を作ります）。</p>`;
    return false;
  }
  const csp = [
    "default-src 'none'",
    `img-src ${webview.cspSource} blob: data:`,
    `style-src ${webview.cspSource} 'unsafe-inline'`,
    `script-src ${webview.cspSource} 'wasm-unsafe-eval'`,
    `connect-src ${[webview.cspSource, ...connect].join(' ')}`,
  ].join('; ');
  webview.html = fs.readFileSync(html, 'utf8')
    .replace(/\{\{csp\}\}/g, csp)
    .replace(/\{\{base\}\}/g, webview.asWebviewUri(studioRoot).toString() + '/');
  return true;
}

/** Webview の read（フォルダの番号と相対パス）に答える */
export async function answerRead(webview: vscode.Webview, roots: FolderRoot[], msg: { id: number; root: number; path: string }): Promise<void> {
  const root = roots[msg.root];
  const full = root && path.resolve(root.dir, msg.path);
  if (!root || !full.startsWith(path.resolve(root.dir) + path.sep)) {
    await webview.postMessage({ type: 'fileError', id: msg.id, message: 'bad path' });
    return;
  }
  try {
    const openDoc = vscode.workspace.textDocuments.find((d) => d.uri.scheme === 'file' && path.resolve(d.uri.fsPath) === full);
    const bytes = openDoc
      ? Buffer.from(openDoc.getText(), 'utf8')
      : await vscode.workspace.fs.readFile(vscode.Uri.file(full));
    await webview.postMessage({ type: 'file', id: msg.id, bytes: Array.from(bytes) });
  } catch (e) {
    await webview.postMessage({ type: 'fileError', id: msg.id, message: String(e) });
  }
}

export interface FolderRoot {
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

/** 地形データ・スペシャルパワー・データ・画像（マップ・ユニット・パイロット）・マップ・イベントだけを一覧にする（SRC#Studio の folders.js の wanted と同じ） */
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
        if (top === 'bitmap' ? lower === 'bitmap' || /^bitmap\/(map|unit|pilot)(\/|$)/.test(lower) : !skip.includes(top)) {
          await walk(p);
        }
      } else if (lower === 'data/system/terrain.txt' || lower === 'data/system/sp.txt'
        || (lower.startsWith('data/') && /(^|\/)(robot|unit|pilot|non_pilot|sp)\.txt$/.test(lower))
        || (/^bitmap\/(map|unit|pilot)\//.test(lower) && /\.(bmp|png|jpg|gif)$/.test(lower))
        || lower.endsWith('.map')
        || lower.endsWith('.eve')) {
        result.push(p);
      }
    }
  }
  await walk('');
  return result.sort();
}
