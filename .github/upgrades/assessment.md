# Projects and dependencies analysis

This document provides a comprehensive overview of the projects and their dependencies in the context of upgrading to .NETCoreApp,Version=v10.0.

## Table of Contents

- [Executive Summary](#executive-Summary)
  - [Highlevel Metrics](#highlevel-metrics)
  - [Projects Compatibility](#projects-compatibility)
  - [Package Compatibility](#package-compatibility)
  - [API Compatibility](#api-compatibility)
- [Aggregate NuGet packages details](#aggregate-nuget-packages-details)
- [Top API Migration Challenges](#top-api-migration-challenges)
  - [Technologies and Features](#technologies-and-features)
  - [Most Frequent API Issues](#most-frequent-api-issues)
- [Projects Relationship Graph](#projects-relationship-graph)
- [Project Details](#project-details)

  - [build\_build.csproj](#build_buildcsproj)
  - [source\ProvisionData.Common\ProvisionData.Common.csproj](#sourceprovisiondatacommonprovisiondatacommoncsproj)
  - [tests\ProvisionData.Common.UnitTests\ProvisionData.Common.UnitTests.csproj](#testsprovisiondatacommonunittestsprovisiondatacommonunittestscsproj)


## Executive Summary

### Highlevel Metrics

| Metric | Count | Status |
| :--- | :---: | :--- |
| Total Projects | 3 | All require upgrade |
| Total NuGet Packages | 14 | All compatible |
| Total Code Files | 34 |  |
| Total Code Files with Incidents | 3 |  |
| Total Lines of Code | 2597 |  |
| Total Number of Issues | 3 |  |
| Estimated LOC to modify | 0+ | at least 0.0% of codebase |

### Projects Compatibility

| Project | Target Framework | Difficulty | Package Issues | API Issues | Est. LOC Impact | Description |
| :--- | :---: | :---: | :---: | :---: | :---: | :--- |
| [build\_build.csproj](#build_buildcsproj) | net5.0 | 🟢 Low | 0 | 0 |  | DotNetCoreApp, Sdk Style = True |
| [source\ProvisionData.Common\ProvisionData.Common.csproj](#sourceprovisiondatacommonprovisiondatacommoncsproj) | net5.0;net48 | 🟢 Low | 0 | 0 |  | ClassLibrary, Sdk Style = True |
| [tests\ProvisionData.Common.UnitTests\ProvisionData.Common.UnitTests.csproj](#testsprovisiondatacommonunittestsprovisiondatacommonunittestscsproj) | net5.0 | 🟢 Low | 0 | 0 |  | DotNetCoreApp, Sdk Style = True |

### Package Compatibility

| Status | Count | Percentage |
| :--- | :---: | :---: |
| ✅ Compatible | 14 | 100.0% |
| ⚠️ Incompatible | 0 | 0.0% |
| 🔄 Upgrade Recommended | 0 | 0.0% |
| ***Total NuGet Packages*** | ***14*** | ***100%*** |

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 2342 |  |
| ***Total APIs Analyzed*** | ***2342*** |  |

## Aggregate NuGet packages details

| Package | Current Version | Suggested Version | Projects | Description |
| :--- | :---: | :---: | :--- | :--- |
| Bogus | 33.1.1 |  | [ProvisionData.Common.UnitTests.csproj](#testsprovisiondatacommonunittestsprovisiondatacommonunittestscsproj) | ✅Compatible |
| Bogus.Tools.Analyzer | 33.1.1 |  | [ProvisionData.Common.UnitTests.csproj](#testsprovisiondatacommonunittestsprovisiondatacommonunittestscsproj) | ✅Compatible |
| FluentAssertions | 6.1.0 |  | [ProvisionData.Common.UnitTests.csproj](#testsprovisiondatacommonunittestsprovisiondatacommonunittestscsproj) | ✅Compatible |
| GitVersion.MsBuild | 5.7.0 |  | [ProvisionData.Common.csproj](#sourceprovisiondatacommonprovisiondatacommoncsproj)<br/>[ProvisionData.Common.UnitTests.csproj](#testsprovisiondatacommonunittestsprovisiondatacommonunittestscsproj) | ✅Compatible |
| Microsoft.NET.Test.Sdk | 16.11.0 |  | [ProvisionData.Common.UnitTests.csproj](#testsprovisiondatacommonunittestsprovisiondatacommonunittestscsproj) | ✅Compatible |
| Microsoft.SourceLink.GitHub | 1.0.0 |  | [ProvisionData.Common.csproj](#sourceprovisiondatacommonprovisiondatacommoncsproj) | ✅Compatible |
| Microsoft.VisualStudio.Threading.Analyzers | 16.10.56 |  | [ProvisionData.Common.csproj](#sourceprovisiondatacommonprovisiondatacommoncsproj)<br/>[ProvisionData.Common.UnitTests.csproj](#testsprovisiondatacommonunittestsprovisiondatacommonunittestscsproj) | ✅Compatible |
| Nuke.Common | 5.3.0 |  | [_build.csproj](#build_buildcsproj) | ✅Compatible |
| Serilog | 2.10.0 |  | [ProvisionData.Common.UnitTests.csproj](#testsprovisiondatacommonunittestsprovisiondatacommonunittestscsproj) | ✅Compatible |
| Serilog.Sinks.Debug | 2.0.0 |  | [ProvisionData.Common.UnitTests.csproj](#testsprovisiondatacommonunittestsprovisiondatacommonunittestscsproj) | ✅Compatible |
| Serilog.Sinks.TextWriter | 2.1.0 |  | [ProvisionData.Common.UnitTests.csproj](#testsprovisiondatacommonunittestsprovisiondatacommonunittestscsproj) | ✅Compatible |
| Shouldly | 4.0.3 |  | [ProvisionData.Common.UnitTests.csproj](#testsprovisiondatacommonunittestsprovisiondatacommonunittestscsproj) | ✅Compatible |
| xunit | 2.4.1 |  | [ProvisionData.Common.UnitTests.csproj](#testsprovisiondatacommonunittestsprovisiondatacommonunittestscsproj) | ✅Compatible |
| xunit.runner.visualstudio | 2.4.3 |  | [ProvisionData.Common.UnitTests.csproj](#testsprovisiondatacommonunittestsprovisiondatacommonunittestscsproj) | ✅Compatible |

## Top API Migration Challenges

### Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |

### Most Frequent API Issues

| API | Count | Percentage | Category |
| :--- | :---: | :---: | :--- |

## Projects Relationship Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart LR
    P1["<b>📦&nbsp;_build.csproj</b><br/><small>net5.0</small>"]
    P2["<b>📦&nbsp;ProvisionData.Common.csproj</b><br/><small>net5.0;net48</small>"]
    P3["<b>📦&nbsp;ProvisionData.Common.UnitTests.csproj</b><br/><small>net5.0</small>"]
    P3 --> P2
    click P1 "#build_buildcsproj"
    click P2 "#sourceprovisiondatacommonprovisiondatacommoncsproj"
    click P3 "#testsprovisiondatacommonunittestsprovisiondatacommonunittestscsproj"

```

## Project Details

<a id="build_buildcsproj"></a>
### build\_build.csproj

#### Project Info

- **Current Target Framework:** net5.0
- **Proposed Target Framework:** net10.0
- **SDK-style**: True
- **Project Kind:** DotNetCoreApp
- **Dependencies**: 0
- **Dependants**: 0
- **Number of Files**: 3
- **Number of Files with Incidents**: 1
- **Lines of Code**: 162
- **Estimated LOC to modify**: 0+ (at least 0.0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["_build.csproj"]
        MAIN["<b>📦&nbsp;_build.csproj</b><br/><small>net5.0</small>"]
        click MAIN "#build_buildcsproj"
    end

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 279 |  |
| ***Total APIs Analyzed*** | ***279*** |  |

<a id="sourceprovisiondatacommonprovisiondatacommoncsproj"></a>
### source\ProvisionData.Common\ProvisionData.Common.csproj

#### Project Info

- **Current Target Framework:** net5.0;net48
- **Proposed Target Framework:** net5.0;net48;net10.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 0
- **Dependants**: 1
- **Number of Files**: 20
- **Number of Files with Incidents**: 1
- **Lines of Code**: 1750
- **Estimated LOC to modify**: 0+ (at least 0.0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (1)"]
        P3["<b>📦&nbsp;ProvisionData.Common.UnitTests.csproj</b><br/><small>net5.0</small>"]
        click P3 "#testsprovisiondatacommonunittestsprovisiondatacommonunittestscsproj"
    end
    subgraph current["ProvisionData.Common.csproj"]
        MAIN["<b>📦&nbsp;ProvisionData.Common.csproj</b><br/><small>net5.0;net48</small>"]
        click MAIN "#sourceprovisiondatacommonprovisiondatacommoncsproj"
    end
    P3 --> MAIN

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 1168 |  |
| ***Total APIs Analyzed*** | ***1168*** |  |

<a id="testsprovisiondatacommonunittestsprovisiondatacommonunittestscsproj"></a>
### tests\ProvisionData.Common.UnitTests\ProvisionData.Common.UnitTests.csproj

#### Project Info

- **Current Target Framework:** net5.0
- **Proposed Target Framework:** net10.0
- **SDK-style**: True
- **Project Kind:** DotNetCoreApp
- **Dependencies**: 1
- **Dependants**: 0
- **Number of Files**: 14
- **Number of Files with Incidents**: 1
- **Lines of Code**: 685
- **Estimated LOC to modify**: 0+ (at least 0.0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["ProvisionData.Common.UnitTests.csproj"]
        MAIN["<b>📦&nbsp;ProvisionData.Common.UnitTests.csproj</b><br/><small>net5.0</small>"]
        click MAIN "#testsprovisiondatacommonunittestsprovisiondatacommonunittestscsproj"
    end
    subgraph downstream["Dependencies (1"]
        P2["<b>📦&nbsp;ProvisionData.Common.csproj</b><br/><small>net5.0;net48</small>"]
        click P2 "#sourceprovisiondatacommonprovisiondatacommoncsproj"
    end
    MAIN --> P2

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 895 |  |
| ***Total APIs Analyzed*** | ***895*** |  |

