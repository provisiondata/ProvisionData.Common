# .NET 10.0 Upgrade - Execution Tasks

**Scenario:** Upgrade to .NET 10.0 (LTS)  
**Strategy:** All-At-Once  
**Branch:** upgrade-to-NET10  
**Plan Reference:** `.github/upgrades/plan.md`

---

## Progress Dashboard

**Overall Progress:** 9/9 tasks complete (100%) ![100%](https://progress-bar.xyz/100)

**Phase Status:**
### [?] TASK-002: Establish Pre-Upgrade Baseline *(Completed: 2026-01-26 14:29)*
- [ ] Phase 1: Atomic Framework Upgrade (0/4 complete)
- [ ] Phase 2: Validation & Testing (0/2 complete)
- [ ] Phase 3: Source Control & Documentation (0/1 complete)

**Last Updated:** 2026-01-26 14:27

---

## Task List

### Phase 0: Prerequisites & Preparation

### [?] TASK-001: Verify .NET 10.0 SDK Installation *(Completed: 2026-01-26 14:27)*
**Priority:** High | **Risk:** Low | **Est. Impact:** None

Verify that .NET 10.0 SDK is installed and available on the development machine.

**Actions:**
- [?] (1) Run `dotnet --list-sdks` command
- [?] (2) Verify .NET 10.0.x appears in the SDK list
- [?] (3) If missing, download and install from https://dotnet.microsoft.com/download/dotnet/10.0
- [?] (4) Verify installation successful

**Success Criteria:**
- .NET 10.0 SDK listed in `dotnet --list-sdks` output
- SDK version is 10.0.x or higher

**Reference:** Plan §Phase 0: Preparation

---

### [?] TASK-002: Establish Pre-Upgrade Baseline
**Priority:** High | **Risk:** Low | **Est. Impact:** None

Build and test the solution on current frameworks (.NET 5.0/.NET Framework 4.8) to establish baseline before upgrade.

**Actions:**
- [?] (1) Verify current branch is `upgrade-to-NET10`
- [?] (2) Run `dotnet restore ProvisionData.Common.sln`
- [?] (3) Run `dotnet build ProvisionData.Common.sln`
        Verify: Build succeeds with 0 errors
- [?] (4) Run `dotnet test ProvisionData.Common.sln`
        Verify: All tests pass
- [?] (5) Document baseline test count and results

**Success Criteria:**
- Solution builds successfully on current frameworks
- All unit tests pass
- Baseline metrics documented

**Reference:** Plan §Testing & Validation Strategy > Phase 0: Pre-Upgrade Validation

---

### Phase 1: Atomic Framework Upgrade

### [?] TASK-003: Update ProvisionData.Common.csproj to Multi-Target .NET 10.0 *(Completed: 2026-01-26 14:31)*
**Priority:** High | **Risk:** Low | **Est. Impact:** 1 file modified

Add net10.0 to the TargetFrameworks property while maintaining backward compatibility with net5.0 and net48.

**Actions:**
- [?] (1) Open `source\ProvisionData.Common\ProvisionData.Common.csproj`
- [?] (2) Locate `<TargetFrameworks>net5.0;net48</TargetFrameworks>`
- [?] (3) Change to `<TargetFrameworks>net5.0;net48;net10.0</TargetFrameworks>`
- [?] (4) Save the file

**Success Criteria:**
- TargetFrameworks property value is exactly `net5.0;net48;net10.0`
- File saved without errors

**Reference:** Plan §Project 1: source\ProvisionData.Common\ProvisionData.Common.csproj > Step 1

---

### [?] TASK-004: Update ProvisionData.Common.UnitTests.csproj to .NET 10.0 *(Completed: 2026-01-26 14:34)*
**Priority:** High | **Risk:** Low | **Est. Impact:** 1 file modified

Change the test project from net5.0 to net10.0.

**Actions:**
- [?] (1) Open `tests\ProvisionData.Common.UnitTests\ProvisionData.Common.UnitTests.csproj`
- [?] (2) Locate `<TargetFramework>net5.0</TargetFramework>`
- [?] (3) Change to `<TargetFramework>net10.0</TargetFramework>`
- [?] (4) Save the file

**Success Criteria:**
- TargetFramework property value is exactly `net10.0`
- File saved without errors

**Reference:** Plan §Project 3: tests\ProvisionData.Common.UnitTests\ProvisionData.Common.UnitTests.csproj > Step 1

---

### [?] TASK-005: Update _build.csproj to .NET 10.0 *(Completed: 2026-01-26 14:36)*
**Priority:** High | **Risk:** Low | **Est. Impact:** 1 file modified

Change the build automation project from net5.0 to net10.0.

**Actions:**
- [?] (1) Open `build\_build.csproj`
- [?] (2) Locate `<TargetFramework>net5.0</TargetFramework>`
- [?] (3) Change to `<TargetFramework>net10.0</TargetFramework>`
- [?] (4) Save the file

**Success Criteria:**
- TargetFramework property value is exactly `net10.0`
- File saved without errors

**Reference:** Plan §Project 2: build\_build.csproj > Step 1

---

### [?] TASK-006: Restore Dependencies and Build Solution *(Completed: 2026-01-26 14:38)*
**Priority:** High | **Risk:** Medium | **Est. Impact:** Solution-wide

Restore NuGet packages and build the entire solution with updated target frameworks.

**Actions:**
- [?] (1) Run `dotnet restore ProvisionData.Common.sln --force`
        Verify: Restore completes without errors
- [?] (2) Run `dotnet build ProvisionData.Common.sln --no-restore`
        Verify: Build succeeds with 0 errors
- [?] (3) Check for build warnings (document if any)
- [?] (4) Verify ProvisionData.Common builds for all 3 targets (net5.0, net48, net10.0)
- [?] (5) Verify ProvisionData.Common.UnitTests builds for net10.0
- [?] (6) Verify _build builds for net10.0

**Success Criteria:**
- `dotnet restore` completes with no errors
- `dotnet build` completes with 0 errors
- 0 warnings (or warnings documented and acceptable)
- All 3 projects build successfully
- Multi-targeting works for ProvisionData.Common

**Reference:** Plan §Testing & Validation Strategy > Phase 1: Post-Upgrade Build Validation > Test 1 & 2

---

### Phase 2: Validation & Testing

### [?] TASK-007: Run Unit Tests on .NET 10.0 *(Completed: 2026-01-26 14:40)*
**Priority:** High | **Risk:** Medium | **Est. Impact:** None

Execute all unit tests to verify functionality on .NET 10.0 runtime.

**Actions:**
- [?] (1) Run `dotnet test tests\ProvisionData.Common.UnitTests\ProvisionData.Common.UnitTests.csproj --verbosity normal`
        Verify: All tests pass with 0 failures
- [?] (2) Check for skipped tests (document if any)
- [?] (3) Verify tests run on .NET 10.0 runtime (check test output)
- [?] (4) Compare test count with baseline from TASK-002
- [?] (5) Check test execution time (no significant regression)

**Success Criteria:**
- All unit tests execute successfully
- 0 test failures
- 0 skipped tests (unless intentionally skipped before)
- Test count matches baseline
- Tests run on .NET 10.0 runtime
- Execution time acceptable

**Reference:** Plan §Testing & Validation Strategy > Phase 2: Test Execution & Validation > Test 4

---

### [?] TASK-008: Validate Multi-Targeting and Backward Compatibility *(Completed: 2026-01-26 14:44)*
**Priority:** High | **Risk:** Low | **Est. Impact:** None

Verify that ProvisionData.Common still builds correctly for net5.0 and net48 (backward compatibility).

**Actions:**
- [?] (1) Run `dotnet build source\ProvisionData.Common\ProvisionData.Common.csproj -f net10.0`
        Verify: Build succeeds
- [?] (2) Run `dotnet build source\ProvisionData.Common\ProvisionData.Common.csproj -f net5.0`
        Verify: Build succeeds (regression check)
- [?] (3) Run `dotnet build source\ProvisionData.Common\ProvisionData.Common.csproj -f net48`
        Verify: Build succeeds (regression check)
- [?] (4) Verify output assemblies exist in bin\Debug\net10.0, bin\Debug\net5.0, bin\Debug\net48

**Success Criteria:**
- ProvisionData.Common builds successfully for all 3 target frameworks
- Output assemblies generated for each framework
- No build errors or warnings
- Backward compatibility maintained

**Reference:** Plan §Testing & Validation Strategy > Phase 1: Post-Upgrade Build Validation > Test 3

---

### Phase 3: Source Control & Documentation

### [?] TASK-009: Commit Upgrade Changes *(Completed: 2026-01-26 14:46)*
**Priority:** High | **Risk:** Low | **Est. Impact:** 3 files modified

Commit all project file changes with a clear, conventional commit message.

**Actions:**
- [?] (1) Run `git status` to verify changes
        Expected: 3 modified files (.csproj files)
- [?] (2) Run `git add source\ProvisionData.Common\ProvisionData.Common.csproj`
- [?] (3) Run `git add tests\ProvisionData.Common.UnitTests\ProvisionData.Common.UnitTests.csproj`
- [?] (4) Run `git add build\_build.csproj`
- [?] (5) Run `git commit -m "feat: Upgrade to .NET 10.0

- ProvisionData.Common: Add net10.0 to multi-targeting (net5.0;net48;net10.0)
- ProvisionData.Common.UnitTests: Upgrade from net5.0 to net10.0
- _build: Upgrade from net5.0 to net10.0

All packages compatible, no code changes required.
All tests pass."`
- [?] (6) Verify commit successful
- [?] (7) Note commit hash for reference

**Success Criteria:**
- All 3 project files staged for commit
- Commit message follows conventional format
- Commit completes successfully
- Git history clean with single atomic commit

**Reference:** Plan §Source Control Strategy > Commit Strategy > Commit 1: Atomic Framework Upgrade

---

## Execution Log

_(This section will be updated as tasks are executed)_

**Execution Start:** Not started  
**Current Status:** Ready to begin  
**Last Task Completed:** None  
**Next Task:** TASK-001

---

## Notes

**Important Reminders:**
- This is an All-At-Once upgrade - all project files updated in Phase 1 before validation
- Single atomic commit includes all 3 project file changes
- If any validation fails, DO NOT commit - investigate and resolve first
- Refer to Plan Risk Management section for contingency plans

**Quick Reference:**
- Plan: `.github/upgrades/plan.md`
- Assessment: `.github/upgrades/assessment.md`
- Current Branch: `upgrade-to-NET10`
- Target Framework: .NET 10.0 (LTS)
