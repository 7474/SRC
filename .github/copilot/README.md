# GitHub Copilot Agent Instructions / エージェント指示書

このディレクトリには、GitHub Copilot AgentがSRC#の**品質検証・精度向上フェーズ**を運用するための指示書が含まれています。

This directory contains instructions for GitHub Copilot Agents to operate the SRC# **quality verification and accuracy improvement phase**.

## 📌 現在のフェーズ / Current Phase

**品質検証・精度向上フェーズ** (2026-02-21〜)

- TODO消化フェーズは完了（137+ TODO解消済み、約88%）
- 残存18件のTODOは精査・最適化・リファクタリング課題
- 現在の焦点：テストカバレッジ向上、VB6版との精度検証、残存TODO解消

## 🤖 基本コマンド / Basic Commands

```
@copilot ユニットテストを補完してください
@copilot 移植精度を検証してください
@copilot 進捗を更新してください
```

---

## 📚 Files / ファイル

### 1. [agent-instructions.md](./agent-instructions.md) ⭐ **最優先 / Top Priority**
**完全な運用手順書 / Complete Operations Manual**

品質検証フェーズの詳細な運用手順：
- 3つのエージェント役割（テスト補完・実装・レビュー）
- テスト作成ガイドライン
- 残存TODOの場所と内容
- ベストプラクティス

**Use this for**: Understanding roles, workflows, and detailed procedures

### 2. [copilot-instructions-examples.md](./copilot-instructions-examples.md)
**Copilotへの指示例 / Example Copilot Instructions**

品質検証フェーズの具体的な指示例：
- ユニットテスト補完（全体・特定コマンド）
- 残存TODOの修正
- セーブデータ互換性検証
- MockGUIスタブ追加
- 移植精度検証

**Use this for**: Ready-to-use instruction templates

### 3. [quick-reference.md](./quick-reference.md)
**クイックリファレンスカード / Quick Reference Card**

簡潔なリファレンス：
- 基本コマンド
- テスト対象優先順位
- チェックリスト
- KPI

**Use this for**: Day-to-day quick lookups

---

## 🎯 3 Agent Roles Summary / 3つのエージェント役割サマリー

### 1. Test Completion Agent / テスト補完エージェント
- **Input**: 未テストコマンド + `SRC.Sharp.Help/src/[コマンド名]コマンド.md`
- **Output**: ユニットテスト（PR ≤1000行）
- **Run**: `cd SRC.Sharp && dotnet test SRCCoreTests/SRCCoreTests.csproj`

### 2. Implementation Agent / 実装エージェント
- **Input**: 残存TODOコメント + `SRC/SRC_20121125/`（VB6元コード）
- **Output**: 修正 + 回帰テスト（PR ≤1000行）
- **Format**: `[Quality] Brief description (Closes #IssueNumber)`

### 3. Review Agent / レビューエージェント
- **Check**: PR規模、テスト、後方互換性、品質計画との整合
- **Reference**: `docs/porting/porting-quality-plan.md`

---

## 📖 Documentation Structure / ドキュメント構造

```
.github/copilot/           # Agent instructions (you are here)
├── agent-instructions.md  # Full operational manual (quality phase)
├── copilot-instructions-examples.md  # Instruction examples (quality phase)
├── quick-reference.md     # Quick reference card
└── README.md              # This file

docs/porting/              # Documentation root
├── migration-plan.md      # Remaining TODOs and current status
├── porting-quality-plan.md  # Quality improvement roadmap (Phase Q1-Q4)
└── porting-assessment.md  # Comprehensive porting assessment

SRC.Sharp.Help/src/        # Help documentation (test expected values)
SRC/SRC_20121125/          # Original VB6 code (accuracy reference)
SRC.Sharp/SRCCoreTests/    # Unit tests
```

---

**Version**: 2.0.0  
**Last Updated**: 2026-02-21  
**Phase**: Quality Verification & Accuracy Improvement
