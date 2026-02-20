# SRC# 移植完了計画 / Migration Completion Plan

## 概要 / Overview

本ドキュメントは、SRC（Simulation RPG Construction）のC# .NET移植版であるSRC#の移植完了に向けた計画を記載しています。
C#コード内のTODOコメントとコメントアウトコードを分析し、移植完了までのタスクを整理しました。

This document outlines the plan to complete the SRC# (C# .NET port of SRC - Simulation RPG Construction).
Tasks are organized based on analysis of TODO comments and commented-out code in the C# codebase.

## 統計 / Statistics

- **Total TODO Comments**: 155+
- **Major Commented-Out Code Blocks**: 5+ (200+ lines total)
- **Affected Files**: 70+ files
- **Main Project**: SRC.Sharp/SRCCore

## 現在の状況 / Current Status

> **2026-02-20 時点**

本計画の策定以前から、リポジトリには移植作業に関連する既存Issueがあります。これらは本計画と連携させて管理します。

### 関連する既存Issue / Pre-existing Related Issues

- **#162** [TODO/未実装の処理](https://github.com/7474/SRC/issues/162) - TODOコメントと`NotImplementedException`の網羅的な追跡
- **#172** [バグや目についた未実装メモ](https://github.com/7474/SRC/issues/172) - 発見した未実装・バグのメモ

これらの既存Issueは本計画の各Epicと重複する内容を含んでいます。今後のIssue作成時はこれらへのリンクも考慮してください。

### 運用方針 / Operation Policy

本計画は、Copilot自律エージェントによる単一コマンド運用を基本とします：

- **移植を進める**: `@copilot 移植を進行してください`
- **進捗を更新する**: `@copilot 進捗を更新してください`

GitHub ProjectsやWebUIの手動セットアップは必須ではありません。

---

## カテゴリ別分類 / Category Classification

### 1. 戦闘システム実装 / Combat System Implementation
**優先度**: 高 / Priority: High
**推定作業量**: 大 / Effort: Large

戦闘に関連する機能の実装が必要です。

#### 含まれる機能 / Features:
- 攻撃タイプの実装（回避、受け流し、ダミー、シールド防御、反撃など）
- 合体・融合技の実装
- 特殊効果と効果解除攻撃
- 変身・形態変更メカニズム
- 援護攻撃・援護防御システム

#### 主要な対象ファイル / Key Files:
- `SRC.Sharp/SRCCore/Units/Unit.attackcheck.cs` (13 TODOs)
- `SRC.Sharp/SRCCore/Units/Unit.attack.cs`
- `SRC.Sharp/SRCCore/Units/Unit.attackmap.cs` (4 TODOs)
- `SRC.Sharp/SRCCore/Commands/Command.attack.cs`

---

### 2. ユニット・パイロットシステム / Unit & Pilot System
**優先度**: 高 / Priority: High
**推定作業量**: 中 / Effort: Medium

ユニットとパイロットに関する機能の完成が必要です。

#### 含まれる機能 / Features:
- スキル有効性チェック
- 能力可用性検証
- 変身・形態切替
- パイロット搭乗メカニズム
- ユニット配置可能判定

#### 主要な対象ファイル / Key Files:
- `SRC.Sharp/SRCCore/Units/Unit.lookup.cs` (9 TODOs)
- `SRC.Sharp/SRCCore/Units/Unit.ability.cs`
- `SRC.Sharp/SRCCore/Pilots/Pilot.skill.cs` (4 TODOs + commented code)
- `SRC.Sharp/SRCCore/Units/Unit.otherform.cs`

---

### 3. GUI・UIシステム / GUI & UI System
**優先度**: 中 / Priority: Medium
**推定作業量**: 中 / Effort: Medium

ユーザーインターフェース関連の実装と改善が必要です。

#### 含まれる機能 / Features:
- 武器リストボックス
- アビリティリストボックス
- メッセージフォーム状態管理
- 背景設定とフィルター
- ダイアログシステムの再生

#### 主要な対象ファイル / Key Files:
- `SRC.Sharp/SRCSharpForm/Forms/Main.gui.cs`
- `SRC.Sharp/SRCSharpForm/Forms/Main.guistatus.cs`
- `SRC.Sharp/SRCCore/UIInterface/*.cs`
- `SRC.Sharp/SRCCore/Statuses/Status.cs` (200+ lines commented)

---

### 4. イベント・コマンドシステム / Event & Command System
**優先度**: 中 / Priority: Medium
**推定作業量**: 中 / Effort: Medium

イベントコマンドの実装と改善が必要です。

#### 含まれる機能 / Features:
- Question コマンドの実装
- イベントデータの読込制約対応
- コマンドパース最適化
- 各種未実装コマンドの実装

#### 主要な対象ファイル / Key Files:
- `SRC.Sharp/SRCCore/CmdDatas/Commands/Input/QuestionCmd.cs` (85 lines commented)
- `SRC.Sharp/SRCCore/Events/Event.data.cs` (4 TODOs)
- `SRC.Sharp/SRCCore/CmdDatas/Commands/Screan/PaintStringCmd.cs`
- `SRC.Sharp/SRCCore/CmdDatas/Commands/NotImplementedCmd.cs`

---

### 5. データ管理・永続化 / Data Management & Persistence
**優先度**: 中 / Priority: Medium
**推定作業量**: 小 / Effort: Small

セーブ・ロード機能の改善とデータ管理が必要です。

#### 含まれる機能 / Features:
- セーブ・ロード機能の改善
- パス正規化
- エラーハンドリング強化
- 設定管理システム

#### 主要な対象ファイル / Key Files:
- `SRC.Sharp/SRCCore/SRC.save.cs`
- `SRC.Sharp/SRCCore/SRC.config.cs`
- `SRC.Sharp/SRCCore/Config/LocalFileConfig.cs`

---

### 6. VB6レガシー関数置換 / VB6 Legacy Function Replacement
**優先度**: 低 / Priority: Low
**推定作業量**: 小 / Effort: Small

VB6から移行されていない文字列処理関数などの実装が必要です。

#### 含まれる機能 / Features:
- バイト単位文字列関数（Instrb, Instrrevb, Leftb, Lenb, Midb, Rightb）
- ファイルダイアログ（LoadFileDialog, SaveFileDialog）
- ワイド文字列サポート

#### 主要な対象ファイル / Key Files:
- `SRC.Sharp/SRCCore/VB/Strings.cs`
- `SRC.Sharp/SRCCore/Lib/FileSystem.cs`

---

### 7. パフォーマンス最適化 / Performance Optimization
**優先度**: 低 / Priority: Low
**推定作業量**: 小 / Effort: Small

コードの最適化とリファクタリングが必要です。

#### 含まれる機能 / Features:
- ランダムシーケンス実装
- キャッシング最適化（Sound システムの重複検索）
- エイリアス参照実装の改善
- 配列操作ユーティリティ

#### 主要な対象ファイル / Key Files:
- `SRC.Sharp/SRCCore/Sound.cs`
- `SRC.Sharp/SRCCore/Models/AliasData.cs`
- `SRC.Sharp/SRCCore/VB/SrcArray.cs`

---

### 8. バグ修正・エッジケース対応 / Bug Fixes & Edge Cases
**優先度**: 中〜低 / Priority: Medium-Low
**推定作業量**: 小 / Effort: Small

既知のバグやエッジケースへの対応が必要です。

#### 含まれる機能 / Features:
- 武器選択失敗の対応
- レベルベース除算の処理
- 1オフセット処理の改善
- イベントファイル読込制約

#### 主要な対象ファイル / Key Files:
- Various files with TODO markers for edge cases

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

- 2026-02-20: 現在の状況セクションを追加（既存Issue連携・運用方針の明確化）
- 2026-02-19: 初版作成 (Initial version created)
