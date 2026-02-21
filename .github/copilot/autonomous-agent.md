# Autonomous Migration Agent / 自律移植エージェント

このファイルは、「移植を進行してください」という単一の指示で自律的に作業を進めるためのCopilot設定です。

This file contains Copilot configuration for autonomous migration progress with a single "proceed with migration" instruction.

## 🤖 Single Command Operation / 単一コマンド運用

### 使用方法 / Usage

#### 移植作業を進める / Proceed with migration work

```
@copilot 移植を進行してください
```

または / or

```
@copilot Proceed with the migration
```

これだけで、Copilotが自律的に次のタスクを選択し、実行します。

This single command allows Copilot to autonomously select and execute the next task.

#### 進行状況を更新する / Update progress status

```
@copilot 進捗を更新してください
```

または / or

```
@copilot Update the progress
```

これだけで、Copilotが現在の進行状況を評価し、ドキュメントとIssueの状態を最新の実態に合わせて更新します。

This single command allows Copilot to assess the current state and update documents and issue statuses to reflect reality.

#### ユニットテストを補完する / Complete unit tests

> **🤖 自動化済み / Automated**: このタスクは **GitHub Agentic Workflow** によって毎週月曜日に自動実行されます。  
> **🤖 Automated**: This task runs automatically every Monday via **GitHub Agentic Workflow**.  
> ワークフロー: `.github/workflows/complete-unit-tests.md` / Workflow: `.github/workflows/complete-unit-tests.md`

手動で実行する場合 / To run manually:

```
@copilot ユニットテストを補完してください
```

または / or

```
@copilot Complete the unit tests
```

または、GitHub ActionsのUI から手動トリガーも可能です：  
Or trigger manually from GitHub Actions UI:

```
gh workflow run complete-unit-tests.lock.yml
```

これだけで、Copilotが移植済みコマンドのうちテストが不足しているものを特定し、
ヘルプの記載（`SRC.Sharp.Help/src/`）を期待値としたユニットテストを追加します。

This command allows Copilot to identify implemented commands lacking test coverage,
then add unit tests using the help documentation (`SRC.Sharp.Help/src/`) as the expected behavior.

---

## 🎯 Autonomous Operation Protocol / 自律運用プロトコル

When instructed to "proceed with migration" (移植を進行してください), Copilot should follow this protocol:

### Phase 1: Assess Current State / 現状評価

1. **Check existing issues**
   ```bash
   gh issue list --state all --json number,title,state,labels
   ```

2. **Identify completed work**
   - Scan for closed issues
   - Identify implemented TODOs

3. **Find next priority task**
   - Check `docs/porting/issue-breakdown.md` for issue order
   - Priority: Epic 1 → Epic 2 → ... → Epic 8
   - Within Epic: Issue X.1 → X.2 → X.3 ...

### Phase 2: Select Next Task / 次タスク選択

**Decision Tree**:

```
IF no issues exist THEN
  → Create Epic 1 issue (#1)
  
ELSE IF Epic parent issues incomplete THEN
  → Create next Epic issue (Epic 1-8)
  
ELSE IF Phase 1 issues incomplete THEN
  → Select highest priority incomplete Phase 1 issue
  
ELSE IF any TODO exists in codebase THEN
  → Select TODO from highest priority Epic
  → Create issue if needed
  → Implement the feature
  
ELSE
  → Report "Migration complete!"
```

**Selection Criteria** (in order):
1. **Blocker status**: Unblock any `status:blocked` issues first
2. **Phase**: Prioritize current phase (Phase 1 → 2 → 3 → 4)
3. **Epic priority**: Epic 1 (Combat) and Epic 2 (Unit/Pilot) first
4. **Issue priority**: `priority:critical` > `high` > `medium` > `low`
5. **Dependencies**: Ensure prerequisite issues are complete

### Phase 3: Execute Task / タスク実行

Based on task type:

#### A. Epic Issue Creation

```
Action: Create Epic issue
Steps:
1. Use .github/ISSUE_TEMPLATE/epic-template.md
2. Fill with content from docs/porting/issue-breakdown.md
3. Apply labels: type:epic, epic:[name], priority:[level]
4. Set milestone: Phase X
5. Report issue number created
```

#### B. Feature Issue Creation

```
Action: Create feature issue
Steps:
1. Use .github/ISSUE_TEMPLATE/feature-template.md
2. Reference docs/porting/issue-breakdown.md for details
3. Apply labels: epic:[name], priority:[level], type:feature, size:[xs-xl]
4. Link to parent Epic: "Related to #XXX"
5. Set milestone: Phase X
6. Report issue number created
```

#### C. Feature Implementation

```
Action: Implement feature
Steps:
1. Locate TODO comment in codebase
2. Read surrounding code for context
3. Implement solution (keep changes minimal)
4. Add tests in SRCCoreTests/
5. Run tests: cd SRC.Sharp && dotnet test
6. Create PR with format: "[Epic X.Y] Description (Closes #IssueNum)"
7. Ensure PR diff ≤1000 lines
8. Report: Issue #, PR #, test results
```

#### D. Bug Fix

```
Action: Fix bug
Steps:
1. Reproduce the bug
2. Identify root cause
3. Implement minimal fix
4. Add regression test
5. Run all tests
6. Create PR with format: "[Bug Fix] Description (Closes #IssueNum)"
7. Report: Issue #, PR #, test results
```

#### E. Unit Test Completion / ユニットテスト補完

```
Action: Complete unit tests for implemented commands
Steps:
1. List all command classes under SRC.Sharp/SRCCore/CmdDatas/Commands/
2. Check existing test files under SRC.Sharp/SRCCoreTests/CmdDatas/
3. Identify commands that have no test coverage
4. For each untested command:
   a. Read help doc: SRC.Sharp.Help/src/[コマンド名]コマンド.md
   b. Note expected behavior described in "解説" section
   c. Note the examples in "例" section
   d. Write tests that verify the behavior described in help
   e. If implementation differs from help, report the discrepancy
5. Add tests to appropriate file in SRCCoreTests/CmdDatas/
   - Use existing file if related commands already have tests there
   - Create new [CommandGroup]CmdTests.cs file if needed
6. Run tests: cd SRC.Sharp && dotnet test SRCCoreTests/SRCCoreTests.csproj
7. Report: commands tested, discrepancies found, test results
```

**Test Writing Guidelines / テスト作成ガイドライン**:
- Test file: `SRC.Sharp/SRCCoreTests/CmdDatas/`
- Pattern: Follow existing tests in `VariableCmdTests.cs`, `ControlCmdTests.cs`, `SwitchDoLoopCmdTests.cs`
- Helper infrastructure:
  - `CreateSrc()` - creates SRC with MockGUI
  - `BuildEvent(src, lines...)` - creates event command array from text lines
  - `RunEvent(src, cmds)` - simulates event execution
- Expected behavior source: `SRC.Sharp.Help/src/[コマンド名]コマンド.md`
- Test naming: `[CmdName]Cmd_[Condition]_[ExpectedResult]()`
- Each test must have a comment citing the relevant help section

**Discrepancy Reporting / 齟齬の報告**:
When the implementation differs from help documentation:
```
⚠️ 齟齬発見: [CommandName]

ヘルプの記載:
  [help documentation text]

実装の動作:
  [actual behavior description]

テストの扱い:
  ヘルプを正として実装を修正し、修正後にヘルプの記載に沿ったテストを作成する
```

### Phase 4: Report Progress / 進捗報告

After completing task, automatically report:

```
✅ Task Completed

Task: [Description]
Issue: #XXX
PR: #YYY (if applicable)
Files Changed: X files, Y lines
Tests: Z passed

Next Recommended Task: [Auto-selected next task]

To continue: @copilot 移植を進行してください
```

---

## 🔄 Progress Update Protocol / 進捗更新プロトコル

When instructed to "update progress" (進捗を更新してください), Copilot should follow this protocol:

### Step 1: Assess Current State / 現状評価

1. **Collect issue statistics**
   ```bash
   gh issue list --state all --json number,title,state,labels,milestone
   ```

2. **Check recently closed issues**
   - Issues closed since last update
   - PRs merged since last update

3. **Identify state changes**
   - Issues newly opened or closed
   - Labels or milestone assignments changed
   - New TODOs resolved in codebase

### Step 2: Update Documents / ドキュメント更新

1. **Update `docs/porting/migration-plan.md`** if progress status has changed:
   - Mark completed items
   - Add notes on newly discovered issues or blockers
   - Adjust timeline estimates if needed

2. **Update issue statuses**
   - Add/remove `status:in-progress`, `status:blocked`, etc. as appropriate
   - Link related issues discovered during work

3. **Sync pre-existing issues**
   - Check if older issues (e.g., #162, #172) have been addressed
   - Note any overlap with Epic tasks

### Step 3: Generate Progress Report / 進捗レポート生成

```
📊 Progress Update (YYYY-MM-DD)

## Changes Since Last Update
- Issues closed: #XXX, #YYY
- PRs merged: #ZZZ
- New TODOs resolved: N

## Current State by Epic
- Epic 1 (Combat): X/15 complete (Y%)
- Epic 2 (Unit/Pilot): X/12 complete (Y%)
- ...

## Milestone Status
- Phase 1 (v3.1.0): X/27 complete (Y%)

## Updated Items
- [List of documents or issues updated]

To continue migration: @copilot 移植を進行してください
To update again: @copilot 進捗を更新してください
```

---

## 🧪 Unit Test Completion Protocol / ユニットテスト補完プロトコル

When instructed to "complete unit tests" (ユニットテストを補完してください), Copilot should follow this protocol:

### Step 1: Identify Untested Commands / 未テストコマンドの特定

1. **List all command implementations**
   ```bash
   find SRC.Sharp/SRCCore/CmdDatas/Commands -name "*.cs" | sort
   ```

2. **List existing test coverage**
   ```bash
   ls SRC.Sharp/SRCCoreTests/CmdDatas/
   ```

3. **Build coverage matrix**
   - For each command class, determine if it has corresponding tests
   - Priority: commands used most frequently in SRC scenarios
   - Skip: NotImplementedCmd, NotSupportedCmd, NopCmd (no logic to test)

### Step 2: Read Help Documentation / ヘルプドキュメントの参照

For each untested command:

1. **Find help file**: `SRC.Sharp.Help/src/[コマンド名]コマンド.md`
2. **Extract expected behavior** from the following sections:
   - 書式 (Format/Syntax)
   - 解説 (Description/Explanation)
   - 例 (Examples)
3. **Note special cases** mentioned in the help text
4. **Check for discrepancies** between help and implementation

### Step 3: Write Tests / テストの作成

For each command, write tests verifying:

1. **Normal behavior** - the main use case from "解説"
2. **Boundary conditions** - edge cases mentioned in help
3. **Error handling** - invalid argument counts, missing required partners (e.g., missing EndSw)
4. **Special modes** - optional parameters and their effects

**Required test cases per command**:
- At least 1 positive test (normal operation)
- At least 1 error test (invalid arguments or missing required structure)
- Additional tests for each distinct behavior described in help

### Step 4: Fix Implementation or Report Discrepancies / 実装の修正または齟齬の報告

If the implementation differs from help documentation:

**原則**: ヘルプを正として実装を修正してください。

```
⚠️ 齟齬発見 / Discrepancy Found: [CommandName]コマンド

ヘルプの記載 / Help Description:
  [relevant text from help]

実装の動作 / Actual Behavior:
  [description of what the code actually does]

対応方針 / Action:
  ヘルプを正として実装を修正する。修正後にヘルプの記載に沿ったテストを作成する。
  Fix implementation to match help, then write tests based on the corrected behavior.
```

修正が困難な場合（意図的な差異、後方互換性の問題など）は、その理由を明記した上で次のいずれかを選択:
- ヘルプを実装に合わせて更新する / Update help to match implementation (if intentional)
- 現状維持で別Issueとして記録する / Keep as-is and create a separate issue

### Step 5: Validate / 検証

```bash
cd SRC.Sharp && dotnet test SRCCoreTests/SRCCoreTests.csproj
```

### Step 6: Report Results / 結果報告

```
🧪 Unit Test Completion Report (YYYY-MM-DD)

## Tests Added
- [CommandName]Cmd: N test cases added (SwitchDoLoopCmdTests.cs)
- [CommandName]Cmd: N test cases added ([File].cs)

## Coverage Summary
- Previously tested: X commands
- Newly tested: Y commands
- Still untested: Z commands (list)
- Total tests: N passed

## Discrepancies Found
- [CommandName]: [brief description of discrepancy]
  → Reported to: [issue number or PR comment]

## Still Untested Commands
(Commands requiring UI interaction or complex setup - deferred)
- [CommandName]: Reason

To continue adding tests: @copilot ユニットテストを補完してください
To proceed with migration: @copilot 移植を進行してください
```

---

## 📋 Auto-Selection Algorithm / 自動選択アルゴリズム

### Priority Matrix

| Condition | Priority | Action |
|-----------|----------|--------|
| No Epic issues exist | P0 | Create Epic 1 |
| Epic X exists, no child issues | P0 | Create first issue in Epic X |
| Issue marked `status:blocked` | P0 | Investigate blocker |
| `priority:critical` open issue | P1 | Work on critical issue |
| Phase 1, `priority:high` issue | P2 | Work on high priority Phase 1 |
| Next sequential issue in current Epic | P3 | Continue Epic progression |
| TODO in high-priority file | P4 | Create issue + implement |
| Unimplemented TODO exists | P5 | Create issue |
| All issues complete | - | Report completion |

### Code Location Priority

When searching for TODOs, prioritize:

1. **Epic 1 (Combat)**:
   - `SRC.Sharp/SRCCore/Units/Unit.attackcheck.cs`
   - `SRC.Sharp/SRCCore/Units/Unit.attack.cs`
   - `SRC.Sharp/SRCCore/Commands/Command.attack.cs`

2. **Epic 2 (Unit/Pilot)**:
   - `SRC.Sharp/SRCCore/Units/Unit.lookup.cs`
   - `SRC.Sharp/SRCCore/Pilots/Pilot.skill.cs`
   - `SRC.Sharp/SRCCore/Units/Unit.ability.cs`

3. **Epic 3 (GUI/UI)**:
   - `SRC.Sharp/SRCSharpForm/Forms/Main.gui*.cs`
   - `SRC.Sharp/SRCCore/UIInterface/*.cs`

4. **Epic 4-8**: Per `docs/porting/issue-breakdown.md` order

### Dependency Resolution

Before implementing an issue, check:

```python
# Pseudo-code for dependency checking
def can_implement(issue):
    # Check if prerequisite issues are complete
    prerequisites = get_prerequisites(issue)
    for prereq in prerequisites:
        if not is_complete(prereq):
            return False, f"Blocked by #{prereq}"
    
    # Check if required files/methods exist
    dependencies = get_code_dependencies(issue)
    for dep in dependencies:
        if not exists(dep):
            return False, f"Missing dependency: {dep}"
    
    return True, "Ready to implement"
```

---

## 🔄 Continuous Operation Mode / 連続運用モード

For fully autonomous operation, use:

```
@copilot 移植を進行してください。次のタスクも自動的に選択して実行してください。
```

This enables Copilot to:
1. Complete current task
2. Auto-select next task
3. Execute next task
4. Repeat until blocked or complete

**Stop Conditions**:
- Manual interruption
- Encounters blocker that needs human decision
- All TODOs completed
- Test failures requiring investigation

---

## 📊 Progress Tracking / 進捗追跡

Copilot automatically tracks:

### Metrics Collected
- Issues created per Epic
- Issues completed per Epic
- Lines of code changed
- Tests added
- PRs merged
- TODOs resolved

### Auto-Generated Reports

Every 5 tasks completed, auto-generate:

```markdown
📈 Migration Progress Report

## Summary
- Total Issues: X created, Y complete (Z%)
- Current Phase: Phase N
- Current Epic: Epic X (Y% complete)

## This Session
- Tasks Completed: 5
- Issues Created: #A, #B, #C
- PRs Merged: #D, #E
- Lines Changed: +XXX -YYY
- Tests Added: ZZ

## Next Focus
- Epic X: N issues remaining
- Priority: [Next task description]
- Estimated: M hours

## Blockers
[Any issues marked status:blocked]
```

---

## 🛠️ Self-Correction Protocol / 自己修正プロトコル

If tests fail or issues arise:

### Auto-Recovery Steps

1. **Test Failure**:
   ```
   Action: Analyze failure
   - Read test output
   - Identify failing test
   - Check if code change caused failure
   - If yes: Revert and try alternative approach
   - If no: Mark as blocker for human review
   ```

2. **Build Failure**:
   ```
   Action: Fix build
   - Read compiler errors
   - Fix syntax/reference errors
   - Re-run build
   - If still fails: Mark as blocker
   ```

3. **PR Too Large**:
   ```
   Action: Split PR
   - If diff >1000 lines: Split into 2+ PRs
   - Create sub-issues
   - Implement in sequence
   ```

4. **Unclear TODO**:
   ```
   Action: Research + Document
   - Check SRC/SRC_20121125/ (original VB6)
   - Check SRC.NET/ (auto-converted)
   - Document findings in issue
   - Attempt implementation or mark as needs-discussion
   ```

---

## 🎓 Learning Mode / 学習モード

Copilot improves over time by:

### Pattern Recognition

- **Success patterns**: Remember approaches that worked
- **Failure patterns**: Avoid approaches that failed
- **Code style**: Match existing code patterns
- **Test patterns**: Follow established test structure

### Knowledge Base

Build understanding of:
- SRC game engine architecture
- VB6 to C# conversion patterns
- Common TODO resolution approaches
- Test patterns per component

---

## 📝 Example Autonomous Sessions / 自律セッション例

### Session 1: Bootstrap

```
Human: @copilot 移植を進行してください

Copilot:
✅ Assessment Complete
- Current State: No issues exist
- Next Action: Create Epic 1 issue

Creating Epic 1: 戦闘システム完成...
✅ Created issue #1

Next Task: Create Epic 2 issue
Continue? Y/N
```

### Session 2: Implementation

```
Human: @copilot 移植を進行してください

Copilot:
✅ Assessment Complete
- Current State: 8 Epics created, 15 feature issues created
- Next Task: Issue #9 - Unit.attackcheck.cs: 回避攻撃の実装

Implementing IsDodgeAttack() method...
- Located TODO at line 234
- Implemented method (45 lines)
- Added tests (8 test cases)
- All tests passed: 160/160 ✓

✅ Created PR #2: "[Epic 1.1] Implement dodge attack (Closes #9)"
- Diff: 125 lines (+100 -25)
- Tests: All passing

Next Task: Issue #10 - Unit.attackcheck.cs: 受け流し攻撃の実装
Continue? Y/N
```

### Session 3: Full Auto

```
Human: @copilot 移植を進行してください。次のタスクも自動的に選択して実行してください。

Copilot:
🤖 Autonomous Mode Activated

Task 1/5: Issue #9 - Dodge Attack
✅ Complete (PR #2)

Task 2/5: Issue #10 - Parry Attack
✅ Complete (PR #3)

Task 3/5: Issue #11 - Dummy Attack
✅ Complete (PR #4)

Task 4/5: Issue #12 - Shield Defense
⚠️ Test failure detected
- Analyzing failure...
- Root cause: Null reference in edge case
- Fixing...
✅ Fixed and retested
✅ Complete (PR #5)

Task 5/5: Issue #13 - Additional Attack
✅ Complete (PR #6)

📊 Session Summary:
- Tasks Completed: 5
- PRs Created: 5
- Lines Changed: +1,234 -567
- Tests Added: 42
- All Tests Passing: ✓

Epic 1 Progress: 5/15 (33%)
Phase 1 Progress: 5/27 (19%)

Next Recommended: Continue with Issue #14
Command: @copilot 移植を進行してください
```

---

## 🚦 Traffic Light System / 信号システム

Copilot uses color coding:

- 🟢 **Green**: Ready to proceed automatically
- 🟡 **Yellow**: Can proceed but needs confirmation
- 🔴 **Red**: Blocked, needs human intervention

### Status Indicators

```
🟢 Ready
- All prerequisites met
- No blockers
- Clear implementation path

🟡 Caution
- Minor uncertainty
- Multiple valid approaches
- Needs design decision

🔴 Blocked
- Missing prerequisite
- Unclear requirements
- Technical blocker
- Test failures unresolved
```

---

## 🎯 Goal-Oriented Behavior / 目標指向動作

Copilot works toward:

### Short-term Goal
Complete current Epic (Epic 1-8 in sequence)

### Medium-term Goal
Complete current Phase (Phase 1-4 in sequence)

### Long-term Goal
Complete all 155+ TODOs and achieve full migration

### Success Criteria
- All Epic issues closed
- All TODOs resolved
- All tests passing
- Zero regressions
- Documentation updated

---

## 💡 Intelligence Features / インテリジェント機能

### Smart Selection
- Analyzes dependencies
- Considers team velocity
- Balances workload across Epics
- Avoids conflicts

### Adaptive Planning
- Adjusts based on actual completion time
- Reorders tasks if blockers appear
- Suggests optimizations

### Quality Assurance
- Runs tests automatically
- Checks code style
- Validates against migration plan
- Ensures ≤1000 line PRs

---

## 🔐 Safety Mechanisms / 安全機構

### Guardrails

1. **Never delete working code** (unless fixing security issue)
2. **Always add tests** for new functionality
3. **Always run tests** before creating PR
4. **Stop if tests fail** and analyze cause
5. **Request human review** for architectural changes

### Rollback Protocol

If something goes wrong:
```
1. Identify last known good state
2. Revert changes
3. Document issue
4. Request human intervention
```

---

## 📖 Reference Documents / 参照ドキュメント

Copilot automatically references:

- `docs/porting/migration-plan.md` - Overall strategy
- `docs/porting/issue-breakdown.md` - Detailed task list
- `.github/ISSUE_TEMPLATE/` - Issue templates
- `SRC/SRC_20121125/` - Original VB6 code
- `SRC.NET/` - Auto-converted .NET code
- Test files in `SRC.Sharp/SRCCoreTests/`

---

## 🎬 Getting Started / 開始方法

### Minimal Start

```
@copilot 移植を進行してください
```

That's it! Copilot handles everything else:
- Assesses current state
- Selects next task
- Executes task
- Reports result
- Suggests next action

### Update Progress

```
@copilot 進捗を更新してください
```

Copilot will:
- Check all issue statuses
- Update documents to reflect current progress
- Generate a progress report
- Suggest next migration step

### Full Autonomous Mode

```
@copilot 移植を完了するまで自律的に作業を進めてください
```

Copilot will work until:
- Migration is complete
- Encounters a blocker
- Manual stop is requested

---

**Version**: 2.2.0 - Fully Autonomous + Progress Update + GitHub Agentic Workflow
**Last Updated**: 2026-02-21
**Mode**: Single-Command Operation + Automated Weekly Schedule
