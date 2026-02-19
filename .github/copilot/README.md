# GitHub Copilot Agent Instructions / エージェント指示書

このディレクトリには、GitHub Copilot Agentが SRC# 移植完了プロジェクトを運用するための指示書が含まれています。

This directory contains instructions for GitHub Copilot Agents to operate the SRC# migration completion project.

## 📚 Files / ファイル

### 1. [agent-instructions.md](./agent-instructions.md)
**完全な運用手順書 / Complete Operations Manual**

Comprehensive instructions covering:
- 4 specialized agent roles (Issue Creation, Implementation, Review, Progress Tracking)
- Detailed workflows and best practices
- Label system and milestone definitions
- Common tasks and troubleshooting
- Code locations by Epic

**Use this for**: Detailed reference and complete operational guidelines

### 2. [quick-reference.md](./quick-reference.md)
**クイックリファレンスカード / Quick Reference Card**

Condensed reference including:
- Checklists for issue creation, implementation, and review
- Quick command reference
- Epic summary table
- Common issues and solutions
- Label quick guide

**Use this for**: Day-to-day quick lookups and checklists

## 🎯 Purpose / 目的

These instructions enable GitHub Copilot Agents to:

これらの指示により、GitHub Copilot Agentは以下を実行できます：

1. **Create Issues** from the migration plan (`docs/issue-breakdown.md`)
2. **Implement Features** according to TODO comments with proper testing
3. **Review PRs** for quality and adherence to migration guidelines
4. **Track Progress** and generate reports on completion status

## 🚀 Quick Start for Agents / エージェント向けクイックスタート

### First Time Setup
```bash
# 1. Read the overview
cat .github/copilot/quick-reference.md

# 2. Setup labels and milestones
bash docs/scripts/create-labels.sh
bash docs/scripts/create-milestones.sh

# 3. Read your role-specific section
# - For issue creation: Section "1. Issue Creation Agent" in agent-instructions.md
# - For implementation: Section "2. Implementation Agent" in agent-instructions.md
# - For review: Section "3. Review Agent" in agent-instructions.md
# - For tracking: Section "4. Progress Tracking Agent" in agent-instructions.md
```

### Daily Workflow
```bash
# Generate progress report
bash docs/scripts/progress-report.sh

# Check your assigned role tasks
# - Issue Creator: Check docs/issue-breakdown.md for next issue to create
# - Implementer: Check assigned issues with status:in-progress
# - Reviewer: Check PRs awaiting review
# - Tracker: Generate weekly progress report
```

## 📖 Documentation Structure / ドキュメント構造

```
.github/copilot/          # Agent instructions (you are here)
├── agent-instructions.md # Full operational manual
├── quick-reference.md    # Quick reference card
└── README.md            # This file

docs/                     # Migration plan documentation
├── README.md            # Documentation index
├── quick-start.md       # Project overview
├── migration-plan.md    # 8 Epics and overall strategy
├── issue-breakdown.md   # ~70 specific issues
├── github-projects-setup.md  # Project management setup
└── scripts/             # Automation scripts
    ├── create-labels.sh
    ├── create-milestones.sh
    └── progress-report.sh

.github/ISSUE_TEMPLATE/   # Issue templates
├── epic-template.md     # For Epic issues
├── feature-template.md  # For feature implementation
└── bugfix-template.md   # For bug fixes
```

## 🏷️ Label System Overview / ラベルシステム概要

**必須ラベル / Required Labels (4 per issue)**:
1. Epic: `epic:combat`, `epic:unit-pilot`, `epic:ui`, `epic:events`, `epic:data`, `epic:vb6-legacy`, `epic:performance`, `epic:bugfix`
2. Priority: `priority:critical`, `priority:high`, `priority:medium`, `priority:low`
3. Type: `type:epic`, `type:feature`, `type:enhancement`, `type:bugfix`, `type:refactor`, `type:docs`
4. Size: `size:xs`, `size:s`, `size:m`, `size:l`, `size:xl`

**オプションラベル / Optional Labels**:
- Status: `status:blocked`, `status:in-progress`, `status:review`, `status:on-hold`

## 🎯 4 Agent Roles Summary / 4つのエージェント役割サマリー

### 1. Issue Creation Agent / Issue作成エージェント
- **Reads**: `docs/issue-breakdown.md`
- **Creates**: GitHub Issues using templates
- **Applies**: Proper labels and milestones
- **Links**: Child issues to parent Epics

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

### 4. Progress Tracking Agent / 進捗管理エージェント
- **Runs**: `bash docs/scripts/progress-report.sh` weekly
- **Monitors**: Epic completion, milestone progress, blockers
- **Updates**: Project board (Backlog → Ready → In Progress → Review → Done)
- **Reports**: Statistics and trends

## 📊 Project Scope / プロジェクト規模

- **Total Issues**: ~70
- **Total TODOs**: 155+
- **Epics**: 8 categories
- **Milestones**: 4 phases (Q2'26 - Q1'27)
- **Estimated Changes**: 18,000-25,000 lines
- **Estimated Duration**: 12-18 months

## ⚡ Quick Commands / クイックコマンド

```bash
# Setup project management
bash docs/scripts/create-labels.sh
bash docs/scripts/create-milestones.sh

# Generate progress report
bash docs/scripts/progress-report.sh

# List issues by category
gh issue list --label "epic:combat" --state all
gh issue list --label "priority:high" --state open
gh issue list --label "status:in-progress"

# Development
cd SRC.Sharp
dotnet test
dotnet build
```

## 🔗 Related Resources / 関連リソース

- **Migration Plan**: [docs/migration-plan.md](../../docs/migration-plan.md)
- **Issue Breakdown**: [docs/issue-breakdown.md](../../docs/issue-breakdown.md)
- **Quick Start**: [docs/quick-start.md](../../docs/quick-start.md)
- **GitHub Projects Setup**: [docs/github-projects-setup.md](../../docs/github-projects-setup.md)

## 💡 Tips for Agents / エージェント向けTips

1. **Always reference documentation first** - Check `docs/` before starting any work
2. **Follow the established patterns** - Use templates, labels, and workflows consistently
3. **Keep PRs small and focused** - ≤1000 lines unless justified
4. **Add tests for everything** - No feature without tests
5. **Update documentation** - Keep docs in sync with code changes
6. **Communicate blockers early** - Use `status:blocked` label and comment
7. **Run progress reports weekly** - Stay informed on project status

## 📞 Support / サポート

For questions or issues with these instructions:
- **Create an issue**: Tag with `type:docs` label
- **Mention**: @7474 (repository owner)
- **Refer to**: [docs/README.md](../../docs/README.md) for full documentation

---

**Version**: 1.0.0  
**Last Updated**: 2026-02-19  
**Maintainer**: GitHub Copilot Agent System
