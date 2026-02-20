# SRC# 移植完了計画 / Migration Completion Plan

## 概要 / Overview

本ドキュメントは、SRC（Simulation RPG Construction）のC# .NET移植版であるSRC#の移植完了に向けた計画を記載しています。
C#コード内のTODOコメントとコメントアウトコードを分析し、移植完了までのタスクを整理しました。

This document outlines the plan to complete the SRC# (C# .NET port of SRC - Simulation RPG Construction).
Tasks are organized based on analysis of TODO comments and commented-out code in the C# codebase.

## 統計 / Statistics

> **2026-02-20 更新 (3回目)** / Updated 2026-02-20

- **残存 TODO コメント (SRCCore)**: 10 / Remaining TODO comments (SRCCore): 10
- **残存 TODO コメント (SRCSharpForm)**: 8 / Remaining TODO comments (SRCSharpForm): 8
- **解消済み TODO**: 137+ (約88%完了) / Resolved TODOs: 137+ (~88% complete)
- **Main Project**: SRC.Sharp/SRCCore

### 最近のマージ済みPR / Recently Merged PRs (2026-02-20)

| # | タイトル / Title | 日付 |
|---|---|---|
| #744 | 移植完了: 全TODO解消・CreateIniFile実装・イベント検証コード整理 | 2026-02-20 |
| #741 | Port ValidateEnvironment, add Plana safety check in TryInstantAbility, clean up resolved TODOs | 2026-02-20 |
| #740 | Remove obsolete Router.PreferExactMatches property | 2026-02-20 |
| #739 | Remove obsolete `PreferExactMatches` from Router component | 2026-02-20 |
| #738 | CI: ユニットテスト実行時のコードカバレッジ収集とワークフローサマリへのレポート | 2026-02-20 |
| #737 | Copilot agentにユニットテスト補完操作モードを追加 | 2026-02-20 |
| #736 | VB6互換: UpVarLevel累積修正・BGMパス解決・対相手メッセージ50%確率修正 | 2026-02-20 |
| #735 | Port LIPS (timed Question command) from VB6 to C# | 2026-02-20 |
| #734 | ユニットテスト補完: Variable/Controlコマンドのテスト追加とReturnCmd/GotoCmdのバグ修正 | 2026-02-20 |
| #733 | Fix several porting bugs: ECM range, message probability, berserk | 2026-02-20 |
| #732 | Add Copilot instructions to respond in Japanese | 2026-02-20 |
| #731 | Fix Unit SaveData: restore Summoner/Master/UnitOnBoard/Servant on load; fix #627 summoned unit revival | 2026-02-20 |
| #730 | Port Help info functions, fix UseAction cap and additional pilot exp bug | 2026-02-20 |
| #729 | Port remaining TODO items: file dialogs, status display, map | 2026-02-20 |
| #728 | Port MainPilot() additional-pilot and berserk-pilot resolution | 2026-02-20 |
| #716 | Port remaining features: Help info functions, UseAction cap, additional pilot exp fix | 2026-02-20 |
| #715 | Fix CD-pages workflow: MSB1003 due to missing project path | 2026-02-20 |
| #714 | docs: 移植ドキュメントへの運用見直しと進捗更新コマンドの追加 | 2026-02-20 |
| #713 | Implement DeleteTemporaryOtherForm and IsAbilityEffective | 2026-02-20 |
| #712 | Preserve empty-string semantics in Loadfiledialog/Savefiledialog | 2026-02-20 |
| #711 | Port commented-out VB6 logic to working C# across combat, pilot, and unit systems | 2026-02-20 |
| #710 | Port random number series and Hiragana StrConv from VB6 | 2026-02-19 |
| #709 | SRC# 移植: 戦闘システム主要TODO実装 | 2026-02-19 |
| #708 | SRC# 移植: 主要TODOの実装 | 2026-02-19 |
| #707 | 移植: LookForAttackHelp / LookForGuardHelp / LookForSupportGuard / LookForSupport / GetExp | 2026-02-19 |
| #706 | 移植: IsAbleToEnter, IsAvailable, IsNecessarySkillSatisfied, IsNecessarySkillSatisfied2 | 2026-02-19 |

### Epic別 残存TODO数 / Remaining TODOs by Epic

| Epic | 残存 TODO (SRCCore) | 残存 TODO (SRCSharpForm) | 主な残タスク |
|------|---------------------|--------------------------|-------------|
| Epic 1: 戦闘システム | 0 ✅ | 0 ✅ | すべて完了 |
| Epic 2: ユニット・パイロット | 1 | 0 | Unit.pilot.cs: 追加サポート精査 |
| Epic 3: GUI・UI | 1 | 5 | フィルタ, タイルUI, インタフェース分割 |
| Epic 4: イベント・コマンド | 2 | 0 | CmdParser Talk, PaintString最適化 |
| Epic 5: データ管理 | 2 | 1 | セーブ精査, 設定反映 |
| Epic 6: VB6レガシー | 2 | 0 | GeneralLib Byte length, Expression ref→out |
| Epic 7: パフォーマンス | 2 | 0 | Sound 再検索, PaintString |
| Epic 8: バグ・エッジケース | 2 | 1 | COM 武器選択, アルファチャネル |
| **合計** | **10** | **8** | |

## 現在の状況 / Current Status

> **2026-02-20 時点**

本計画の策定以前から、リポジトリには移植作業に関連する既存Issueがあります。これらは本計画と連携させて管理します。

### 関連する既存Issue / Pre-existing Related Issues

- **#162** [TODO/未実装の処理](https://github.com/7474/SRC/issues/162) - TODOコメントと`NotImplementedException`の網羅的な追跡
- **#172** [バグや目についた未実装メモ](https://github.com/7474/SRC/issues/172) - 発見した未実装・バグのメモ

これらの既存Issueは本計画の各Epicと重複する内容を含んでいます。今後のIssue作成時はこれらへのリンクも考慮してください。

### 残存 TODO 一覧 / Remaining TODO List

コード内に `// TODO` タグとして残した課題の一覧。`grep -rn "// TODO" SRC.Sharp/` で検索できます。

#### SRCCore (10件)

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

#### SRCSharpForm (8件)

| ファイル | 内容 |
|---------|------|
| `SRCSharpForm/Forms/Main.guimap.cs` | インタフェースの切り方見直す (Issue #367) |
| `SRCSharpForm/Forms/Main.guimap.cs` | ユニットタイルの読み込み元を変える |
| `SRCSharpForm/Forms/Main.guimap.cs` | フィルタ実装（地形ユニットの特別処理） |
| `SRCSharpForm/Forms/Main.guiscrean.cs` | インタフェースの切り方見直す (Issue #367) |
| `SRCSharpForm/Forms/Main.guiscrean.cs` | FillStyle の全種類対応 |
| `SRCSharpForm/Forms/Main.guistatus.cs` | インタフェースの切り方見直す (Issue #367) |
| `SRCSharpForm/Resoruces/ImageBuffer.cs` | アルファチャネル精査 |
| `SRCSharpForm/RootForm.cs` | 設定の反映処理を設ける（SoundVolume 以外も） |

### 運用方針 / Operation Policy

本計画は、Copilot自律エージェントによる単一コマンド運用を基本とします：

- **移植を進める**: `@copilot 移植を進行してください`
- **進捗を更新する**: `@copilot 進捗を更新してください`

GitHub ProjectsやWebUIの手動セットアップは必須ではありません。

---

## カテゴリ別分類 / Category Classification

### 1. 戦闘システム実装 / Combat System Implementation
**優先度**: 高 / Priority: High
**推定作業量**: 完了 ✅ / Effort: Complete ✅
**残存TODO**: 0 ✅

#### 完了した主な機能 / Completed Features:
- ✅ 攻撃タイプの実装（回避、受け流し、ダミー、シールド防御、反撃など）
- ✅ 援護攻撃・援護防御システム（LookForAttackHelp / LookForGuardHelp）
- ✅ 経験値取得（GetExp）
- ✅ 合体・融合技、エイリアス参照、変身した場合の能力処理

#### 主要な対象ファイル / Key Files:
- `SRC.Sharp/SRCCore/Units/Unit.attackcheck.cs` (完了)
- `SRC.Sharp/SRCCore/Units/Unit.attack.cs` (完了)
- `SRC.Sharp/SRCCore/Units/Unit.ability.cs` (完了)
- `SRC.Sharp/SRCCore/Commands/Command.attack.cs` (完了)

---

### 2. ユニット・パイロットシステム / Unit & Pilot System
**優先度**: 高 / Priority: High
**推定作業量**: 中 → 極小（ほぼ完了）/ Effort: Medium → Minimal (nearly complete)
**残存TODO**: 1

#### 完了した主な機能 / Completed Features:
- ✅ IsAbleToEnter（配置可能判定）
- ✅ IsAvailable（ユニット有効性）
- ✅ IsNecessarySkillSatisfied（スキル条件）
- ✅ IsAbilityEffective（アビリティ有効性）
- ✅ DeleteTemporaryOtherForm（一時形態削除）
- ✅ MainPilot() の追加パイロット・バーサーク解決
- ✅ SkillName / SpecialEffect

#### 残存する主な機能 / Remaining Features:
- `Unit.pilot.cs`: 追加サポートの処理箇所精査

#### 主要な対象ファイル / Key Files:
- `SRC.Sharp/SRCCore/Units/Unit.pilot.cs` (1 TODO)

---

### 3. GUI・UIシステム / GUI & UI System
**優先度**: 中 / Priority: Medium
**推定作業量**: 中 / Effort: Medium
**残存TODO**: 9 (SRCCore: 1, SRCSharpForm: 8)

#### 残存する主な機能 / Remaining Features:
- 発進時のユニット表示（母艦の代わり）
- ユニットタイル読み込み元変更、フィルタ実装
- GUI インタフェース分割見直し（guimap, guiscrean, guistatus）
- 縦横比の解決、アルファチャネル精査

#### 主要な対象ファイル / Key Files:
- `SRC.Sharp/SRCCore/Commands/Command.launch.cs` (1 TODO)
- `SRC.Sharp/SRCSharpForm/Forms/Main.gui.cs` (1 TODO)
- `SRC.Sharp/SRCSharpForm/Forms/Main.guimap.cs` (3 TODOs)
- `SRC.Sharp/SRCSharpForm/Forms/Main.guiscrean.cs` (2 TODOs)
- `SRC.Sharp/SRCSharpForm/Forms/Main.guistatus.cs` (1 TODO)
- `SRC.Sharp/SRCSharpForm/Resoruces/ImageBuffer.cs` (1 TODO)

---

### 4. イベント・コマンドシステム / Event & Command System
**優先度**: 中 / Priority: Medium
**推定作業量**: 中 / Effort: Medium
**残存TODO**: 8

#### 完了した主な機能 / Completed Features:
- ✅ LIPS（タイムド Question コマンド）移植 (PR #735)
- ✅ ReturnCmd / GotoCmd のバグ修正 (PR #734)

#### 残存する主な機能 / Remaining Features:
- イベントファイルのロード時の禁則処理
- PaintString最適化（あらかじめ構文解析）
- Wait コマンドの実行権譲渡
- 1オフセット処理の整理
- CmdParser: Talk の中身が壊れるケース

#### 主要な対象ファイル / Key Files:
- `SRC.Sharp/SRCCore/Events/Event.data.cs` (4 TODOs)
- `SRC.Sharp/SRCCore/CmdDatas/Commands/Screan/PaintStringCmd.cs` (1 TODO)
- `SRC.Sharp/SRCCore/CmdDatas/CmdParser.cs` (1 TODO)
- `SRC.Sharp/SRCCore/CmdDatas/Commands/Other/MakePilotListCmd.cs` (1 TODO)
- `SRC.Sharp/SRCCore/CmdDatas/Commands/Screan/ArcCmd.cs` (1 TODO)

---

### 5. データ管理・永続化 / Data Management & Persistence
**優先度**: 中 / Priority: Medium
**推定作業量**: 小 / Effort: Small
**残存TODO**: 4 (SRCCore: 3, SRCSharpForm: 1)

#### 残存する主な機能 / Remaining Features:
- セーブ・ロード機能の改善
- 設定管理システムの独立化
- LocalFileConfig: 設定ファイルへの説明書き出し
- RootForm: 設定の反映処理

#### 主要な対象ファイル / Key Files:
- `SRC.Sharp/SRCCore/SRC.save.cs` (1 TODO)
- `SRC.Sharp/SRCCore/SRC.config.cs` (1 TODO)
- `SRC.Sharp/SRCCore/Config/LocalFileConfig.cs` (1 TODO)
- `SRC.Sharp/SRCSharpForm/RootForm.cs` (1 TODO)

---

### 6. VB6レガシー関数置換 / VB6 Legacy Function Replacement
**優先度**: 低 / Priority: Low
**推定作業量**: 極小 / Effort: Minimal
**残存TODO**: 1

#### 完了した主な機能 / Completed Features:
- ✅ ランダムシーケンス実装 (PR #710)
- ✅ Hiragana StrConv (PR #710)
- ✅ UpdateSupportMod (PR #656, 旧対応)
- ✅ Loadfiledialog, Savefiledialog (PR #712, #729)

#### 残存する主な機能 / Remaining Features:
- バイト系文字列関数の整理（GeneralLib.cs）

#### 主要な対象ファイル / Key Files:
- `SRC.Sharp/SRCCore/Lib/GeneralLib.cs` (1 TODO)

---

### 7. パフォーマンス最適化 / Performance Optimization
**優先度**: 低 / Priority: Low
**推定作業量**: 小 / Effort: Small
**残存TODO**: 2

#### 残存する主な機能 / Remaining Features:
- Sound システムのキャッシュ最適化
- Expression.replace.cs: ref/out 引数への変換判断

#### 主要な対象ファイル / Key Files:
- `SRC.Sharp/SRCCore/Sound.cs` (1 TODO)
- `SRC.Sharp/SRCCore/Expressions/Expression.replace.cs` (1 TODO)

---

### 8. バグ修正・エッジケース対応 / Bug Fixes & Edge Cases
**優先度**: 中〜低 / Priority: Medium-Low
**推定作業量**: 小 / Effort: Small
**残存TODO**: 4

#### 完了した主な機能 / Completed Features:
- ✅ 召喚ユニット復活バグ修正 (#627 解消済み、PR #731)
- ✅ ECM射程・メッセージ確率・バーサーク修正 (PR #733)
- ✅ UpVarLevel累積・BGMパス・対相手メッセージ確率修正 (PR #736)

#### 残存する主な機能 / Remaining Features:
- COM: 武器選択失敗ケースの対応
- COM: EN以外の使用条件確認
- SRC.main: 実行環境依存処理

#### 主要な対象ファイル / Key Files:
- `SRC.Sharp/SRCCore/COM.cs` (2 TODOs)
- `SRC.Sharp/SRCCore/SRC.main.cs` (2 TODOs)

---

## 親Issue構成案 / Parent Issue Structure

以下の親Issueを立て、個別のIssueを関連付けることを推奨します：

### Epic 1: 戦闘システム完成 (Combat System Completion)
- Epic Label: `epic:combat`
- 推定Issue数: 15-20個

### Epic 2: ユニット・パイロットシステム完成 (Unit & Pilot System Completion)
- Epic Label: `epic:unit-pilot`
- 推定Issue数: 12-15個

### Epic 3: GUI・UIシステム改善 (GUI & UI System Enhancement)
- Epic Label: `epic:ui`
- 推定Issue数: 8-10個

### Epic 4: イベント・コマンドシステム完成 (Event & Command System Completion)
- Epic Label: `epic:events`
- 推定Issue数: 10-12個

### Epic 5: データ管理・永続化 (Data Management & Persistence)
- Epic Label: `epic:data`
- 推定Issue数: 5-7個

### Epic 6: VB6レガシー関数置換 (VB6 Legacy Function Replacement)
- Epic Label: `epic:vb6-legacy`
- 推定Issue数: 5-8個

### Epic 7: パフォーマンス最適化 (Performance Optimization)
- Epic Label: `epic:performance`
- 推定Issue数: 5-7個

### Epic 8: バグ修正・エッジケース対応 (Bug Fixes & Edge Cases)
- Epic Label: `epic:bugfix`
- 推定Issue数: 8-10個

---

## 作業の粒度とPR規模 / Task Granularity and PR Size

各Issueは以下の基準で作成することを推奨します：

### 基本原則 / Basic Principles:
- **1 Issue = 1 PR** (原則)
- **PR差分**: 1000行以下を目安（横断的な関心事を除く）
- **単一責任**: 1つのIssueは1つの明確な目的を持つ
- **テスト可能**: 各PRは個別にテスト可能であること

### Issue例 / Example Issues:

#### ✅ 良い粒度 (Good Granularity):
- "Unit.attackcheck.cs: 回避攻撃の実装" (~200-400行)
- "Pilot.skill.cs: IsSkillAvailable()の実装" (~150-300行)
- "QuestionCmd.cs: Question コマンドの実装" (~200-300行)

#### ❌ 大きすぎる粒度 (Too Large):
- "戦闘システムの完全実装" (1500+ 行)
- "全てのTODOを解決" (多数のファイル)

#### 🔄 横断的な関心事の例外 (Cross-Cutting Concerns Exception):
- "VB6文字列関数の一括置換" (複数ファイル、単純な置換)
- "コメントアウトコードの削除" (複数ファイル、削除のみ)

---

## 推奨管理手法 / Recommended Management Approach

### GitHub Projects の活用 / Using GitHub Projects

以下のような Project Board を作成することを推奨します：

#### ボード構成 / Board Structure:
```
┌─────────────┬─────────────┬─────────────┬─────────────┬─────────────┐
│   Backlog   │   Ready     │   In Prog   │   Review    │    Done     │
├─────────────┼─────────────┼─────────────┼─────────────┼─────────────┤
│ Epic 1      │ Issue #1    │ Issue #2    │ Issue #3    │ Issue #4    │
│ Epic 2      │ Issue #5    │             │             │ Issue #6    │
│ Epic 3      │             │             │             │             │
└─────────────┴─────────────┴─────────────┴─────────────┴─────────────┘
```

### ラベル体系 / Label System:

#### Epic ラベル / Epic Labels:
- `epic:combat` - 戦闘システム
- `epic:unit-pilot` - ユニット・パイロット
- `epic:ui` - GUI・UI
- `epic:events` - イベント・コマンド
- `epic:data` - データ管理
- `epic:vb6-legacy` - VB6レガシー
- `epic:performance` - パフォーマンス
- `epic:bugfix` - バグ修正

#### 優先度ラベル / Priority Labels:
- `priority:high` - 高優先度
- `priority:medium` - 中優先度
- `priority:low` - 低優先度

#### 作業タイプラベル / Work Type Labels:
- `type:feature` - 新機能実装
- `type:enhancement` - 改善
- `type:bugfix` - バグ修正
- `type:refactor` - リファクタリング
- `type:docs` - ドキュメント

#### サイズラベル / Size Labels:
- `size:xs` - ~100行
- `size:s` - ~200-400行
- `size:m` - ~400-700行
- `size:l` - ~700-1000行
- `size:xl` - 1000行以上（横断的な関心事のみ）

### マイルストーン / Milestones:

#### Phase 1: コア機能完成 (v3.1.0)
- Epic 1: 戦闘システム
- Epic 2: ユニット・パイロット
- 目標: 2026年Q2

#### Phase 2: UI/UX改善 (v3.2.0)
- Epic 3: GUI・UI
- Epic 4: イベント・コマンド
- 目標: 2026年Q3

#### Phase 3: 品質向上 (v3.3.0)
- Epic 5: データ管理
- Epic 8: バグ修正
- 目標: 2026年Q4

#### Phase 4: 最適化・完成 (v3.4.0)
- Epic 6: VB6レガシー
- Epic 7: パフォーマンス
- 目標: 2027年Q1

---

## 作業開始手順 / Getting Started

### 1. Epic Issue の作成
各Epic（親Issue）を作成し、以下を記載：
- Epic の目的と範囲
- 含まれる機能一覧
- 推定作業量
- 関連ファイル

### 2. 個別 Issue の作成
各Epicに対して、具体的な作業Issueを作成：
- 明確なタイトル（ファイル名: 実装内容）
- TODOコメントの引用
- 実装方針
- テスト方針
- 推定PR規模

### 3. Project Board への登録
- 全Issueを Project Board に登録
- 適切なラベルを付与
- マイルストーンを設定

### 4. 優先順位付け
- Epic 1 (戦闘システム) から開始
- 依存関係を考慮して順序を決定

---

## その他の推奨事項 / Additional Recommendations

### CI/CD の活用
- 各PRに対して自動テストを実行
- コードカバレッジの測定
- 静的解析の実施

### ドキュメント整備
- 実装したTODOについて、APIドキュメントを整備
- ユーザー向けドキュメントの更新

### コミュニティへの情報発信
- 進捗状況の定期的な共有
- リリースノートの作成

---

## 参考情報 / References

- [SRC 公式サイト](http://www.src-srpg.jpn.org/)
- [SRC# GitHub Repository](https://github.com/7474/SRC)
- [SRC# 動作確認 URL](https://7474.github.io/SRC/)

---

## 更新履歴 / Change History

- 2026-02-20 (4回目): 残存TODO再整理 — 課題が残る TODO タグを復元して検索性を維持 (SRCCore: 10件, SRCSharpForm: 8件)、残存TODO一覧セクションを docs/migration-plan.md に追加 (#744)
- 2026-02-20 (3回目): 移植作業進捗 — CreateIniFile実装、イベント検証コード整理、発進ユニット表示説明追加 (#744)
- 2026-02-20 (2回目): 進捗更新 — 残存TODO SRCCore 20件・SRCSharpForm 9件に更新、Epic1戦闘システム完了、最近のPR一覧を追加 (#728-#740)
- 2026-02-20: 進捗更新 — 残存TODO 54件に更新、Epic別進捗状況を反映、最近のPR一覧を追加
- 2026-02-20: 現在の状況セクションを追加（既存Issue連携・運用方針の明確化）
- 2026-02-19: 初版作成 (Initial version created)
