# .NET 10.0 Upgrade Plan

---

## Table of Contents
1. [Executive Summary](#executive-summary)
2. [Migration Strategy](#migration-strategy)
3. [Detailed Dependency Analysis](#detailed-dependency-analysis)
4. [Project-by-Project Plans](#project-by-project-plans)
5. [Package Update Reference](#package-update-reference)
6. [Breaking Changes Catalog](#breaking-changes-catalog)
7. [Testing & Validation Strategy](#testing--validation-strategy)
8. [Risk Management](#risk-management)
9. [Complexity & Effort Assessment](#complexity--effort-assessment)
10. [Source Control Strategy](#source-control-strategy)
11. [Success Criteria](#success-criteria)

---

## 1. Executive Summary

### Scenario Description
Upgrade all projects in the solution from .NET 8.0 to .NET 10.0 (Long Term Support).

### Scope
- 16 projects (application, libraries, tests, Docker)
- All currently target .NET 8.0
- Includes a Razor Pages frontend, API, core libraries, and test projects

### Selected Strategy
**All-At-Once Strategy** - All projects upgraded simultaneously in a single operation.

**Rationale:**
- Medium solution (16 projects)
- Homogeneous codebase (all .NET 8.0)
- Clear dependency structure (see Dependency Analysis)
- All package updates and framework changes can be coordinated
- Good candidate for atomic upgrade

### Complexity Assessment
- Moderate complexity: multiple project types, but no legacy .NET Framework
- Some projects have security-vulnerable or deprecated packages
- Several projects have binary/source incompatibilities and behavioral changes flagged

### Critical Issues
- Security vulnerabilities in some NuGet packages (see Risk Management)
- Deprecated and incompatible packages must be updated
- All projects require TargetFramework update

### Recommended Approach
- All-At-Once migration for maximum efficiency and consistency
- Single coordinated upgrade and validation phase

### Iteration Strategy Used
- Iterative plan generation, grouped by logical project type and dependency order
- All projects included in atomic upgrade phase

## 2. Migration Strategy

### Approach Selection
**All-At-Once Strategy**
- All projects will be upgraded to .NET 10.0 in a single atomic operation.
- All TargetFramework and NuGet package updates will be applied simultaneously.
- Compilation errors and breaking changes will be addressed in the same coordinated phase.

### Justification
- Solution size and structure allow for a unified upgrade.
- Ensures all dependencies are compatible and reduces risk of partial upgrades.
- Enables single test and validation phase for the entire solution.

### Dependency-Based Ordering
- All projects will be updated together, respecting dependency order for validation and testing.
- No intermediate states; all projects move to .NET 10.0 at once.

### Execution Phases
- **Phase 0: Preparation** (SDK, global.json)
- **Phase 1: Atomic Upgrade** (all project and package updates)
- **Phase 2: Test Validation** (all tests executed post-upgrade)

### Parallel vs Sequential Execution
- All project file and package updates are performed in parallel as a single batch.
- Testing and validation are performed after the atomic upgrade.

## 3. Detailed Dependency Analysis

### Dependency Graph Summary
Projects in topological dependency order (leaf to root):
1. Dfe.Complete.Utils
2. Dfe.Complete.Domain
3. Dfe.Complete.Application
4. Dfe.Complete.Infrastructure
5. Dfe.Complete.Logging
6. Dfe.Complete.UserContext
7. Dfe.Complete.Tests.Common
8. Dfe.Complete (Razor Pages frontend)
9. Dfe.Complete.Api
10. Dfe.Complete.Api.Client
11. docker-compose.dcproj
12. Dfe.Complete.Tests
13. Dfe.Complete.UserContext.Tests
14. Dfe.Complete.Domain.Tests
15. Dfe.Complete.Application.Tests
16. Dfe.Complete.Api.Tests.Integration

### Project Groupings
- **Core Libraries:** Dfe.Complete.Utils, Dfe.Complete.Domain, Dfe.Complete.Application, Dfe.Complete.Infrastructure
- **Frontend:** Dfe.Complete (Razor Pages), Dfe.Complete.Logging, Dfe.Complete.UserContext
- **API:** Dfe.Complete.Api, Dfe.Complete.Api.Client
- **Tests:** Dfe.Complete.Tests, Dfe.Complete.Tests.Common, Dfe.Complete.UserContext.Tests, Dfe.Complete.Domain.Tests, Dfe.Complete.Application.Tests, Dfe.Complete.Api.Tests.Integration
- **Docker:** docker-compose.dcproj

### Critical Path Identification
- Core libraries must be compatible before application, API, and tests can succeed.
- All projects will be upgraded together, but validation will follow dependency order.

### Circular Dependencies
- No circular dependencies detected.

## 4. Project-by-Project Plans

### Project: Dfe.Complete.Utils
**Current State:** net8.0, core utility library, low complexity
**Target State:** net10.0
**Migration Steps:**
1. Update TargetFramework to net10.0
2. Update all package references as per assessment
3. Address any source incompatibilities
4. Validate build and tests

### Project: Dfe.Complete.Domain
**Current State:** net8.0, domain models, depends on Utils
**Target State:** net10.0
**Migration Steps:**
1. Update TargetFramework to net10.0
2. Update package references
3. Address source incompatibilities
4. Validate build and tests

### Project: Dfe.Complete.Application
**Current State:** net8.0, application logic, depends on Domain/Utils
**Target State:** net10.0
**Migration Steps:**
1. Update TargetFramework to net10.0
2. Update packages (including security-vulnerable/deprecated)
3. Address binary/source incompatibilities
4. Validate build and tests

### Project: Dfe.Complete.Infrastructure
**Current State:** net8.0, infrastructure, depends on Application/Domain
**Target State:** net10.0
**Migration Steps:**
1. Update TargetFramework to net10.0
2. Update packages
3. Address binary/source incompatibilities
4. Validate build and tests

### Project: Dfe.Complete.Logging
**Current State:** net8.0, logging, depends on core
**Target State:** net10.0
**Migration Steps:**
1. Update TargetFramework to net10.0
2. Update deprecated packages
3. Validate build and tests

### Project: Dfe.Complete.UserContext
**Current State:** net8.0, user context, depends on core
**Target State:** net10.0
**Migration Steps:**
1. Update TargetFramework to net10.0
2. Update deprecated packages
3. Validate build and tests

### Project: Dfe.Complete (Razor Pages frontend)
**Current State:** net8.0, Razor Pages, depends on core
**Target State:** net10.0
**Migration Steps:**
1. Update TargetFramework to net10.0
2. Update incompatible/deprecated packages
3. Address binary/source/behavioral changes
4. Validate build and tests

### Project: Dfe.Complete.Api
**Current State:** net8.0, API, depends on core
**Target State:** net10.0
**Migration Steps:**
1. Update TargetFramework to net10.0
2. Update incompatible/deprecated packages
3. Address binary/source/behavioral changes
4. Validate build and tests

### Project: Dfe.Complete.Api.Client
**Current State:** net8.0, API client, depends on API
**Target State:** net10.0
**Migration Steps:**
1. Update TargetFramework to net10.0
2. Update deprecated packages
3. Address behavioral changes
4. Validate build and tests

### Project: Dfe.Complete.Tests.Common
**Current State:** net8.0, test utilities
**Target State:** net10.0
**Migration Steps:**
1. Update TargetFramework to net10.0
2. Update packages (including security-vulnerable)
3. Validate build and tests

### Project: Dfe.Complete.Tests
**Current State:** net8.0, main test project
**Target State:** net10.0
**Migration Steps:**
1. Update TargetFramework to net10.0
2. Update deprecated packages
3. Address source incompatibilities
4. Validate build and tests

### Project: Dfe.Complete.UserContext.Tests
**Current State:** net8.0, user context tests
**Target State:** net10.0
**Migration Steps:**
1. Update TargetFramework to net10.0
2. Update deprecated packages
3. Validate build and tests

### Project: Dfe.Complete.Domain.Tests
**Current State:** net8.0, domain tests
**Target State:** net10.0
**Migration Steps:**
1. Update TargetFramework to net10.0
2. Validate build and tests

### Project: Dfe.Complete.Application.Tests
**Current State:** net8.0, application tests
**Target State:** net10.0
**Migration Steps:**
1. Update TargetFramework to net10.0
2. Update packages
3. Validate build and tests

### Project: Dfe.Complete.Api.Tests.Integration
**Current State:** net8.0, API integration tests
**Target State:** net10.0
**Migration Steps:**
1. Update TargetFramework to net10.0
2. Update packages
3. Address binary incompatibilities
4. Validate build and tests

### Project: docker-compose.dcproj
**Current State:** net8.0, Docker orchestration
**Target State:** net10.0
**Migration Steps:**
1. Update TargetFramework to net10.0
2. Validate orchestration compatibility

## 5. Package Update Reference

### Common Package Updates (affecting multiple projects)
| Package | Current | Target | Projects Affected | Update Reason |
|---------|---------|--------|-------------------|---------------|
| [See assessment] | [varies] | [as recommended] | Multiple | Framework compatibility, security, deprecation |

### Security Vulnerabilities
- Some packages in Dfe.Complete.Application and Dfe.Complete.Tests.Common contain security vulnerabilities. These must be updated as part of the upgrade.

### Deprecated/Incompatible Packages
- Deprecated and incompatible packages are flagged in Dfe.Complete.Api, Dfe.Complete.Api.Client, Dfe.Complete.Logging, Dfe.Complete.UserContext, Dfe.Complete.Tests, Dfe.Complete.UserContext.Tests.

### Project-Specific Exceptions
- See assessment for full package/project matrix. All flagged packages must be updated to the suggested version.

## 6. Breaking Changes Catalog

### Framework Breaking Changes
- Binary incompatibilities (API, Application, Infrastructure, Frontend, Tests)
- Source incompatibilities (API, Application, Infrastructure, Domain, Frontend, Tests)
- Behavioral changes (API, API Client, Infrastructure, Frontend, Tests)

### Package/API Changes
- Deprecated and incompatible NuGet packages must be replaced or updated
- Some package functionality now included in framework (see assessment)

### Configuration Updates
- Razor Pages frontend may require updates to Startup/Program, DI, or configuration patterns
- Docker orchestration may require validation for .NET 10.0 compatibility

### Areas Needing Review
- All projects: review for obsolete API usage, method signature changes, and configuration patterns
- Test projects: ensure test frameworks are compatible with .NET 10.0

## 7. Testing & Validation Strategy

### Phase 1: Atomic Upgrade Validation
- Build all projects after upgrade; solution must build with 0 errors
- Run all test projects (unit, integration, common, domain, application, API)
- Validate that all tests pass

### Validation Checklist (per project)
- [ ] Builds without errors
- [ ] Builds without warnings
- [ ] Unit/integration tests pass
- [ ] No deprecated/incompatible packages remain
- [ ] No security vulnerabilities remain

### Additional Validation
- Razor Pages frontend: verify startup, routing, and DI patterns
- Docker: validate orchestration and service startup

## 8. Risk Management

### High-Risk Changes Table
| Project | Risk Level | Description | Mitigation |
|---------|------------|-------------|------------|
| Dfe.Complete.Application | High | Security-vulnerable/deprecated packages, binary incompatibility | Update all flagged packages, validate with tests |
| Dfe.Complete.Api | High | Binary/source/behavioral incompatibilities, deprecated/incompatible packages | Update all flagged packages, review API changes |
| Dfe.Complete (Frontend) | High | Razor Pages, binary/source/behavioral changes, package updates | Review Razor Pages migration guidance, validate startup/config |
| Dfe.Complete.Tests.Common | Medium | Security-vulnerable packages | Update all flagged packages, validate tests |
| Dfe.Complete.Infrastructure | Medium | Binary/source incompatibilities | Review and update as per assessment |

### Security Vulnerabilities
- All flagged packages must be updated to suggested versions
- Validate no vulnerabilities remain post-upgrade

### Contingency Plans
- If blocking issues arise, revert to previous branch and investigate
- If package updates break functionality, seek alternatives or defer non-critical updates

## 9. Complexity & Effort Assessment

### Per-Project Complexity Table
| Project | Complexity | Dependencies | Risk |
|---------|------------|--------------|------|
| Dfe.Complete.Utils | Low | None | Low |
| Dfe.Complete.Domain | Low | Utils | Low |
| Dfe.Complete.Application | Medium | Domain, Utils | High |
| Dfe.Complete.Infrastructure | Medium | Application, Domain | Medium |
| Dfe.Complete.Logging | Low | Core | Low |
| Dfe.Complete.UserContext | Low | Core | Low |
| Dfe.Complete (Frontend) | High | Core | High |
| Dfe.Complete.Api | High | Core | High |
| Dfe.Complete.Api.Client | Medium | API | Medium |
| Dfe.Complete.Tests.Common | Low | Core | Medium |
| Dfe.Complete.Tests | Low | Core | Medium |
| Dfe.Complete.UserContext.Tests | Low | UserContext | Low |
| Dfe.Complete.Domain.Tests | Low | Domain | Low |
| Dfe.Complete.Application.Tests | Low | Application | Low |
| Dfe.Complete.Api.Tests.Integration | Medium | API | Medium |
| docker-compose.dcproj | Low | All | Low |

### Phase Complexity Assessment
- All projects included in single atomic upgrade phase
- Highest complexity in Application, API, and Frontend (Razor Pages)

### Resource Requirements
- .NET 10.0 SDK installed
- Experience with Razor Pages, API, and package management
- Ability to validate Docker orchestration

## 10. Source Control Strategy

- All changes performed on dedicated upgrade branch: `upgrade-to-NET10`
- All project and package updates committed in a single atomic commit if possible
- Use clear commit message: `Upgrade all projects to .NET 10.0 LTS and update packages`
- Pull requests must include:
  - Build and test validation
  - Review of breaking changes and package updates
  - Confirmation that no security vulnerabilities remain
- Merge to main branch only after all validation steps pass

## 11. Success Criteria

### Technical Criteria
- All projects target .NET 10.0
- All flagged package updates applied
- Solution builds with 0 errors and warnings
- All tests pass
- No deprecated/incompatible packages remain
- No security vulnerabilities remain

### Quality Criteria
- Code quality and test coverage maintained
- Documentation updated if required

### Process Criteria
- All-at-once strategy followed
- Source control and review process completed
- All validation steps documented
