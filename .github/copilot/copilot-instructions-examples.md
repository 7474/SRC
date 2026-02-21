# Copilotへの指示例 / Example Copilot Instructions

このファイルには、GitHub Projectsなどの事前セットアップなしに、Copilotへの指示のみで作業を進めるための具体的な指示例が含まれています。

This file contains specific instruction examples for working with Copilot without requiring prior GitHub Projects setup.

## 基本方針 / Basic Approach

Copilotは以下の情報を元に自律的に作業します：
- `docs/porting/migration-plan.md` - 8つのEpicと全体戦略
- `docs/porting/issue-breakdown.md` - 約70個の具体的Issue定義
- `.github/ISSUE_TEMPLATE/` - Issueテンプレート
- コードベース内のTODOコメント

Copilot works autonomously based on:
- `docs/porting/migration-plan.md` - 8 Epics and overall strategy
- `docs/porting/issue-breakdown.md` - ~70 specific issue definitions
- `.github/ISSUE_TEMPLATE/` - Issue templates
- TODO comments in the codebase

---

## 指示例 1: Epic Issueの作成 / Example 1: Creating Epic Issues

### 指示 / Instruction

```
@copilot Epic 1: 戦闘システム完成 のIssueを作成してください。

以下の情報を使用：
- タイトル: "Epic 1: 戦闘システム完成 (Combat System Completion)"
- 内容: docs/porting/issue-breakdown.md の Epic 1 セクションを参照
- ラベル: type:epic, epic:combat, priority:high
- マイルストーン: Phase 1: コア機能完成 (v3.1.0)

Epic 2-8についても同様に作成してください。
```

### 実行結果 / Expected Result

Copilotが8つのEpic Issueを作成し、それぞれのIssue番号を報告します。

---

## 指示例 2: 最初の機能実装 / Example 2: First Feature Implementation

### 指示 / Instruction

```
@copilot Issue 1.1: 回避攻撃の実装を開始してください。

実装手順：
1. docs/porting/issue-breakdown.md の Issue 1.1 の内容でGitHub Issueを作成
2. SRC.Sharp/SRCCore/Units/Unit.attackcheck.cs のTODOコメントを確認
3. IsDodgeAttack() メソッドを実装
4. SRCCoreTests にテストを追加
5. PRを作成（差分は1000行以下）

実装後、テスト結果とPR番号を報告してください。
```

### 期待される応答 / Expected Response

```
Issue #123 を作成しました: "Unit.attackcheck.cs: 回避攻撃の実装"

実装内容：
- IsDodgeAttack() メソッドを実装（Unit.attackcheck.cs 250行追加/変更）
- UnitAttackCheckTests.cs にテストケース追加（80行）
- 全テスト通過: 152 passed

PR #456 を作成しました: "Epic 1.1: Implement dodge attack"
- 差分: 330行
- ラベル: epic:combat, priority:high, type:feature, size:s
- Closes #123
```

---

## 指示例 3: 複数Issueの並行作成 / Example 3: Creating Multiple Issues

### 指示 / Instruction

```
@copilot docs/porting/issue-breakdown.md の Epic 1（戦闘システム）の Issue 1.1 から 1.6 までのGitHub Issueを作成してください。

各Issueには以下を設定：
- .github/ISSUE_TEMPLATE/feature-template.md を使用
- 適切なラベル（epic:combat, priority, type:feature, size）
- マイルストーン: Phase 1
- 親Issue（Epic 1）へのリンク

作成完了後、Issue番号のリストを報告してください。
```

### 期待される応答 / Expected Response

```
Epic 1 の Issue を6個作成しました：

#124: Unit.attackcheck.cs: 回避攻撃の実装 (priority:high, size:s)
#125: Unit.attackcheck.cs: 受け流し攻撃の実装 (priority:high, size:s)
#126: Unit.attackcheck.cs: ダミー攻撃の実装 (priority:medium, size:s)
#127: Unit.attackcheck.cs: シールド防御の実装 (priority:high, size:s)
#128: Unit.attackcheck.cs: 追加攻撃の実装 (priority:high, size:m)
#129: Unit.attackcheck.cs: 反撃攻撃の実装 (priority:high, size:s)

全てのIssueを Epic #123 にリンクしました。
```

---

## 指示例 4: 進捗レポートの生成 / Example 4: Generating Progress Report

### 指示 / Instruction

```
@copilot 現在の移植プロジェクトの進捗状況をレポートしてください。

以下の情報を含めてください：
1. 各Epic（1-8）の進捗率
2. Phase 1 マイルストーンの完了状況
3. 優先度が高い未完了Issue
4. ブロックされているIssue
5. 今週完了したIssue

docs/porting/migration-plan.md の情報と現在のGitHub Issueの状態を参照してください。
```

### 期待される応答 / Expected Response

```
📊 SRC# 移植プロジェクト進捗レポート (2026-02-19)

## Epic別進捗
- Epic 1 (Combat): 3/15 完了 (20%)
- Epic 2 (Unit/Pilot): 1/12 完了 (8%)
- Epic 3 (GUI/UI): 0/8 未着手
- Epic 4 (Events): 0/10 未着手
- Epic 5 (Data): 0/5 未着手
- Epic 6 (VB6 Legacy): 0/6 未着手
- Epic 7 (Performance): 0/5 未着手
- Epic 8 (Bugfix): 0/8 未着手

## マイルストーン進捗
Phase 1 (v3.1.0): 4/27 完了 (15%)
- 目標: 2026年Q2
- 残り日数: 132日

## 優先度が高い未完了Issue
#125: 受け流し攻撃の実装 (status:in-progress)
#127: シールド防御の実装 (status:ready)
#128: 追加攻撃の実装 (status:ready)

## 今週完了したIssue
#124: 回避攻撃の実装 (2026-02-18)
#130: IsSkillAvailable()の実装 (2026-02-17)
```

---

## 指示例 5: 特定のTODOの解決 / Example 5: Resolving Specific TODO

### 指示 / Instruction

```
@copilot SRC.Sharp/SRCCore/Pilots/Pilot.skill.cs の "TODO Impl IsSkillAvailable" を解決してください。

手順：
1. docs/porting/issue-breakdown.md の Issue 2.1 を確認
2. GitHubにIssueがなければ作成
3. コードを確認し、コメントアウトされたコードを参考に実装
4. テストを追加
5. PRを作成して Issue をクローズ

実装完了後、PR番号とテスト結果を報告してください。
```

---

## 指示例 6: レビューの実施 / Example 6: Conducting Review

### 指示 / Instruction

```
@copilot PR #456 をレビューしてください。

確認項目：
1. PR差分が1000行以下か
2. テストが追加されているか
3. TODOコメントが適切に解決されているか
4. docs/porting/migration-plan.md の方針に沿っているか
5. 既存機能への影響がないか

レビュー結果とコメントを報告してください。
```

### 期待される応答 / Expected Response

```
✅ PR #456 のレビュー結果

## 基本チェック
- ✅ PR差分: 330行 (1000行以下)
- ✅ テスト追加: UnitAttackCheckTests.cs に8ケース
- ✅ TODOコメント解決: // TODO Impl Dodge が削除され実装完了
- ✅ 移植計画との整合性: Epic 1.1 の定義に従って実装
- ✅ 既存テスト: 全152テストが通過

## コードレビュー
- ✅ IsDodgeAttack() の実装が適切
- ✅ エッジケースの処理が含まれている
- ⚠️ 軽微: XMLドキュメントコメントの追加を推奨

## 推奨事項
1. IsDodgeAttack() メソッドにXMLドキュメントコメントを追加
2. 承認後マージ可能

総評: 承認 (Approve with minor suggestions)
```

---

## 指示例 7: バグ修正の実施 / Example 7: Fixing a Bug

### 指示 / Instruction

```
@copilot docs/porting/issue-breakdown.md の Issue 8.1: 武器選択失敗の修正を実施してください。

手順：
1. GitHubにIssueを作成（bugfixテンプレート使用）
2. SRC.Sharp/SRCCore/Units/Unit.weapon.cs のTODOを確認
3. バグを修正
4. 再現テストと修正確認テストを追加
5. PRを作成

修正内容とテスト結果を報告してください。
```

---

## 指示例 8: コメントアウトコードの処理 / Example 8: Handling Commented Code

### 指示 / Instruction

```
@copilot SRC.Sharp/SRCCore/CmdDatas/Commands/Input/QuestionCmd.cs のコメントアウトコード（約85行）を確認してください。

タスク：
1. コメントアウトされた理由を調査
2. 実装が必要か判断
3. 必要なら Issue 4.1 として実装
4. 不要なら削除の提案

調査結果と推奨アクションを報告してください。
```

### 期待される応答 / Expected Response

```
📋 QuestionCmd.cs コメントアウトコード調査結果

## コード概要
- 行数: 85行
- 機能: Questionコマンドの実装（ユーザー選択ダイアログ）
- コメント理由: "NotImplementedException" で置換

## 分析
- オリジナルVB6コードからの自動変換の失敗痕跡
- イベントシステムの重要機能
- docs/porting/issue-breakdown.md の Issue 4.1 に該当

## 推奨アクション
✅ 実装が必要（高優先度）

理由：
1. ユーザー選択機能はゲームの基本動作
2. Epic 4（イベント・コマンド）の一部
3. Phase 2 で実装予定

次のステップ：
1. Issue #XXX として作成
2. コメントアウトコードを参考に再実装
3. テスト追加
4. PR作成
```

---

## 指示例 9: 一括Issue作成 / Example 9: Bulk Issue Creation

### 指示 / Instruction

```
@copilot docs/porting/issue-breakdown.md の全Issue（約70個）のGitHub Issueを作成してください。

作成条件：
- Epic 1-8 の親Issueを先に作成
- 各機能Issueを適切な親Epicにリンク
- 全てのラベルとマイルストーンを設定
- 優先度順にナンバリング

作成完了後、Epicごとの作成Issue数を報告してください。
```

### 期待される応答 / Expected Response

```
✅ 全Issueの作成が完了しました

## Epic Issues (親Issue)
#101: Epic 1: 戦闘システム完成
#102: Epic 2: ユニット・パイロットシステム完成
#103: Epic 3: GUI・UIシステム改善
#104: Epic 4: イベント・コマンドシステム完成
#105: Epic 5: データ管理・永続化
#106: Epic 6: VB6レガシー関数置換
#107: Epic 7: パフォーマンス最適化
#108: Epic 8: バグ修正・エッジケース対応

## 機能Issues (Epic別)
Epic 1: 15 issues (#109-#123)
Epic 2: 12 issues (#124-#135)
Epic 3: 8 issues (#136-#143)
Epic 4: 10 issues (#144-#153)
Epic 5: 5 issues (#154-#158)
Epic 6: 6 issues (#159-#164)
Epic 7: 5 issues (#165-#169)
Epic 8: 8 issues (#170-#177)

合計: 8 Epic + 69 機能Issue = 77 Issue

全Issueにラベルとマイルストーンを設定しました。
```

---

## 指示例 10: カスタム進捗レポート / Example 10: Custom Progress Report

### 指示 / Instruction

```
@copilot Phase 1 マイルストーンの詳細な進捗レポートを作成してください。

含める情報：
1. Epic 1, 2 の全Issue状態（未着手/進行中/完了）
2. 完了したIssueの差分行数合計
3. 推定残り作業量
4. 現在のベロシティ（週あたり完了Issue数）
5. Phase 1 完了予測日

docs/porting/migration-plan.md と現在のGitHub Issue状態を分析してください。
```

---

## 指示例 11: 依存関係の確認 / Example 11: Checking Dependencies

### 指示 / Instruction

```
@copilot Issue #125（受け流し攻撃）を実装する前に、依存関係を確認してください。

確認事項：
1. 先に完了すべきIssueがあるか
2. 影響を受けるファイルと機能
3. 実装の前提条件
4. 関連するTODOコメント

依存関係の分析結果と実装推奨順序を報告してください。
```

---

## 指示例 12: テストカバレッジの確認 / Example 12: Checking Test Coverage

### 指示 / Instruction

```
@copilot SRC.Sharp/SRCCore/Units/Unit.attackcheck.cs のテストカバレッジを確認し、不足している部分のテストを追加してください。

手順：
1. 既存テストを確認（SRCCoreTests/Units/）
2. カバーされていないメソッドを特定
3. 優先度の高いメソッドのテストを追加
4. テスト実行結果を報告

テストカバレッジレポートとPR番号を報告してください。
```

---

## 指示例 13: リファクタリング提案 / Example 13: Refactoring Suggestion

### 指示 / Instruction

```
@copilot Epic 7（パフォーマンス最適化）の Issue 7.1: Sound システムのキャッシングを実装してください。

実装内容：
1. docs/porting/issue-breakdown.md の Issue 7.1 を確認
2. SRC.Sharp/SRCCore/Sound.cs の重複検索を特定
3. キャッシング機構を実装
4. パフォーマンステストを追加
5. 最適化効果を測定

実装完了後、パフォーマンス改善結果（処理時間短縮率）を報告してください。
```

---

## 指示例 14: ドキュメント更新 / Example 14: Documentation Update

### 指示 / Instruction

```
@copilot Epic 1 の Issue 1.1-1.6 が完了したら、docs/porting/migration-plan.md を更新してください。

更新内容：
1. Epic 1 の進捗状況を反映
2. 完了したIssueをチェックマーク
3. 実装中に判明した新しい課題があれば追記
4. 次のフェーズへの影響を分析

更新したドキュメントのPRを作成してください。
```

---

## 指示例 15: 緊急バグの対応 / Example 15: Emergency Bug Fix

### 指示 / Instruction

```
@copilot 緊急: Unit.attackcheck.cs で null reference exception が発生しています。

対応手順：
1. priority:critical でIssueを作成
2. エラーの原因を特定
3. 最小限の修正で問題を解決
4. リグレッションテストを追加
5. 即座にPRを作成してレビュー依頼

修正完了後、原因と対応内容を報告してください。
```

---

## 指示例 16: ユニットテストの補完（全体）/ Example 16: Complete Unit Tests (All)

### 指示 / Instruction

```
@copilot ユニットテストを補完してください。

手順：
1. SRC.Sharp/SRCCore/CmdDatas/Commands/ の全コマンドクラスを列挙
2. SRC.Sharp/SRCCoreTests/CmdDatas/ の既存テストと比較してカバレッジを確認
3. 未テストのコマンドを特定
4. 各コマンドについて SRC.Sharp.Help/src/[コマンド名]コマンド.md を参照
5. ヘルプの「解説」と「例」を期待値としてユニットテストを作成
6. 実装とヘルプに齟齬がある場合はその旨を報告
7. テストを実行して全件パスを確認（dotnet test SRCCoreTests/SRCCoreTests.csproj）

完了後、追加したテスト数・齟齬一覧・テスト結果を報告してください。
```

### 期待される応答 / Expected Response

```
🧪 ユニットテスト補完 レポート

## 未テストコマンドの特定
対象: 15コマンドが未テスト
- DoCmd / LoopCmd → SwitchDoLoopCmdTests.cs に追加済み
- SwitchCmd / CaseCmd / EndSwCmd → SwitchDoLoopCmdTests.cs に追加済み
- ForEachCmd → ForEachCmdTests.cs に新規作成
- ...

## 追加したテスト
SwitchDoLoopCmdTests.cs: 14件
ForEachCmdTests.cs: 8件
合計: 22件追加

## 齟齬の報告
なし / No discrepancies found.

## テスト結果
Passed: 245, Skipped: 19, Failed: 0

次のステップ: @copilot ユニットテストを補完してください（残り X コマンド）
```

---

## 指示例 17: 特定コマンドのユニットテスト追加 / Example 17: Add Tests for Specific Command

### 指示 / Instruction

```
@copilot ForEachコマンドのユニットテストを追加してください。

手順：
1. SRC.Sharp.Help/src/ForEachコマンド.md を確認
2. 書式1（ユニット対象）・書式2（配列対象）の期待値をヘルプから読み取る
3. SRCCoreTests/CmdDatas/ForEachCmdTests.cs を作成
4. ヘルプの期待値に基づいてテストケースを実装
5. 実装とヘルプに齟齬があれば報告

テスト実行結果を報告してください。
```

---

## まとめ / Summary

これらの指示例により、Copilotは以下を自律的に実行できます：

1. **Issue管理**: Epicと機能Issueの作成・管理
2. **実装作業**: TODOの解決、テスト追加、PR作成
3. **コードレビュー**: PR品質チェック、移植計画との整合性確認
4. **進捗管理**: レポート生成、依存関係分析、予測
5. **バグ修正**: 問題の特定・修正・テスト
6. **最適化**: パフォーマンス改善の実装と測定
7. **ユニットテスト補完**: ヘルプドキュメントを期待値としたテストの追加・齟齬の報告

全ての作業は `docs/` のドキュメントとコードベースの情報のみで完結します。

With these instruction examples, Copilot can autonomously:

1. **Issue Management**: Create and manage Epics and feature issues
2. **Implementation**: Resolve TODOs, add tests, create PRs
3. **Code Review**: Check PR quality and alignment with migration plan
4. **Progress Tracking**: Generate reports, analyze dependencies, predict completion
5. **Bug Fixing**: Identify, fix, and test problems
6. **Optimization**: Implement and measure performance improvements
7. **Unit Test Completion**: Add tests based on help documentation; report discrepancies

All work is self-contained using only `docs/` documentation and codebase information.

---

**Note**: これらの指示はGitHub ProjectsやWebUIのセットアップを必要としません。Copilotはドキュメントとコードベースの情報のみで動作します。

**Note**: These instructions do not require GitHub Projects or Web UI setup. Copilot operates solely on documentation and codebase information.
