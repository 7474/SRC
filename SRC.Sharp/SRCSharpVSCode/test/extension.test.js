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

  it('.map をマップ作成ツール（SRC#Studio）で開ける（7474/SRC#1641）', async function () {
    this.timeout(90 * 1000);
    const folder = vscode.workspace.workspaceFolders[0].uri.fsPath;
    const uri = vscode.Uri.file(path.join(folder, 'test.map'));
    await vscode.commands.executeCommand('vscode.openWith', uri, 'srcsharp.mapEditor');
    const tab = vscode.window.tabGroups.activeTabGroup.activeTab;
    assert.ok(tab.input instanceof vscode.TabInputCustom, '開いたタブがカスタムエディタでない');
    assert.strictEqual(tab.input.viewType, 'srcsharp.mapEditor');
    // 画面（SRC#Studio の WebAssembly）が地形（拡張に入れた最小の汎用データ）とマップを読めた
    let status;
    for (let i = 0; i < 100 && !(status && status.terrains !== undefined) && !(status && status.errors); i++) {
      await new Promise((r) => setTimeout(r, 500));
      status = await vscode.commands.executeCommand('srcsharp._mapEditorStatus', uri);
    }
    assert.ok(status && status.terrains !== undefined, `マップ作成ツールの画面が読み込みを終えない: ${JSON.stringify(status)}`);
    assert.deepStrictEqual(status.errors ?? [], [], '画面のエラー');
    assert.ok(status.terrains > 0, '地形が無い');
    assert.deepStrictEqual([status.width, status.height], [2, 1]);
    assert.strictEqual(status.placements, 1, 'イベントの配置数が合わない');
    await vscode.commands.executeCommand('workbench.action.closeActiveEditor');
  });

  it('データのプレビューが、カーソルのあるユニットを出す（7474/SRC#1643）', async function () {
    this.timeout(90 * 1000);
    const folder = vscode.workspace.workspaceFolders[0].uri.fsPath;
    const document = await vscode.workspace.openTextDocument(path.join(folder, 'Data', 'robot.txt'));
    const editor = await vscode.window.showTextDocument(document);
    await vscode.commands.executeCommand('srcsharp.showPreview');
    const wait = async (check) => {
      let status;
      for (let i = 0; i < 100; i++) {
        status = await vscode.commands.executeCommand('srcsharp._previewStatus');
        if (status && check(status)) {
          return status;
        }
        await new Promise((r) => setTimeout(r, 500));
      }
      assert.fail(`プレビューが出ない: ${JSON.stringify(status)}`);
    };
    let status = await wait((s) => s.entries.length === 2);
    assert.deepStrictEqual(status.entries, ['試作機', '量産機']);
    assert.strictEqual(status.selected, '試作機');
    // 2 つ目のユニットの行へ動くと、プレビューも移る
    await vscode.window.showTextDocument(document, editor.viewColumn);
    const pos = new vscode.Position(11, 0);
    vscode.window.activeTextEditor.selection = new vscode.Selection(pos, pos);
    status = await wait((s) => s.selected === '量産機');
    assert.deepStrictEqual(status.errors, []);
    await vscode.commands.executeCommand('workbench.action.closeAllEditors');
  });

  it('データの規則のグループ（srcsharp.rules）の既定は src だけ', () => {
    assert.deepStrictEqual(vscode.workspace.getConfiguration('srcsharp').get('rules'), ['src']);
  });
});
