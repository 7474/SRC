# Copilot Agent Instructions for SRC# Migration

このファイルは、GitHub Copilot Agentが SRC# 移植完了プロジェクトを運用するための指示書です。

## 🚀 Self-Contained Operation / 自己完結型運用

**重要**: このプロジェクトは、GitHub ProjectsやWebUIのセットアップなしに、Copilotへの指示のみで作業を進められるよう設計されています。

**Important**: This project is designed to work with Copilot instructions alone, without requiring GitHub Projects or Web UI setup.

### 必要な情報源 / Required Information Sources

Copilotは以下の情報のみで自律的に動作します：

Copilot operates autonomously using only:

1. **`docs/porting/migration-plan.md`** - 8 Epics and overall migration strategy
2. **`docs/porting/issue-breakdown.md`** - Historical issue definitions (~70 issues, for reference)
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

### 2. Implementation Agent / 実装エージェント

**Purpose**: Implement features and fix bugs according to issues.

**Instructions**:
- Before starting, read:
  - The specific issue description
  - Related TODO comments in the codebase
  - Surrounding code context
  - `docs/porting/migration-plan.md` for the overall context
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
- Reference `docs/porting/migration-plan.md` to ensure changes align with overall strategy

## Key Files Reference / 主要ファイル参照

### Documentation / ドキュメント
- `docs/porting/migration-plan.md` - Overall strategy and remaining tasks
- `docs/porting/porting-quality-plan.md` - Quality verification phase roadmap
- `docs/porting/issue-breakdown.md` - Historical issue definitions (~70 issues)
- `docs/README.md` - Documentation index

### Templates / テンプレート
- `.github/ISSUE_TEMPLATE/epic-template.md` - For Epic issues (8 total)
- `.github/ISSUE_TEMPLATE/feature-template.md` - For feature implementation
- `.github/ISSUE_TEMPLATE/bugfix-template.md` - For bug fixes


## Common Tasks / 共通タスク

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

## Workflow Example / ワークフロー例

### Scenario: Implementing Issue 1.1 (Dodge Attack)

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

## Best Practices / ベストプラクティス

### For All Agents
- Always reference documentation in `docs/`
- Follow the established label system
- Maintain bilingual (Japanese/English) communication
- Keep changes focused and minimal
- Test thoroughly before marking complete

### For Implementation
- Read TODO comment and surrounding code first
- Keep PR diff ≤1000 lines
- Add comprehensive tests
- Run existing tests to prevent regression
- Update documentation for public APIs

### For Review
- Check alignment with migration plan
- Verify PR size constraints
- Ensure tests are comprehensive
- Look for unintended side effects
- Confirm documentation is updated

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

## Contact / 連絡先

- **Repository Owner**: @7474
- **Issue Discussions**: GitHub Issues
- **General Discussions**: GitHub Discussions

---

**Version**: 1.0.0
**Last Updated**: 2026-02-19
**Maintainer**: GitHub Copilot Agent System
