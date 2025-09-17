# Epic G: レガシーコードクリーンアップ

**優先度**: 低  
**スプリント**: 4+  
**予想工数**: 0.5-1日  
**リスクレベル**: 極低（メンテナンス）

## 概要

レガシーコードクリーンアップは、機能に影響を与えることなく、少数の残存VB.NET変換アーティファクトの削除と全体的なコード品質の向上を含みます。実際の残存アーティファクトは数行程度と非常に限定的です。

## Issue・タスク

### G.1: VB.NET Comment Removal
**Priority**: Low  
**Effort**: 0.5 days  
**Sprint**: 4

#### 問題ステートメント
Minimal commented VB.NET conversion artifacts remain in the codebase (approximately few lines), with very limited impact on readability and maintenance.

#### 技術要件
- Identify and catalog remaining VB.NET comment artifacts
- Verify commented code is truly obsolete
- Remove confirmed obsolete comments
- Preserve any comments with historical or reference value

#### 影響ファイル
- Limited files across codebase
- Minimal presence in SRCCore/* directories
- Some legacy artifacts in test files

#### 受け入れ基準
- [ ] VB.NET comment artifacts cataloged
- [ ] Obsolete comments verified and removed
- [ ] Code readability improved
- [ ] No functional code accidentally removed
- [ ] Historical comments preserved where appropriate

#### テスト要件
- Functionality regression testing
- Code review for accidental removals
- Documentation impact assessment
- Build and test verification

---

### G.2: Code Quality Improvements
**Priority**: Low  
**Effort**: 1 day  
**Sprint**: 4

#### 問題ステートメント
Overall code quality can be improved through refactoring, standardization, and modern C# pattern adoption.

#### 技術要件
- Apply modern C# patterns and conventions
- Standardize coding style across files
- Remove code duplication
- Improve variable and method naming

#### 影響ファイル
- Files identified during VB.NET cleanup
- Components with high TODO comment density
- Legacy conversion patterns

#### 受け入れ基準
- [ ] Modern C# patterns applied
- [ ] Coding style standardized
- [ ] Code duplication reduced
- [ ] Naming conventions improved
- [ ] Code quality metrics improved

#### テスト要件
- Functionality regression testing
- Code style validation
- Performance impact assessment
- Code review process

---

### G.3: Documentation Cleanup
**Priority**: Low  
**Effort**: 0.5 days  
**Sprint**: 4

#### 問題ステートメント
Documentation may contain outdated references to VB.NET and conversion process that should be updated or removed.

#### 技術要件
- Review documentation for VB.NET references
- Update outdated conversion information
- Improve code documentation quality
- Standardize documentation format

#### 影響ファイル
- README files
- Code comments and XML documentation
- Developer documentation
- API documentation

#### 受け入れ基準
- [ ] VB.NET references updated or removed
- [ ] Documentation accuracy improved
- [ ] Code documentation enhanced
- [ ] Documentation format standardized
- [ ] Outdated conversion info removed

#### テスト要件
- Documentation accuracy verification
- Link validation
- Format consistency checking
- Content review process

## 実装戦略

### Phase 1: Analysis and Planning
1. Catalog all VB.NET comment artifacts
2. Identify code quality improvement opportunities
3. Plan safe removal strategy

### Phase 2: Safe Removal
1. Remove confirmed obsolete comments
2. Apply code quality improvements
3. Update documentation

### Phase 3: Validation
1. Comprehensive testing
2. Code review
3. Documentation verification

## 依存関係

### 外部依存関係
- Code analysis tools
- Documentation generators
- Style checking tools

### 内部依存関係
- All other epics (cleanup should not interfere)
- Testing infrastructure (Epic F)
- Build and CI systems

## リスク評価

### Very Low Risks
- **Accidental Removal**: Removing functional code by mistake
- **Historical Loss**: Losing important conversion history
- **Style Conflicts**: Introducing inconsistent styles

### 軽減戦略
- Careful code review process
- Preserve important historical comments
- Use automated style checking tools
- Incremental changes with testing

## Technical Implementation Details

### VB.NET Comment Patterns to Remove
```
' VB.NET conversion artifact patterns:
' TODO: Convert from VB.NET
' VB.NET: <original VB code>
' Legacy VB implementation
' Converted from VB.NET on [date]
```

### Code Quality Improvements
```csharp
// Before (VB.NET style)
if (condition == true)
{
    return true;
}
else
{
    return false;
}

// After (Modern C#)
return condition;
```

### Documentation Updates
- Remove "Converted from VB.NET" notices
- Update API documentation
- Standardize XML documentation format
- Remove conversion timestamps

## Automation Tools

### Static Analysis Tools
- Use regex patterns to find VB.NET artifacts
- Code quality analyzers for improvement suggestions
- Documentation linting tools

### Safe Removal Process
```bash
# Example automated detection
grep -r "VB\.NET\|VB.NET" --include="*.cs" SRCCore/
grep -r "TODO.*VB" --include="*.cs" SRCCore/
grep -r "Legacy.*VB" --include="*.cs" SRCCore/
```

## Testing Strategy

### Regression Testing
```
1. Full Test Suite: Run all existing tests
2. Build Verification: Ensure code still compiles
3. Functionality Testing: Verify no features broken
4. Performance Testing: Check for performance regressions
```

### Quality Verification
```
1. Code Review: Manual review of all changes
2. Style Checking: Automated style validation
3. Documentation Review: Content accuracy verification
4. Diff Analysis: Verify only comments/style changed
```

## 完了定義

### Epic完了基準
- [ ] VB.NET comment artifacts removed
- [ ] Code quality improvements applied
- [ ] Documentation updated and cleaned
- [ ] No functionality regressions
- [ ] Code style standardized
- [ ] Build and tests passing
- [ ] Code review completed
- [ ] Historical preservation verified

### 品質ゲート
- Full regression testing passed
- Code review approval
- Build verification successful
- Documentation accuracy confirmed

---

**Epic Owner**: Development Team  
**Code Review**: Required for all changes  
**Related Epics**: All epics (non-interference requirement)  
**GitHub Labels**: `epic:legacy-cleanup`, `priority:low`, `maintenance`, `code-quality`