# SRC# 移植ドキュメント / Porting Documentation

このディレクトリには、SRC#（Simulation RPG Construction Sharp）のVB6→C#移植に関するドキュメントが含まれています。

> **現在のフェーズ**: TODO消化フェーズは完了。現在は**品質検証・精度向上フェーズ**にあります。

## 📚 ドキュメント一覧 / Document List

### 🤖 Copilot自律運用 / Copilot Autonomous Operation

**[Copilot完全自律エージェント](../../.github/copilot/autonomous-agent.md)** 🚀

単一コマンドで品質検証を進行:
```
@copilot ユニットテストを補完してください
```

単一コマンドで進捗を更新:
```
@copilot 進捗を更新してください
```

With single commands:
- ✅ Copilotが次のタスクを自動選択・実装・PR作成
- ✅ Copilotが進行状況をドキュメントに反映
- ✅ 人間が考えることは最小限
- ✅ セットアップ不要

**詳細**: [Copilot Agent Instructions](../../.github/copilot/README.md)

---

### メインドキュメント / Main Documents

1. **[移植計画・進捗](./migration-plan.md)**
   - プロジェクト全体の概要・カテゴリ分類・残存課題一覧
   - Overall project overview, category classification, and remaining tasks

2. **[移植状況 総合評価レポート](./porting-assessment.md)**
   - 定量的な移植進捗分析・リスク分析・GUI固有の評価
   - Comprehensive quantitative porting assessment with risk analysis

3. **[移植精度 評価・向上計画](./porting-quality-plan.md)**
   - 移植精度の評価方法論・品質向上ロードマップ（現フェーズの主要文書）
   - Quality evaluation methodology and improvement roadmap (primary doc for current phase)

4. **[個別Issue詳細](./issue-breakdown.md)**（参考資料）
   - TODO消化フェーズで定義した約70個のIssueリスト（歴史的参考）
   - ~70 specific issues defined during TODO resolution phase (historical reference)

## 🎯 プロジェクト概要 / Project Overview

### 統計 / Statistics (2026-02-21 更新)

- **残存 TODO Comments**: 18件（SRCCore: 10, SRCSharpForm: 8）— 初期155+件から88%解消（残存は精査・最適化課題）
- **移植フェーズ**: **品質検証・精度向上フェーズ**（TODO消化フェーズ完了）
- **ユニットテスト**: 253テストメソッド（31ファイル）
- **Epic Categories**: 8個
- **詳細分析**: [移植状況 総合評価レポート](./porting-assessment.md) を参照

### Epic一覧 / Epic List

1. **Epic 1: 戦闘システム完成** (`epic:combat`) - 15-20個のIssue
2. **Epic 2: ユニット・パイロットシステム完成** (`epic:unit-pilot`) - 12-15個のIssue
3. **Epic 3: GUI・UIシステム改善** (`epic:ui`) - 8-10個のIssue
4. **Epic 4: イベント・コマンドシステム完成** (`epic:events`) - 10-12個のIssue
5. **Epic 5: データ管理・永続化** (`epic:data`) - 5-7個のIssue
6. **Epic 6: VB6レガシー関数置換** (`epic:vb6-legacy`) - 5-8個のIssue
7. **Epic 7: パフォーマンス最適化** (`epic:performance`) - 5-7個のIssue
8. **Epic 8: バグ修正・エッジケース対応** (`epic:bugfix`) - 8-10個のIssue

## 🚀 始め方 / Getting Started

### ステップ1: ドキュメントを読む

1. [移植計画・進捗](./migration-plan.md)で全体像と現在のフェーズを把握
2. [移植精度 評価・向上計画](./porting-quality-plan.md)で現フェーズの作業内容を確認
3. [移植状況 総合評価レポート](./porting-assessment.md)で定量的な状況を確認

### ステップ2: Copilotに作業を依頼する（推奨）

セットアップ不要。そのままCopilotに指示するだけです：

```
@copilot ユニットテストを補完してください
```

Copilotが現在の状態を評価し、次のタスクを自動的に選択・実行します。

### ステップ3: 進捗を確認・更新する

```
@copilot 進捗を更新してください
```

Copilotがissueの状態を確認し、ドキュメントと進行状況を最新の実態に合わせて更新します。

## 📊 進捗管理 / Progress Tracking

### 進捗確認・更新

```
@copilot 進捗を更新してください
```

Copilotが現在のissue状態を確認し、ドキュメントと進行状況を更新します。

### 推奨作業フロー

1. **Issue作成** → 機能Issueを作成
2. **作業開始** → Issueをアサイン
3. **PR作成** → `Closes #XXX`でIssueをリンク
4. **レビュー** → レビュー後マージ

## 🤝 貢献方法 / How to Contribute

1. **Issue選択** - Backlogから取り組むIssueを選ぶ
2. **Fork & Branch** - リポジトリをフォークしてブランチ作成
3. **実装** - 機能を実装してテストを追加
4. **PR作成** - Pull Requestを作成
5. **レビュー対応** - フィードバックに対応
6. **マージ** - レビュー承認後にマージ

詳細は[CONTRIBUTING.md](../../CONTRIBUTING.md)を参照してください。

## 📞 サポート / Support

### 質問・提案
- GitHub Issuesで質問や提案を投稿
- Discussionsで議論

### 参考リンク
- [SRC 公式サイト](http://www.src-srpg.jpn.org/)
- [SRC# GitHub](https://github.com/7474/SRC)
- [SRC# デモ](https://7474.github.io/SRC/)

## 📝 更新履歴 / Changelog

- **2026-02-21**: issue・label・projects運用ガイドとスクリプトを削除（運用しないため）
- **2026-02-21**: ドキュメント構造見直しとフェーズ更新
  - `docs/porting/` サブディレクトリへ移動（docs直下から整理）
  - TODO消化フェーズ完了を明記し、品質検証・精度向上フェーズへの移行を反映
  - Copilot運用コマンドを品質検証フェーズ向けに更新

- **2026-02-20**: 移植状況の総合評価と品質向上計画を追加
  - [移植状況 総合評価レポート](./porting-assessment.md) を新規作成
  - [移植精度 評価・向上計画](./porting-quality-plan.md) を新規作成
  - 統計情報の更新（残存TODO 18件、テスト253メソッド）

- **2026-02-20**: 運用の見直しと進捗更新コマンドの追加
  - `@copilot 進捗を更新してください` コマンドの追加
  - 自律運用を優先した手順への見直し（セットアップ不要を明確化）
  - 進行状況の変化を反映

- **2026-02-19**: 初版作成
  - 移植完了計画の策定
  - Epic分類とIssue詳細の定義
  - GitHub Projects設定ガイドの作成
  - 自動化スクリプトの追加

---

**品質検証フェーズでSRC#をより完成度の高いプロジェクトへ！ 🚀**
