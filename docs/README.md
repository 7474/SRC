# SRC# 移植完了計画 ドキュメント / Migration Plan Documentation

このディレクトリには、SRC#（Simulation RPG Construction Sharp）のC#移植完了に向けた計画ドキュメントが含まれています。

## 📚 ドキュメント一覧 / Document List

### 🤖 Copilot自律運用 / Copilot Autonomous Operation

**最優先 / Top Priority**: **[Copilot完全自律エージェント](../.github/copilot/autonomous-agent.md)** 🚀

単一コマンドで移植を進行:
```
@copilot 移植を進行してください
```

With single command:
- ✅ Copilotが次のタスクを自動選択
- ✅ Copilotが自動実装・テスト・PR作成
- ✅ 人間が考えることは最小限
- ✅ セットアップ不要

**詳細**: [Copilot Agent Instructions](../.github/copilot/README.md)

---

### メインドキュメント / Main Documents

1. **[クイックスタートガイド](./quick-start.md)**
   - プロジェクトを始めるための簡易ガイド
   - Quick guide to get started with the project

2. **[移植完了計画](./migration-plan.md)**
   - プロジェクト全体の概要とカテゴリ分類
   - Overall project overview and category classification

3. **[個別Issue詳細](./issue-breakdown.md)**
   - 約70個の具体的なIssueリスト
   - Detailed list of ~70 specific issues

4. **[GitHub Projects設定ガイド](./github-projects-setup.md)**
   - プロジェクト管理の設定方法（オプション）
   - How to set up GitHub Projects for management (Optional)

### 自動化スクリプト / Automation Scripts

`scripts/` ディレクトリには、プロジェクト管理を自動化するスクリプトが含まれています：

- **[create-labels.sh](./scripts/create-labels.sh)** - ラベルの一括作成
- **[create-milestones.sh](./scripts/create-milestones.sh)** - マイルストーンの一括作成
- **[progress-report.sh](./scripts/progress-report.sh)** - 進捗レポートの生成

使用方法：
```bash
cd /path/to/SRC
bash docs/scripts/create-labels.sh
bash docs/scripts/create-milestones.sh
bash docs/scripts/progress-report.sh
```

## 🎯 プロジェクト概要 / Project Overview

### 統計 / Statistics

- **Total TODO Comments**: 155+
- **Total Issues to Create**: 約70個
- **Epic Categories**: 8個
- **Estimated Total Changes**: 18,000-25,000行
- **Estimated Duration**: 12-18ヶ月

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

1. [クイックスタートガイド](./quick-start.md)を読む
2. [移植完了計画](./migration-plan.md)で全体像を把握
3. [個別Issue詳細](./issue-breakdown.md)で具体的な作業内容を確認

### ステップ2: プロジェクト環境を整える

1. ラベルを作成:
   ```bash
   bash docs/scripts/create-labels.sh
   ```

2. マイルストーンを作成:
   ```bash
   bash docs/scripts/create-milestones.sh
   ```

3. GitHub Projectsを設定:
   - [GitHub Projects設定ガイド](./github-projects-setup.md)を参照

### ステップ3: Issueを作成

1. Epic Issueを8個作成（テンプレート: [epic-template.md](../.github/ISSUE_TEMPLATE/epic-template.md)）
2. 最初の機能Issueを作成（テンプレート: [feature-template.md](../.github/ISSUE_TEMPLATE/feature-template.md)）
3. GitHub Projectsに登録

### ステップ4: 作業開始

1. Phase 1の最優先Issueから着手
2. PR作成時は差分1000行以下を目安に
3. テストを追加して既存テストも通過することを確認

## 📊 進捗管理 / Progress Tracking

### 進捗確認

```bash
# 進捗レポート生成
bash docs/scripts/progress-report.sh

# Epic別の進捗
gh issue list --label "epic:combat" --state all

# マイルストーンの進捗
gh api repos/7474/SRC/milestones
```

### 推奨作業フロー

1. **Issue作成** → Epic配下に機能Issueを作成
2. **作業開始** → Issueをアサイン、`status:in-progress`ラベル付与
3. **PR作成** → `Closes #XXX`でIssueをリンク
4. **レビュー** → `status:review`ラベル付与
5. **マージ** → 自動的に`Done`カラムに移動

## 🏷️ ラベル体系 / Label System

### Epic ラベル（色: 青 #0052CC）
- `epic:combat`, `epic:unit-pilot`, `epic:ui`, `epic:events`
- `epic:data`, `epic:vb6-legacy`, `epic:performance`, `epic:bugfix`

### 優先度ラベル（色: 赤系）
- `priority:critical` (赤), `priority:high` (薄赤)
- `priority:medium` (黄), `priority:low` (緑)

### タイプラベル（色: 黄 #FBCA04）
- `type:epic`, `type:feature`, `type:enhancement`
- `type:bugfix`, `type:refactor`, `type:docs`

### サイズラベル（色: 青緑 #006B75）
- `size:xs` (~100行), `size:s` (200-400行)
- `size:m` (400-700行), `size:l` (700-1000行)
- `size:xl` (1000行以上)

### ステータスラベル（色: 紫 #5319E7）
- `status:blocked`, `status:in-progress`
- `status:review`, `status:on-hold`

## 🎯 フェーズとマイルストーン / Phases and Milestones

### Phase 1: コア機能完成 (v3.1.0) - 2026年Q2
- Epic 1: 戦闘システム (基本機能)
- Epic 2: ユニット・パイロット (基本機能)

### Phase 2: UI/UX改善 (v3.2.0) - 2026年Q3
- Epic 1: 戦闘システム (高度な機能)
- Epic 3: GUI・UI
- Epic 4: イベント・コマンド

### Phase 3: 品質向上 (v3.3.0) - 2026年Q4
- Epic 5: データ管理
- Epic 8: バグ修正

### Phase 4: 最適化・完成 (v3.4.0) - 2027年Q1
- Epic 6: VB6レガシー
- Epic 7: パフォーマンス

## 📋 Issue テンプレート / Issue Templates

プロジェクトには3つのIssueテンプレートが用意されています：

1. **[Epic Template](../.github/ISSUE_TEMPLATE/epic-template.md)**
   - 親Issueの作成用
   - 関連する機能群をグルーピング

2. **[Feature Template](../.github/ISSUE_TEMPLATE/feature-template.md)**
   - 新機能実装用
   - 最も一般的なテンプレート

3. **[Bugfix Template](../.github/ISSUE_TEMPLATE/bugfix-template.md)**
   - バグ修正用
   - 再現手順と期待される動作を記載

## 💡 ベストプラクティス / Best Practices

### Issue作成時
- ✅ 明確で検索可能なタイトル
- ✅ 適切なラベル（最低3つ: epic, priority, type）
- ✅ マイルストーンの設定
- ✅ 推定工数の記載

### PR作成時
- ✅ 1 Issue = 1 PR が原則
- ✅ 差分1000行以下を目安
- ✅ テストの追加
- ✅ 既存テストの実行確認

### レビュー時
- ✅ コードの品質と可読性
- ✅ テストの網羅性
- ✅ ドキュメントの更新
- ✅ 既存機能への影響確認

## 🤝 貢献方法 / How to Contribute

1. **Issue選択** - Backlogから取り組むIssueを選ぶ
2. **Fork & Branch** - リポジトリをフォークしてブランチ作成
3. **実装** - 機能を実装してテストを追加
4. **PR作成** - Pull Requestを作成
5. **レビュー対応** - フィードバックに対応
6. **マージ** - レビュー承認後にマージ

詳細は[CONTRIBUTING.md](../CONTRIBUTING.md)を参照してください。

## 📞 サポート / Support

### 質問・提案
- GitHub Issuesで質問や提案を投稿
- Discussionsで議論

### 参考リンク
- [SRC 公式サイト](http://www.src-srpg.jpn.org/)
- [SRC# GitHub](https://github.com/7474/SRC)
- [SRC# デモ](https://7474.github.io/SRC/)

## 📝 更新履歴 / Changelog

- **2026-02-19**: 初版作成
  - 移植完了計画の策定
  - Epic分類とIssue詳細の定義
  - GitHub Projects設定ガイドの作成
  - 自動化スクリプトの追加

---

**Let's complete the SRC# migration together! 🚀**
