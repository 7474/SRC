# GitHub Copilot Agent Instructions / エージェント指示書

このディレクトリには、GitHub Copilot Agentが SRC# 移植完了プロジェクトを運用するための指示書が含まれています。

This directory contains instructions for GitHub Copilot Agents to operate the SRC# migration completion project.

## 🤖 Autonomous Operation / 自律運用モード

**最も簡単な使用方法 / Easiest Way to Use**:

移植を進める:
```
@copilot 移植を進行してください
```

進捗を更新する:
```
@copilot 進捗を更新してください
```

これらの一文だけで、Copilotが自律的に作業を進めたり、現在の進行状況をドキュメントに反映したりします。

With these single commands, Copilot autonomously advances migration work or reflects the current progress status in documents.

**詳細**: [autonomous-agent.md](./autonomous-agent.md) を参照してください。

**Details**: See [autonomous-agent.md](./autonomous-agent.md)

---

## 🔄 GitHub Agentic Workflows / 自動化ワークフロー

**ユニットテスト補完は自動化されています / Unit test completion is automated:**

`.github/workflows/complete-unit-tests.md` により、毎週月曜日に自動実行されます。

The workflow at `.github/workflows/complete-unit-tests.md` runs automatically every Monday.

手動実行も可能です / Can also run manually:
```bash
gh workflow run complete-unit-tests.lock.yml
```

> **セットアップ注意 / Setup Note**: ワークフローを有効化するには `gh aw compile` の実行が必要です。  
> `compile-agentic-workflows.yml` が自動でコンパイルします (`.md` ファイルのpush時)。
>
> To activate the workflow, run `gh aw compile`. The `compile-agentic-workflows.yml`  
> workflow automatically compiles it when the `.md` file is pushed to master.

---

## 📚 Files / ファイル

### 1. [autonomous-agent.md](./autonomous-agent.md) ⭐ **最優先 / Top Priority**
**完全自律型エージェント / Fully Autonomous Agent**

2つの単一コマンドで移植を運用：
- 「移植を進行してください」で作業を進める
- 「進捗を更新してください」で進行状況をドキュメントに反映
- 自動的にタスク選択・実装・テスト・PR作成
- 進捗レポート自動生成
- GitHub Projects等のセットアップ不要

Two-command migration operation:
- "Proceed with migration" to advance work
- "Update progress" to reflect current state in documents
- Auto-selects, implements, tests, creates PRs
- Auto-generates progress reports
- No GitHub Projects or setup required

**🚀 Use this for**: Minimal human intervention, maximum automation

### 2. [copilot-instructions-examples.md](./copilot-instructions-examples.md)
**Copilotへの指示例 / Example Copilot Instructions**

15個の具体的な指示例を提供：
- より細かい制御が必要な場合に使用
- 特定のタスクを指定したい場合
- 手動でタスクを選択したい場合

Provides 15 concrete instruction examples:
- For more granular control
- For specifying particular tasks
- For manual task selection

**Use this for**: Fine-grained control over specific tasks

### 3. [agent-instructions.md](./agent-instructions.md)
**完全な運用手順書 / Complete Operations Manual**

詳細な運用手順：
- 4つの専門エージェント役割
- ワークフローとベストプラクティス
- ラベルシステムとマイルストーン

Comprehensive instructions:
- 4 specialized agent roles
- Detailed workflows and best practices
- Label system and milestone definitions

**Use this for**: Understanding the full system and advanced configuration

### 4. [quick-reference.md](./quick-reference.md)
**クイックリファレンスカード / Quick Reference Card**

簡潔なリファレンス：
- チェックリスト
- クイックコマンド
- Epic一覧

Condensed reference:
- Checklists
- Quick commands
- Epic summary

**Use this for**: Day-to-day quick lookups

## 🎯 Purpose / 目的

**⚡ Fully Autonomous Operation**: 2つのコマンドだけで、移植作業の進行と進捗更新を自律的に管理します。

**⚡ Fully Autonomous Operation**: With just two commands, Copilot autonomously manages both migration work and progress updates.

### 人間が考えることは最小限 / Minimal Human Thinking Required

- ✅ 移植作業: `@copilot 移植を進行してください`
- ✅ 進捗更新: `@copilot 進捗を更新してください`
- ✅ Copilotが自動判断: 次のタスク、実装方法、テスト戦略
- ✅ Copilotが自動実行: Issue作成、コード実装、テスト追加、PR作成
- ✅ Copilotが自動報告: 完了内容、次のタスク提案、進捗状況
- ❌ セットアップ不要: GitHub Projects, ラベル, マイルストーン
- ❌ 詳細指示不要: Copilotがドキュメントとコードベースから自動判断

With autonomous agent:
- ✅ Migration: `@copilot Proceed with migration`
- ✅ Progress update: `@copilot Update the progress`
- ✅ Copilot auto-decides: Next task, implementation approach, test strategy
- ✅ Copilot auto-executes: Issue creation, code implementation, test addition, PR creation
- ✅ Copilot auto-reports: Completion status, next task suggestion, progress status
- ❌ No setup: GitHub Projects, labels, milestones
- ❌ No detailed instructions: Copilot auto-decides from docs and codebase

### 動作原理 / How It Works

```
┌─────────────────────────────────────────────────────────────┐
│  Human Input (Once)                                         │
│  @copilot 移植を進行してください                              │
└──────────────────────┬──────────────────────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────────────┐
│  Copilot Auto-Assessment                                    │
│  ✓ Check current state (issues, PRs, TODOs)                │
│  ✓ Analyze docs/porting/issue-breakdown.md                         │
│  ✓ Scan codebase for TODOs                                 │
│  ✓ Select next highest-priority task                       │
└──────────────────────┬──────────────────────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────────────┐
│  Copilot Auto-Execution                                     │
│  ✓ Create issue (if needed)                                │
│  ✓ Implement feature                                       │
│  ✓ Add tests                                               │
│  ✓ Run tests                                               │
│  ✓ Create PR                                               │
└──────────────────────┬──────────────────────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────────────┐
│  Copilot Auto-Report                                        │
│  ✓ Report completion                                       │
│  ✓ Suggest next task                                       │
│  ✓ Ask: Continue? (Y/N)                                    │
└─────────────────────────────────────────────────────────────┘
```

## 🚀 Quick Start for Agents / エージェント向けクイックスタート

### 最速スタート / Fastest Start (Recommended)

```bash
# 1. 指示例を確認 / Check instruction examples
cat .github/copilot/copilot-instructions-examples.md

# 2. 好きな指示をコピー / Copy any instruction you like

# 3. Copilotチャットに貼り付け / Paste to Copilot chat
@copilot [指示内容をここに貼り付け / Paste instruction here]

# 完了！GitHub Projectsなどのセットアップは不要です
# Done! No GitHub Projects or other setup required
```

### Example: 最初のEpic Issue作成

```
@copilot Epic 1: 戦闘システム完成 のIssueを作成してください。

以下の情報を使用：
- タイトル: "Epic 1: 戦闘システム完成 (Combat System Completion)"
- 内容: docs/porting/issue-breakdown.md の Epic 1 セクションを参照
- マイルストーン: Phase 1: コア機能完成 (v3.1.0)
```

### Daily Workflow
```bash
# Check your assigned role tasks
# - Issue Creator: Check docs/porting/issue-breakdown.md for next issue to create
# - Implementer: Check assigned issues with status:in-progress
# - Reviewer: Check PRs awaiting review
# - Tracker: @copilot 進捗を更新してください
```

## 📖 Documentation Structure / ドキュメント構造

```
.github/copilot/          # Agent instructions (you are here)
├── agent-instructions.md # Full operational manual
├── quick-reference.md    # Quick reference card
└── README.md            # This file

docs/                     # Documentation root
├── README.md            # Documentation index
└── porting/             # Porting-related documentation
    ├── README.md        # Porting docs index
    ├── migration-plan.md    # Overall strategy and remaining tasks
    ├── porting-quality-plan.md  # Quality verification phase plan
    └── issue-breakdown.md   # Historical issue definitions

.github/ISSUE_TEMPLATE/   # Issue templates
├── epic-template.md     # For Epic issues
├── feature-template.md  # For feature implementation
└── bugfix-template.md   # For bug fixes
```

## 🎯 4 Agent Roles Summary / 4つのエージェント役割サマリー

### 2. Implementation Agent / 実装エージェント
- **Reads**: Issue description, TODO comments, surrounding code
- **Implements**: Feature with tests (PR ≤1000 lines)
- **Commits**: `[Epic X.Y] Description (Closes #XXX)`
- **Tests**: Runs `dotnet test` before PR

### 3. Review Agent / レビューエージェント
- **Reviews**: PR size, tests, documentation, code quality
- **Verifies**: Alignment with migration plan
- **Checks**: No regression or unintended side effects
- **Approves**: Only after all criteria met

---

**Version**: 1.1.0  
**Last Updated**: 2026-02-20  
**Maintainer**: GitHub Copilot Agent System
