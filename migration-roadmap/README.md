# SRC# Migration Roadmap

This directory contains the comprehensive roadmap for completing the SRC# (Simulation RPG Construction Sharp) C# .NET migration.

## Directory Structure

- [`roadmap.md`](./roadmap.md) - Main migration roadmap with executive summary and strategy
- [`technical-breakdown.md`](./technical-breakdown.md) - Detailed technical analysis and implementation plans
- [`sprint-plan.md`](./sprint-plan.md) - 4-sprint delivery plan with priorities and dependencies
- [`epics/`](./epics/) - Individual Epic documentation (A-H)
- [`templates/`](./templates/) - GitHub issue templates for task creation

## Progress Overview

**Status**: Planning Complete ✅  
**Current Phase**: Ready for Implementation  
**Total Epics**: 8 (A-H)  
**Total Issues**: 26 specific implementation tasks  

### Epic Status

- [ ] **Epic A**: Save/Load System (High Priority)
- [ ] **Epic B**: Game Commands (High Priority)  
- [ ] **Epic C**: Expression System (High Priority)
- [ ] **Epic D**: Configuration System (Medium Priority)
- [ ] **Epic E**: System Libraries (Medium Priority)
- [ ] **Epic F**: Test Infrastructure (Medium Priority)
- [ ] **Epic G**: Legacy Code Cleanup (Low Priority)
- [ ] **Epic H**: UI Platform Support (Low Priority)

## Quick Links

### High Priority Items (Sprint 1-2)
1. [Save/Load System Security](./epics/epic-a-save-load.md#path-normalization-and-security)
2. [AI Weapon Selection Bug](./epics/epic-b-game-commands.md#ai-weapon-selection-bug)
3. [Attack Re-movement](./epics/epic-b-game-commands.md#attack-re-movement-logic)
4. [String Byte Functions](./epics/epic-c-expression-system.md#string-byte-functions)

### Implementation Guidelines

- Focus on core functionality first (Epics A-C)
- Maintain backward compatibility where possible
- Prioritize security and stability
- Test infrastructure improvements in parallel

---

**Related Issue**: #663  
**Created**: Migration Planning Phase  
**Last Updated**: {{ current_date }}