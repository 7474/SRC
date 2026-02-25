# Copilot Agent Instructions for SRC# Quality Phase

このファイルは、GitHub Copilot AgentがSRC#の**品質検証・精度向上フェーズ**を運用するための指示書です。

This file contains instructions for GitHub Copilot Agents to operate the SRC# **quality verification and accuracy improvement phase**.

## 概要 / Overview

SRC# の移植作業（TODO消化フェーズ）は2026-02-21に完了しました。残存する18件のTODOは新規移植ではなく、精査・最適化・リファクタリングの性質が強い課題です。現在は**品質検証・精度向上フェーズ**にあります。

The SRC# porting (TODO resolution) phase completed on 2026-02-21. The remaining 18 TODOs are quality/optimization tasks, not unimplemented features. The project is now in the **quality verification and accuracy improvement phase**.

### 必要な情報源 / Required Information Sources

1. **`docs/porting/migration-plan.md`** - 残存TODO一覧と現在の状況
2. **`docs/porting/porting-quality-plan.md`** - 品質向上ロードマップ（Phase Q1〜Q4）
3. **`docs/porting/porting-assessment.md`** - 移植状況総合評価と精度リスク分析
4. **`SRC.Sharp.Help/src/`** - コマンドのヘルプドキュメント（テスト期待値の基準）
5. **`SRC/SRC_20121125/`** - 元のVB6コード（動作比較の基準）

## Agent Roles / エージェントの役割

### 1. Test Completion Agent / テスト補完エージェント

**目的 / Purpose**: ユニットテストを補完し、コードカバレッジを向上させる。

**指示 / Instructions**:
- 優先領域（`docs/porting/porting-quality-plan.md` の層1参照）：
  - `SRC.Sharp/SRCCore/Units/` — 86,480行に対しテスト53件（要150件+）
  - `SRC.Sharp/SRCCore/Events/` — 8,789行に対しテスト0件（要30件+）
  - `SRC.Sharp/SRCCore/CmdDatas/Commands/` — 188コマンドに対しテスト99件
- 手順：
  1. `SRC.Sharp/SRCCore/CmdDatas/Commands/` の全コマンドクラスを列挙
  2. `SRC.Sharp/SRCCoreTests/CmdDatas/` の既存テストと比較してカバレッジを確認
  3. 未テストのコマンドを特定
  4. 各コマンドについて `SRC.Sharp.Help/src/[コマンド名]コマンド.md` を参照
  5. ヘルプの「解説」と「例」を期待値としてユニットテストを作成
  6. 実装とヘルプに齟齬がある場合は報告し、実装を修正する
  7. 将来に向けての注意点がある場合は `XXX` タグをつけてコメントを残す
- テストファイル：`SRC.Sharp/SRCCoreTests/CmdDatas/`
- テストパターン：既存の `VariableCmdTests.cs`、`ControlCmdTests.cs`、`SwitchDoLoopCmdTests.cs` に倣う
- テスト名：`[CmdName]Cmd_[Condition]_[ExpectedResult]()`
- 実行：`cd SRC.Sharp && dotnet test SRCCoreTests/SRCCoreTests.csproj`

**齟齬発見時の対応 / Discrepancy Handling**:
```
⚠️ 齟齬発見: [CommandName]コマンド

ヘルプの記載:
  [relevant text from help]

実装の動作:
  [description of what the code actually does]

対応方針:
  ヘルプを正として実装を修正する。修正後にヘルプの記載に沿ったテストを作成する。
```

### 2. Implementation Agent / 実装エージェント

**目的 / Purpose**: 残存する18件のTODO（精査・最適化・リファクタリング課題）を解決する。

**指示 / Instructions**:
- 着手前に読むもの：
  - `docs/porting/migration-plan.md` の「残存TODO一覧」セクション
  - 対象ファイルのTODOコメントと周辺コード
  - VB6の元コード（`SRC/SRC_20121125/`）を参照して期待動作を確認
- 制約：
  - PR差分は1000行以下（横断的関心事を除く）
  - 1 Issue = 1 PR
  - 新機能にはテストを追加
  - 既存テストを実行して回帰がないことを確認
- コミットメッセージ：`[Quality] Brief description (Closes #IssueNumber)`

**残存TODOの場所 / Remaining TODO Locations**:

| ファイル | 内容 |
|---------|------|
| `SRCCore/COM.cs` | 武器選択に失敗してるケースがある |
| `SRCCore/CmdDatas/CmdParser.cs` | Talkの中身が壊れる場面がある可能性（Issue #172） |
| `SRCCore/CmdDatas/Commands/Other/MakePilotListCmd.cs` | パイロット愛称描画をGUIに切り出す |
| `SRCCore/CmdDatas/Commands/Screan/PaintStringCmd.cs` | PaintString文のあらかじめ構文解析（高速化） |
| `SRCCore/Config/LocalFileConfig.cs` | 項目の説明を設定ファイルに書けるようにする |
| `SRCCore/Expressions/Expression.replace.cs` | ref → 戻り値/out 変数の決断 |
| `SRCCore/Lib/GeneralLib.cs` | Byte length 系関数と統合 |
| `SRCCore/SRC.save.cs` | セーブ・ロードの精査（互換性検証） |
| `SRCCore/Sound.cs` | 一度検索したものを再検索している（絶対パス API） |
| `SRCCore/Units/Unit.pilot.cs` | 追加サポートを処理していたか精査 |
| `SRCSharpForm/Forms/Main.guimap.cs` | インタフェースの切り方見直す (Issue #367) |
| `SRCSharpForm/Forms/Main.guimap.cs` | ユニットタイルの読み込み元を変える |
| `SRCSharpForm/Forms/Main.guimap.cs` | フィルタ実装（地形ユニットの特別処理） |
| `SRCSharpForm/Forms/Main.guiscrean.cs` | インタフェースの切り方見直す (Issue #367) |
| `SRCSharpForm/Forms/Main.guiscrean.cs` | FillStyle の全種類対応 |
| `SRCSharpForm/Forms/Main.guistatus.cs` | インタフェースの切り方見直す (Issue #367) |
| `SRCSharpForm/Resoruces/ImageBuffer.cs` | アルファチャネル精査 |
| `SRCSharpForm/RootForm.cs` | 設定の反映処理を設ける（SoundVolume 以外も） |

### 3. Review Agent / レビューエージェント

**目的 / Purpose**: PRの品質と品質向上計画への適合性を確認する。

**指示 / Instructions**:
- 品質基準を確認：
  - PR差分が1000行以下か（超過する場合は理由を明記）
  - テストが追加・更新されているか
  - ドキュメントが更新されているか（APIが変わった場合）
  - 不必要な変更が含まれていないか
  - 既存テストが全て通過しているか
- `docs/porting/porting-quality-plan.md` を参照して変更が品質向上方針に沿っているか確認
- 後方互換性が維持されているか（特にセーブデータ、シナリオファイル）

## 🤖 Autonomous Operation Protocol / 自律運用プロトコル

単一コマンドを受けた際の自律実行手順。各コマンドは「現状評価 → 実行 → 報告 → 継続確認」のサイクルで動作します。

### `@copilot ユニットテストを補完してください`

#### Step 1: 現状評価 / Assess Current State

```bash
# 実装済みコマンド数を確認
find SRC.Sharp/SRCCore/CmdDatas/Commands -name "*.cs" | sort

# 既存テストファイルを確認
ls SRC.Sharp/SRCCoreTests/CmdDatas/

# 現在のテスト数
cd SRC.Sharp && dotnet test --list-tests 2>/dev/null | wc -l
```

#### Step 2: 次のタスク選択 / Select Next Task

未テストコマンドを優先度順で選択：
1. 最も頻繁に使われるコマンド（ヘルプドキュメントが充実しているもの）
2. 既存テストファイルに追加できるもの（新規ファイル不要）
3. 依存関係が少なくテスト書きやすいもの

スキップ対象（テストの意味が薄いもの）：
- `NotImplementedCmd`、`NotSupportedCmd`、`NopCmd`

#### Step 3: 実行 / Execute

1. `SRC.Sharp.Help/src/[コマンド名]コマンド.md` を読んで期待動作を把握
2. 「書式」「解説」「例」セクションからテストケースを設計
3. テスト実装（パターン: 既存の `SwitchDoLoopCmdTests.cs` などを参照）
4. 齟齬があれば実装を修正してからテストを作成
5. `cd SRC.Sharp && dotnet test SRCCoreTests/SRCCoreTests.csproj` で確認
6. PRを作成

#### Step 4: 報告 / Report

```
🧪 ユニットテスト補完 結果 (YYYY-MM-DD)

## 追加したテスト
- [CommandName]Cmd: N件追加 ([FileName].cs)
- ...

## テスト結果
Passed: X, Skipped: Y, Failed: 0

## 齟齬の報告（あれば）
- [CommandName]: [内容] → 実装修正済み / Issue #XXX として記録

## 残り未テストコマンド（抜粋）
- [次の候補コマンド名]
- ...（全体: Z件）

続けますか？ Y/N
```

---

### `@copilot 移植精度を検証してください`

#### Step 1: 現状評価 / Assess Current State

```bash
# テスト件数と直近の変化
cd SRC.Sharp && dotnet test --list-tests 2>/dev/null | wc -l

# MockGUI の未実装数
grep -rn "throw new NotImplementedException" SRC.Sharp/SRCCoreTests/ | wc -l

# 残存TODO数
grep -rn "// TODO" SRC.Sharp/SRCCore/ SRC.Sharp/SRCSharpForm/ | wc -l
```

#### Step 2: 検証対象の選択 / Select Verification Target

`docs/porting/porting-quality-plan.md` の優先度順で対象を選択：
1. 🔴 高優先：Units/ のテスト補強、Events/ のテスト作成
2. 🟡 中優先：CmdDatas/ の統合テスト、セーブデータラウンドトリップ
3. 🟢 低優先：MockGUI NotImplementedException の削減

#### Step 3: 実行 / Execute

- VB6元コード（`SRC/SRC_20121125/`）と比較してテスト期待値を決定
- テスト追加 or MockGUI stub 追加 or 実装修正
- テスト実行で確認

#### Step 4: 報告 / Report

```
🔍 移植精度検証 結果 (YYYY-MM-DD)

## 検証内容
- 対象: [ファイル/領域]
- VB6元コードとの差異: [なし / N件発見]

## 修正した差異
- [ファイル]: [内容] → 修正済み（PR #XXX）

## テスト追加
- 追加: N件（合計: M件）

## MockGUI NotImplementedException
- 削減: X → Y件

続けますか？ Y/N
```

---

### `@copilot 進捗を更新してください`

#### Step 1: 現状収集 / Collect Current State

```bash
# クローズされたIssueを確認
gh issue list --state closed --limit 10 --json number,title,closedAt

# 最近マージされたPR
gh pr list --state merged --limit 10 --json number,title,mergedAt

# 残存TODO数
grep -rn "// TODO" SRC.Sharp/SRCCore/ | wc -l
grep -rn "// TODO" SRC.Sharp/SRCSharpForm/ | wc -l

# テスト件数
cd SRC.Sharp && dotnet test --list-tests 2>/dev/null | wc -l
```

#### Step 2: ドキュメント更新 / Update Documents

変化があった場合、`docs/porting/migration-plan.md` の以下を更新：
- 残存TODO数（Epic別）
- 統計セクション（テスト数など）
- 最近マージされたPR一覧

#### Step 3: 報告 / Report

```
📊 進捗レポート (YYYY-MM-DD)

## 前回更新からの変化
- クローズされたIssue: #XXX, #YYY
- マージされたPR: #ZZZ
- 追加されたテスト: +N件

## 現在の状態
- 残存TODO (SRCCore): X件
- 残存TODO (SRCSharpForm): Y件
- テストメソッド数: Z件

## 更新したドキュメント
- [更新内容があれば記載]

次のアクション候補:
- @copilot ユニットテストを補完してください
- @copilot 移植精度を検証してください
```

---

### 自己修正プロトコル / Self-Correction Protocol

#### テスト失敗時
```
1. 失敗テストのスタックトレースを読む
2. 追加した変更が原因か確認
3. 原因が自分の変更 → 修正して再テスト
4. 原因が既存コードの問題 → Issue として記録し、作業を分離
5. 解決できない場合 → 「🔴 ブロッカー」として人間の判断を求める
```

#### ビルド失敗時
```
1. コンパイルエラーを読む
2. 構文エラー・参照エラーを修正
3. 再ビルド
4. 解決できない場合 → 変更を revert して別アプローチを検討
```

## Common Tasks / 共通タスク

### ユニットテストを補完する
```bash
# 1. 未テストコマンドを特定
find SRC.Sharp/SRCCore/CmdDatas/Commands -name "*.cs" | sort
ls SRC.Sharp/SRCCoreTests/CmdDatas/

# 2. ヘルプドキュメントを参照
cat "SRC.Sharp.Help/src/[コマンド名]コマンド.md"

# 3. テスト追加後に実行
cd SRC.Sharp && dotnet test SRCCoreTests/SRCCoreTests.csproj
```

### 残存TODOを修正する
```bash
# 1. TODOコメントを確認
grep -rn "// TODO" SRC.Sharp/SRCCore/ SRC.Sharp/SRCSharpForm/

# 2. VB6元コードを参照
ls SRC/SRC_20121125/

# 3. 実装後にテスト
cd SRC.Sharp && dotnet test

# 4. PRを作成
gh pr create
```

### 移植精度を検証する
```bash
# 1. 現在のテスト状況を確認
cd SRC.Sharp && dotnet test --collect:"XPlat Code Coverage"

# 2. MockGUIのNotImplementedException数を確認
grep -rn "throw new NotImplementedException" SRC.Sharp/SRCCoreTests/

# 3. VB6版との比較対象を確認
ls SRC/SRC_20121125/
```

## Best Practices / ベストプラクティス

### テスト作成
- ヘルプドキュメント (`SRC.Sharp.Help/src/`) を期待値の基準とする
- 実装とヘルプに齟齬がある場合はヘルプを正として実装を修正する
- 1コマンドにつき：正常動作テスト1件以上、エラーテスト1件以上
- テストヘルパー：
  - `CreateSrc()` — MockGUI付きSRCを生成
  - `BuildEvent(src, lines...)` — テキスト行からイベントコマンド配列を生成
  - `RunEvent(src, cmds)` — イベント実行をシミュレート

### 品質優先順位
1. **高優先**: ユニットテスト追加（Units/, Events/）
2. **高優先**: セーブ・ロード互換性検証
3. **中優先**: 残存TODOの解決
4. **低優先**: パフォーマンス最適化

## Troubleshooting / トラブルシューティング

### テスト失敗
**解決**: 変更による意図しない副作用を確認。既存の動作仮定を壊していないか確認。

### VB6コードとの動作差異
**解決**: `SRC/SRC_20121125/` の元コードを確認。差異を Issue に記録し、VB6版を正として修正するかどうかを判断。

### MockGUI NotImplementedException
**解決**: テストのスコープをコアロジックに限定するか、必要に応じてMockGUIにstubを追加。

## Quick Reference Commands / クイックリファレンスコマンド

```bash
# テスト実行
cd SRC.Sharp && dotnet test

# TODOを検索
grep -rn "// TODO" SRC.Sharp/SRCCore/
grep -rn "// TODO" SRC.Sharp/SRCSharpForm/

# PR作成
gh pr create

# 未テストコマンドを調査
find SRC.Sharp/SRCCore/CmdDatas/Commands -name "*.cs" | wc -l
ls SRC.Sharp/SRCCoreTests/CmdDatas/ | wc -l
```

## Resources / リソース

- **品質計画**: `docs/porting/porting-quality-plan.md`
- **総合評価**: `docs/porting/porting-assessment.md`
- **残存TODO**: `docs/porting/migration-plan.md`
- **元VB6コード**: `SRC/SRC_20121125/`
- **ヘルプドキュメント**: `SRC.Sharp.Help/src/`
- **テストコード**: `SRC.Sharp/SRCCoreTests/`
- **Issue Templates**: `.github/ISSUE_TEMPLATE/`

---

**Version**: 2.1.0  
**Last Updated**: 2026-02-21  
**Phase**: Quality Verification & Accuracy Improvement
