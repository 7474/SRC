---
name: SRC# 移行タスク
about: SRC#移行のタスクをCopilot agentに依頼する
title: '[移行] タスク名'
labels: ['migration']
assignees: ['copilot']
---

## 実装するタスク
**Epic**: (例: Epic 5: データ管理・永続化)
**タスク**: (例: セーブ・ロードの互換性の精査)

## 参照ドキュメント
**必読**: `docs/porting.md`（移植の現状・規約）、`docs/testing.md`（テストの責務）。Epic の元の計画は `docs/history/porting/migration-plan.md`

## @copilot への指示
上記参照ドキュメントのタスクを実装してください。
- 日本語でコメント・コミットメッセージを記述
- 最小限の変更で実装
- 既存テストが通ることを確認