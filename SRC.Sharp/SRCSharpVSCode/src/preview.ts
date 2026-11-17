import * as path from 'path';
import * as vscode from 'vscode';
import { answerRead, folderRoots, listWanted, setStudioHtml } from './mapEditor';

const DATA_FILE = /^(robot|unit|pilot|pilot_message|pilot_dialog)\.txt$/i;
const MESSAGE_FILE = /^(pilot_message|pilot_dialog)\.txt$/i;

/**
 * データのプレビュー（7474/SRC#1643・#1651）。データのファイル（robot.txt・unit.txt・pilot.txt・pilot_message.txt・pilot_dialog.txt）の横に、
 * カーソルのあるユニット・パイロットを SRC#Studio の画面で出す。打つたび・カーソルを動かすたびに追う
 */
export class DataPreviewPanel implements vscode.Disposable {
  private panel: vscode.WebviewPanel | undefined;
  private document: vscode.TextDocument | undefined;
  private roots: { name: string; dir: string }[] = [];
  private ready = false;
  private timer: NodeJS.Timeout | undefined;
  private readonly subscriptions: vscode.Disposable[] = [];
  /** 最後に出したもの（テスト用の srcsharp._previewStatus が返す） */
  public lastShown: { kind: string; entries: string[]; selected: string | null; errors: string[] } | undefined;

  constructor(private readonly context: vscode.ExtensionContext) {
    this.subscriptions.push(
      vscode.window.onDidChangeActiveTextEditor((e) => e && this.follow(e.document)),
      vscode.workspace.onDidChangeTextDocument((e) => e.document === this.document && this.schedule()),
      vscode.window.onDidChangeTextEditorSelection((e) => e.textEditor.document === this.document && this.schedule()),
    );
  }

  public static isDataFile(document: vscode.TextDocument): boolean {
    return document.uri.scheme === 'file' && DATA_FILE.test(path.basename(document.uri.fsPath));
  }

  /** 開いているデータのファイルのプレビューを横に出す */
  public async show(): Promise<void> {
    const editor = vscode.window.activeTextEditor;
    if (!editor || !DataPreviewPanel.isDataFile(editor.document)) {
      void vscode.window.showErrorMessage('SRC#: データのファイル（robot.txt・unit.txt・pilot.txt・pilot_message.txt・pilot_dialog.txt）を開いてください。');
      return;
    }
    const studioRoot = vscode.Uri.joinPath(this.context.extensionUri, 'studio');
    if (!this.panel) {
      this.panel = vscode.window.createWebviewPanel('srcsharp.dataPreview', 'SRC# データのプレビュー', { viewColumn: vscode.ViewColumn.Beside, preserveFocus: true },
        { enableScripts: true, localResourceRoots: [studioRoot], retainContextWhenHidden: true });
      this.panel.onDidDispose(() => {
        this.panel = undefined;
        this.ready = false;
        this.document = undefined;
      });
      this.panel.webview.onDidReceiveMessage((msg) => this.receive(msg));
      // 「比べる」は 7474/SRC-Data の Pages の評価の JSON を読む
      setStudioHtml(this.panel.webview, studioRoot, 'vscode-preview.html', ['https://7474.github.io']);
    } else {
      this.panel.reveal(vscode.ViewColumn.Beside, true);
    }
    await this.follow(editor.document);
  }

  private async follow(document: vscode.TextDocument): Promise<void> {
    if (!this.panel || !DataPreviewPanel.isDataFile(document)) {
      return;
    }
    const changedFolder = !this.document || path.dirname(this.document.uri.fsPath) !== path.dirname(document.uri.fsPath);
    this.document = document;
    this.panel.title = `SRC# プレビュー: ${path.basename(document.uri.fsPath)}`;
    if (changedFolder) {
      this.roots = folderRoots(document.uri.fsPath, path.join(this.context.extensionUri.fsPath, 'studio', 'basic-data'));
      if (this.ready) {
        await this.sendInit();
      }
    }
    this.schedule(0);
  }

  private async sendInit(): Promise<void> {
    await this.panel?.webview.postMessage({
      type: 'init',
      roots: await Promise.all(this.roots.map(async (r) => ({ name: r.name, files: await listWanted(r.dir) }))),
    });
  }

  private schedule(delay = 300): void {
    if (this.timer) {
      clearTimeout(this.timer);
    }
    this.timer = setTimeout(() => void this.send(), delay);
  }

  private async send(): Promise<void> {
    const document = this.document;
    if (!this.panel || !this.ready || !document) {
      return;
    }
    const editor = vscode.window.visibleTextEditors.find((e) => e.document === document);
    const line = (editor?.selection.active.line ?? 0) + 1;
    const fileName = path.basename(document.uri.fsPath);
    await this.panel.webview.postMessage({
      type: 'data',
      name: fileName,
      text: document.getText(),
      line,
      before: await this.systemTexts(MESSAGE_FILE.test(fileName)),
    });
  }

  /**
   * スペシャルパワーの参照を解くため、SRC 本体 → シナリオの Data/System/sp.txt を先に読む。
   * メッセージ・ダイアログデータの場合は、パイロット・ユニット・イベントも一緒に渡す。
   */
  private async systemTexts(includeScenarioContext: boolean): Promise<{ name: string; text: string }[]> {
    const result: { name: string; text: string }[] = [];
    for (const root of [...this.roots].reverse()) {
      for (const rel of ['Data/System/sp.txt', 'data/System/sp.txt']) {
        try {
          const bytes = await vscode.workspace.fs.readFile(vscode.Uri.file(path.join(root.dir, rel)));
          result.push({ name: `${root.name}/${rel}`, text: decode(bytes) });
          break;
        } catch {
          // 無ければ飛ばす
        }
      }
    }
    if (!includeScenarioContext || this.roots.length === 0) {
      return result;
    }
    const scenarioDir = this.roots[0].dir;
    const files = await listWanted(scenarioDir);
    for (const rel of files) {
      const lower = rel.toLowerCase();
      if ((lower.startsWith('data/') && /(^|\/)(pilot|non_pilot|robot|unit)\.txt$/.test(lower)) || lower.endsWith('.eve')) {
        try {
          const full = path.join(scenarioDir, rel);
          const openDoc = vscode.workspace.textDocuments.find((d) => d.uri.scheme === 'file' && path.resolve(d.uri.fsPath) === path.resolve(full));
          const text = openDoc ? openDoc.getText() : decode(await vscode.workspace.fs.readFile(vscode.Uri.file(full)));
          result.push({ name: rel, text });
        } catch {
          // 読めないファイルは飛ばす
        }
      }
    }
    return result;
  }

  private async receive(msg: { type: string; [key: string]: unknown }): Promise<void> {
    switch (msg.type) {
      case 'ready':
        this.ready = true;
        await this.sendInit();
        this.schedule(0);
        break;
      case 'read':
        if (this.panel) {
          await answerRead(this.panel.webview, this.roots, msg as unknown as { id: number; root: number; path: string });
        }
        break;
      case 'editLines': {
        const document = this.document;
        if (document) {
          const edit = new vscode.WorkspaceEdit();
          for (const e of msg.edits as { line: number; text: string }[]) {
            if (e.line >= 1 && e.line <= document.lineCount) {
              edit.replace(document.uri, document.lineAt(e.line - 1).range, e.text);
            }
          }
          await vscode.workspace.applyEdit(edit);
        }
        break;
      }
      case 'insertLines': {
        const document = this.document;
        if (document) {
          const rawAfter = Number(msg.afterLine);
          const text = String(msg.text ?? '');
          const edit = new vscode.WorkspaceEdit();
          let targetLine = 0;
          if (document.lineCount === 1 && document.lineAt(0).text === '') {
            edit.insert(document.uri, new vscode.Position(0, 0), text);
            targetLine = 0;
          } else {
            const afterLine = rawAfter > 0 ? Math.min(rawAfter, document.lineCount) : document.lineCount;
            const pos = document.lineAt(afterLine - 1).range.end;
            edit.insert(document.uri, pos, '\n' + text);
            targetLine = afterLine;
          }
          await vscode.workspace.applyEdit(edit);
          const editor = await vscode.window.showTextDocument(document, { preserveFocus: false, viewColumn: vscode.window.visibleTextEditors.find((e) => e.document === document)?.viewColumn });
          const targetPos = new vscode.Position(Math.min(document.lineCount - 1, targetLine), 0);
          editor.selection = new vscode.Selection(targetPos, targetPos);
          editor.revealRange(new vscode.Range(targetPos, targetPos), vscode.TextEditorRevealType.InCenter);
        }
        break;
      }
      case 'shown':
        this.lastShown = msg as unknown as DataPreviewPanel['lastShown'];
        break;
      case 'select': {
        // 一覧で選んだデータの名前の行へ
        const document = this.document;
        if (document) {
          const editor = await vscode.window.showTextDocument(document, { preserveFocus: false, viewColumn: vscode.window.visibleTextEditors.find((e) => e.document === document)?.viewColumn });
          const pos = new vscode.Position(Math.max(0, (msg.line as number) - 1), 0);
          editor.selection = new vscode.Selection(pos, pos);
          editor.revealRange(new vscode.Range(pos, pos), vscode.TextEditorRevealType.InCenter);
        }
        break;
      }
      case 'error':
        void vscode.window.showWarningMessage(`SRC#Studio: ${msg.message as string}`);
        break;
    }
  }

  dispose(): void {
    this.panel?.dispose();
    this.subscriptions.forEach((s) => s.dispose());
  }
}

/** UTF-8 として読めなければ Shift_JIS */
function decode(bytes: Uint8Array): string {
  try {
    return new TextDecoder('utf-8', { fatal: true }).decode(bytes).replace(/^﻿/, '');
  } catch {
    return new TextDecoder('shift_jis').decode(bytes);
  }
}
