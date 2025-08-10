# GitHub Issue Template

Use this template to create individual issues for each Epic task. Copy and customize the sections below for each specific implementation task.

## Issue Title Format
`[Epic X.Y] Brief Task Description`

**Examples:**
- `[Epic A.1] Implement Save/Load Path Normalization and Security`
- `[Epic B.1] Fix AI Weapon Selection Bug`
- `[Epic C.1] Implement String Byte Functions`

## Issue Template

### Epic Information
- **Epic**: [Epic Letter] - [Epic Name]
- **Priority**: [Critical/High/Medium/Low]
- **Sprint**: [Sprint Number]
- **Estimated Effort**: [X days]
- **Risk Level**: [Critical/High/Medium/Low/Very Low]

### Problem Statement
[Describe the specific problem this task addresses]

### Technical Requirements
[List specific technical requirements and implementation details]

#### Files Affected
- `[File path 1]` ([Primary/Secondary/New])
- `[File path 2]` ([Primary/Secondary/New])
- `[File path 3]` ([Primary/Secondary/New])

### Acceptance Criteria
- [ ] [Specific criterion 1]
- [ ] [Specific criterion 2]
- [ ] [Specific criterion 3]
- [ ] [Specific criterion 4]
- [ ] [Specific criterion 5]

### Testing Requirements
- [Testing requirement 1]
- [Testing requirement 2]
- [Testing requirement 3]
- [Testing requirement 4]

### Dependencies
#### External Dependencies
- [External dependency 1]
- [External dependency 2]

#### Internal Dependencies
- [Internal dependency 1]
- [Internal dependency 2]

### Implementation Notes
[Any specific implementation notes, patterns to follow, or architectural considerations]

### Definition of Done
- [ ] Implementation complete
- [ ] All acceptance criteria met
- [ ] Testing requirements satisfied
- [ ] Code review passed
- [ ] Documentation updated (if applicable)
- [ ] No regressions introduced
- [ ] Performance impact assessed

---

## Labels to Apply

### Epic Labels
- `epic:save-load` (Epic A)
- `epic:commands` (Epic B)
- `epic:expressions` (Epic C)
- `epic:configuration` (Epic D)
- `epic:system-libraries` (Epic E)
- `epic:test-infrastructure` (Epic F)
- `epic:legacy-cleanup` (Epic G)
- `epic:ui-platform` (Epic H)

### Priority Labels
- `priority:critical`
- `priority:high`
- `priority:medium`
- `priority:low`

### Sprint Labels
- `sprint:1`
- `sprint:2`
- `sprint:3`
- `sprint:4`
- `sprint:4+`

### Type Labels
- `type:bug` (for bug fixes)
- `type:feature` (for new features)
- `type:enhancement` (for improvements)
- `type:maintenance` (for cleanup/refactoring)

### Area Labels
- `area:security`
- `area:performance`
- `area:ui`
- `area:ai`
- `area:testing`
- `area:cross-platform`
- `area:accessibility`

### Size Labels
- `size:xs` (< 0.5 days)
- `size:s` (0.5-1 days)
- `size:m` (1-3 days)
- `size:l` (3-5 days)
- `size:xl` (5+ days)

---

## Example Issue

### [Epic A.1] Implement Save/Load Path Normalization and Security

#### Epic Information
- **Epic**: A - Save/Load System
- **Priority**: Critical
- **Sprint**: 1
- **Estimated Effort**: 3 days
- **Risk Level**: Critical (Security)

#### Problem Statement
Current save/load operations lack proper path validation, creating potential security vulnerabilities through path traversal attacks. Users could potentially access files outside the intended save directory.

#### Technical Requirements
- Implement path normalization for all file operations
- Add security validation to prevent directory traversal
- Validate file extensions and content types
- Add proper error handling for invalid paths
- Ensure cross-platform path handling compatibility

#### Files Affected
- `SRCCore/SRC.cs` (Primary)
- `SRCCore/IO/FileOperations.cs` (Secondary)
- `SRCCore/Security/PathValidator.cs` (New)

#### Acceptance Criteria
- [ ] All file paths properly normalized before use
- [ ] Path traversal attempts blocked and logged
- [ ] File extension validation implemented
- [ ] Security tests passing
- [ ] Backwards compatibility maintained
- [ ] Cross-platform path handling working

#### Testing Requirements
- Security penetration testing with malicious paths
- Path traversal attack simulation
- Cross-platform path handling validation
- Performance impact assessment
- Backwards compatibility testing

#### Dependencies
##### External Dependencies
- Security testing tools
- Cross-platform file system testing
- Path validation libraries

##### Internal Dependencies
- Logging system (Epic E)
- Configuration system (Epic D)
- Error handling utilities

#### Implementation Notes
- Use System.IO.Path.GetFullPath() for normalization
- Implement allowlist approach for valid directories
- Log all security violations for monitoring
- Maintain performance for legitimate file operations
- Follow OWASP guidelines for file upload security

#### Definition of Done
- [ ] Implementation complete
- [ ] All acceptance criteria met
- [ ] Security testing passed
- [ ] Performance benchmarks maintained
- [ ] Code review passed
- [ ] Documentation updated
- [ ] No regressions in save/load functionality

**Labels**: `epic:save-load`, `priority:critical`, `sprint:1`, `type:enhancement`, `area:security`, `size:m`

---

This template should be used to create all 26 individual issues identified in the migration roadmap. Each Epic document contains the detailed information needed to populate these templates.