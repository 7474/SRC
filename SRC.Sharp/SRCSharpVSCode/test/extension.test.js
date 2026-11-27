// 拡張が有効になり、package.json のコマンドが登録され、.eve の言語が src-eve になることを確かめる（#1364）。
// 最初の組は srcsharp（Language Server）が無くても動くものだけ。出来事のフォーム・名前を変える・見通しから会話のプレビューは、
// 後の組で srcsharp.dll（SRCAuthoringTests を作るとできる。SRCSHARP_TEST_SERVER でも指せる）を dotnet で動かして確かめる（無ければ飛ばす）
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

  it('イベントの流れの図を表示して描画できる（7474/SRC#1655）', async function () {
    this.timeout(30 * 1000);
    const folder = vscode.workspace.workspaceFolders[0].uri.fsPath;
    await vscode.commands.executeCommand('srcsharp._showEventFlowData', {
      file: 'ep01.eve',
      fullFile: path.join(folder, 'test.eve'),
      scenarioRoot: folder,
      nodes: [
        { id: 'lbl0', name: 'スタート', kind: 'Entry', file: 'ep01.eve', fullFile: path.join(folder, 'test.eve'), line: 1, exists: true },
        { id: 'lbl1', name: '会話サブルーチン', kind: 'Subroutine', file: 'ep01.eve', fullFile: path.join(folder, 'test.eve'), line: 10, exists: true },
        { id: 'miss0', name: '無いサブルーチン', kind: 'Missing', file: '無いサブルーチン', line: 0, exists: false },
      ],
      edges: [
        { from: 'lbl0', to: 'lbl1', kind: 'Call', file: 'ep01.eve', fullFile: path.join(folder, 'test.eve'), line: 3, command: 'Call', target: '会話サブルーチン', condition: 'フラグ = 1' },
        { from: 'lbl0', to: 'miss0', kind: 'Call', file: 'ep01.eve', line: 5, command: 'Call', target: '無いサブルーチン', missing: true },
      ],
      warnings: [
        { file: 'ep01.eve', line: 5, target: '無いサブルーチン', message: 'Call 先のラベル「無いサブルーチン」が見つかりません' },
      ],
      mermaid: 'flowchart TD\n  lbl0(["スタート"]) --> lbl1["会話サブルーチン"]\n',
    });
    let status;
    for (let i = 0; i < 60; i++) {
      status = await vscode.commands.executeCommand('srcsharp._eventFlowStatus');
      if (status && status.rendered) {
        break;
      }
      await new Promise((r) => setTimeout(r, 200));
    }
    assert.ok(status && status.rendered, `イベントの流れの図が描画を終えない: ${JSON.stringify(status)}`);
    assert.strictEqual(status.nodes, 3);
    assert.strictEqual(status.edges, 2);
    assert.strictEqual(status.warnings, 1);
    assert.strictEqual(status.error, undefined);
    await vscode.commands.executeCommand('workbench.action.closeAllEditors');
  });

  it('会話のプレビューを表示してページ情報を描画できる（7474/SRC#1655）', async function () {
    this.timeout(30 * 1000);
    const folder = vscode.workspace.workspaceFolders[0].uri.fsPath;
    // 1x1 の透明 PNG
    const samplePng = 'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNkYAAAAAYAAjCB0C8AAAAASUVORK5CYII=';
    await vscode.commands.executeCommand('srcsharp._showTalkPreviewData', {
      file: path.join(folder, 'test.eve'),
      scenarioRoot: folder,
      cursorLine: 2,
      selectedBlockIndex: 0,
      startPageIndex: 1,
      blocks: [
        { index: 0, command: 'Talk', startLine: 2, endLine: 6, endKind: 'End', label: 'スタート', stepCount: 2, previewText: 'システム: 1ページ目' },
      ],
      pages: [
        { pageIndex: 0, stepIndex: 0, line: 3, command: 'Talk', speaker: 'システム', text: '1ページ目', renderedLines: ['1ページ目'], pngBase64: samplePng },
        { pageIndex: 1, stepIndex: 1, line: 4, command: 'Talk', speaker: 'システム', text: '2ページ目', renderedLines: ['2ページ目'], pngBase64: samplePng },
      ],
      warnings: [],
    });
    let status;
    for (let i = 0; i < 60; i++) {
      status = await vscode.commands.executeCommand('srcsharp._talkPreviewStatus');
      if (status && status.rendered) {
        break;
      }
      await new Promise((r) => setTimeout(r, 200));
    }
    assert.ok(status && status.rendered, `会話のプレビューが描画を終えない: ${JSON.stringify(status)}`);
    assert.strictEqual(status.blocks, 1);
    assert.strictEqual(status.pages, 2);
    assert.strictEqual(status.currentPage, 1);
    assert.strictEqual(status.error, undefined);
    await vscode.commands.executeCommand('workbench.action.closeAllEditors');
  });

  it('データの規則のグループ（srcsharp.rules）の既定は src だけ', () => {
    assert.deepStrictEqual(vscode.workspace.getConfiguration('srcsharp').get('rules'), ['src']);
  });

  it('シナリオの見通しを SRC#Studio の画面で出し、台本の行から会話のプレビューを出す（7474/SRC#1692）', async function () {
    this.timeout(60 * 1000);
    const folder = vscode.workspace.workspaceFolders[0].uri.fsPath;
    await vscode.commands.executeCommand('srcsharp._showOutlineData', folder, {
      scenarioRoot: folder,
      episodes: [{
        file: 'test.eve', title: 'test', joins: ['テスト'], launches: [], leaves: [], enemies: [], next: [], speakers: { 'システム': 1 },
        events: [{ file: 'test.eve', line: 2, label: 'スタート', kind: 'start', when: '戦闘の始め', always: false, conditions: [], actions: ['加入・出撃: テスト（試作機）', '会話'], talkLines: 1 }],
      }],
      script: [{ episode: 'test', event: 'スタート', file: 'test.eve', line: 5, kind: 'talk', speaker: 'システム', text: '拡張のテスト' }],
      findings: [{ file: 'test.eve', line: 2, message: 'テストの気づいたこと' }],
      variables: [],
    });
    const wait = async (check, what) => {
      let status;
      for (let i = 0; i < 100; i++) {
        status = await vscode.commands.executeCommand('srcsharp._outlineStatus');
        assert.ok(!status?.error, `見通しの画面のエラー: ${status?.error}`);
        if (status && check(status)) {
          return status;
        }
        await new Promise((r) => setTimeout(r, 200));
      }
      assert.fail(`${what}: ${JSON.stringify(status)}`);
    };
    let status = await wait((s) => s.episodes === 1, '見通しが出ない');
    assert.deepStrictEqual([status.events, status.script, status.findings, status.rows], [1, 1, 1, 1]);
    // 台本の 1 行目を選ぶと、その行を開き、会話のプレビューを出す
    await vscode.commands.executeCommand('srcsharp._outlineSelect', 'script', 0);
    status = await wait((s) => s.talk, '台本の行から会話のプレビューを出さない');
    assert.deepStrictEqual(status.talk, { file: 'test.eve', line: 5 });
    assert.deepStrictEqual(status.selected, { file: 'test.eve', line: 5, speaker: 'システム' });
    const opened = vscode.window.visibleTextEditors.find((e) => e.document.uri.fsPath === path.join(folder, 'test.eve'));
    assert.ok(opened, '台本の行のファイルを開かない');
    assert.strictEqual(opened.selection.active.line, 4);
    await vscode.commands.executeCommand('workbench.action.closeAllEditors');
  });
});

// srcsharp（Language Server と event-text）が要る機能。CI では先に SRCAuthoringTests を作るので、そこでできた srcsharp.dll を使う。
// 無ければ（SRCSHARP_TEST_SERVER でも指せる）飛ばす
describe('SRC# Authoring（srcsharp を使う）', () => {
  const fs = require('node:fs');
  const os = require('node:os');
  const { execFileSync } = require('node:child_process');
  const dll = process.env.SRCSHARP_TEST_SERVER || path.resolve(__dirname, '..', '..', 'SRCSharpTools', 'bin', 'Debug', 'net11.0', 'srcsharp.dll');
  let dotnet;
  try {
    dotnet = fs.realpathSync(execFileSync(process.platform === 'win32' ? 'where' : 'which', ['dotnet']).toString().split(/\r?\n/)[0].trim());
  } catch {
    dotnet = [path.join(os.homedir(), '.dotnet', 'dotnet')].find((f) => fs.existsSync(f));
  }
  const available = fs.existsSync(dll) && dotnet;
  let scenario;

  before(async function () {
    if (!available) {
      // CI（SRCSHARP_TEST_REQUIRED）では飛ばさずに失敗にする
      assert.ok(!process.env.SRCSHARP_TEST_REQUIRED, `srcsharp が見つからない: ${dll}・${dotnet}`);
      this.skip();
    }
    const config = vscode.workspace.getConfiguration('srcsharp');
    await config.update('serverPath', dotnet, vscode.ConfigurationTarget.Global);
    await config.update('serverArgs', [dll], vscode.ConfigurationTarget.Global);
    // フォームで書き込むシナリオ（テストのたびに作る）
    scenario = fs.mkdtempSync(path.join(os.tmpdir(), 'srcsharp-form-'));
    fs.mkdirSync(path.join(scenario, 'Data'));
    fs.writeFileSync(path.join(scenario, '第1話.eve'), 'スタート:\r\nCreate 味方 試作機 0 テスト 1 1 1\r\nExit\r\n');
  });

  after(async () => {
    await vscode.commands.executeCommand('srcsharp._eventFormAnswers', undefined);
    if (scenario) {
      await vscode.commands.executeCommand('workbench.action.closeAllEditors');
      fs.rmSync(scenario, { recursive: true, force: true });
    }
    if (available) {
      const config = vscode.workspace.getConfiguration('srcsharp');
      await config.update('serverPath', undefined, vscode.ConfigurationTarget.Global);
      await config.update('serverArgs', undefined, vscode.ConfigurationTarget.Global);
    }
  });

  it('出来事をフォームで足し、直す（7474/SRC#1692）', async function () {
    this.timeout(120 * 1000);
    const file = path.join(scenario, '第1話.eve');
    const document = await vscode.workspace.openTextDocument(file);
    const editor = await vscode.window.showTextDocument(document);
    // きっかけ: ターン・3 ターン目・敵のフェイズ。会話: テスト「来たぞ」
    await vscode.commands.executeCommand('srcsharp._eventFormAnswers', ['ターン', '3', '敵', 'テスト', '来たぞ', '（会話はここまで）']);
    await vscode.commands.executeCommand('srcsharp.addEvent');
    let text = document.getText();
    assert.match(text, /^ターン 3 敵:$/m, text);
    assert.match(text, /^Talk テスト\r?\n来たぞ\r?\nEnd$/m, text);
    assert.match(text, /^Create 味方 試作機 0 テスト 1 1 1$/m, '元の行が残らない');
    // 足した出来事にカーソルを置いて直す: 4 ターン目の味方のフェイズへ。会話はそのまま
    const label = document.getText().split(/\r?\n/).findIndex((l) => l === 'ターン 3 敵:');
    editor.selection = new vscode.Selection(label, 0, label, 0);
    await vscode.commands.executeCommand('srcsharp._eventFormAnswers', ['ターン', '4', '味方', 'そのまま']);
    await vscode.commands.executeCommand('srcsharp.editEvent');
    text = document.getText();
    assert.match(text, /^ターン 4 味方:$/m, text);
    assert.doesNotMatch(text, /^ターン 3 敵:$/m, text);
    assert.match(text, /^Talk テスト\r?\n来たぞ\r?\nEnd$/m, text);
    // やめた（答えが尽きた）ときは書き込まない
    await vscode.commands.executeCommand('srcsharp._eventFormAnswers', []);
    await vscode.commands.executeCommand('srcsharp.addEvent');
    assert.strictEqual(document.getText(), text);
    await document.save();
  });

  it('シナリオの見通し（srcsharp outline）の台本の行から、SRCRender で描いた会話のプレビューを出す（7474/SRC#1692）', async function () {
    this.timeout(120 * 1000);
    const folder = vscode.workspace.workspaceFolders[0].uri.fsPath;
    await vscode.window.showTextDocument(await vscode.workspace.openTextDocument(path.join(folder, 'test.eve')));
    await vscode.commands.executeCommand('srcsharp.showOutline');
    let status;
    for (let i = 0; i < 100 && !(status && status.script >= 1); i++) {
      await new Promise((r) => setTimeout(r, 300));
      status = await vscode.commands.executeCommand('srcsharp._outlineStatus');
    }
    assert.ok(status && status.episodes === 1 && status.script === 1, `見通しが出ない: ${JSON.stringify(status)}`);
    await vscode.commands.executeCommand('srcsharp._outlineSelect', 'script', 0);
    let talk;
    for (let i = 0; i < 200 && !(talk && talk.rendered); i++) {
      await new Promise((r) => setTimeout(r, 300));
      talk = await vscode.commands.executeCommand('srcsharp._talkPreviewStatus');
    }
    assert.ok(talk && talk.rendered && !talk.error, `会話のプレビューが出ない: ${JSON.stringify(talk)}`);
    assert.strictEqual(path.basename(talk.file ?? ''), 'test.eve');
    assert.ok(talk.pages >= 1);
    // 作った JSON はシナリオの .srcsharp に置く（後片付け）
    fs.rmSync(path.join(folder, '.srcsharp'), { recursive: true, force: true });
    await vscode.commands.executeCommand('workbench.action.closeAllEditors');
  });

  it('名前を変える（F2）が、イベントとデータの全体を変える（7474/SRC#1692）', async function () {
    this.timeout(120 * 1000);
    const folder = vscode.workspace.workspaceFolders[0].uri.fsPath;
    const uri = vscode.Uri.file(path.join(folder, 'test.eve'));
    await vscode.window.showTextDocument(await vscode.workspace.openTextDocument(uri));
    await vscode.commands.executeCommand('srcsharp.restartServer');
    // 「Create 味方 試作機 0 テスト 1 1 1」の 試作機（2 行目の 10 文字目）
    const position = new vscode.Position(2, 10);
    let edit;
    for (let i = 0; i < 120 && !(edit && edit.size > 0); i++) {
      try {
        edit = await vscode.commands.executeCommand('vscode.executeDocumentRenameProvider', uri, position, '新型機');
      } catch {
        edit = undefined;
      }
      if (!(edit && edit.size > 0)) {
        await new Promise((r) => setTimeout(r, 500));
      }
    }
    assert.ok(edit && edit.size > 0, 'Language Server が名前を変えない');
    // 書き換えはせず、変える所だけを確かめる（テストのフォルダは変えない）
    const files = edit.entries().map(([u, edits]) => [path.relative(folder, u.fsPath).replace(/\\/g, '/'), edits.map((e) => e.newText)]);
    const byFile = Object.fromEntries(files);
    assert.deepStrictEqual(byFile['test.eve'], ['新型機']);
    assert.ok(byFile['Data/robot.txt']?.length >= 1 && byFile['Data/robot.txt'].every((x) => x === '新型機'), JSON.stringify(files));
    await vscode.commands.executeCommand('workbench.action.closeAllEditors');
  });
});
