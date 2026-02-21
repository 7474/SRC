# Copilotへの指示例 / Example Copilot Instructions

このファイルには、SRC#の**品質検証・精度向上フェーズ**でCopilotに指示するための具体的な例が含まれています。

This file contains specific instruction examples for the SRC# **quality verification and accuracy improvement phase**.

## 基本方針 / Basic Approach

Copilotは以下の情報を元に自律的に作業します：
- `docs/porting/porting-quality-plan.md` — 品質向上ロードマップ（Phase Q1〜Q4）
- `docs/porting/porting-assessment.md` — 移植状況総合評価と精度リスク分析
- `docs/porting/migration-plan.md` — 残存TODO一覧
- `SRC.Sharp.Help/src/` — コマンドヘルプドキュメント（テスト期待値の基準）

---

## 指示例 1: ユニットテストの補完（全体）/ Example 1: Complete Unit Tests (All)

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

## 指示例 2: 特定コマンドのユニットテスト追加 / Example 2: Add Tests for Specific Command

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

## 指示例 3: 残存TODOの修正 / Example 3: Fix Remaining TODO

### 指示 / Instruction

```
@copilot SRC.Sharp/SRCCore/COM.cs の「武器選択に失敗してるケースがある」TODOを修正してください。

手順：
1. docs/porting/migration-plan.md の残存TODO一覧を確認
2. SRC/SRC_20121125/ の元VB6コードで期待動作を確認
3. TODOコメントと周辺コードを読んで問題を特定
4. 最小限の修正で問題を解決
5. 回帰テストを追加
6. PR を作成（差分は1000行以下）

修正内容とテスト結果を報告してください。
```

---

## 指示例 4: セーブデータ互換性の検証 / Example 4: Save Data Compatibility Verification

### 指示 / Instruction

```
@copilot SRC.Sharp/SRCCore/SRC.save.cs のセーブ・ロードの精査を行ってください。

確認内容：
1. 既存のセーブ・ロードのフローを確認
2. SRC/SRC_20121125/ の元VB6コードとの差異を調査
3. セーブ→ロードのラウンドトリップテストを実装（可能な場合）
4. 互換性の問題があれば Issue として記録

調査結果と推奨アクションを報告してください。
```

---

## 指示例 5: MockGUI のスタブ追加 / Example 5: Add MockGUI Stubs

### 指示 / Instruction

```
@copilot SRC.Sharp/SRCCoreTests/TestLib/MockGUI.cs の NotImplementedException を減らしてください。

手順：
1. 現在の NotImplementedException 数を確認
2. テストコードで頻繁に呼ばれるが未実装のプロパティ/メソッドを特定
3. 最小限のstub実装を追加（実際のUIは不要、状態を保持するだけでよい）
4. 既存テストが通ることを確認

対応後のNotImplementedException数とテスト結果を報告してください。
```

---

## 指示例 6: 移植精度の検証 / Example 6: Verify Porting Accuracy

### 指示 / Instruction

```
@copilot 移植精度を検証してください。

対象: SRC.Sharp/SRCCore/Units/Unit.attackcheck.cs の戦闘計算ロジック

手順：
1. VB6元コード（SRC/SRC_20121125/Unit.bas）の該当ロジックを確認
2. C#実装との差異を調査
3. 差異があれば差異の内容を記録し、修正すべきかを判断
4. テストケースを追加（VB6版と同等の動作を検証）

検証結果と発見した差異を報告してください。
```

---

## 指示例 7: 進捗を更新する / Example 7: Update Progress

### 指示 / Instruction

```
@copilot 進捗を更新してください。

確認内容：
1. 残存TODOの数（grep -rn "// TODO" SRC.Sharp/SRCCore/）
2. 最近マージされたPR
3. ユニットテスト数の変化
4. docs/porting/migration-plan.md の更新が必要かどうか

進捗レポートを生成し、必要であればドキュメントを更新してください。
```

### 期待される応答 / Expected Response

```
📊 進捗レポート (YYYY-MM-DD)

## 変更された内容
- クローズされたIssue: #XXX, #YYY
- マージされたPR: #ZZZ

## 現在のテスト状況
- テストメソッド数: 253 → 275（+22）
- 残存TODO数: 18

## 更新されたドキュメント
- docs/porting/migration-plan.md: テスト数を更新

次のステップ: @copilot ユニットテストを補完してください
```

---

## 指示例 8: コードレビュー / Example 8: Code Review

### 指示 / Instruction

```
@copilot PR #XXX をレビューしてください。

確認項目：
1. PR差分が1000行以下か
2. テストが追加されているか
3. docs/porting/porting-quality-plan.md の方針に沿っているか
4. 既存機能への影響がないか
5. 後方互換性（セーブデータ、シナリオファイル）が維持されているか

レビュー結果とコメントを報告してください。
```

---

## 指示例 9: FillStyle の全種類対応 / Example 9: Implement FillStyle

### 指示 / Instruction

```
@copilot SRC.Sharp/SRCSharpForm/Forms/Main.guiscrean.cs の「FillStyle の全種類対応」TODOを実装してください。

手順：
1. VB6元コード（SRC/SRC_20121125/）でFillStyleの使用箇所を確認
2. VB6のFillStyle定数（6種類）とC#のSystem.Drawing.Brushingの対応を調査
3. HatchBrush等を使って未対応スタイルを実装
4. 手動テストまたは自動テストで描画結果を確認

実装内容を報告してください。
```

---

## まとめ / Summary

これらの指示例により、Copilotは品質検証・精度向上フェーズで以下を自律的に実行できます：

1. **テスト補完**: ヘルプドキュメントを期待値としたユニットテストの追加
2. **精度検証**: VB6版との動作差異の特定と修正
3. **TODO解決**: 残存18件のTODO（精査・最適化・リファクタリング）
4. **進捗管理**: 品質メトリクスの追跡とドキュメント更新
5. **コードレビュー**: 品質基準への適合確認

全ての作業は `docs/porting/` のドキュメント、ヘルプドキュメント（`SRC.Sharp.Help/src/`）、および元VB6コード（`SRC/SRC_20121125/`）を情報源として完結します。

---

**Note**: 詳細な運用手順は [agent-instructions.md](./agent-instructions.md) を参照してください。
