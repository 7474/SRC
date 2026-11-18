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
        if (status && status.webviewError) {
          assert.fail(`プレビュー画面のエラー: ${JSON.stringify(status)}`);
        }
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

  it('話のつながりの図を表示して描画できる（7474/SRC#1652）', async function () {
    this.timeout(30 * 1000);
    const folder = vscode.workspace.workspaceFolders[0].uri.fsPath;
    await vscode.commands.executeCommand('srcsharp._showFlowData', {
      scenarioRoot: folder,
      scenarioTitle: 'テストシナリオ',
      nodes: [
        { id: 's0', path: 'ep01.eve', fullPath: path.join(folder, 'test.eve'), title: 'ep01', kind: 'Stage', exists: true, entry: true, line: 1, labels: ['プロローグ'] },
        { id: 's1', path: 'ep02.eve', title: 'ep02', kind: 'Stage', exists: true, entry: false, line: 1, labels: [] },
        { id: 'im0', path: 'Lib/shop.eve', title: 'Lib/shop', kind: 'Intermission', exists: true, entry: false, line: 1, labels: [] },
        { id: 'load0', path: 'Data/enemy', title: 'enemy', kind: 'LoadData', exists: true, entry: false, line: 1, labels: [] },
        { id: 'clear', path: '(GameClear)', title: 'クリア', kind: 'GameClear', exists: true, entry: false, line: 1, labels: [] },
        { id: 'miss0', path: 'missing.eve', title: 'missing.eve', kind: 'Missing', exists: false, entry: false, line: 1, labels: [] },
      ],
      edges: [
        { from: 's0', to: 's1', kind: 'Continue', file: 'ep01.eve', fullFile: path.join(folder, 'test.eve'), line: 10, command: 'Continue', target: 'ep02.eve' },
        { from: 's0', to: 'im0', kind: 'IntermissionCommand', file: 'ep01.eve', line: 5, command: 'IntermissionCommand', commandArg: 'ショップ', target: 'Lib/shop.eve' },
        { from: 'im0', to: 'im0', kind: 'IntermissionReturn', file: 'Lib/shop.eve', line: 12, command: 'Continue' },
        { from: 's0', to: 'load0', kind: 'Load', file: 'ep01.eve', line: 3, command: 'Load', target: 'enemy' },
        { from: 's1', to: 'clear', kind: 'GameClear', file: 'ep02.eve', line: 20, command: 'GameClear' },
      ],
      warnings: [
        { file: 'ep01.eve', line: 15, target: 'missing.eve', message: 'Continue 先のイベントファイル「missing.eve」が見つかりません' },
      ],
      mermaid: 'flowchart TD\n  s0["ep01.eve"] --> s1["ep02.eve"]\n',
    });
    let status;
    for (let i = 0; i < 60; i++) {
      status = await vscode.commands.executeCommand('srcsharp._flowStatus');
      if (status && status.rendered) {
        break;
      }
      await new Promise((r) => setTimeout(r, 200));
    }
    assert.ok(status && status.rendered, `話のつながりの図が描画を終えない: ${JSON.stringify(status)}`);
    assert.strictEqual(status.nodes, 6);
    assert.strictEqual(status.edges, 5);
    assert.strictEqual(status.warnings, 1);
    assert.strictEqual(status.error, undefined);
    await vscode.commands.executeCommand('workbench.action.closeAllEditors');
  });

  it('素材の一覧を表示できる（7474/SRC#1654）', async function () {
    this.timeout(30 * 1000);
    const folder = vscode.workspace.workspaceFolders[0].uri.fsPath;
    await vscode.commands.executeCommand('srcsharp._showAssetsData', {
      scenarioRoot: folder,
      roots: [{ kind: 'scenario', name: folder, files: 3 }, { kind: 'app', name: 'basic-data', files: 10 }],
      assets: [
        { name: 'Event/無い.bmp', kind: 'image', missing: true, usages: [{ file: 'test.eve', line: 3 }] },
        { name: '戦闘.mid', kind: 'sound', missing: false, foundIn: 'app', foundPath: 'Midi/戦闘.mid', usages: [{ file: 'test.eve', line: 2 }] },
      ],
      dynamic: [{ file: 'test.eve', line: 5, text: 'PaintPicture $(x).bmp 0 0' }],
      unused: ['Bitmap/Event/使っていない.bmp'],
      checkedFiles: 1,
      missing: 1,
    });
    let status;
    for (let i = 0; i < 60; i++) {
      status = await vscode.commands.executeCommand('srcsharp._assetsStatus');
      if (status && status.rendered) {
        break;
      }
      await new Promise((r) => setTimeout(r, 200));
    }
    assert.ok(status && status.rendered, `素材の一覧が描画を終えない: ${JSON.stringify(status)}`);
    assert.strictEqual(status.assets, 2);
    assert.strictEqual(status.missing, 1);
    assert.strictEqual(status.dynamic, 1);
    assert.strictEqual(status.unused, 1);
    await vscode.commands.executeCommand('workbench.action.closeAllEditors');
  });

  it('データビューアからの URI でユニット・パイロットのデータを開いて追記できる（7474/SRC#1656）', async () => {
    const fs = require('node:fs');
    const os = require('node:os');
    const sampleUnit = '新機体\nしんきたい, しんきたい, 汎用, 1, 2\n陸, 4, M, 1000, 100\n特殊能力なし\n3000, 100, 600, 60\nAAAA, -.bmp\n===\n';
    const uri = vscode.Uri.parse(`vscode://koudenpa.src-vscode/addData?kind=unit&name=${encodeURIComponent('新機体')}&text=${encodeURIComponent(sampleUnit)}`);
    const untitled = await vscode.commands.executeCommand('srcsharp._handleUri', uri, 'new');
    assert.ok(untitled, '無題のドキュメントが開かない');
    assert.strictEqual(untitled.languageId, 'src-data');
    assert.ok(untitled.getText().includes('新機体'));

    const tmpDir = fs.mkdtempSync(path.join(os.tmpdir(), 'src-uri-'));
    try {
      const pilotPath = path.join(tmpDir, 'pilot.txt');
      fs.writeFileSync(pilotPath, '既存人\n既存人, きぞん, 男性, 汎用, AAAA, 100\n特殊能力なし\n100, 100, 100, 100, 100, 100, 普通\nＳＰ, 0\n-.bmp, -.mid\n', 'utf8');
      const samplePilot = '追加人\nついかじん, ついか, 女性, 汎用, AAAA, 120\n特殊能力なし\n140, 145, 150, 150, 140, 155, 強気\nＳＰ, 0\n-.bmp, -.mid\n';
      const pilotUri = vscode.Uri.parse(`vscode://koudenpa.src-vscode/addData?kind=pilot&name=${encodeURIComponent('追加人')}&text=${encodeURIComponent(samplePilot)}`);
      const doc = await vscode.commands.executeCommand('srcsharp._handleUri', pilotUri, vscode.Uri.file(pilotPath));
      assert.ok(doc);
      assert.ok(doc.getText().includes('既存人'));
      assert.ok(doc.getText().includes('\n\n追加人\n'));
    } finally {
      await vscode.commands.executeCommand('workbench.action.closeAllEditors');
      fs.rmSync(tmpDir, { recursive: true, force: true });
    }
  });

  it('データの規則のグループ（srcsharp.rules）の既定は src だけ', () => {
    assert.deepStrictEqual(vscode.workspace.getConfiguration('srcsharp').get('rules'), ['src']);
  });
});
