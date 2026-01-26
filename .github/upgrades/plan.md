# .NET 10.0 Upgrade Plan

## Table of Contents

- [Executive Summary](#executive-summary)
- [Migration Strategy](#migration-strategy)
- [Detailed Dependency Analysis](#detailed-dependency-analysis)
- [Project-by-Project Plans](#project-by-project-plans)
- [Risk Management](#risk-management)
- [Testing & Validation Strategy](#testing--validation-strategy)
- [Complexity & Effort Assessment](#complexity--effort-assessment)
- [Source Control Strategy](#source-control-strategy)
- [Success Criteria](#success-criteria)

---

## Executive Summary

### Scenario Description

Upgrade all projects in the ProvisionData.Common solution from .NET 5.0/.NET Framework 4.8 to .NET 10.0 (Long Term Support).

### Scope

**Projects Affected:** 3
- `build\_build.csproj` - Build automation project
- `source\ProvisionData.Common\ProvisionData.Common.csproj` - Main class library (multi-targeting)
- `tests\ProvisionData.Common.UnitTests\ProvisionData.Common.UnitTests.csproj` - Unit test project

**Current State:**
- Total Lines of Code: 2,597
- Total NuGet Packages: 14 (all compatible)
- All projects are SDK-style
- API Compatibility: 100% (2,342 APIs analyzed, all compatible)

### Target State

All projects targeting .NET 10.0 with maintained multi-targeting support where applicable.

### Selected Strategy

**All-At-Once Strategy** - All projects upgraded simultaneously in a single coordinated operation.

**Rationale:**
- **Small solution** (3 projects)
- **Simple dependency structure** (linear: UnitTests ? Common, _build independent)
- **Zero package updates required** (all 14 packages already compatible with .NET 10.0)
- **Zero API breaking changes** (all APIs compatible)
- **Low complexity** (all projects rated ?? Low difficulty)
- **Excellent compatibility** (100% package and API compatibility)

This solution is ideal for All-At-Once approach - the upgrade can be completed as a single atomic operation with minimal risk.

### Discovered Metrics

- **Project Count:** 3
- **Dependency Depth:** 1 (shallow)
- **High-Risk Projects:** 0
- **Security Vulnerabilities:** 0
- **Package Updates Required:** 0
- **API Breaking Changes:** 0
- **Target Framework Heterogeneity:** Homogeneous (all targeting .NET 10.0)

### Complexity Classification

**Simple** - This solution meets all criteria for simple classification:
- ? ?5 projects (3 total)
- ? Dependency depth ?2 (depth is 1)
- ? No high-risk projects
- ? No security vulnerabilities
- ? No package updates required
- ? No API breaking changes

### Iteration Strategy

**Fast Batch Approach** - Due to simple classification, plan will be generated using 2-3 detail iterations:
1. Foundation iterations (dependency analysis, strategy description, project stubs)
2. Single batch iteration for all project details (all 3 projects together)
3. Final iteration for success criteria and source control

**Expected Total Iterations:** 6-7

### Critical Issues

**None identified** ?

All compatibility checks passed:
- ? All packages compatible with .NET 10.0
- ? All APIs compatible (0 binary/source incompatibilities)
- ? No security vulnerabilities
- ? No blocking issues

### Recommended Approach

All-at-once atomic upgrade with the following sequence:
1. Update all project files simultaneously (ProvisionData.Common adds net10.0 to multi-targeting, others replace net5.0 with net10.0)
2. Restore dependencies
3. Build entire solution
4. Run all unit tests
5. Verify success

## Migration Strategy

### Approach Selection

**Selected: All-At-Once Strategy**

All projects in the solution will be upgraded simultaneously in a single coordinated operation.

### Justification

This solution is ideally suited for All-At-Once migration:

| Criteria | Assessment | Supports All-At-Once? |
|----------|------------|----------------------|
| Solution Size | 3 projects, 2,597 LOC | ? Yes - Small solution |
| Dependency Complexity | 1 level deep, linear | ? Yes - Simple structure |
| Package Updates | 0 required | ? Yes - No compatibility conflicts |
| API Breaking Changes | 0 identified | ? Yes - No code changes needed |
| Security Vulnerabilities | 0 found | ? Yes - No urgent fixes |
| Test Coverage | Unit test project exists | ? Yes - Can validate quickly |
| Current Frameworks | net5.0, net48 (modern) | ? Yes - No legacy barriers |

**Conclusion:** This is a textbook case for All-At-Once strategy - minimal risk, maximum efficiency.

### All-At-Once Strategy Rationale

**Advantages for This Solution:**
- ? **Fastest completion** - Single atomic operation
- ? **No multi-targeting complexity** - Except for ProvisionData.Common which already multi-targets
- ? **Clean dependency resolution** - All projects on same framework simultaneously
- ? **Simple coordination** - All developers upgrade at once
- ? **Minimal risk** - No breaking changes, no package updates

**Challenges (Mitigated):**
- ?? All projects must upgrade together - *Mitigated: Only 3 projects*
- ?? Larger testing surface - *Mitigated: Comprehensive unit test project exists*
- ?? Higher initial risk - *Mitigated: Zero breaking changes identified*

### Dependency-Based Ordering Rationale

While All-At-Once updates all projects simultaneously, the logical order for understanding and validation is:

1. **ProvisionData.Common** - Leaf node, no dependencies, depended on by tests
2. **ProvisionData.Common.UnitTests** - Depends on ProvisionData.Common
3. **_build** - Independent, no dependencies or dependents

This order ensures that when we verify the build:
- The library builds first
- Tests can reference the updated library
- Build automation runs independently

However, all project files will be **updated in the same commit** as a single atomic change.

### Execution Approach

**Sequential Build Validation** (after atomic update):
- Update all project files simultaneously
- Restore dependencies once for entire solution
- Build entire solution (validates all projects together)
- Run all tests (validates functionality)

**Not Parallel Execution:**
While the updates are atomic, the build system will naturally build in dependency order (Common ? UnitTests), which is correct.

### Phase Definitions

**Phase 0: Preparation** (if needed)
- Verify .NET 10.0 SDK installed
- Ensure on correct branch (upgrade-to-NET10)

**Phase 1: Atomic Upgrade**
All projects updated in single operation:
- Update ProvisionData.Common.csproj TargetFrameworks property (add net10.0)
- Update ProvisionData.Common.UnitTests.csproj TargetFramework property (net5.0 ? net10.0)
- Update _build.csproj TargetFramework property (net5.0 ? net10.0)
- Restore dependencies for entire solution
- Build entire solution
- Verify 0 errors, 0 warnings

**Phase 2: Validation**
- Run all unit tests in ProvisionData.Common.UnitTests
- Verify all tests pass
- Confirm solution builds without issues

**Phase 3: Source Control**
- Commit all changes with clear message
- Push to remote (if applicable)

## Detailed Dependency Analysis

### Dependency Graph Summary

The solution has a simple, linear dependency structure with minimal complexity:

```
_build.csproj (independent)
    ?? (no dependencies)

ProvisionData.Common.csproj (leaf node)
    ?? (no dependencies)
    ?? Depended on by: ProvisionData.Common.UnitTests.csproj

ProvisionData.Common.UnitTests.csproj (root node)
    ?? Depends on: ProvisionData.Common.csproj
```

**Dependency Depth:** 1 level (shallow)
**Circular Dependencies:** None
**Independent Projects:** 1 (_build.csproj)

### Project Groupings by Migration Phase

Since this is an All-At-Once strategy, all projects are migrated in a single phase:

**Phase 1: Atomic Upgrade (All Projects)**
- `source\ProvisionData.Common\ProvisionData.Common.csproj` - Multi-targeting library (adds net10.0)
- `build\_build.csproj` - Build automation (replaces net5.0 with net10.0)
- `tests\ProvisionData.Common.UnitTests\ProvisionData.Common.UnitTests.csproj` - Test project (replaces net5.0 with net10.0)

**Rationale for Single-Phase Approach:**
1. **No intermediate states** - All projects updated simultaneously
2. **Simple dependency chain** - Only one project dependency (UnitTests ? Common)
3. **Zero package updates** - No compatibility conflicts to resolve
4. **Zero breaking changes** - No code modifications required

### Critical Path Identification

The critical path is straightforward:

1. **ProvisionData.Common.csproj must be updated** (adds net10.0 to existing net5.0;net48 multi-targeting)
2. **ProvisionData.Common.UnitTests.csproj depends on it** (but updated simultaneously)
3. **_build.csproj is independent** (can be updated in parallel)

However, since we're using All-At-Once strategy, these are all updated in a single atomic operation, so the critical path is effectively the entire upgrade.

### Multi-Targeting Considerations

**ProvisionData.Common.csproj** currently multi-targets `net5.0;net48`. The assessment proposes `net5.0;net48;net10.0`.

**Decision:** Add net10.0 to the existing TargetFrameworks property to maintain backward compatibility:
- **Before:** `<TargetFrameworks>net5.0;net48</TargetFrameworks>`
- **After:** `<TargetFrameworks>net5.0;net48;net10.0</TargetFrameworks>`

This ensures:
- ? Existing .NET Framework 4.8 consumers remain supported
- ? Existing .NET 5.0 consumers remain supported
- ? New .NET 10.0 consumers can use the library
- ? No breaking changes to package consumers

## Project-by-Project Plans

### Project 1: source\ProvisionData.Common\ProvisionData.Common.csproj

**Current State:** net5.0;net48, ClassLibrary, 1,750 LOC, 0 dependencies, 1 dependant
**Target State:** net5.0;net48;net10.0 (multi-targeting)
**Risk Level:** ?? Low

#### Prerequisites

- .NET 10.0 SDK installed and available
- .NET Framework 4.8 Developer Pack installed (for continued net48 support)
- Project currently on branch `upgrade-to-NET10`

#### Migration Steps

**Step 1: Update Project File TargetFrameworks Property**

Modify `source\ProvisionData.Common\ProvisionData.Common.csproj`:

**Change:**
```xml
<TargetFrameworks>net5.0;net48</TargetFrameworks>
```

**To:**
```xml
<TargetFrameworks>net5.0;net48;net10.0</TargetFrameworks>
```

**Rationale:** Add net10.0 to existing multi-targeting to maintain backward compatibility with .NET 5.0 and .NET Framework 4.8 consumers while enabling .NET 10.0 consumers.

**Step 2: Package Updates**

**No package updates required** ?

All current packages are compatible with .NET 10.0:
- GitVersion.MsBuild 5.7.0
- Microsoft.SourceLink.GitHub 1.0.0
- Microsoft.VisualStudio.Threading.Analyzers 16.10.56

**Step 3: Expected Breaking Changes**

**No breaking changes identified** ?

Assessment shows:
- 1,168 APIs analyzed
- 0 binary incompatibilities
- 0 source incompatibilities
- 0 behavioral changes

**Step 4: Code Modifications**

**No code modifications expected** ?

All APIs used in the project are compatible with .NET 10.0. The codebase should build without changes.

**Potential Areas to Review (Post-Build):**
- Conditional compilation directives (`#if NET5_0` etc.) - verify they work correctly with net10.0
- Framework-specific code paths (if any exist)
- Obsolete API usage warnings (if any appear)

**Step 5: Testing Strategy**

**Unit Tests:** Covered by ProvisionData.Common.UnitTests project (upgraded simultaneously)

**Build Validation:**
```bash
dotnet build source\ProvisionData.Common\ProvisionData.Common.csproj
```

Expected: Builds successfully for all 3 target frameworks (net5.0, net48, net10.0)

**Framework-Specific Build:**
```bash
dotnet build source\ProvisionData.Common\ProvisionData.Common.csproj -f net10.0
dotnet build source\ProvisionData.Common\ProvisionData.Common.csproj -f net5.0
dotnet build source\ProvisionData.Common\ProvisionData.Common.csproj -f net48
```

Expected: Each framework builds independently without errors

**Step 6: Validation Checklist**

- [ ] Project file updated with net10.0 added to TargetFrameworks
- [ ] `dotnet restore` completes without errors
- [ ] Project builds successfully for net10.0 target
- [ ] Project builds successfully for net5.0 target (regression check)
- [ ] Project builds successfully for net48 target (regression check)
- [ ] No build errors
- [ ] No build warnings (or warnings understood and acceptable)
- [ ] Output DLL generated for each target framework in bin\Debug or bin\Release
- [ ] Dependent project (UnitTests) can reference the net10.0 build

---

### Project 2: build\_build.csproj

**Current State:** net5.0, DotNetCoreApp, 162 LOC, 0 dependencies, 0 dependants
**Target State:** net10.0
**Risk Level:** ?? Low

#### Prerequisites

- .NET 10.0 SDK installed and available
- Project currently on branch `upgrade-to-NET10`
- Nuke.Common package compatible with .NET 10.0

#### Migration Steps

**Step 1: Update Project File TargetFramework Property**

Modify `build\_build.csproj`:

**Change:**
```xml
<TargetFramework>net5.0</TargetFramework>
```

**To:**
```xml
<TargetFramework>net10.0</TargetFramework>
```

**Rationale:** Build automation project should use the latest .NET version for best tooling support and performance. No need to multi-target as this is a build-time tool.

**Step 2: Package Updates**

**No package updates required** ?

Current package is compatible with .NET 10.0:
- Nuke.Common 5.3.0

**Step 3: Expected Breaking Changes**

**No breaking changes identified** ?

Assessment shows:
- 279 APIs analyzed
- 0 binary incompatibilities
- 0 source incompatibilities
- 0 behavioral changes

**Step 4: Code Modifications**

**No code modifications expected** ?

All APIs used in the build automation are compatible with .NET 10.0. The build scripts should continue to work without changes.

**Potential Areas to Review (Post-Build):**
- Build task definitions (verify they execute correctly on .NET 10.0)
- Any framework-specific build logic
- File paths or SDK references that might be version-specific

**Step 5: Testing Strategy**

**Build Validation:**
```bash
dotnet build build\_build.csproj
```

Expected: Builds successfully without errors or warnings

**Build Execution Test:**
```bash
dotnet run --project build\_build.csproj
```

Expected: Build tasks execute successfully (specific targets may vary based on your build configuration)

**Step 6: Validation Checklist**

- [ ] Project file updated with net10.0 TargetFramework
- [ ] `dotnet restore` completes without errors
- [ ] Project builds successfully
- [ ] No build errors
- [ ] No build warnings (or warnings understood and acceptable)
- [ ] Build automation executable runs without errors
- [ ] Build tasks complete successfully

---

### Project 3: tests\ProvisionData.Common.UnitTests\ProvisionData.Common.UnitTests.csproj

**Current State:** net5.0, DotNetCoreApp, 685 LOC, 1 dependency (ProvisionData.Common)
**Target State:** net10.0
**Risk Level:** ?? Low

#### Prerequisites

- .NET 10.0 SDK installed and available
- Project currently on branch `upgrade-to-NET10`
- ProvisionData.Common.csproj updated to include net10.0 (happens simultaneously in All-At-Once)

#### Migration Steps

**Step 1: Update Project File TargetFramework Property**

Modify `tests\ProvisionData.Common.UnitTests\ProvisionData.Common.UnitTests.csproj`:

**Change:**
```xml
<TargetFramework>net5.0</TargetFramework>
```

**To:**
```xml
<TargetFramework>net10.0</TargetFramework>
```

**Rationale:** Test projects should target the same framework being tested. Since the primary target for ProvisionData.Common is now net10.0 (with multi-targeting support), tests should run on net10.0.

**Step 2: Package Updates**

**No package updates required** ?

All current test-related packages are compatible with .NET 10.0:
- Bogus 33.1.1
- Bogus.Tools.Analyzer 33.1.1
- FluentAssertions 6.1.0
- GitVersion.MsBuild 5.7.0
- Microsoft.NET.Test.Sdk 16.11.0
- Microsoft.VisualStudio.Threading.Analyzers 16.10.56
- Serilog 2.10.0
- Serilog.Sinks.Debug 2.0.0
- Serilog.Sinks.TextWriter 2.1.0
- Shouldly 4.0.3
- xunit 2.4.1
- xunit.runner.visualstudio 2.4.3

**Note:** While all packages are marked compatible, some are older versions (e.g., Microsoft.NET.Test.Sdk 16.11.0 from 2021). They will work, but consider updating to newer versions in a future maintenance cycle for better .NET 10.0 support.

**Step 3: Expected Breaking Changes**

**No breaking changes identified** ?

Assessment shows:
- 895 APIs analyzed
- 0 binary incompatibilities
- 0 source incompatibilities
- 0 behavioral changes

**Step 4: Code Modifications**

**No code modifications expected** ?

All test code APIs are compatible with .NET 10.0. Tests should run without code changes.

**Potential Areas to Review (Post-Build):**
- Test assertions relying on framework-specific behavior
- Timing-sensitive tests (performance characteristics may differ)
- Tests using reflection or internal APIs (check compatibility)
- Any test fixtures or setup code with framework dependencies

**Step 5: Testing Strategy**

**Build Validation:**
```bash
dotnet build tests\ProvisionData.Common.UnitTests\ProvisionData.Common.UnitTests.csproj
```

Expected: Builds successfully, references ProvisionData.Common net10.0 build

**Test Execution:**
```bash
dotnet test tests\ProvisionData.Common.UnitTests\ProvisionData.Common.UnitTests.csproj --verbosity normal
```

Expected: All tests pass with 0 failures

**Detailed Test Run:**
```bash
dotnet test tests\ProvisionData.Common.UnitTests\ProvisionData.Common.UnitTests.csproj --verbosity detailed --logger "console;verbosity=detailed"
```

Expected: Detailed test output showing all tests executing on .NET 10.0 runtime

**Step 6: Validation Checklist**

- [ ] Project file updated with net10.0 TargetFramework
- [ ] `dotnet restore` completes without errors
- [ ] Project builds successfully
- [ ] Project correctly references ProvisionData.Common net10.0 build
- [ ] No build errors
- [ ] No build warnings (or warnings understood and acceptable)
- [ ] All unit tests execute successfully
- [ ] All unit tests pass (0 failures, 0 skipped)
- [ ] Test execution time acceptable (no significant performance regression)
- [ ] Test output shows tests running on .NET 10.0 runtime

## Risk Management

### High-Level Risk Assessment

**Overall Risk Level: ?? LOW**

This upgrade presents minimal risk due to:
- Zero package updates required
- Zero API breaking changes identified
- 100% package and API compatibility
- Small codebase (2,597 LOC)
- Simple dependency structure
- Comprehensive unit test coverage

### Risk Analysis by Category

| Risk Category | Level | Description | Mitigation |
|---------------|-------|-------------|------------|
| Package Compatibility | ?? None | All 14 packages compatible with .NET 10.0 | No action needed |
| API Breaking Changes | ?? None | All 2,342 APIs analyzed are compatible | No action needed |
| Security Vulnerabilities | ?? None | No vulnerabilities detected | No action needed |
| Build Failures | ?? Low | Framework change could reveal hidden issues | Comprehensive build validation |
| Test Failures | ?? Low | Behavioral changes possible (unlikely) | Run all unit tests |
| Dependency Conflicts | ?? None | No package updates needed | No action needed |

### Contingency Plans

#### If Build Fails After Framework Update

**Symptoms:** Solution fails to build after updating TargetFramework properties

**Diagnosis Steps:**
1. Check build output for specific error messages
2. Verify .NET 10.0 SDK is installed (`dotnet --list-sdks`)
3. Check for conditional compilation directives tied to framework version
4. Verify all package references restored correctly

**Resolution Options:**
1. **Missing SDK:** Install .NET 10.0 SDK from https://dotnet.microsoft.com/download/dotnet/10.0
2. **Conditional Compilation:** Review and update `#if` directives if framework-specific code exists
3. **Package Restore Issues:** Run `dotnet restore --force` to force re-download
4. **Rollback:** Revert TargetFramework changes and investigate specific errors

#### If Unit Tests Fail

**Symptoms:** Tests that passed on .NET 5.0 fail on .NET 10.0

**Diagnosis Steps:**
1. Review test failure messages for behavioral changes
2. Check for .NET version-specific test assumptions
3. Verify test data/fixtures are framework-agnostic
4. Check for timing-sensitive tests (behavior may change)

**Resolution Options:**
1. **Behavioral Changes:** Update test expectations to match .NET 10.0 behavior
2. **Test Framework Issues:** Update test framework packages if compatibility issues found
3. **Investigate:** Review .NET 10.0 release notes for behavioral changes affecting failing tests
4. **Temporary Skip:** Mark failing tests with `[Fact(Skip = "Investigation needed")]` to unblock, then investigate

#### If Multi-Targeting Conflicts Arise

**Symptoms:** ProvisionData.Common fails to build for one target framework but succeeds for others

**Diagnosis Steps:**
1. Build each target framework individually: `dotnet build -f net10.0`, `dotnet build -f net5.0`, `dotnet build -f net48`
2. Identify which framework is causing issues
3. Review conditional compilation or framework-specific code

**Resolution Options:**
1. **Add Conditional Logic:** Use `#if NET10_0` directives if framework-specific code needed
2. **Package Version Conflicts:** Add framework-specific PackageReference if needed: `<PackageReference Include="..." Version="X.Y.Z" Condition="'$(TargetFramework)' == 'net10.0'" />`
3. **API Availability:** Use API availability attributes or conditional compilation for APIs not available in all frameworks

### Rollback Strategy

**If critical issues prevent upgrade completion:**

1. **Immediate Rollback:**
   ```bash
   git checkout main
   git branch -D upgrade-to-NET10
   ```

2. **Preserve Investigation:**
   ```bash
   git checkout main
   git checkout -b upgrade-to-NET10-investigation
   # Work remains available for investigation
   ```

3. **Partial Rollback:** Not applicable (All-At-Once strategy - all or nothing)

### Alternative Approaches (If All-At-Once Fails)

If unexpected issues arise with All-At-Once approach:

1. **Incremental Approach:** Upgrade projects one at a time:
   - Phase 1: ProvisionData.Common only (multi-target net5.0;net48;net10.0)
   - Phase 2: ProvisionData.Common.UnitTests (change to net10.0)
   - Phase 3: _build (change to net10.0)

2. **Drop .NET Framework 4.8:** If multi-targeting causes issues:
   - Remove net48 from ProvisionData.Common
   - Update to net5.0;net10.0 only
   - **Note:** This is a breaking change for .NET Framework consumers

3. **Stay on .NET 9.0:** Fallback to shorter migration path:
   - Target net9.0 instead of net10.0
   - Smaller framework jump (net5.0 ? net9.0)

## Testing & Validation Strategy

### Multi-Level Testing Approach

Since this is an All-At-Once upgrade, testing occurs after all projects are updated in a single atomic operation.

### Phase-by-Phase Testing Requirements

#### Phase 0: Pre-Upgrade Validation

**Before making any changes:**

```bash
# Verify .NET 10.0 SDK installed
dotnet --list-sdks

# Expected: Should include 10.0.x in the list
```

```bash
# Verify current branch
git branch --show-current

# Expected: upgrade-to-NET10
```

```bash
# Baseline: Build current solution (should succeed on .NET 5.0)
dotnet build ProvisionData.Common.sln

# Expected: Build succeeds, establish baseline
```

```bash
# Baseline: Run tests (should pass on .NET 5.0)
dotnet test ProvisionData.Common.sln

# Expected: All tests pass, establish baseline test results
```

#### Phase 1: Post-Upgrade Build Validation

**After updating all project files:**

**Test 1: Solution-Wide Restore**
```bash
dotnet restore ProvisionData.Common.sln --force
```

Expected outcome:
- ? Restore completes without errors
- ? All packages compatible with .NET 10.0
- ? Multi-targeting packages resolved correctly for net5.0, net48, and net10.0

**Test 2: Solution-Wide Build**
```bash
dotnet build ProvisionData.Common.sln --no-restore
```

Expected outcome:
- ? Build completes successfully
- ? 0 build errors
- ? 0 build warnings (or only acceptable warnings)
- ? All 3 projects build successfully
- ? ProvisionData.Common builds for all 3 targets (net5.0, net48, net10.0)
- ? ProvisionData.Common.UnitTests builds for net10.0
- ? _build builds for net10.0

**Test 3: Individual Project Builds**

```bash
# Build ProvisionData.Common for each target framework
dotnet build source\ProvisionData.Common\ProvisionData.Common.csproj -f net10.0
dotnet build source\ProvisionData.Common\ProvisionData.Common.csproj -f net5.0
dotnet build source\ProvisionData.Common\ProvisionData.Common.csproj -f net48
```

Expected outcome:
- ? Each target framework builds independently
- ? Output assemblies generated in correct bin\Debug\{framework} folders

```bash
# Build test project
dotnet build tests\ProvisionData.Common.UnitTests\ProvisionData.Common.UnitTests.csproj
```

Expected outcome:
- ? Test project builds successfully
- ? Correctly references ProvisionData.Common net10.0 build

```bash
# Build automation project
dotnet build build\_build.csproj
```

Expected outcome:
- ? Build automation compiles successfully

#### Phase 2: Test Execution & Validation

**Test 4: Unit Test Execution**

```bash
dotnet test tests\ProvisionData.Common.UnitTests\ProvisionData.Common.UnitTests.csproj --verbosity normal
```

Expected outcome:
- ? All tests execute
- ? All tests pass (0 failures)
- ? 0 skipped tests
- ? Tests run on .NET 10.0 runtime

**Test 5: Detailed Test Run with Logging**

```bash
dotnet test ProvisionData.Common.sln --verbosity detailed --logger "console;verbosity=detailed"
```

Expected outcome:
- ? Detailed test output confirms .NET 10.0 runtime
- ? All test assemblies load correctly
- ? No runtime errors or exceptions
- ? Test execution time within acceptable range

**Test 6: Build Automation Validation**

```bash
dotnet run --project build\_build.csproj
```

Expected outcome:
- ? Build automation executes successfully
- ? All build tasks complete without errors

### Comprehensive Validation Checklist

**Solution-Level Validation:**
- [ ] All 3 projects updated to target .NET 10.0 (or include net10.0 in multi-targeting)
- [ ] Solution builds without errors
- [ ] Solution builds without warnings (or warnings are documented and acceptable)
- [ ] All NuGet packages restored successfully
- [ ] No package dependency conflicts
- [ ] No security vulnerabilities (verify with `dotnet list package --vulnerable`)

**ProvisionData.Common.csproj Validation:**
- [ ] TargetFrameworks includes net10.0 (net5.0;net48;net10.0)
- [ ] Builds for net10.0 target
- [ ] Builds for net5.0 target (backward compatibility)
- [ ] Builds for net48 target (backward compatibility)
- [ ] Output assemblies exist for all 3 targets
- [ ] No build errors or warnings

**ProvisionData.Common.UnitTests.csproj Validation:**
- [ ] TargetFramework is net10.0
- [ ] Project builds successfully
- [ ] References ProvisionData.Common net10.0 build
- [ ] All unit tests pass
- [ ] Test execution time acceptable
- [ ] Tests run on .NET 10.0 runtime (verify in test output)

**_build.csproj Validation:**
- [ ] TargetFramework is net10.0
- [ ] Build automation compiles successfully
- [ ] Build tasks execute without errors
- [ ] Build automation functionality verified

**Regression Testing:**
- [ ] Functionality equivalent to .NET 5.0 baseline
- [ ] No performance degradation
- [ ] No behavioral changes in tests
- [ ] All existing features work as expected

### Performance Validation

While not strictly required for this upgrade, consider these optional performance checks:

**Optional: Benchmark Build Times**
- Compare build times before/after upgrade
- Expected: Similar or faster on .NET 10.0

**Optional: Benchmark Test Execution Times**
- Compare test execution times before/after upgrade
- Expected: Similar or faster on .NET 10.0

### Smoke Testing

**Manual Smoke Tests (if applicable):**

If ProvisionData.Common is consumed by other projects:
1. Create a simple console app targeting net10.0
2. Reference ProvisionData.Common
3. Verify basic functionality works
4. Confirm no runtime errors

### Final Validation Gate

**All checks must pass before proceeding to source control commit:**

? All builds succeed  
? All tests pass  
? No security vulnerabilities  
? No dependency conflicts  
? Backward compatibility maintained (net5.0, net48 targets still build)  
? Documentation updated (if applicable)  

**If any validation fails:** 
- Do NOT commit changes
- Investigate and resolve the issue
- Re-run full validation checklist
- Refer to Risk Management section for contingency plans

## Complexity & Effort Assessment

### Overall Solution Complexity: ?? LOW

**Justification:**
- Small solution (3 projects, 2,597 LOC)
- Zero package updates required
- Zero breaking changes identified
- Simple linear dependencies
- All projects SDK-style (modern format)

### Per-Project Complexity

| Project | Complexity | LOC | Dependencies | Risk Factors | Justification |
|---------|------------|-----|--------------|--------------|---------------|
| ProvisionData.Common.csproj | ?? Low | 1,750 | 0 | Multi-targeting | Simple addition of net10.0 to existing multi-targeting |
| _build.csproj | ?? Low | 162 | 0 | None | Build automation, minimal code |
| ProvisionData.Common.UnitTests.csproj | ?? Low | 685 | 1 | Test framework compatibility | Test project, straightforward upgrade |

### Phase Complexity Assessment

**Phase 1: Atomic Upgrade**
- **Complexity:** ?? Low
- **Operations:** Update 3 project files (TargetFramework properties only)
- **Dependencies:** Linear (Common ? UnitTests, _build independent)
- **Expected Challenges:** None identified

**Phase 2: Validation**
- **Complexity:** ?? Low
- **Operations:** Build solution, run unit tests
- **Expected Challenges:** Minimal (no breaking changes identified)

### Resource Requirements

**Skill Levels Required:**
- Basic understanding of .NET project file structure
- Familiarity with MSBuild TargetFramework property
- Understanding of multi-targeting (for ProvisionData.Common)
- Basic git/source control knowledge

**Parallel Capacity:**
All updates are in a single atomic operation, so parallelization is not applicable. However, the build system will naturally build projects in dependency order.

**Estimated Relative Effort by Phase:**

| Phase | Relative Complexity | Key Activities |
|-------|-------------------|----------------|
| Phase 0: Preparation | Minimal | Verify SDK, branch validation |
| Phase 1: Atomic Upgrade | Low | Update 3 project files, restore, build |
| Phase 2: Validation | Low | Run tests, verify results |
| Phase 3: Source Control | Minimal | Commit changes, push |

**Note:** Time estimates are intentionally omitted as duration varies significantly based on machine performance, network speed, solution size, and environmental factors. Focus is on relative complexity ratings to guide effort allocation.

## Source Control Strategy

### Branching Strategy

**Main Branch:** `main` (protected)  
**Source Branch:** `main` (starting point)  
**Upgrade Branch:** `upgrade-to-NET10` (already created and switched)  
**Merge Target:** `main` (after validation complete)

### Branch Protection

- Main branch should remain stable
- All upgrade work happens on `upgrade-to-NET10` branch
- Merge to main only after all validation passes

### Commit Strategy

**All-At-Once Strategy Commit Approach:**

Since this is an All-At-Once upgrade, all project file changes should be committed together in a **single atomic commit**.

**Recommended Commit Sequence:**

**Commit 1: Atomic Framework Upgrade**
- Include: All 3 project file TargetFramework changes
- Include: Any other project file modifications (if needed)
- Message: `feat: Upgrade to .NET 10.0`

**Commit Body Template:**
```
feat: Upgrade to .NET 10.0

- ProvisionData.Common: Add net10.0 to multi-targeting (net5.0;net48;net10.0)
- ProvisionData.Common.UnitTests: Upgrade from net5.0 to net10.0
- _build: Upgrade from net5.0 to net10.0

All packages compatible, no code changes required.
All tests pass.

Ref: #[issue-number] (if applicable)
```

**Why Single Commit:**
- Atomic operation - all projects upgraded together
- Easy rollback if needed (single commit to revert)
- Clear history showing coordinated upgrade
- Bisect-friendly (no intermediate broken states)

### Alternative Multi-Commit Approach (If Needed)

If issues arise and you need to track changes separately:

**Commit 1: Update ProvisionData.Common**
```
feat(common): Add net10.0 target to ProvisionData.Common

- Add net10.0 to TargetFrameworks (net5.0;net48;net10.0)
- Maintain backward compatibility with net5.0 and net48
```

**Commit 2: Update Test Project**
```
feat(tests): Upgrade UnitTests to net10.0

- Update TargetFramework from net5.0 to net10.0
- All tests pass on net10.0 runtime
```

**Commit 3: Update Build Automation**
```
feat(build): Upgrade build automation to net10.0

- Update TargetFramework from net5.0 to net10.0
- Build tasks verified on net10.0
```

**However, single atomic commit is preferred for All-At-Once strategy.**

### Commit Message Format

Follow conventional commits format:

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types:**
- `feat`: New feature or capability (framework upgrade qualifies)
- `fix`: Bug fix
- `docs`: Documentation changes
- `chore`: Maintenance tasks

**Scopes (optional):**
- `common`: ProvisionData.Common project
- `tests`: Unit test project
- `build`: Build automation
- `all`: All projects

### Review and Merge Process

**Pull Request Requirements:**

1. **Create PR from `upgrade-to-NET10` to `main`**

2. **PR Title:**
   ```
   Upgrade solution to .NET 10.0 (LTS)
   ```

3. **PR Description Template:**
   ```markdown
   ## Summary
   Upgrades all projects in ProvisionData.Common solution to .NET 10.0 (Long Term Support).

   ## Changes
   - ? ProvisionData.Common: Add net10.0 to multi-targeting (maintains net5.0, net48)
   - ? ProvisionData.Common.UnitTests: Upgrade net5.0 ? net10.0
   - ? _build: Upgrade net5.0 ? net10.0

   ## Compatibility
   - All 14 NuGet packages compatible (no updates required)
   - All 2,342 APIs analyzed: 100% compatible
   - Zero breaking changes identified

   ## Validation
   - [x] Solution builds successfully
   - [x] All unit tests pass
   - [x] Multi-targeting works (net5.0, net48, net10.0)
   - [x] No security vulnerabilities
   - [x] No dependency conflicts

   ## Risk Assessment
   ?? Low Risk - Simple upgrade, full compatibility, comprehensive testing

   ## Related Issues
   Closes #[issue-number]
   ```

4. **PR Checklist:**
   - [ ] All validation tests pass
   - [ ] Code builds without errors or warnings
   - [ ] All unit tests pass
   - [ ] No security vulnerabilities
   - [ ] Backward compatibility maintained (net5.0, net48)
   - [ ] Commit messages follow conventional format
   - [ ] PR description complete

5. **Review Requirements:**
   - At least one approving review (team policy dependent)
   - All CI/CD checks pass (if applicable)
   - No merge conflicts with main

6. **Merge Strategy:**
   - **Preferred:** Squash and merge (creates single clean commit on main)
   - **Alternative:** Merge commit (preserves upgrade branch history)
   - **Not recommended:** Rebase (can complicate history for multi-file atomic changes)

### Post-Merge Actions

**After successful merge to main:**

1. **Delete upgrade branch** (if no longer needed):
   ```bash
   git checkout main
   git pull
   git branch -d upgrade-to-NET10
   git push origin --delete upgrade-to-NET10
   ```

2. **Tag the release** (optional, recommended):
   ```bash
   git tag -a v2.0.0-net10 -m "Upgrade to .NET 10.0"
   git push origin v2.0.0-net10
   ```

3. **Update documentation:**
   - README.md (if it mentions framework requirements)
   - CHANGELOG.md (document the upgrade)
   - Any developer setup guides

4. **Notify team:**
   - Announce upgrade completion
   - Update any dependent projects that consume ProvisionData.Common
   - Share upgrade experience/lessons learned

### Rollback Plan

**If merge to main causes issues:**

**Option 1: Revert the merge commit**
```bash
git checkout main
git revert -m 1 <merge-commit-sha>
git push origin main
```

**Option 2: Reset to pre-merge state** (if no other commits on main)
```bash
git checkout main
git reset --hard HEAD~1
git push origin main --force
```

**Option 3: Create fix-forward commit**
- Keep the upgrade
- Create new commit with fixes
- Preferred for public repositories

### Git History Best Practices

- **Clear commit messages** - Future developers should understand why upgrade happened
- **Atomic commits** - Each commit represents a complete, buildable state
- **No broken commits** - Every commit in history should build successfully
- **Tag important milestones** - Framework upgrades are significant events worth tagging

## Success Criteria

### All-At-Once Strategy Success Criteria

The .NET 10.0 upgrade is considered **successfully complete** when ALL of the following criteria are met:

### Technical Criteria

#### Framework Migration
- ? **ProvisionData.Common.csproj**
  - TargetFrameworks property includes net10.0
  - Complete value: `net5.0;net48;net10.0`
  - Builds successfully for all 3 target frameworks
  - Output assemblies generated for all targets

- ? **ProvisionData.Common.UnitTests.csproj**
  - TargetFramework property is net10.0
  - Builds successfully for net10.0
  - Correctly references ProvisionData.Common net10.0 build

- ? **_build.csproj**
  - TargetFramework property is net10.0
  - Builds successfully for net10.0
  - Build automation executes without errors

#### Package Compatibility
- ? All 14 NuGet packages remain compatible
- ? No package dependency conflicts
- ? `dotnet restore` completes without errors
- ? No security vulnerabilities detected

#### Build Success
- ? Solution builds without errors: `dotnet build ProvisionData.Common.sln`
- ? Solution builds without warnings (or all warnings documented and acceptable)
- ? Each project builds independently
- ? Multi-targeting builds work for ProvisionData.Common (net5.0, net48, net10.0)
- ? Build output clean and expected

#### Test Success
- ? All unit tests execute successfully
- ? All unit tests pass (0 failures)
- ? 0 skipped tests (unless intentionally skipped before upgrade)
- ? Tests run on .NET 10.0 runtime
- ? Test execution time within acceptable range (no significant performance regression)

#### API Compatibility
- ? All 2,342 APIs remain compatible
- ? No binary incompatibilities introduced
- ? No source incompatibilities introduced
- ? No unexpected behavioral changes

#### Backward Compatibility
- ? ProvisionData.Common still builds for net5.0 (regression check)
- ? ProvisionData.Common still builds for net48 (regression check)
- ? Existing consumers can continue using net5.0 or net48 builds
- ? No breaking changes to public APIs

### Quality Criteria

#### Code Quality
- ? No new compiler warnings introduced
- ? Code quality maintained (no degradation)
- ? No obsolete API usage warnings (or documented and acceptable)
- ? All existing functionality works as expected

#### Test Coverage
- ? Test coverage maintained at pre-upgrade levels
- ? No tests removed or disabled (unless justified)
- ? All test scenarios still valid on .NET 10.0

#### Documentation
- ? README.md updated if it mentions framework requirements
- ? CHANGELOG.md updated with upgrade details
- ? Any developer setup guides updated
- ? Package documentation updated (if ProvisionData.Common is published as NuGet)

### Process Criteria

#### All-At-Once Strategy Principles Applied
- ? All projects updated simultaneously in single atomic operation
- ? No intermediate broken states in git history
- ? Single coordinated commit (or logically grouped commits)
- ? Entire solution validated as a unit

#### Source Control
- ? All changes committed to `upgrade-to-NET10` branch
- ? Commit messages follow conventional format
- ? Clear commit history showing upgrade rationale
- ? Branch ready for PR to main

#### Validation
- ? All validation steps from Testing & Validation Strategy completed
- ? Pre-upgrade baseline established
- ? Post-upgrade validation passed
- ? Regression testing completed

#### Risk Management
- ? No high-risk issues encountered
- ? All contingency plans reviewed
- ? Rollback strategy documented and understood

### Completion Checklist

**Before declaring upgrade complete, verify:**

**Phase 1: Atomic Upgrade**
- [ ] All 3 project files updated with correct TargetFramework(s)
- [ ] Solution restores without errors
- [ ] Solution builds without errors
- [ ] Solution builds without warnings (or warnings documented)

**Phase 2: Validation**
- [ ] All unit tests pass
- [ ] Build automation works
- [ ] Multi-targeting validated (net5.0, net48, net10.0 all build)
- [ ] No security vulnerabilities
- [ ] No dependency conflicts

**Phase 3: Source Control**
- [ ] Changes committed with clear message
- [ ] Commit follows conventional format
- [ ] Branch ready for PR

**Phase 4: Documentation**
- [ ] README.md updated (if applicable)
- [ ] CHANGELOG.md updated
- [ ] Any setup guides updated

**Phase 5: Team Communication**
- [ ] Team notified of upgrade
- [ ] Dependent projects identified
- [ ] Upgrade experience documented

### Definition of Done

The upgrade is **DONE** when:

1. ? All projects target .NET 10.0 (or include net10.0 in multi-targeting)
2. ? Solution builds successfully with 0 errors, 0 warnings
3. ? All tests pass with 0 failures
4. ? All validation checks complete
5. ? Changes committed to git
6. ? PR created and ready for review (or merged to main if auto-merge)
7. ? Documentation updated
8. ? Team notified

### Acceptance Criteria

The upgrade will be **accepted** when:

- ? All technical criteria met
- ? All quality criteria met
- ? All process criteria met
- ? Stakeholders approve the changes
- ? PR merged to main branch
- ? Upgrade tagged in git (optional but recommended)

---

## Appendix

### Useful Commands Reference

**Verify SDK Installation:**
```bash
dotnet --list-sdks
dotnet --version
```

**Build Commands:**
```bash
# Restore packages
dotnet restore ProvisionData.Common.sln

# Build entire solution
dotnet build ProvisionData.Common.sln

# Build specific project
dotnet build source\ProvisionData.Common\ProvisionData.Common.csproj

# Build for specific framework
dotnet build source\ProvisionData.Common\ProvisionData.Common.csproj -f net10.0
```

**Test Commands:**
```bash
# Run all tests
dotnet test ProvisionData.Common.sln

# Run tests with detailed output
dotnet test --verbosity detailed

# Run specific test project
dotnet test tests\ProvisionData.Common.UnitTests\ProvisionData.Common.UnitTests.csproj
```

**Package Commands:**
```bash
# List all packages
dotnet list package

# Check for vulnerable packages
dotnet list package --vulnerable

# Check for outdated packages
dotnet list package --outdated
```

**Git Commands:**
```bash
# Check current branch
git branch --show-current

# View commit history
git log --oneline

# View changes
git status
git diff

# Commit changes
git add .
git commit -m "feat: Upgrade to .NET 10.0"

# Push to remote
git push origin upgrade-to-NET10
```

### Framework Version Reference

| Framework | Version | Support Type | End of Support |
|-----------|---------|--------------|----------------|
| .NET Framework 4.8 | 4.8 | LTS | Indefinite (Windows only) |
| .NET 5.0 | 5.0 | STS | May 8, 2022 (EOL) |
| .NET 8.0 | 8.0 | LTS | November 10, 2026 |
| .NET 9.0 | 9.0 | STS | May 2026 (estimated) |
| .NET 10.0 | 10.0 | LTS | November 2028 (estimated) |

### Additional Resources

- [.NET 10.0 Release Notes](https://github.com/dotnet/core/tree/main/release-notes/10.0)
- [Breaking Changes in .NET 10.0](https://learn.microsoft.com/en-us/dotnet/core/compatibility/10.0)
- [Multi-Targeting Best Practices](https://learn.microsoft.com/en-us/dotnet/standard/library-guidance/cross-platform-targeting)
- [SDK-Style Project Files](https://learn.microsoft.com/en-us/dotnet/core/project-sdk/overview)

---

**Plan Version:** 1.0  
**Created:** 2025  
**Solution:** ProvisionData.Common  
**Target Framework:** .NET 10.0 (LTS)  
**Strategy:** All-At-Once
