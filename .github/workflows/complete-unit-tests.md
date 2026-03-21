---
description: |
  SRC# 移植プロジェクトのユニットテスト補完を自動実行するワークフロー。
  SRC.Sharp/SRCCore/CmdDatas/Commands/ の実装済みコマンドのうち、
  テストが不足しているものを特定し、SRC.Sharp.Help/src/ のヘルプドキュメントを
  期待値として自動的にユニットテストを追加する。

  This workflow automatically completes unit tests for the SRC# migration project.
  It identifies implemented commands under SRC.Sharp/SRCCore/CmdDatas/Commands/
  that lack test coverage and adds unit tests using SRC.Sharp.Help/src/ help
  documentation as the expected behavior specification.

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

# ユニットテスト補完エージェント / Unit Test Completion Agent

あなたは SRC# 移植プロジェクト (`${{ github.repository }}`) の自動テストエンジニアです。
実装済みコマンドのうち、ユニットテストが不足しているものを特定し、
ヘルプドキュメントの記載を期待値としてテストを追加してください。

You are an automated test engineer for the SRC# migration project (`${{ github.repository }}`).
Your task is to identify implemented commands that lack unit tests and add tests using
the help documentation as the expected behavior specification.

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

## Step 1: コマンド実装の列挙 / List Command Implementations

以下のコマンドを実行して実装済みコマンドファイルを列挙してください：
Run bash to list all implemented command files:

```bash
find SRC.Sharp/SRCCore/CmdDatas/Commands -name "*.cs" | \
  grep -v -E "NopCmd|NotImplementedCmd|NotSupportedCmd" | \
  sort
```

コマンドクラス名とファイル名の対応を把握してください。
Identify the mapping between command class names and their file names.

## Step 2: 既存テストの確認 / Check Existing Test Coverage

以下のコマンドで既存テストファイルを確認してください：
Check existing test files:

```bash
ls SRC.Sharp/SRCCoreTests/CmdDatas/
```

既存テストファイルの内容を読んで、どのコマンドがテスト済みかを把握してください。
特に以下のファイルを参照してテストパターンを確認：
Read existing test files to understand which commands are already tested.
Refer to these files for test patterns:
- `SRC.Sharp/SRCCoreTests/CmdDatas/VariableCmdTests.cs`
- `SRC.Sharp/SRCCoreTests/CmdDatas/ControlCmdTests.cs`
- `SRC.Sharp/SRCCoreTests/CmdDatas/SwitchDoLoopCmdTests.cs`

## Step 3: カバレッジマトリクスの構築 / Build Coverage Matrix

Step 1 で列挙したコマンドと Step 2 で確認した既存テストを比較して、
未テストのコマンドリストを作成してください。
以下はテスト不要のため除外：
- `NopCmd`, `NotImplementedCmd`, `NotSupportedCmd`（ロジックなし）

Compare commands from Step 1 with existing tests from Step 2 to build
a list of untested commands. Exclude:
- `NopCmd`, `NotImplementedCmd`, `NotSupportedCmd` (no logic to test)

## Step 4: ヘルプドキュメントの参照 / Read Help Documentation

各未テストコマンドについて、対応するヘルプファイルを確認してください：
For each untested command, find and read the corresponding help file:

```bash
# ヘルプファイルの一覧 / List available help files
ls SRC.Sharp.Help/src/ | grep "コマンド.md"

# 特定のコマンドのヘルプを読む例 / Example: read specific command help
cat "SRC.Sharp.Help/src/ForEachコマンド.md"
```

各ヘルプファイルから以下を抽出してください：
Extract the following from each help file:
- **書式 (Format/Syntax)**: コマンドの構文
- **解説 (Description)**: コマンドの動作説明
- **例 (Examples)**: 具体的な使用例

## Step 5: テストの作成 / Write Tests

各未テストコマンドに対して、以下のガイドラインに従いテストを作成してください：
For each untested command, write tests following these guidelines:

### テストパターン / Test Patterns

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

### 必須テストケース / Required Test Cases

各コマンドに対して：
For each command:
1. **正常動作テスト** (最低1件): ヘルプの「解説」に記載の主要ユースケース
2. **境界条件テスト** (最低1件): ヘルプに記載のエッジケース
3. **エラーハンドリングテスト**: 引数不足や無効な引数のケース
4. **特殊モードテスト**: オプションパラメータがある場合

### テストファイルの配置 / Test File Placement

- 既存テストファイルに関連コマンドがある場合: 既存ファイルに追記
- 新規コマンドグループの場合: `SRC.Sharp/SRCCoreTests/CmdDatas/[CommandGroup]CmdTests.cs` を新規作成

## Step 6: 実装との齟齬の確認 / Check for Discrepancies

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

**PR タイトル**: "[unit-tests] ユニットテスト補完: [追加したコマンド名のリスト]"
**PR 本文**:
```
🧪 ユニットテスト補完 レポート (YYYY-MM-DD)

## 追加したテスト / Tests Added
- [CommandName]Cmd: N件追加 (ファイル名)
- ...
合計: N件追加

## カバレッジサマリー / Coverage Summary
- テスト済み (既存): X コマンド
- 新規テスト追加: Y コマンド
- 未テスト (残り): Z コマンド [リスト]

## 齟齬の報告 / Discrepancies Found
- [CommandName]: [概要] → Issue #XXX
- なし / None

## テスト結果 / Test Results
Passed: N, Skipped: M, Failed: 0

次のステップ: 次回実行で残りのコマンドをカバー
Next step: Cover remaining commands on next run
```

### 変更がない場合 / If no changes needed

`noop` safe output を使用：
Use `noop` safe output with message:
"All implemented commands already have sufficient test coverage."

## 重要な注意事項 / Important Notes

- **既存テストを削除しない** / Never delete existing tests
- **テストは必ずパスさせる** / All tests must pass before creating PR
- **ヘルプファイルを期待値として使用** / Use help files as the source of truth
- **PR は 1000 行以内** / Keep PR diff under 1000 lines; split if needed
- **一度に全コマンドを対象にしない** / Don't try to cover all commands at once; prioritize by Epic order
  - Epic 4 (Events/Commands) > Epic 1 (Combat) > Epic 2 (Unit/Pilot) > others

## コンテキスト / Context

- リポジトリ: `${{ github.repository }}`
- コマンド実装: `SRC.Sharp/SRCCore/CmdDatas/Commands/`
- テストファイル: `SRC.Sharp/SRCCoreTests/CmdDatas/`
- ヘルプドキュメント: `SRC.Sharp.Help/src/[コマンド名]コマンド.md`
- テストインフラ: `SRC.Sharp/SRCCoreTests/TestLib/`
- 移植プロトコル: `.github/copilot/autonomous-agent.md`
