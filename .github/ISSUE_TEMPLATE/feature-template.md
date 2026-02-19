---
name: Feature Implementation Issue Template
about: Template for creating feature implementation issues
title: ''
labels: ['type:feature']
assignees: ''
---

## 概要 / Overview

[実装する機能の説明 / Description of the feature to implement]

## 対象ファイル / Target Files

- `ファイルパス` (行数: XXX行)

## TODOコメント / TODO Comment

```csharp
// TODO コメントの内容をここに引用
```

## 実装内容 / Implementation Details

- [ ] 実装項目1 / Implementation item 1
- [ ] 実装項目2 / Implementation item 2
- [ ] 実装項目3 / Implementation item 3
- [ ] テストケースの追加 / Add test cases
- [ ] ドキュメント更新 / Update documentation

## テスト方針 / Test Plan

- [ ] テスト項目1 / Test item 1
- [ ] テスト項目2 / Test item 2
- [ ] 既存テストの実行確認 / Verify existing tests pass

## 推定工数 / Estimated Effort

- 推定差分: XXX-YYY行
- 推定時間: Z時間

## 依存関係 / Dependencies

- [ ] Issue #XXX が完了していること
- [ ] Issue #YYY が完了していること

## 完了条件 / Definition of Done

- [ ] 実装が完了している
- [ ] テストが追加され、全て通過している
- [ ] コードレビューが完了している
- [ ] ドキュメントが更新されている
- [ ] PR差分が1000行以下である（横断的な関心事を除く）

## 関連Epic / Related Epic

Epic #XXX: [Epic名]

## Labels

このIssueには以下のラベルを付けてください:
- `type:feature` (必須)
- `epic:[epic名]` (例: `epic:combat`, `epic:unit-pilot`)
- `priority:[優先度]` (例: `priority:high`, `priority:medium`, `priority:low`)
- `size:[サイズ]` (例: `size:s` (200-400行), `size:m` (400-700行), `size:l` (700-1000行))
