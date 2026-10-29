# Aeroneer Implementation Status

> Codex must update this file after each meaningful implementation increment and before stopping. Status must be based on the actual repository, not assumptions.

## Current stage
- Stage: Implement
- Current phase: Implement module flows and complete persistence/results
- Last updated: 2026-10-29
- Environment: macOS development; Windows WPF runtime validation is deferred.

## Current objective
Implement the Aeroneer application first. Complete PostgreSQL persistence, results, and scoring accuracy. Defer builds, tests, migrations execution, app runtime, hardening, publishing, and demo validation until a real Windows 10/11 environment is available.

## Requirement status
Use one of: `Implemented (not validated)`, `Partially implemented`, `Blocked`, `Not started`, `Deferred`.

| Area | Status | Evidence / files | Remaining work |
|---|---|---|---|
| Repository inspection and project context | Partially implemented | Read repository instructions/specification; workspace initially contained Markdown only. No `.git` metadata; macOS arm64; `dotnet` unavailable. | Continue source implementation; use a Windows/.NET environment for build and runtime validation. |
| Solution and project structure | Partially implemented | `Aeroneer.sln`, `Directory.Build.props`, three project files | Add test project; verify package/SDK compatibility on Windows. |
| WPF shell, theme, navigation | Partially implemented | `src/Aeroneer.App/App.xaml`, `src/Aeroneer.App/MainWindow.xaml` | Add dashboard, reusable module flows, navigation, settings, history/results, and recovery screens. |
| Candidate registration | Partially implemented | `src/Aeroneer.App/ViewModels/RegistrationViewModel.cs`, `src/Aeroneer.Core/Validation/`, `src/Aeroneer.Infrastructure/Data/EfCandidateRepository.cs` | Add candidate re-entry/selection and navigate to dashboard after persistence; add focused validation tests later. |
| Keyboard settings and navigation | Partially implemented | `src/Aeroneer.Core/Input/` | Add persistence, WPF capture/settings UI, navigation, focus handling, and active input adapter. |
| Shared assessment lifecycle | Partially implemented | `src/Aeroneer.Core/Assessments/Assessment*.cs`, `IAssessmentLifecycleStore.cs` | Implement lifecycle orchestration and PostgreSQL transactional store; wire UI state transitions. |
| Memory module | Partially implemented | `src/Aeroneer.Core/Assessments/Memory/` | Add practice/assessment screens and lifecycle, persisted round details, result review, and tests during deferred validation. |
| Mathematics module | Partially implemented | `src/Aeroneer.Core/Assessments/Mathematics/` | Add practice/assessment flows, 10-minute/20-question lifecycle, persistence, results UI, and tests during deferred validation. |
| Control module | Partially implemented | `src/Aeroneer.Core/Assessments/Control/` | Add module instructions and screens, lifecycle/persistence, detailed charts, and later deterministic tests. |
| Slalom module | Partially implemented | `src/Aeroneer.Core/Assessments/Slalom/` | Add moving-gate simulation and screens, per-run lifecycle/persistence/feedback, and later deterministic tests. |
| Orientation module | Partially implemented | `src/Aeroneer.Core/Assessments/Orientation/` | Add WPF instrument/symbol rendering, timed keyboard answer flow, persistence/results, and later question/scoring tests. |
| Task Manager module | Partially implemented | `src/Aeroneer.Core/Assessments/TaskManager/` | Add WPF concurrent-task interaction, keyboard response flow, persistence/results, and later scheduler/scoring tests. |
| PostgreSQL / EF Core / migrations source | Partially implemented | `src/Aeroneer.Infrastructure/Data/`, `src/Aeroneer.Infrastructure/Data/Migrations/` | Add full module/result/key-binding schema and review generated migration/snapshot consistency during Windows validation. |
| Persistence wired to all modules | Not started | | |
| Results, history, charts, exports | Not started | | |
| Scoring formulas and explanations | Partially implemented | Core module scoring in `src/Aeroneer.Core/Assessments/`; defaults/formulas in `DESIGN.md` | Add result aggregation/labels and verify formulas with deterministic tests. |
| README.md and DESIGN.md | Partially implemented | `README.md`, `DESIGN.md` document actual current source and limitations | Expand Windows setup/run/migration instructions as setup scripts and persistence scope are completed. |
| Unit/integration test implementation | Deferred | See `DEFERRED_WINDOWS_VALIDATION.md` | Implement during validation stage |
| Build and runtime validation | Deferred | See `DEFERRED_WINDOWS_VALIDATION.md` | Run on appropriate Windows environment |
| Windows smoke test and publishing | Deferred | See `DEFERRED_WINDOWS_VALIDATION.md` | Run later on Windows |

## Completed in this session
- Read `AGENTS.md`, `PROJECT_SPEC.md`, `IMPLEMENTATION_STATUS.md`, and `DEFERRED_WINDOWS_VALIDATION.md`.
- Inspected workspace files and environment: only project Markdown files exist; Git metadata and .NET SDK are unavailable; host is macOS arm64.
- Established `Aeroneer.sln` and shared build properties with a standalone `net10.0` Core project.
- Added framework-independent candidate/attempt contracts, attempt lifecycle persistence port, stable keyboard action IDs/default presets/conflict checking, and Memory generation, round schedule, answer parsing, and bounded difficulty-weighted scoring.
- Added seeded Mathematics question generation across five categories, typed response records, explicit tolerance checks, and bounded accuracy/speed scoring.
- Added Control fixed-step seeded simulation and bounded trajectory metrics; Slalom seeded bounded gate generation and run scoring; Orientation seeded internally consistent question generation and no-penalty scoring; and Task Manager deterministic event scheduling, expiry rules, and dual-task scoring.
- Added WPF app/theme and a keyboard-first registration form with field validation, offline country choices, and EF-backed local candidate save; added local PostgreSQL configuration template, Docker Compose, candidate/attempt/progress entities, initial migration, transactional lifecycle store, practice prerequisite, and scored-attempt cap.
- Added `README.md` and `DESIGN.md` describing the actual partial implementation, architecture, implemented formula choices, setup outline, privacy boundary, and deferred validation.
- Source-level check: Core contains no WPF, EF Core, or Infrastructure references; editor diagnostics report no errors. No compilation was possible or attempted.

## Changed files
- `IMPLEMENTATION_STATUS.md`
- `DEFERRED_WINDOWS_VALIDATION.md`
- `Aeroneer.sln`
- `Directory.Build.props`
- `src/Aeroneer.Core/Aeroneer.Core.csproj`
- `src/Aeroneer.Core/Models/CandidateProfile.cs`
- `src/Aeroneer.Core/Assessments/AssessmentModule.cs`
- `src/Aeroneer.Core/Assessments/AssessmentMode.cs`
- `src/Aeroneer.Core/Assessments/AssessmentStatus.cs`
- `src/Aeroneer.Core/Assessments/AssessmentAttempt.cs`
- `src/Aeroneer.Core/Assessments/AttemptStartResult.cs`
- `src/Aeroneer.Core/Assessments/IAssessmentLifecycleStore.cs`
- `src/Aeroneer.Core/Input/InputAction.cs`
- `src/Aeroneer.Core/Input/InputBindingSet.cs`
- `src/Aeroneer.Core/Input/InputBindingService.cs`
- `src/Aeroneer.Core/Assessments/Memory/MemoryField.cs`
- `src/Aeroneer.Core/Assessments/Memory/MemoryParameters.cs`
- `src/Aeroneer.Core/Assessments/Memory/MemoryRoundPlan.cs`
- `src/Aeroneer.Core/Assessments/Memory/MemoryRoundScore.cs`
- `src/Aeroneer.Core/Assessments/Memory/MemoryGenerator.cs`
- `src/Aeroneer.Core/Assessments/Memory/MemoryAnswerParser.cs`
- `src/Aeroneer.Core/Assessments/Memory/MemoryScoring.cs`
- `src/Aeroneer.Core/Assessments/Mathematics/MathQuestionType.cs`
- `src/Aeroneer.Core/Assessments/Mathematics/MathQuestion.cs`
- `src/Aeroneer.Core/Assessments/Mathematics/MathQuestionGenerator.cs`
- `src/Aeroneer.Core/Assessments/Mathematics/MathQuestionResult.cs`
- `src/Aeroneer.Core/Assessments/Mathematics/MathScoring.cs`
- `src/Aeroneer.Core/Assessments/Control/ControlTrajectorySample.cs`
- `src/Aeroneer.Core/Assessments/Control/ControlSimulation.cs`
- `src/Aeroneer.Core/Assessments/Control/ControlScoring.cs`
- `src/Aeroneer.Core/Assessments/Slalom/SlalomGate.cs`
- `src/Aeroneer.Core/Assessments/Slalom/SlalomRunResult.cs`
- `src/Aeroneer.Core/Assessments/Slalom/SlalomRules.cs`
- `src/Aeroneer.Core/Assessments/Slalom/SlalomGateGenerator.cs`
- `src/Aeroneer.Core/Assessments/Slalom/SlalomScoring.cs`
- `src/Aeroneer.Core/Assessments/Orientation/OrientationQuestion.cs`
- `src/Aeroneer.Core/Assessments/Orientation/OrientationQuestionGenerator.cs`
- `src/Aeroneer.Core/Assessments/Orientation/OrientationScoring.cs`
- `src/Aeroneer.Core/Assessments/TaskManager/TaskManagerEvents.cs`
- `src/Aeroneer.Core/Assessments/TaskManager/TaskManagerEventGenerator.cs`
- `src/Aeroneer.Core/Assessments/TaskManager/TaskManagerScoring.cs`
- `src/Aeroneer.Core/Models/CandidateRegistrationRequest.cs`
- `src/Aeroneer.Core/Models/CountryOption.cs`
- `src/Aeroneer.Core/Models/CountryOptions.cs`
- `src/Aeroneer.Core/Models/ICandidateRepository.cs`
- `src/Aeroneer.Core/Validation/FieldValidationError.cs`
- `src/Aeroneer.Core/Validation/CandidateRegistrationValidator.cs`
- `Directory.Packages.props`
- `.gitignore`
- `.env.example`
- `docker-compose.yml`
- `src/Aeroneer.App/Aeroneer.App.csproj`
- `src/Aeroneer.App/App.xaml`
- `src/Aeroneer.App/App.xaml.cs`
- `src/Aeroneer.App/MainWindow.xaml`
- `src/Aeroneer.App/MainWindow.xaml.cs`
- `src/Aeroneer.App/ViewModels/RegistrationViewModel.cs`
- `src/Aeroneer.Infrastructure/Aeroneer.Infrastructure.csproj`
- `src/Aeroneer.Infrastructure/Data/CandidateEntity.cs`
- `src/Aeroneer.Infrastructure/Data/AssessmentAttemptEntity.cs`
- `src/Aeroneer.Infrastructure/Data/CandidateModuleProgressEntity.cs`
- `src/Aeroneer.Infrastructure/Data/AppDbContext.cs`
- `src/Aeroneer.Infrastructure/Data/EfCandidateRepository.cs`
- `src/Aeroneer.Infrastructure/Data/EfAssessmentLifecycleStore.cs`
- `src/Aeroneer.Infrastructure/Data/Migrations/20261029000000_Initial.cs`
- `src/Aeroneer.Infrastructure/Data/Migrations/AppDbContextModelSnapshot.cs`
- `README.md`
- `DESIGN.md`

## Known incomplete work / risks
- Candidate, attempt, and module-progress persistence exist; key bindings, module-specific result persistence, result/history/export, and the rest of the database schema are incomplete.
- Each of the six modules is a partial domain slice only; no end-to-end user flow is implemented.
- Registration currently saves a candidate but does not yet transition to a dashboard or expose existing-candidate selection.
- Windows PowerShell setup/run scripts and tests are not yet present.
- The shared lifecycle store is implemented for practice completion, attempt state, and scored attempt claiming, but module flows do not invoke it yet; keyboard rebinding remains Core-only.
- Package-version compatibility, XAML bindings, migration generation consistency, and all runtime/database behavior remain unverified because no .NET SDK is installed.
- No local Git repository is present, so Git status/history and safe commits are unavailable.
- .NET SDK is not installed; source cannot be compiled or checked locally. Do not run builds/tests/migrations/app launch in this stage regardless.

## Last validation
- Editor diagnostics over `src` reported no errors; source scan found no WPF/EF/Infrastructure references in Core.
- No compilation, tests, database migrations, database connection, app runtime, publishing, or Windows smoke check was performed.

## Exact next task
1. Complete registration success navigation and existing-candidate selection, then add the keyboard-operable dashboard and route module launch through the shared lifecycle store.

## Resume instructions
Read `AGENTS.md`, `PROJECT_SPEC.md`, this file, and `DEFERRED_WINDOWS_VALIDATION.md`. Inspect actual source and Git status, then continue from the exact next task above without regenerating completed work.
