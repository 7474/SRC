import * as path from 'path';
import * as vscode from 'vscode';

export interface AddDataRequest {
  kind: 'unit' | 'pilot' | string;
  name: string;
  text: string;
  fromClipboard: boolean;
}

/** vscode://koudenpa.src-vscode/addData?kind=...&name=...&text=... を解釈する（7474/SRC#1656） */
export function parseAddDataUri(uri: vscode.Uri): AddDataRequest | undefined {
  const route = uri.path.replace(/^\/+/, '');
  if (route.toLowerCase() !== 'adddata') {
    return undefined;
  }
  const params = new URLSearchParams(uri.query);
  const kind = (params.get('kind') ?? 'unit').toLowerCase();
  const name = params.get('name') ?? '';
  const text = params.get('text') ?? '';
  const fromClipboard = params.get('fromClipboard') === '1';
  return { kind, name, text, fromClipboard };
}

function matchesTargetFile(filePath: string, kind: string): boolean {
  const base = path.basename(filePath).toLowerCase();
  if (kind === 'pilot') {
    return base === 'pilot.txt';
  }
  return base === 'robot.txt' || base === 'unit.txt';
}

export class SrcUriHandler implements vscode.UriHandler {
  async handleUri(uri: vscode.Uri): Promise<void> {
    await this.handle(uri);
  }

  /** URI を処理して開いたドキュメントを返す（テスト用に保存先の指定を受け取れる） */
  async handle(uri: vscode.Uri, targetOverride?: 'new' | vscode.Uri): Promise<vscode.TextDocument | undefined> {
    const req = parseAddDataUri(uri);
    if (!req) {
      return undefined;
    }
    let body = req.text;
    if (!body && req.fromClipboard) {
      body = await vscode.env.clipboard.readText();
    }
    const normalized = body.replace(/\r\n/g, '\n').trim();
    if (!normalized) {
      void vscode.window.showWarningMessage('SRC#: 受け取ったデータが空です。');
      return undefined;
    }

    let target: 'new' | vscode.Uri | undefined = targetOverride;
    if (!target) {
      const candidates = await this.findCandidateFiles(req.kind);
      if (candidates.length === 0) {
        target = 'new';
      } else {
        type PickItem = vscode.QuickPickItem & { uri?: vscode.Uri };
        const items: PickItem[] = [
          ...candidates.map((u) => {
            const folder = vscode.workspace.getWorkspaceFolder(u);
            const rel = folder ? path.relative(folder.uri.fsPath, u.fsPath) : u.fsPath;
            return {
              label: path.basename(u.fsPath),
              description: rel !== path.basename(u.fsPath) ? rel : path.dirname(u.fsPath),
              uri: u,
            };
          }),
          { label: '$(new-file) 新しいファイルで開く', description: '無題のデータファイルとして開く' },
        ];
        const titleName = req.name || 'データ';
        const picked = await vscode.window.showQuickPick(items, {
          title: `SRC#: 「${titleName}」を追加するファイル`,
          placeHolder: '追加先のファイルを選んでください',
        });
        if (!picked) {
          return undefined;
        }
        target = picked.uri ?? 'new';
      }
    }

    if (target === 'new') {
      const doc = await vscode.workspace.openTextDocument({
        language: 'src-data',
        content: normalized + '\n',
      });
      await vscode.window.showTextDocument(doc);
      return doc;
    }

    const doc = await vscode.workspace.openTextDocument(target);
    const eol = doc.eol === vscode.EndOfLine.CRLF ? '\r\n' : '\n';
    const existing = doc.getText().replace(/\r\n/g, '\n');
    const prefix = existing.length === 0
      ? ''
      : existing.endsWith('\n\n')
        ? ''
        : existing.endsWith('\n')
          ? '\n'
          : '\n\n';
    const insertText = (prefix + normalized + '\n').replace(/\n/g, eol);
    const insertPos = doc.lineCount > 0 ? doc.lineAt(doc.lineCount - 1).range.end : new vscode.Position(0, 0);
    const targetStartLine = doc.lineCount > 0 ? doc.lineCount - 1 + (prefix.match(/\n/g)?.length ?? 0) : 0;

    const edit = new vscode.WorkspaceEdit();
    edit.insert(doc.uri, insertPos, insertText);
    await vscode.workspace.applyEdit(edit);

    const editor = await vscode.window.showTextDocument(doc);
    const pos = new vscode.Position(Math.min(doc.lineCount - 1, targetStartLine), 0);
    editor.selection = new vscode.Selection(pos, pos);
    editor.revealRange(new vscode.Range(pos, pos), vscode.TextEditorRevealType.InCenter);
    const displayName = req.name ? `「${req.name}」` : 'データ';
    void vscode.window.showInformationMessage(`SRC#: ${displayName}を ${path.basename(doc.fileName)} に追加しました。`);
    return doc;
  }

  private async findCandidateFiles(kind: string): Promise<vscode.Uri[]> {
    const found = await vscode.workspace.findFiles('**/*.txt', '{**/node_modules/**,**/.git/**}', 200);
    const matched = found
      .filter((u) => matchesTargetFile(u.fsPath, kind))
      .sort((a, b) => a.fsPath.localeCompare(b.fsPath));
    const activeUri = vscode.window.activeTextEditor?.document.uri;
    if (activeUri && activeUri.scheme === 'file' && matchesTargetFile(activeUri.fsPath, kind)) {
      return [activeUri, ...matched.filter((u) => u.fsPath !== activeUri.fsPath)];
    }
    return matched;
  }
}
