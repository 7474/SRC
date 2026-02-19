# Copilot Quick Reference / クイックリファレンス

このファイルは、GitHub Copilot Agentが SRC# 移植プロジェクトを運用するための簡易リファレンスです。

## 📋 Migration Plan Documentation

### Start Here / ここから始める
1. **[docs/quick-start.md](../../docs/quick-start.md)** - プロジェクト概要とクイックスタート
2. **[docs/migration-plan.md](../../docs/migration-plan.md)** - 8つのEpicと全体戦略
3. **[docs/issue-breakdown.md](../../docs/issue-breakdown.md)** - 約70個の具体的Issue
4. **[agent-instructions.md](./agent-instructions.md)** - 詳細な運用手順

## 🎯 4 Agent Roles

### 1. Issue Creation Agent
- **Input**: `docs/issue-breakdown.md`
- **Output**: GitHub Issues with proper labels/milestones
- **Templates**: `.github/ISSUE_TEMPLATE/*.md`

### 2. Implementation Agent
- **Input**: GitHub Issue with TODO reference
- **Output**: PR ≤1000 lines with tests
- **Test**: `cd SRC.Sharp && dotnet test`

### 3. Review Agent
- **Check**: Size, tests, docs, no side effects
- **Reference**: `docs/migration-plan.md` for alignment

### 4. Progress Tracking Agent
- **Run**: `bash docs/scripts/progress-report.sh`
- **Report**: Epic completion, milestone progress, blockers

## 🏷️ Label Quick Guide

**Epic** (8): `combat`, `unit-pilot`, `ui`, `events`, `data`, `vb6-legacy`, `performance`, `bugfix`
**Priority** (4): `critical`, `high`, `medium`, `low`
**Type** (6): `epic`, `feature`, `enhancement`, `bugfix`, `refactor`, `docs`
**Size** (5): `xs` (~100), `s` (200-400), `m` (400-700), `l` (700-1000), `xl` (1000+)
**Status** (4): `blocked`, `in-progress`, `review`, `on-hold`

## 🎯 Milestones

- **Phase 1** (v3.1.0, Q2'26): Combat + Unit/Pilot basics
- **Phase 2** (v3.2.0, Q3'26): Advanced combat + UI + Events
- **Phase 3** (v3.3.0, Q4'26): Data + Bugfix
- **Phase 4** (v3.4.0, Q1'27): VB6 Legacy + Performance

## 📂 Code Locations by Epic

| Epic | Location |
|------|----------|
| 1. Combat | `Units/Unit.attackcheck.cs`, `Unit.attack.cs`, `Command.attack.cs` |
| 2. Unit/Pilot | `Units/Unit.lookup.cs`, `Pilots/Pilot.skill.cs`, `Unit.ability.cs` |
| 3. GUI/UI | `SRCSharpForm/Forms/Main.gui*.cs`, `UIInterface/*.cs` |
| 4. Events | `CmdDatas/Commands/**/*.cs`, `Events/Event.*.cs` |
| 5. Data | `SRC.save.cs`, `SRC.config.cs`, `Config/*.cs` |
| 6. VB6 Legacy | `VB/*.cs`, `Lib/FileSystem.cs` |
| 7. Performance | `Sound.cs`, various |
| 8. Bugfix | Various per issue |

All paths relative to `SRC.Sharp/SRCCore/`

## ⚡ Quick Commands

```bash
# Setup
bash docs/scripts/create-labels.sh
bash docs/scripts/create-milestones.sh

# Progress
bash docs/scripts/progress-report.sh
gh issue list --label "epic:combat" --state all

# Development
cd SRC.Sharp
dotnet test
dotnet build

# Issue Management
gh issue create                          # Create new issue
gh issue list --label "status:blocked"  # List blocked
gh pr create                             # Create PR
```

## ✅ Issue Creation Checklist

- [ ] Reference `docs/issue-breakdown.md` for content
- [ ] Use appropriate template (epic/feature/bugfix)
- [ ] Apply 4 labels: epic, priority, type, size
- [ ] Set milestone (Phase 1-4)
- [ ] Link to parent Epic: "Related to #XXX"

## ✅ Implementation Checklist

- [ ] Read issue + TODO comment + surrounding code
- [ ] Keep PR ≤1000 lines
- [ ] Add tests in `SRCCoreTests/`
- [ ] Run tests: `dotnet test`
- [ ] Update docs if API changed
- [ ] Commit: `[Epic X.Y] Description (Closes #XXX)`
- [ ] PR description: `Closes #XXX`

## ✅ Review Checklist

- [ ] PR size ≤1000 lines (or justified)
- [ ] Tests added/updated
- [ ] Documentation updated (if needed)
- [ ] No unrelated changes
- [ ] TODO comment addressed
- [ ] No regression (existing tests pass)
- [ ] Code quality + consistency

## 🚨 Common Issues

| Issue | Solution |
|-------|----------|
| PR > 1000 lines | Split into smaller issues (unless cross-cutting) |
| Test failures | Check for side effects, update tests |
| Unclear TODO | Check `SRC/SRC_20121125/` or `SRC.NET/`, ask in issue |
| Dependency blocked | Add `status:blocked`, work on other issues |
| Scope creep | Stick to TODO, create new issues for discoveries |

## 📊 Epic Summary

| # | Epic | Issues | Priority | Phase |
|---|------|--------|----------|-------|
| 1 | Combat System | 15-20 | High | 1-2 |
| 2 | Unit/Pilot | 12-15 | High | 1 |
| 3 | GUI/UI | 8-10 | Medium | 2 |
| 4 | Events/Commands | 10-12 | Medium | 2 |
| 5 | Data Management | 5-7 | Medium | 3 |
| 6 | VB6 Legacy | 5-8 | Low | 4 |
| 7 | Performance | 5-7 | Low | 4 |
| 8 | Bug Fixes | 8-10 | Medium | 3 |

**Total**: ~70 issues, 18-25k line changes, 12-18 months

## 📝 Commit Message Format

```
[Epic X.Y] Brief description (Closes #IssueNumber)

Example:
[Epic 1.1] Implement dodge attack in Unit.attackcheck.cs (Closes #123)
```

## 🔗 Key Links

- **Docs Index**: [docs/README.md](../../docs/README.md)
- **Quick Start**: [docs/quick-start.md](../../docs/quick-start.md)
- **Full Instructions**: [agent-instructions.md](./agent-instructions.md)
- **Issue Breakdown**: [docs/issue-breakdown.md](../../docs/issue-breakdown.md)

---

**For detailed instructions, see [agent-instructions.md](./agent-instructions.md)**
