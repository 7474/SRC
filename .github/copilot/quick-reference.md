# Copilot Quick Reference / クイックリファレンス

SRC# **品質検証・精度向上フェーズ**の簡易リファレンス。

## 📋 基本ドキュメント / Key Documents

1. **[docs/porting/porting-quality-plan.md](../../docs/porting/porting-quality-plan.md)** — 品質向上ロードマップ（Phase Q1〜Q4）
2. **[docs/porting/porting-assessment.md](../../docs/porting/porting-assessment.md)** — 移植状況総合評価
3. **[docs/porting/migration-plan.md](../../docs/porting/migration-plan.md)** — 残存TODO一覧と現在の状況
4. **[agent-instructions.md](./agent-instructions.md)** — 詳細な運用手順

## 🤖 基本コマンド / Basic Commands

```
@copilot ユニットテストを補完してください
@copilot 移植精度を検証してください
@copilot 進捗を更新してください
```

## 🎯 3つのエージェント役割 / 3 Agent Roles

### 1. Test Completion Agent / テスト補完エージェント
- **入力**: 未テストコマンド + ヘルプドキュメント
- **出力**: ユニットテスト（PR ≤1000行）
- **基準**: `SRC.Sharp.Help/src/[コマンド名]コマンド.md`
- **実行**: `cd SRC.Sharp && dotnet test SRCCoreTests/SRCCoreTests.csproj`

### 2. Implementation Agent / 実装エージェント
- **入力**: 残存TODOコメント + VB6元コード
- **出力**: 修正 + テスト（PR ≤1000行）
- **参照**: `SRC/SRC_20121125/`（VB6元コード）
- **実行**: `cd SRC.Sharp && dotnet test`

### 3. Review Agent / レビューエージェント
- **確認**: PR規模、テスト、後方互換性、品質計画との整合
- **参照**: `docs/porting/porting-quality-plan.md`

## 📂 テスト対象の優先順位 / Test Priority

| 領域 | コード行数 | 現テスト数 | 目標 | 優先度 |
|------|-----------|-----------|------|--------|
| Units/ | 86,480行 | 53件 | 150件+ | 🔴 高 |
| Events/ | 8,789行 | 0件 | 30件+ | 🔴 高 |
| CmdDatas/ | 21,172行 | 99件 | 150件+ | 🟡 中 |
| Pilots/ | 4,530行 | 9件 | 30件+ | 🟡 中 |

## ⚡ クイックコマンド / Quick Commands

```bash
# テスト実行
cd SRC.Sharp && dotnet test

# 残存TODOを確認
grep -rn "// TODO" SRC.Sharp/SRCCore/
grep -rn "// TODO" SRC.Sharp/SRCSharpForm/

# 未テストコマンドを調査
find SRC.Sharp/SRCCore/CmdDatas/Commands -name "*.cs" | wc -l
ls SRC.Sharp/SRCCoreTests/CmdDatas/

# MockGUI NotImplementedException数を確認
grep -rn "throw new NotImplementedException" SRC.Sharp/SRCCoreTests/ | wc -l

# PR作成
gh pr create
```

## ✅ テスト作成チェックリスト / Test Checklist

- [ ] ヘルプドキュメント（`SRC.Sharp.Help/src/[コマンド名]コマンド.md`）を確認
- [ ] 正常動作テストを1件以上追加
- [ ] エラーテストを1件以上追加
- [ ] 実装とヘルプの齟齬を確認（あれば実装を修正）
- [ ] `dotnet test` でパスを確認
- [ ] PR差分が1000行以下

## ✅ TODO修正チェックリスト / TODO Fix Checklist

- [ ] VB6元コード（`SRC/SRC_20121125/`）で期待動作を確認
- [ ] 最小限の変更で修正
- [ ] 回帰テストを追加
- [ ] `dotnet test` でパスを確認
- [ ] PR説明に `Closes #XXX`

## ✅ レビューチェックリスト / Review Checklist

- [ ] PR差分が1000行以下（超過なら理由を確認）
- [ ] テストが追加・更新されている
- [ ] 後方互換性が維持されている（セーブデータ、シナリオファイル）
- [ ] 不必要な変更が含まれていない
- [ ] 既存テストが全て通過

## 🚨 よくある問題 / Common Issues

| 問題 | 解決策 |
|------|--------|
| テスト失敗 | 副作用を確認、既存動作仮定を壊していないか確認 |
| VB6との動作差異 | `SRC/SRC_20121125/` で確認、差異をIssueに記録 |
| MockGUI NotImplementedException | テストをコアロジックに限定、または必要なstubを追加 |
| PR > 1000行 | 小さいIssueに分割（横断的変更を除く） |

## 📊 品質フェーズ KPI / Quality Phase KPIs

| 指標 | 現状（2026-02-21） | Phase Q1目標 |
|------|-------------------|-------------|
| テストメソッド数 | ~253 | 400+ |
| コードカバレッジ（SRCCore） | 推定15% | 25% |
| MockGUI NotImplementedException | 132件 | 100件以下 |
| 残存TODO数 | 18 | 縮小 |

## 🔗 主要リンク / Key Links

- **品質計画**: [porting-quality-plan.md](../../docs/porting/porting-quality-plan.md)
- **評価レポート**: [porting-assessment.md](../../docs/porting/porting-assessment.md)
- **詳細手順**: [agent-instructions.md](./agent-instructions.md)

---

**For detailed instructions, see [agent-instructions.md](./agent-instructions.md)**
