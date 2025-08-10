# SRC# 移行ロードマップ

このディレクトリには、SRC#（Simulation RPG Construction Sharp）C# .NET移行を完了するための包括的なロードマップが含まれています。

## ディレクトリ構造

- [`roadmap.md`](./roadmap.md) - エグゼクティブサマリーと戦略を含むメイン移行ロードマップ
- [`technical-breakdown.md`](./technical-breakdown.md) - 詳細な技術分析と実装計画
- [`sprint-plan.md`](./sprint-plan.md) - 優先度と依存関係を含む4スプリント配信計画
- [`copilot-agent-workflow.md`](./copilot-agent-workflow.md) - **Copilot agentワークフロー手順書** 🤖
- [`epics/`](./epics/) - 個別Epic文書（A-H）
- [`templates/`](./templates/) - タスク作成用GitHub Issueテンプレートとガイド

## 🚀 すぐに始める

**Copilot agentでロードマップを進めたい場合:**
1. **[クイックスタートガイド](./templates/quickstart-guide.md)** で5分セットアップ
2. **[Epic A: セーブ/ロードシステム](./epics/epic-a-save-load.md)** から開始
3. **[チェックリスト](./templates/checklists.md)** で進捗管理

## 進捗概要

**ステータス**: 計画完了 ✅  
**現在のフェーズ**: 実装準備完了  
**総Epic数**: 8個（A-H）  
**総Issue数**: 26個の具体的実装タスク  

### Epic進捗状況

- [ ] **Epic A**: セーブ/ロードシステム（高優先度）
- [ ] **Epic B**: ゲームコマンド（高優先度）  
- [ ] **Epic C**: 式システム（高優先度）
- [ ] **Epic D**: 設定システム（中優先度）
- [ ] **Epic E**: システムライブラリ（中優先度）
- [ ] **Epic F**: テストインフラ（中優先度）
- [ ] **Epic G**: レガシーコードクリーンアップ（低優先度）
- [ ] **Epic H**: UIプラットフォーム対応（低優先度）

## クイックリンク

### 高優先度項目（スプリント1-2）
1. [セーブ/ロードシステムセキュリティ](./epics/epic-a-save-load.md#path-normalization-and-security)
2. [AI武器選択バグ](./epics/epic-b-game-commands.md#ai-weapon-selection-bug)
3. [攻撃後再移動](./epics/epic-b-game-commands.md#attack-re-movement-logic)
4. [文字列バイト関数](./epics/epic-c-expression-system.md#string-byte-functions)

### 実装ガイドライン

- コア機能を最優先（Epic A-C）
- 可能な限り後方互換性を維持
- セキュリティと安定性を優先
- テストインフラの改善を並行実施

---

**関連Issue**: #663  
**作成日**: 移行計画フェーズ  
**最終更新**: {{ current_date }}