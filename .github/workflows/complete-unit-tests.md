---
description: |
  SRC# 移植プロジェクトのカバレッジ補強を自動実行するワークフロー。
  フェーズ1: SRC.Sharp/SRCCore/CmdDatas/Commands/ の残コマンド (ABGMCmd, AIfCmd, ATalkCmd, FontCmd) を完了する。
  フェーズ2: SRC.Sharp/SRCCore/Commands/ (戦闘・ゲーム操作ロジック) のテストを拡充する。
  フェーズ3: SRC.Sharp/SRCCore/Intermissions/ のテストをゼロから追加する。
  各フェーズをスケジュール実行（日次）や手動実行で反復し、1回の実行でできるだけ多くのテストを追加してカバレッジを向上させる。

  This workflow incrementally raises test coverage for the SRC# migration project.
  Phase 1: Finish remaining CmdDatas commands (ABGMCmd, AIfCmd, ATalkCmd, FontCmd).
  Phase 2: Expand coverage for Commands/ (battle/game-action logic; ~8700 lines, ~11% covered).
  Phase 3: Add tests for Intermissions/ (2200 lines, currently 0% covered).
  Each phase iterates on schedule (daily) or on demand, maximizing the amount of tests added per run.

  NOTE: This workflow runs on windows-latest because SRC.Sharp contains Windows-targeting
  projects (SRCSharpForm, SRCTestForm targeting net8.0-windows). Running on Windows avoids
  the need for EnableWindowsTargeting workarounds and allows full solution build and test.

on:
  schedule: daily
  workflow_dispatch:

# windowsターゲットのプロジェクトも対象とする際にはランナーをWindowsにする必要がある
# runs-on: windows-latest

permissions: read-all

timeout-minutes: 60

network:
  allowed:
    - node
    - github

steps:
  - name: Checkout repository
    uses: actions/checkout@v6
    with:
      fetch-depth: 0
      persist-credentials: false

  - name: Setup .NET
    uses: actions/setup-dotnet@v5
    with:
      dotnet-version: 8.0.x

  # -p:EnableWindowsTargeting=true は Linux ランナーで SRCSharpForm / SRCTestForm など
  # net8.0-windows ターゲットのプロジェクトをビルドするために必要。
  # windows-latest ランナーでは不要だが、ランナー変更時の互換性のために明示的に指定する。
  # This flag is required when building net8.0-windows targeting projects on Linux runners.
  # It is redundant on windows-latest but kept explicitly for cross-platform compatibility.
  - name: Restore and Build
    run: |
      dotnet restore SRC.Sharp/SRC.Sharp.sln -p:EnableWindowsTargeting=true
      dotnet build SRC.Sharp/SRC.Sharp.sln --no-restore -p:EnableWindowsTargeting=true

  - name: Run existing tests (baseline)
    run: |
      dotnet test SRC.Sharp/SRCCoreTests/SRCCoreTests.csproj --no-build --verbosity normal 2>&1 | tee /tmp/gh-aw/test-baseline.txt || true

tools:
  github:
    toolsets: [default]
  bash:
    - "*"
  edit:

safe-outputs:
  create-pull-request:
    title-prefix: "[unit-tests] "
    labels: [automation, testing]
    draft: false
    if-no-changes: warn
    github-token-for-extra-empty-commit: ${{ secrets.CI_GITHUB_TOKEN }}
  create-issue:
    max: 3
    labels: [automation, testing, bug]
  noop:

engine: copilot
---

# カバレッジ補強エージェント / Coverage Reinforcement Agent

あなたは SRC# 移植プロジェクト (`${{ github.repository }}`) の自動テストエンジニアです。
以下のフェーズ順に、テストが不足している領域を特定してテストを追加してください。
1 回の実行でできるだけ多くのテストを追加してください。

You are an automated test engineer for the SRC# migration project (`${{ github.repository }}`).
Identify under-tested areas in the priority order below and add as many tests as possible per run.

**フェーズ優先順位 / Phase Priority**
1. **フェーズ1 (CmdDatas 残件)**: `SRC.Sharp/SRCCore/CmdDatas/Commands/` に残るコマンド (ABGMCmd, AIfCmd, ATalkCmd, FontCmd) を優先的に完了する。
2. **フェーズ2 (Commands/ 拡充)**: `SRC.Sharp/SRCCore/Commands/` のゲームロジック (`Command.*.cs`) を対象にテストを追加する。ソース ~8700 行に対しテストは ~1000 行しかない。
3. **フェーズ3 (Intermissions/ 追加)**: `SRC.Sharp/SRCCore/Intermissions/Intermission.cs` はテストがゼロ (2200 行)。追加する。

## Step 0: 既存PRの確認 / Check for Existing Open PRs

まず、このワークフロー (Agentic Workflow) が作成した PR が既にオープンしているか確認してください。
First, check whether a PR created by this Agentic Workflow is already open:

```bash
gh pr list --repo "${{ github.repository }}" --state open --json title,author \
  --jq '[.[] | select(.title | startswith("[unit-tests]"))]'
```

タイトルが `[unit-tests]` で始まるオープン PR が **1件でも存在する場合**、
既にワークフローによる作業が進行中です。`noop` safe output を使用して処理をスキップしてください：
If **any** open PR whose title starts with `[unit-tests]` exists,
an Agentic Workflow run is already in progress. Use `noop` safe output to skip:

> noop: "Skipping: An open Agentic Workflow PR already exists."

オープン PR がない場合のみ、以下の Step 1 以降を続けてください。
Only proceed to Step 1 and beyond if no such open PR exists.

## Step 1: 現在のフェーズを判定する / Determine the Current Phase

### フェーズ1チェック: CmdDatas 残コマンドの確認

```bash
find SRC.Sharp/SRCCore/CmdDatas/Commands -name "*.cs" | \
  grep -v -E "NopCmd|NotImplementedCmd|NotSupportedCmd" | \
  sort
```

既知の残コマンドは以下の 4 件です（既にテスト済みなら次フェーズへ）：
- `ABGMCmd` (抽象基底, PlayMIDICmd/StartBGMCmd の親)
- `AIfCmd` (非同期 If)
- `ATalkCmd` (非同期 Talk)
- `FontCmd` (フォント設定)

これら 4 件のいずれかが未テストであれば **フェーズ1** として Step 2-8 を実行してください。

### フェーズ2チェック: Commands/ のカバレッジ確認

フェーズ1 が完了している場合は以下を確認してください：

```bash
ls SRC.Sharp/SRCCoreTests/Commands/
wc -l SRC.Sharp/SRCCore/Commands/Command.*.cs
```

テスト済みファイル (CommandPropsTests.cs, SelectedStateTests.cs, UiCommandTests.cs) のみが存在し、
`Command.attack.cs` (1546行), `Command.process.cs` (1617行), `Command.unitability.cs` (742行) 等に
対応するテストがなければ **フェーズ2** として Step 2-8 を実行してください。

### フェーズ3チェック: Intermissions/ のカバレッジ確認

フェーズ2 が完了している場合は以下を確認してください：

```bash
ls SRC.Sharp/SRCCoreTests/Intermissions/ 2>/dev/null || echo "テストなし"
wc -l SRC.Sharp/SRCCore/Intermissions/Intermission.cs
```

`SRCCoreTests/Intermissions/` が存在しない、またはテストが少なければ **フェーズ3** として Step 2-8 を実行してください。

## Step 2: 既存テストの確認 / Check Existing Test Coverage

現在のフェーズに応じて既存テストを確認してください：

### フェーズ1 の場合

```bash
ls SRC.Sharp/SRCCoreTests/CmdDatas/
```

既存テストファイルの内容を読んで、どのコマンドがテスト済みかを把握してください。
テストパターンの参照先：
- `SRC.Sharp/SRCCoreTests/CmdDatas/SwitchDoLoopCmdTests.cs`
- `SRC.Sharp/SRCCoreTests/CmdDatas/VariableCmdTests.cs`
- `SRC.Sharp/SRCCoreTests/CmdDatas/ControlCmdTests.cs`

### フェーズ2 の場合

```bash
ls SRC.Sharp/SRCCoreTests/Commands/
```

既存テストは `CommandPropsTests.cs`, `SelectedStateTests.cs`, `UiCommandTests.cs` のみ。
対象ファイル一覧と対応するソースを確認してください：
- `Command.launch.cs` / `Command.mapcommend.cs` / `Command.state.cs`
- `Command.unitcommand.cs` / `Command.unitmove.cs`
- `Command.unitsp.cs` / `Command.unitsupply.cs` / `Command.unitxxx.cs`
- `Command.attack.cs` / `Command.process.cs` (複雑なため後回し)

テストパターンの参照先：
- `SRC.Sharp/SRCCoreTests/Commands/CommandPropsTests.cs`

### フェーズ3 の場合

```bash
ls SRC.Sharp/SRCCoreTests/Intermissions/ 2>/dev/null || echo "テストなし"
```

`SRC.Sharp/SRCCore/Intermissions/Intermission.cs` を読んで public メソッドを把握してください。

## Step 3: カバレッジマトリクスの構築 / Build Coverage Matrix

現在のフェーズに応じて未テスト対象リストを作成してください：

### フェーズ1 の場合

Step 1 で確認した CmdDatas コマンドと Step 2 で確認した既存テストを比較し、
未テストコマンドリストを作成してください。以下はテスト不要のため除外：
- `NopCmd`, `NotImplementedCmd`, `NotSupportedCmd` (ロジックなし)

### フェーズ2 の場合

`SRC.Sharp/SRCCore/Commands/Command.*.cs` の各ファイルについて、
対応するテストが存在しないものをリストアップしてください。
以下の優先順位で、未テストのファイルをすべて一度に対象にしてテストを追加してください：
1. `Command.state.cs` (51行) — 状態定数のテスト
2. `Command.launch.cs` (179行) — 発進・帰投ロジック
3. `Command.unitmove.cs` (467行) — ユニット移動
4. `Command.unitxxx.cs` (346行) — ユニットその他コマンド
5. `Command.unitsupply.cs` (279行) — 補給
6. `Command.unitsp.cs` (485行) — スペシャルパワー
7. `Command.unitcommand.cs` (498行) — ユニットコマンド
8. `Command.mapcommend.cs` (637行) — マップコマンド
9. `Command.unitability.cs` (742行) — アビリティ
10. `Command.preview.cs` (759行) — プレビュー
11. `Command.unitform.cs` (840行) — ユニットフォーム
12. `Command.attack.cs` / `Command.process.cs` (各1500行超, GUI依存が強いため最後)

### フェーズ3 の場合

`SRC.Sharp/SRCCore/Intermissions/Intermission.cs` の public メソッド一覧を作成し、
テストが存在しないものをすべてリストアップしてください。

## Step 4: 仕様の確認 / Read Specifications

現在のフェーズに応じて、テスト作成の根拠となる情報を収集してください：

### フェーズ1 の場合 (CmdDatas — ヘルプドキュメント参照)

各未テストコマンドについて、対応するヘルプファイルを確認してください：

```bash
# ヘルプファイルの一覧 / List available help files
ls SRC.Sharp.Help/src/ | grep "コマンド.md"

# 特定のコマンドのヘルプを読む例 / Example: read specific command help
cat "SRC.Sharp.Help/src/ABGMコマンド.md"
cat "SRC.Sharp.Help/src/AIfコマンド.md"
cat "SRC.Sharp.Help/src/ATalkコマンド.md"
cat "SRC.Sharp.Help/src/Fontコマンド.md"
```

各ヘルプファイルから以下を抽出してください：
- **書式 (Format/Syntax)**: コマンドの構文
- **解説 (Description)**: コマンドの動作説明
- **例 (Examples)**: 具体的な使用例

### フェーズ2 の場合 (Commands/ — ソースコード直接参照)

ヘルプドキュメントが存在しないため、ソースコードを仕様の根拠とします：

```bash
# 対象ファイルを読む
cat SRC.Sharp/SRCCore/Commands/Command.launch.cs
# または対象ファイルのメソッドシグネチャを一覧
grep -n "public\|internal\|private.*void\|private.*bool\|private.*int\|private.*string" \
  SRC.Sharp/SRCCore/Commands/Command.launch.cs
```

各メソッドについて以下を理解してください：
- **メソッドの目的**: コメントや命名から推測
- **入力条件**: 引数・前提となるゲーム状態 (SelectedUnit, Map, etc.)
- **期待される副作用**: ユニット状態変化、Map 変化、GUI 呼び出し

MockGUI のハンドラを設定することで GUI 依存の部分もテスト可能です：
```bash
cat SRC.Sharp/SRCCore/TestLib/MockGUI.cs
```

### フェーズ3 の場合 (Intermissions/ — ソースコード直接参照)

```bash
cat SRC.Sharp/SRCCore/Intermissions/Intermission.cs
```

public メソッドの仕様をソースコードから読み取り、テスト観点を整理してください。

## Step 5: テストの作成 / Write Tests

現在のフェーズに応じたガイドラインに従いテストを作成してください：

### フェーズ1 テストパターン (CmdDatas コマンド)

既存テストのパターンに従ってください：
Follow the existing test patterns from `SwitchDoLoopCmdTests.cs`:

```csharp
// テストクラスのパターン
[TestClass]
public class [CommandGroup]CmdTests
{
    private SRC CreateSrc()
    {
        var src = new SRC { GUI = new MockGUI() };
        src.Event.EventData = new List<EventDataLine>();
        src.Event.EventCmd = new List<CmdData>();
        src.Event.EventFileNames = new List<string>();
        src.Event.AdditionalEventFileNames = new List<string>();
        src.Event.EventQue = new Queue<string>();
        return src;
    }

    private CmdData[] BuildEvent(SRC src, params string[] lines) { ... }
    private void RunEvent(SRC src, CmdData[] cmds, int startId = 0, int maxSteps = 1000) { ... }

    // テストメソッド名の形式: [CmdName]Cmd_[条件]_[期待結果]
    [TestMethod]
    public void [CmdName]Cmd_[Condition]_[ExpectedResult]()
    {
        // ヘルプの当該セクションを引用するコメント
        // Arrange
        // Act
        // Assert
    }
}
```

### フェーズ2 テストパターン (Commands/ ゲームロジック)

Commands/ は SRC の partial class で構成されています。テストには MockGUI を使い、
GUI のハンドラを必要に応じてセットアップしてください：

```csharp
[TestClass]
public class Command[TargetFile]Tests
{
    private SRC CreateSrc()
    {
        var src = new SRC { GUI = new MockGUI() };
        // 必要に応じてゲーム状態を初期化
        return src;
    }

    // プロパティ・定数のテスト例
    [TestMethod]
    public void [ConstName]_HasExpectedValue()
    {
        // Command.define.cs の定数値が正しいことを確認
        Assert.AreEqual(4, Command.AttackCmdID);
    }

    // 状態変化のテスト例
    [TestMethod]
    public void [MethodName]_[Condition]_[ExpectedResult]()
    {
        // Arrange: ゲーム状態を初期化
        var src = CreateSrc();
        var mock = (MockGUI)src.GUI;
        // MockGUI のハンドラを必要に応じて設定
        // mock.SomeHandler = (...) => { ... };

        // Act
        // src.Commands.SomeMethod(...)

        // Assert
        // Assert.AreEqual(expected, actual);
    }
}
```

テストファイルの配置 (Commands/ フェーズ):
- `SRC.Sharp/SRCCoreTests/Commands/Command[TargetFile]Tests.cs`
- 例: `SRC.Sharp/SRCCoreTests/Commands/CommandLaunchTests.cs`

### フェーズ3 テストパターン (Intermissions/)

```csharp
[TestClass]
public class IntermissionTests
{
    private SRC CreateSrc()
    {
        var src = new SRC { GUI = new MockGUI() };
        // 必要なゲーム状態を設定
        return src;
    }

    [TestMethod]
    public void [MethodName]_[Condition]_[ExpectedResult]()
    {
        // Arrange
        // Act: src.InterMission.SomeMethod(...)
        // Assert
    }
}
```

テストファイルの配置 (Intermissions/ フェーズ):
- `SRC.Sharp/SRCCoreTests/Intermissions/IntermissionTests.cs`

### 必須テストケース / Required Test Cases (全フェーズ共通)

各対象に対して：
1. **正常動作テスト** (最低1件): 主要ユースケース
2. **境界条件テスト** (最低1件): エッジケース
3. **エラーハンドリングテスト**: 無効な引数や状態のケース (可能な場合)
4. **特殊モードテスト**: オプションパラメータがある場合

## Step 6: 実装との齟齬の確認 / Check for Discrepancies

### フェーズ1 の場合

実装がヘルプドキュメントと異なる場合は、以下の方針に従ってください：
If the implementation differs from the help documentation, follow this policy:

**原則**: ヘルプを正として実装を修正する。
**Principle**: Treat the help documentation as correct and fix the implementation.

修正が困難な場合（意図的な差異、後方互換性など）は Issueを作成：
If fixing is difficult (intentional differences, backward compatibility), create an Issue:
```
タイトル: [ユニットテスト補完] [CommandName]: ヘルプとの齟齬
Title: [unit-test] [CommandName]: Discrepancy from help documentation

内容:
- ヘルプの記載
- 実装の実際の動作
- 推奨対応（修正 or ヘルプ更新 or 現状維持）
```

### フェーズ2・3 の場合

ソースコードを仕様の根拠とするため、実装と仕様の齟齬は原則として生じません。
テスト作成中に以下の観点で問題を発見した場合は Issue を作成してください：
- メソッドの動作が不明確で仕様を決定できない場合
- 明らかなバグ（コメントと実装が矛盾するなど）
- テスト不可能な設計（外部依存が強すぎるなど）

```
タイトル: [カバレッジ補強] [ClassName.MethodName]: テスト困難
Title: [coverage] [ClassName.MethodName]: Hard to test
```

## Step 7: テストの実行と検証 / Run and Validate Tests

```bash
cd SRC.Sharp && dotnet test SRCCoreTests/SRCCoreTests.csproj --verbosity normal -p:EnableWindowsTargeting=true 2>&1 | tee /tmp/gh-aw/test-results.txt
```

全テストがパスするまで修正を繰り返してください。
Iterate until all tests pass.

## Step 8: 結果の出力 / Output Results

テストの追加が完了したら：
After completing test additions:

### 変更がある場合 / If there are changes

`create-pull-request` safe output を使って PR を作成：
Use `create-pull-request` safe output to create a PR:

**PR タイトル**: "[unit-tests] カバレッジ補強 フェーズN: [追加/対象のサマリー]"
**PR 本文**:
```
🧪 カバレッジ補強レポート (YYYY-MM-DD)

## 現在のフェーズ / Current Phase
- フェーズN: [CmdDatas 残件完了 / Commands/ 拡充 / Intermissions/ 追加]

## 追加したテスト / Tests Added
- [ClassName / FileName]: N件追加 (ファイル名)
- ...
合計: N件追加

## カバレッジサマリー / Coverage Summary
- フェーズ1 (CmdDatas/Commands): X/Y コマンド完了 (残り: ABGMCmd, AIfCmd, ATalkCmd, FontCmd のうち残件)
- フェーズ2 (Commands/): テストファイル Z件 / ソースファイル W件
- フェーズ3 (Intermissions/): テスト有 or なし

## 齟齬の報告 / Discrepancies Found
- [対象]: [概要] → Issue #XXX
- なし / None

## テスト結果 / Test Results
Passed: N, Skipped: M, Failed: 0

次のステップ: 次回実行で次のフェーズ/ファイルをカバー
Next step: Cover the next phase/file on the next run
```

### 変更がない場合 / If no changes needed

`noop` safe output を使用：
Use `noop` safe output with message:
"All target areas already have sufficient test coverage."

## 重要な注意事項 / Important Notes

- **既存テストを削除しない** / Never delete existing tests
- **テストは必ずパスさせる** / All tests must pass before creating PR
- **フェーズ1**: ヘルプファイルを期待値として使用 / Use help files as the source of truth
- **フェーズ2・3**: ソースコードを仕様の根拠として使用 / Use source code as the specification
- **1回の実行でできるだけ多く対象にする** / Cover as many untested targets as possible per run
- **フェーズ2の複雑ファイル注意**: `Command.attack.cs` と `Command.process.cs` は GUI 依存が強く難易度が高いため、
  他のファイルを先に完了させてから最後に取り組んでください
  `Command.attack.cs` and `Command.process.cs` are GUI-heavy; tackle them last

## コンテキスト / Context

- リポジトリ: `${{ github.repository }}`
- **フェーズ1 ターゲット**
  - コマンド実装: `SRC.Sharp/SRCCore/CmdDatas/Commands/`
  - テストファイル: `SRC.Sharp/SRCCoreTests/CmdDatas/`
  - ヘルプドキュメント: `SRC.Sharp.Help/src/[コマンド名]コマンド.md`
- **フェーズ2 ターゲット**
  - コマンド実装: `SRC.Sharp/SRCCore/Commands/Command.*.cs`
  - テストファイル: `SRC.Sharp/SRCCoreTests/Commands/`
- **フェーズ3 ターゲット**
  - ソース: `SRC.Sharp/SRCCore/Intermissions/Intermission.cs`
  - テストファイル: `SRC.Sharp/SRCCoreTests/Intermissions/`
- テストインフラ: `SRC.Sharp/SRCCore/TestLib/`
- 移植プロトコル: `.github/copilot/autonomous-agent.md`
