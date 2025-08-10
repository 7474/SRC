# Templates ディレクトリ

このディレクトリには、SRC#移行ロードマップをCopilot agentが効率的に進行するためのテンプレートとガイドが含まれています。

## 📁 ファイル一覧

### 🚀 すぐに始める
- **[quickstart-guide.md](./quickstart-guide.md)** - 最も簡単な開始方法
  - 5分でセットアップ完了
  - 3ステップでEpic開始
  - 推奨実行順序

### ✅ チェックリスト集
- **[checklists.md](./checklists.md)** - 全ての作業チェックリスト
  - Epic開始チェックリスト
  - 日次・週次確認チェックリスト
  - Pull Requestレビューチェックリスト
  - Epic完了チェックリスト
  - トラブルシューティングチェックリスト

### 📋 GitHub Issue テンプレート
以下のファイルは `.github/ISSUE_TEMPLATE/` ディレクトリに配置して使用します：

- **[epic-task.md](../../.github/ISSUE_TEMPLATE/epic-task.md)** - Epic内タスク用
- **[epic-tracking.md](../../.github/ISSUE_TEMPLATE/epic-tracking.md)** - Epic全体追跡用  
- **[migration-bug.md](../../.github/ISSUE_TEMPLATE/migration-bug.md)** - 移行バグレポート用

## 📖 使用方法

### 初回セットアップ時
1. **[quickstart-guide.md](./quickstart-guide.md)** を参照して最初のEpicを開始
2. **[checklists.md](./checklists.md)** のラベル管理チェックリストでGitHubラベルを作成

### 日常作業時
1. **[checklists.md](./checklists.md)** の該当チェックリストを使用
2. 必要に応じてIssueテンプレートを使用

### トラブル時
1. **[checklists.md](./checklists.md)** のトラブルシューティングチェックリストを確認
2. **[quickstart-guide.md](./quickstart-guide.md)** のよくある問題セクションを参照

## 🔗 関連ドキュメント

- **[copilot-agent-workflow.md](../copilot-agent-workflow.md)** - 完全な作業手順書
- **[roadmap.md](../roadmap.md)** - プロジェクト全体概要
- **[sprint-plan.md](../sprint-plan.md)** - 詳細スケジュール
- **[epics/](../epics/)** - 各Epic詳細仕様

## 💡 推奨順序

1. **まず読む**: [quickstart-guide.md](./quickstart-guide.md)
2. **次に確認**: [checklists.md](./checklists.md) のラベル作成
3. **作業開始**: [../epics/epic-a-save-load.md](../epics/epic-a-save-load.md) から開始
4. **日常使用**: 該当チェックリストを参照

---

**最終更新**: 2024年現在  
**メンテナンス**: プロジェクト進行に合わせて定期更新