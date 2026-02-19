---
name: Bug Fix Issue Template
about: Template for creating bug fix issues
title: ''
labels: ['type:bugfix']
assignees: ''
---

## バグの概要 / Bug Overview

[バグの内容を記述 / Describe the bug]

## 対象ファイル / Target Files

- `ファイルパス` (行数: XXX行)

## 現象 / Current Behavior

[現在の動作を記述 / Describe current behavior]

## 期待される動作 / Expected Behavior

[期待される動作を記述 / Describe expected behavior]

## 再現手順 / Steps to Reproduce

1. [手順1 / Step 1]
2. [手順2 / Step 2]
3. [手順3 / Step 3]

## TODOコメント（該当する場合） / TODO Comment (if applicable)

```csharp
// TODO コメントの内容
```

## 修正内容 / Fix Details

- [ ] 修正項目1 / Fix item 1
- [ ] 修正項目2 / Fix item 2
- [ ] テストケースの追加 / Add test cases

## テスト方針 / Test Plan

- [ ] テスト項目1 / Test item 1
- [ ] テスト項目2 / Test item 2
- [ ] リグレッションテストの実行 / Run regression tests

## 推定工数 / Estimated Effort

- 推定差分: XXX-YYY行
- 推定時間: Z時間

## 影響範囲 / Impact Area

[このバグ修正による影響範囲 / Impact area of this bug fix]

## 完了条件 / Definition of Done

- [ ] バグが修正されている
- [ ] テストが追加され、全て通過している
- [ ] コードレビューが完了している
- [ ] 影響範囲のテストが完了している

## 関連Epic / Related Epic

Epic #XXX: [Epic名]

## Labels

このIssueには以下のラベルを付けてください:
- `type:bugfix` (必須)
- `epic:[epic名]` (例: `epic:bugfix`)
- `priority:[優先度]` (例: `priority:high`, `priority:medium`, `priority:low`)
- `size:[サイズ]` (例: `size:xs` (~100行), `size:s` (200-400行))
