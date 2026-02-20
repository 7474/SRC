# Copilot Agent Instructions for SRC# Migration

このファイルは、GitHub Copilot Agentが SRC# 移植完了プロジェクトを運用するための指示書です。

## 🚀 Self-Contained Operation / 自己完結型運用

**重要**: このプロジェクトは、GitHub ProjectsやWebUIのセットアップなしに、Copilotへの指示のみで作業を進められるよう設計されています。

**Important**: This project is designed to work with Copilot instructions alone, without requiring GitHub Projects or Web UI setup.

### 必要な情報源 / Required Information Sources

Copilotは以下の情報のみで自律的に動作します：

Copilot operates autonomously using only:

1. **`docs/migration-plan.md`** - 8 Epics and overall migration strategy
2. **`docs/issue-breakdown.md`** - Detailed definitions of ~70 issues
3. **`.github/ISSUE_TEMPLATE/`** - Issue templates (Epic, Feature, Bugfix)
4. **Codebase TODO comments** - Specific implementation requirements
5. **`.github/copilot/copilot-instructions-examples.md`** - Concrete instruction examples

### セットアップ不要 / No Setup Required

- ❌ GitHub Projects board creation - Not needed
- ❌ Manual label creation - Copilot creates as needed
- ❌ Manual milestone creation - Copilot creates as needed
- ✅ Direct instructions to Copilot - All you need

**指示例**: 詳細な使用例は [copilot-instructions-examples.md](./copilot-instructions-examples.md) を参照してください。

**Example Instructions**: See [copilot-instructions-examples.md](./copilot-instructions-examples.md) for detailed usage examples.

## Overview / 概要

SRC# (Simulation RPG Construction Sharp) is a C# port of the VB6-based SRC game engine. This repository has a comprehensive migration completion plan documented in `docs/` that breaks down 155+ TODO comments into ~70 manageable issues across 8 epic categories.

SRC#は、VB6ベースのSRCゲームエンジンのC#移植版です。このリポジトリには、155以上のTODOコメントを8つのEpicカテゴリの約70個の管理可能なIssueに分解した包括的な移植完了計画が`docs/`にドキュメント化されています。

## Agent Roles / エージェントの役割

When working on this project, Copilot agents should follow these specialized roles:

### 1. Issue Creation Agent / Issue作成エージェント

**Purpose**: Create GitHub issues from the migration plan.

**Instructions**:
- Reference `docs/issue-breakdown.md` for the complete list of ~70 issues
- Use appropriate templates from `.github/ISSUE_TEMPLATE/`:
  - `epic-template.md` for Epic (parent) issues (8 total)
  - `feature-template.md` for feature implementation issues
  - `bugfix-template.md` for bug fix issues
- Apply labels according to `docs/github-projects-setup.md`:
  - One `epic:*` label (combat, unit-pilot, ui, events, data, vb6-legacy, performance, bugfix)
  - One `priority:*` label (critical, high, medium, low)
  - One `type:*` label (epic, feature, enhancement, bugfix, refactor, docs)
  - One `size:*` label (xs, s, m, l, xl) based on estimated line changes
- Assign to appropriate milestone (Phase 1-4)
- Link child issues to parent Epic issues using "Related to #XXX"

**Example Issue Creation**:
```
Title: Unit.attackcheck.cs: 回避攻撃の実装
Labels: epic:combat, priority:high, type:feature, size:s
Milestone: Phase 1: コア機能完成 (v3.1.0)
Body: [Use feature-template.md and fill with content from issue-breakdown.md Issue 1.1]
```

### 2. Implementation Agent / 実装エージェント

**Purpose**: Implement features and fix bugs according to issues.

**Instructions**:
- Before starting, read:
  - The specific issue description
  - Related TODO comments in the codebase
  - Surrounding code context
  - `docs/migration-plan.md` for the overall context
- Follow these constraints:
  - PR diff should be ≤1000 lines (except for cross-cutting concerns)
  - One issue = One PR
  - Add tests for new functionality
  - Update documentation if API changes
  - Run existing tests to ensure no regression
- Commit message format: `[Epic X.Y] Brief description (Closes #IssueNumber)`
- Reference the issue number in PR description with `Closes #XXX`

**Code Locations by Epic**:
- Epic 1 (Combat): `SRC.Sharp/SRCCore/Units/Unit.attackcheck.cs`, `Unit.attack.cs`, `Command.attack.cs`
- Epic 2 (Unit/Pilot): `SRC.Sharp/SRCCore/Units/Unit.lookup.cs`, `Pilots/Pilot.skill.cs`, `Unit.ability.cs`
- Epic 3 (GUI/UI): `SRC.Sharp/SRCSharpForm/Forms/Main.gui*.cs`, `UIInterface/*.cs`
- Epic 4 (Events): `SRC.Sharp/SRCCore/CmdDatas/Commands/**/*.cs`, `Events/Event.*.cs`
- Epic 5 (Data): `SRC.Sharp/SRCCore/SRC.save.cs`, `SRC.config.cs`, `Config/*.cs`
- Epic 6 (VB6 Legacy): `SRC.Sharp/SRCCore/VB/*.cs`, `Lib/FileSystem.cs`
- Epic 7 (Performance): `SRC.Sharp/SRCCore/Sound.cs`, Various optimization targets
- Epic 8 (Bugfix): Various files as specified in issues

### 3. Review Agent / レビューエージェント

**Purpose**: Review PRs for quality and adherence to migration plan.

**Instructions**:
- Check against migration plan guidelines:
  - PR size ≤1000 lines (flag if exceeded without justification)
  - Tests added/updated
  - Documentation updated (if applicable)
  - No unrelated changes
  - Addresses the TODO comment cited in the issue
- Verify:
  - Code quality and consistency with existing codebase
  - Test coverage
  - No introduction of new TODOs (unless justified)
  - Backward compatibility maintained
- Reference `docs/migration-plan.md` to ensure changes align with overall strategy

### 4. Progress Tracking Agent / 進捗管理エージェント

**Purpose**: Track and report project progress.

**Instructions**:
- Run `bash docs/scripts/progress-report.sh` weekly to generate progress reports
- Monitor:
  - Issues completed per Epic
  - Milestone progress (Phase 1-4)
  - Blocked issues (label: `status:blocked`)
  - Issues in progress (label: `status:in-progress`)
- Update project board by moving issues through columns:
  - Backlog → Ready → In Progress → In Review → Done
- Report statistics:
  - Completion percentage by Epic
  - Estimated time to milestone completion
  - Blocker analysis

## Key Files Reference / 主要ファイル参照

### Documentation / ドキュメント
- `docs/quick-start.md` - Start here for overview
- `docs/migration-plan.md` - Overall strategy and Epic definitions
- `docs/issue-breakdown.md` - Complete list of ~70 issues with details
- `docs/github-projects-setup.md` - Project management setup guide
- `docs/README.md` - Documentation index

### Templates / テンプレート
- `.github/ISSUE_TEMPLATE/epic-template.md` - For Epic issues (8 total)
- `.github/ISSUE_TEMPLATE/feature-template.md` - For feature implementation
- `.github/ISSUE_TEMPLATE/bugfix-template.md` - For bug fixes

### Scripts / スクリプト
- `docs/scripts/create-labels.sh` - Create all labels
- `docs/scripts/create-milestones.sh` - Create milestones
- `docs/scripts/progress-report.sh` - Generate progress report

## Label System / ラベルシステム

### Epic Labels (8)
- `epic:combat` - Combat system implementation
- `epic:unit-pilot` - Unit and pilot system
- `epic:ui` - GUI and UI improvements
- `epic:events` - Event and command system
- `epic:data` - Data management and persistence
- `epic:vb6-legacy` - VB6 legacy function replacement
- `epic:performance` - Performance optimization
- `epic:bugfix` - Bug fixes and edge cases

### Priority Labels (4)
- `priority:critical` - Critical issues
- `priority:high` - High priority
- `priority:medium` - Medium priority
- `priority:low` - Low priority

### Type Labels (6)
- `type:epic` - Epic (parent) issue
- `type:feature` - New feature implementation
- `type:enhancement` - Improvement to existing feature
- `type:bugfix` - Bug fix
- `type:refactor` - Code refactoring
- `type:docs` - Documentation

### Size Labels (5)
- `size:xs` - ~100 lines
- `size:s` - 200-400 lines
- `size:m` - 400-700 lines
- `size:l` - 700-1000 lines
- `size:xl` - 1000+ lines (cross-cutting concerns only)

### Status Labels (4)
- `status:blocked` - Blocked by dependency
- `status:in-progress` - Currently being worked on
- `status:review` - In code review
- `status:on-hold` - Temporarily paused

## Milestones / マイルストーン

### Phase 1: Core Features (v3.1.0) - Q2 2026
- Epic 1: Combat System (basic features)
- Epic 2: Unit & Pilot System (basic features)

### Phase 2: UI/UX Improvements (v3.2.0) - Q3 2026
- Epic 1: Combat System (advanced features)
- Epic 3: GUI & UI
- Epic 4: Events & Commands

### Phase 3: Quality Improvements (v3.3.0) - Q4 2026
- Epic 5: Data Management
- Epic 8: Bug Fixes

### Phase 4: Optimization & Completion (v3.4.0) - Q1 2027
- Epic 6: VB6 Legacy
- Epic 7: Performance

## Common Tasks / 共通タスク

### Creating an Epic Issue
```bash
# 1. Go to Issues → New Issue
# 2. Select "Epic Issue Template"
# 3. Fill in:
#    - Title: "Epic X: [Category Name]"
#    - Description from docs/issue-breakdown.md
#    - Labels: type:epic, epic:[category], priority:[level]
#    - Milestone: Appropriate phase
# 4. Create issue
# 5. Note the issue number (#XXX)
```

### Creating a Feature Issue
```bash
# 1. Reference docs/issue-breakdown.md for details
# 2. Go to Issues → New Issue
# 3. Select "Feature Implementation Issue Template"
# 4. Fill in:
#    - Title: "[File]: [Feature Description]"
#    - TODO comment from codebase
#    - Implementation details
#    - Labels: epic:[category], priority:[level], type:feature, size:[xs-xl]
#    - Milestone: Appropriate phase
#    - Link to parent Epic: "Related to #XXX"
# 5. Create issue
```

### Implementing a Feature
```bash
# 1. Assign issue to yourself
# 2. Add label: status:in-progress
# 3. Create branch: git checkout -b feature/issue-XXX-brief-description
# 4. Locate TODO comment in codebase
# 5. Implement feature following guidelines:
#    - Keep changes focused and minimal
#    - Add tests in SRC.Sharp/SRCCoreTests/
#    - Update docs if needed
#    - Run tests: dotnet test
# 6. Commit: git commit -m "[Epic X.Y] Brief description (Closes #XXX)"
# 7. Push and create PR
# 8. In PR description: "Closes #XXX"
# 9. Add label to issue: status:review
# 10. After review approval: merge PR
```

### Tracking Progress
```bash
# Generate progress report
cd /path/to/SRC
bash docs/scripts/progress-report.sh

# View Epic-specific progress
gh issue list --label "epic:combat" --state all

# View milestone progress
gh api repos/7474/SRC/milestones | jq '.[] | {title, open_issues, closed_issues}'
```

## Workflow Example / ワークフロー例

### Scenario: Implementing Issue 1.1 (Dodge Attack)

1. **Issue Creation Agent**:
   - Creates issue from `docs/issue-breakdown.md` Issue 1.1
   - Title: "Unit.attackcheck.cs: 回避攻撃の実装"
   - Labels: `epic:combat`, `priority:high`, `type:feature`, `size:s`
   - Milestone: Phase 1
   - Links to Epic 1 issue

2. **Implementation Agent**:
   - Reads issue and finds TODO in `Unit.attackcheck.cs`
   - Implements `IsDodgeAttack()` method
   - Adds test in `SRCCoreTests/Units/UnitAttackTests.cs`
   - Creates PR with 250 line diff
   - PR description: "Closes #[IssueNumber]"

3. **Review Agent**:
   - Checks PR size (250 lines ✓)
   - Verifies tests added (✓)
   - Reviews code quality (✓)
   - Checks TODO is addressed (✓)
   - Approves PR

4. **Progress Tracking Agent**:
   - Runs progress report
   - Updates: Epic 1 progress 1/15 complete (6.7%)
   - Moves issue to Done column

## Best Practices / ベストプラクティス

### For All Agents
- Always reference documentation in `docs/`
- Follow the established label system
- Maintain bilingual (Japanese/English) communication
- Keep changes focused and minimal
- Test thoroughly before marking complete

### For Issue Creation
- Use exact wording from `docs/issue-breakdown.md`
- Apply all required labels
- Link child issues to parent Epics
- Ensure milestone is set

### For Implementation
- Read TODO comment and surrounding code first
- Keep PR diff ≤1000 lines
- Add comprehensive tests
- Run existing tests to prevent regression
- Update documentation for public APIs
- ユニットテストの補完・作成において、テストで判明した実装の挙動とヘルプ（`SRC.Sharp.Help/src/` 配下のドキュメント）の記載に齟齬がある場合は、ヘルプを正として実装を修正してください。

### For Review
- Check alignment with migration plan
- Verify PR size constraints
- Ensure tests are comprehensive
- Look for unintended side effects
- Confirm documentation is updated

### For Progress Tracking
- Run reports weekly
- Identify blockers early
- Update project board regularly
- Communicate progress to stakeholders

## Troubleshooting / トラブルシューティング

### Issue: PR exceeds 1000 lines
**Solution**: Split into smaller issues unless it's a cross-cutting concern (refactoring, string function replacement). Document justification in PR.

### Issue: Test failures after implementation
**Solution**: Review changes for unintended side effects. Check if new feature breaks existing assumptions. Update or fix tests appropriately.

### Issue: TODO comment unclear
**Solution**: Examine surrounding code, check original VB6 code in `SRC/SRC_20121125/`, or reference `SRC.NET/` for .NET conversion attempts. Ask in issue comments if still unclear.

### Issue: Dependency blocked
**Solution**: Add `status:blocked` label. Comment on issue with blocking dependency. Work on other non-blocked issues in meantime.

### Issue: Scope creep in implementation
**Solution**: Stick to the specific TODO being addressed. Create new issues for discovered problems. Keep focus narrow.

## Quick Reference Commands / クイックリファレンスコマンド

```bash
# Setup project management
bash docs/scripts/create-labels.sh
bash docs/scripts/create-milestones.sh

# Generate progress report
bash docs/scripts/progress-report.sh

# List issues by Epic
gh issue list --label "epic:combat" --state all

# List high priority open issues
gh issue list --label "priority:high" --state open

# List issues in progress
gh issue list --label "status:in-progress"

# View milestone progress
gh api repos/7474/SRC/milestones

# Create issue (interactive)
gh issue create

# Create PR (interactive)
gh pr create

# Run tests
cd SRC.Sharp
dotnet test

# Build project
cd SRC.Sharp/SRCSharpForm
dotnet build
```

## Resources / リソース

- **Documentation**: `docs/` directory
- **Original SRC**: `SRC/SRC_20121125/` (UTF-8 converted VB6)
- **Auto-converted .NET**: `SRC.NET/` (reference only)
- **C# Implementation**: `SRC.Sharp/`
- **Tests**: `SRC.Sharp/SRCCoreTests/`
- **Issue Templates**: `.github/ISSUE_TEMPLATE/`
- **Scripts**: `docs/scripts/`

## Contact / 連絡先

- **Repository Owner**: @7474
- **Issue Discussions**: GitHub Issues
- **General Discussions**: GitHub Discussions

---

**Version**: 1.0.0
**Last Updated**: 2026-02-19
**Maintainer**: GitHub Copilot Agent System
