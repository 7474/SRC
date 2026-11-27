import * as path from 'path';
import * as vscode from 'vscode';
import { setStudioHtml } from './mapEditor';

/** srcsharp outline --format json の結果（SRCAuthoring の ScenarioOutline.ToJson） */
export interface ScenarioOutlineData {
  scenarioRoot?: string;
  episodes: { file: string; title: string; events: unknown[]; [key: string]: unknown }[];
  script: { file: string; line: number; speaker?: string; text: string; [key: string]: unknown }[];
  findings: { file: string; line: number; message: string }[];
  variables: unknown[];
}

export interface OutlineStatus {
  section?: string;
  episodes?: number;
  events?: number;
  script?: number;
  findings?: number;
  variables?: number;
  rows?: number;
  selected?: { file: string; line: number; speaker: string } | null;
  /** 台本の行から出した会話のプレビュー（ファイルと行） */
  talk?: { file: string; line: number };
  error?: string;
}

export interface OutlineHooks {
  /** 作り直す（srcsharp outline をもう一度） */
  refresh(root: string): Promise<void>;
  /** Markdown に書き出して横に出す（.srcsharp/見通し.md） */
  exportMarkdown(root: string): Promise<void>;
  /** 会話のプレビュー（talk-preview）を、そのファイルの行で出す */
  previewTalk(uri: vscode.Uri, line: number): Promise<void>;
}

/**
 * シナリオの見通し（7474/SRC#1692）。SRC#Studio の見通しの画面（studio/vscode-outline.html）を横に出し、
 * 出来事・台本・変数の行からエディタのその行へ飛ぶ。台本の行を選ぶと、会話のプレビュー（SRCRender で描いた画）をその場で出す
 */
export class OutlinePanel implements vscode.Disposable {
  private panel: vscode.WebviewPanel | undefined;
  private ready = false;
  private root = '';
  private data: ScenarioOutlineData | undefined;
  private status: OutlineStatus = {};

  constructor(private readonly context: vscode.ExtensionContext, private readonly hooks: OutlineHooks) {}

  public dispose(): void {
    this.panel?.dispose();
  }

  public getStatus(): OutlineStatus | undefined {
    return this.panel ? { ...this.status } : undefined;
  }

  /** 見通しを出す。root はシナリオのフォルダ（表の file はここからの相対） */
  public async show(root: string, data: ScenarioOutlineData): Promise<void> {
    this.root = data.scenarioRoot && path.isAbsolute(data.scenarioRoot) ? data.scenarioRoot : root;
    this.data = data;
    this.status = {};
    const studioRoot = vscode.Uri.joinPath(this.context.extensionUri, 'studio');
    if (!this.panel) {
      this.ready = false;
      this.panel = vscode.window.createWebviewPanel('srcsharp.outline', 'SRC# シナリオの見通し', { viewColumn: vscode.ViewColumn.Beside, preserveFocus: true },
        { enableScripts: true, localResourceRoots: [studioRoot], retainContextWhenHidden: true });
      this.panel.onDidDispose(() => {
        this.panel = undefined;
        this.ready = false;
      });
      this.panel.webview.onDidReceiveMessage((msg) => void this.receive(msg));
      setStudioHtml(this.panel.webview, studioRoot, 'vscode-outline.html');
    } else {
      this.panel.reveal(undefined, true);
      await this.post();
    }
    this.panel.title = `SRC# 見通し: ${path.basename(this.root)}`;
  }

  /** テスト用: 表と台本の行を選ぶ */
  public async select(section?: string, index?: number): Promise<void> {
    await this.panel?.webview.postMessage({ type: 'select', section, index });
  }

  private async post(): Promise<void> {
    if (this.panel && this.ready && this.data) {
      await this.panel.webview.postMessage({ type: 'outline', outline: this.data });
    }
  }

  private resolve(file: string): vscode.Uri | undefined {
    const full = path.resolve(this.root, file);
    return full.startsWith(path.resolve(this.root)) ? vscode.Uri.file(full) : undefined;
  }

  private async openLine(file: string, line: number): Promise<vscode.Uri | undefined> {
    const uri = this.resolve(file);
    if (!uri) {
      return undefined;
    }
    const pos = new vscode.Position(Math.max(0, line - 1), 0);
    // 見通しの画面に操作を残したまま、エディタ（最初の列）でその行を出す
    await vscode.window.showTextDocument(uri, { viewColumn: vscode.ViewColumn.One, preserveFocus: true, selection: new vscode.Range(pos, pos) });
    return uri;
  }

  private async receive(msg: { type: string; [key: string]: unknown }): Promise<void> {
    switch (msg.type) {
      case 'ready':
        this.ready = true;
        await this.post();
        break;
      case 'shown':
        this.status = { ...this.status, ...(msg.status as OutlineStatus) };
        break;
      case 'openLine':
        await this.openLine(String(msg.file), Number(msg.line));
        break;
      case 'previewTalk': {
        const uri = await this.openLine(String(msg.file), Number(msg.line));
        if (uri) {
          this.status = { ...this.status, talk: { file: String(msg.file), line: Number(msg.line) } };
          await this.hooks.previewTalk(uri, Number(msg.line));
        }
        break;
      }
      case 'refresh':
        await this.hooks.refresh(this.root);
        break;
      case 'export':
        await this.hooks.exportMarkdown(this.root);
        break;
      case 'error':
        this.status = { ...this.status, error: String(msg.message) };
        break;
    }
  }
}
