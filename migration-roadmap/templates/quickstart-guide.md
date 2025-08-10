# クイックスタートガイド

## 🚀 Copilot Agent でロードマップを進める方法

このガイドでは、人間のプロジェクトマネージャーがCopilot agentを使ってSRC#移行ロードマップを効率的に進める最も簡単な方法を説明します。

### 📋 事前準備（5分）

1. **必要なラベルを作成**
   ```bash
   # GitHub CLI を使用する場合
   gh label create "epic:save-load" --color FF6B6B
   gh label create "priority:high" --color FF8C00  
   gh label create "sprint:1" --color E6E6FA
   # ... 他のラベルも同様に作成
   ```

2. **最初のEpicを選択**
   - 推奨: [Epic A: セーブ/ロードシステム](../epics/epic-a-save-load.md)（最高優先度）

### 🎯 Epic開始の3ステップ

#### ステップ1: Epic追跡Issueを作成
```markdown
タイトル: [Epic A] セーブ/ロードシステム
テンプレート: epic-tracking.md を使用
ラベル: epic:save-load, priority:high, tracking
```

#### ステップ2: 最初のタスクIssueを作成  
```markdown
タイトル: [A.1] パス正規化とセキュリティ検証
テンプレート: epic-task.md を使用
ラベル: epic:save-load, priority:critical, sprint:1
```

#### ステップ3: Copilot Agentに割り当て
```markdown
@copilot このタスクの実装をお願いします。

参照ドキュメント:
- Epic詳細: migration-roadmap/epics/epic-a-save-load.md
- 作業指針: .github/copilot-instructions.md

実装時は以下を必ず確認してください:
- [ ] セキュリティ要件の遵守
- [ ] 既存テストの実行と新規テストの追加
- [ ] パフォーマンスへの影響測定
- [ ] クロスプラットフォーム互換性
```

### 📊 日次監視（2分/日）

**毎日確認すること:**
```bash
# アクティブなPRを確認
gh pr list --author @copilot --state open

# 進行中のIssueを確認  
gh issue list --assignee @copilot --state open

# ビルド状況を確認
gh run list --limit 5
```

### 📈 週次レビュー（15分/週）

1. **進捗チェックリスト更新**
   - Epic mdファイルの受け入れ基準を確認
   - roadmap.mdの成功基準を更新

2. **次週の準備**
   - 完了したタスクを確認
   - 次のタスクIssueを作成

### 🔄 Epic完了プロセス

1. **すべてのタスクが完了したことを確認**
2. **Epic完了基準をチェック**
3. **Epic Issueをクローズ**
4. **次のEpicを開始**

---

## 🎯 Epic実行順序（推奨）

### Phase 1: 重要機能（スプリント1-2）
1. **[Epic A: セーブ/ロードシステム](../epics/epic-a-save-load.md)** ⭐ 最優先
   - A.1: パス正規化とセキュリティ検証（3日）
   - A.2: クイックセーブ機能完成（2日）
   - A.3: セーブファイル形式検証（2日）

2. **[Epic B: ゲームコマンド](../epics/epic-b-game-commands.md)**
   - B.1: AI武器選択バグ修正（2日）
   - B.2: 攻撃後再移動ロジック（3日）

3. **[Epic C: 式システム](../epics/epic-c-expression-system.md)**
   - C.1: 文字列バイト関数実装（2日）
   - C.2: ファイルダイアログ統合（2日）

### Phase 2: インフラ（スプリント3）
4. **[Epic D: 設定システム](../epics/epic-d-configuration.md)**
5. **[Epic E: システムライブラリ](../epics/epic-e-system-libraries.md)**
6. **[Epic F: テストインフラ](../epics/epic-f-test-infrastructure.md)**

### Phase 3: 品質・メンテナンス（スプリント4+）
7. **[Epic G: レガシークリーンアップ](../epics/epic-g-legacy-cleanup.md)**
8. **[Epic H: UIプラットフォーム](../epics/epic-h-ui-platform.md)**

---

## 🚨 よくあるトラブルと解決法

### ❌ Copilot agentが作業を開始しない
**解決法:**
1. Issue説明をより詳細に記載
2. `@copilot` メンションを明確に記載
3. 参照ドキュメントのパスを確認

### ❌ ビルドが失敗する  
**解決法:**
1. GitHub Actions ログを確認
2. プラットフォーム固有の問題をチェック
3. 依存関係の問題を調査

### ❌ テストが失敗する
**解決法:**
1. 失敗したテストの詳細を分析
2. 既存機能の破壊がないか確認
3. 具体的な修正指示をコメント

### ❌ セキュリティやパフォーマンスの懸念
**解決法:**
1. 専門家レビューを手配
2. 追加のテストを実行
3. 実装方針を見直し

---

## 📞 エスカレーション基準

以下の場合は追加サポートを検討：
- 3回以上同じ問題で作業が停止
- セキュリティ脆弱性の疑い
- パフォーマンスが大幅に劣化  
- 5日以上進捗がない

---

## 📚 参考リンク

- **[詳細ワークフロー](../copilot-agent-workflow.md)** - 完全な手順書
- **[チェックリスト集](./checklists.md)** - すべてのチェックリスト
- **[全体ロードマップ](../roadmap.md)** - プロジェクト概要
- **[スプリント計画](../sprint-plan.md)** - 詳細スケジュール
- **[技術詳細](../technical-breakdown.md)** - 技術仕様

---

**💡 ヒント**: まずはEpic Aの最初のタスク（A.1）から始めることを強く推奨します。セキュリティは最優先事項です。

**最終更新**: 2024年現在