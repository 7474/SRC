// 拡張が有効になり、package.json のコマンドが登録され、.eve の言語が src-eve になることを確かめる（#1364）。
// srcsharp（Language Server）が無い環境で動かすので、サーバーが要る機能はここでは確かめない
const assert = require('node:assert');
const path = require('node:path');
const vscode = require('vscode');

const extensionId = 'koudenpa.src-vscode';

describe('SRC# Authoring', () => {
  let extension;

  before(async () => {
    extension = vscode.extensions.getExtension(extensionId);
    assert.ok(extension, `${extensionId} が見つからない`);
    await extension.activate();
  });

  it('有効になる', () => {
    assert.strictEqual(extension.isActive, true);
  });

  it('package.json のコマンドがすべて登録される', async () => {
    const declared = extension.packageJSON.contributes.commands.map((c) => c.command);
    assert.ok(declared.length > 0);
    const registered = new Set(await vscode.commands.getCommands(true));
    const missing = declared.filter((c) => !registered.has(c));
    assert.deepStrictEqual(missing, [], `登録されていないコマンド: ${missing.join(', ')}`);
  });

  it('.eve を開くと言語が src-eve になる', async () => {
    const folder = vscode.workspace.workspaceFolders[0].uri.fsPath;
    const document = await vscode.workspace.openTextDocument(path.join(folder, 'test.eve'));
    assert.strictEqual(document.languageId, 'src-eve');
  });

  it('データのファイル（robot.txt）を開くと言語が src-data になる', async () => {
    const document = await vscode.workspace.openTextDocument(vscode.Uri.parse('untitled:robot.txt'));
    assert.strictEqual(document.languageId, 'src-data');
  });
});
