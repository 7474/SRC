# Copilot Quick Reference / クイックリファレンス

このファイルは、GitHub Copilot Agentが SRC# 移植プロジェクトを運用するための簡易リファレンスです。

## 📋 Migration Plan Documentation

### Start Here / ここから始める
1. **[docs/porting/migration-plan.md](../../docs/porting/migration-plan.md)** - 全体戦略と残存課題
2. **[docs/porting/porting-quality-plan.md](../../docs/porting/porting-quality-plan.md)** - 品質検証フェーズのロードマップ
3. **[docs/porting/issue-breakdown.md](../../docs/porting/issue-breakdown.md)** - 過去に定義した約70個のIssue（参考）
4. **[agent-instructions.md](./agent-instructions.md)** - 詳細な運用手順

## 🎯 4 Agent Roles

### 2. Implementation Agent
- **Input**: GitHub Issue with TODO reference
- **Output**: PR ≤1000 lines with tests
- **Test**: `cd SRC.Sharp && dotnet test`

### 3. Review Agent
- **Check**: Size, tests, docs, no side effects
- **Reference**: `docs/migration-plan.md` for alignment

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
# Development
cd SRC.Sharp
dotnet test
dotnet build

# Issue Management
gh issue create    # Create new issue
gh pr create       # Create PR
```

## ✅ Implementation Checklist

- [ ] Read issue + TODO comment + surrounding code
- [ ] Keep PR ≤1000 lines
- [ ] Add tests in `SRCCoreTests/`
- [ ] Run tests: `dotnet test`
- [ ] Update docs if API changed
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
- **Porting Docs**: [docs/porting/README.md](../../docs/porting/README.md)
- **Full Instructions**: [agent-instructions.md](./agent-instructions.md)

---

**For detailed instructions, see [agent-instructions.md](./agent-instructions.md)**
