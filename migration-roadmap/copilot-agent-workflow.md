# Copilot Agent ワークフロー手順書

このドキュメントは、SRC#移行ロードマップをCopilot agentが効率的に進行するため、人間のプロジェクトマネージャーとメンテナーが実行すべきGitHub操作手順を定義します。

## 前提条件

### リポジトリ状態
- [migration-roadmap/](./.) ディレクトリにすべてのEpicとスプリント計画が存在
- [.github/copilot-instructions.md](../.github/copilot-instructions.md) にCopilot agentの作業指針が定義済み
- [roadmap.md](./roadmap.md) にプロジェクト全体の概要と進捗追跡チェックリストが存在

### 必要な権限
- リポジトリのIssue作成・編集権限
- ラベル管理権限
- プルリクエスト作成・マージ権限
- プロジェクトボード管理権限（オプション）

## Phase 1: 初期セットアップ

### 1.1 Epicラベルの作成
各EpicにGitHubラベルを作成します。

**作業手順:**
1. GitHub WebUI: Settings → Labels
2. 以下のラベルを作成:

```
epic:save-load (色: #FF6B6B) - Epic A: セーブ/ロードシステム
epic:game-commands (色: #4ECDC4) - Epic B: ゲームコマンド  
epic:expression-system (色: #45B7D1) - Epic C: 式システム
epic:configuration (色: #96CEB4) - Epic D: 設定システム
epic:system-libraries (色: #FFEAA7) - Epic E: システムライブラリ
epic:test-infrastructure (色: #DDA0DD) - Epic F: テストインフラ
epic:legacy-cleanup (色: #98D8C8) - Epic G: レガシークリーンアップ
epic:ui-platform (色: #F7DC6F) - Epic H: UIプラットフォーム

priority:critical (色: #FF0000) - 重要度: 重要
priority:high (色: #FF8C00) - 重要度: 高
priority:medium (色: #FFD700) - 重要度: 中
priority:low (色: #90EE90) - 重要度: 低

sprint:1 (色: #E6E6FA) - スプリント1
sprint:2 (色: #F0E68C) - スプリント2  
sprint:3 (色: #FFA07A) - スプリント3
sprint:4+ (色: #D3D3D3) - スプリント4以降
```

### 1.2 Issueテンプレートの確認
`.github/ISSUE_TEMPLATE/` ディレクトリに以下のテンプレートが存在することを確認:
- `epic-task.md` - Epic内タスク用テンプレート
- `bug-report.md` - バグレポート用テンプレート
- `feature-request.md` - 新機能リクエスト用テンプレート

存在しない場合は[templates/](./templates/) ディレクトリの対応するファイルから作成してください。

## Phase 2: Epicの開始プロセス

### 2.1 Epic開始時の作業
各Epicを開始する際に実行する手順：

1. **Epic Issueの作成**
   ```
   タイトル: [Epic A] セーブ/ロードシステム
   本文: epic-a-save-load.mdの概要セクションをコピー
   ラベル: epic:save-load, priority:high, sprint:1
   ```

2. **Epic内タスクIssueの作成**
   各Epic mdファイルの「Issue・タスク」セクションから個別Issueを作成:
   ```
   タイトル: [A.1] パス正規化とセキュリティ検証
   本文: epic-a-save-load.mdのA.1セクションをコピー
   ラベル: epic:save-load, priority:critical, sprint:1
   親Issue: Epic AのIssue番号をリンク
   ```

3. **Copilot Agent割り当て**
   - Assignee: @copilot (利用可能な場合)
   - コメント追加: 
   ```
   @copilot このタスクの実装をお願いします。
   
   参照ドキュメント:
   - Epic詳細: migration-roadmap/epics/epic-a-save-load.md
   - 作業指針: .github/copilot-instructions.md
   - 技術仕様: migration-roadmap/technical-breakdown.md
   
   実装時は以下を必ず確認してください:
   - [ ] セキュリティ要件の遵守
   - [ ] 既存テストの実行と新規テストの追加
   - [ ] パフォーマンスへの影響測定
   - [ ] クロスプラットフォーム互換性
   ```

### 2.2 作業開始の確認
Copilot agentが作業を開始したら：

1. **ブランチ確認**
   - ブランチ名規則: `copilot/epic-a-task-1-path-security`
   - 命名パターン: `copilot/epic-{epic-letter}-task-{number}-{brief-description}`

2. **初期プランの確認**
   - Copilot agentの最初のコメントで作業計画が提示される
   - 計画に問題がないか確認し、必要に応じてフィードバック提供

## Phase 3: 進捗監視とサポート

### 3.1 日次チェック項目
毎日以下を確認：

1. **アクティブな作業状況**
   ```bash
   # プルリクエストの確認
   gh pr list --author @copilot --state open
   
   # Issueの進捗確認  
   gh issue list --assignee @copilot --state open
   ```

2. **ビルド状況確認**
   - GitHub Actions の CI/CD パイプライン状況
   - 失敗した場合は原因分析とサポート提供

3. **依存関係の確認**
   - 後続タスクの依存関係をepic mdファイルで確認
   - ブロッカーがないか確認

### 3.2 週次レビュー
毎週末に実行：

1. **Epic進捗の更新**
   - Epic mdファイルの受け入れ基準チェックリストを更新
   - roadmap.mdの成功基準チェックリストを更新

2. **次週計画の調整**
   - sprint-plan.mdの進捗状況を確認
   - 必要に応じてスプリント計画を調整

3. **リスク評価**
   - 各Epicのリスク評価セクションを確認
   - 新しいリスクや課題が発生していないか確認

## Phase 4: 完了とレビュープロセス

### 4.1 タスク完了時の確認
Copilot agentがタスクを完了したら：

1. **プルリクエストレビュー**
   ```
   チェック項目:
   - [ ] コード品質とセキュリティ基準の遵守
   - [ ] テストカバレッジの確保
   - [ ] ドキュメントの更新
   - [ ] パフォーマンスへの影響評価
   - [ ] クロスプラットフォーム互換性確認
   ```

2. **手動検証**
   - 実装された機能の手動テスト実行
   - セキュリティ要件（特にEpic A）の詳細検証
   - ゲームプレイ影響（特にEpic B）の確認

3. **マージ前チェック**
   ```bash
   # すべてのCIが通っていることを確認
   gh pr checks {PR番号}
   
   # 競合がないことを確認
   gh pr view {PR番号} --json mergeable
   ```

### 4.2 Epic完了時の作業
Epic全体が完了したら：

1. **Epic Issueの更新**
   - Epic完了基準のチェックリストを確認
   - すべて完了していれば Issue を Close

2. **ロードマップの更新**
   - roadmap.mdの該当Epic進捗を更新
   - sprint-plan.mdの完了状況を更新

3. **次Epic の準備**
   - 依存関係を確認して次Epicを開始可能か判断
   - 必要に応じて次Epicの初期セットアップを実行

## Phase 5: トラブルシューティング

### 5.1 よくある問題と対処法

**問題1: Copilot agentが作業を開始しない**
```
対処法:
1. Issue の説明が不十分でないか確認
2. 参照ドキュメントが存在するか確認  
3. @copilot メンションで直接指示
4. 作業指針(.github/copilot-instructions.md)の確認
```

**問題2: ビルドが失敗する**
```
対処法:
1. ローカル環境でのビルド再現
2. 依存関係の問題を確認
3. プラットフォーム固有の問題を調査
4. 必要に応じて手動修正コミット
```

**問題3: テストが失敗する**
```
対処法:
1. 失敗したテストの詳細を分析
2. 既存機能の破壊がないか確認
3. テスト環境固有の問題を調査
4. Copilot agentに具体的な修正指示
```

**問題4: セキュリティやパフォーマンスの懸念**
```
対処法:
1. 専門家レビューの手配
2. 追加のセキュリティテスト実行
3. パフォーマンスベンチマークの実行
4. 必要に応じて実装方針の見直し
```

### 5.2 エスカレーション基準
以下の場合は追加サポートを検討：

- 3回以上同じ問題で作業が停止
- セキュリティ脆弱性の疑いがある実装
- パフォーマンスが大幅に劣化
- 5日以上進捗がない状況

## Phase 6: 継続的改善

### 6.1 定期的な手順見直し
月次で以下を実行：

1. **ワークフローの有効性評価**
   - 作業効率の測定
   - ボトルネックの特定
   - プロセス改善案の検討

2. **ドキュメントの更新**
   - 新しく発見された課題の追加
   - 解決策の蓄積
   - ベストプラクティスの文書化

3. **ツールの改善**
   - GitHub テンプレートの改良
   - 自動化可能な作業の特定
   - 監視・アラートの改善

---

**最終更新**: 2024年現在  
**バージョン**: 1.0  
**関連文書**: [roadmap.md](./roadmap.md) | [sprint-plan.md](./sprint-plan.md) | [technical-breakdown.md](./technical-breakdown.md)