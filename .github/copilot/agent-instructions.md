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

**Version**: 2.0.0  
**Last Updated**: 2026-02-21  
**Phase**: Quality Verification & Accuracy Improvement
