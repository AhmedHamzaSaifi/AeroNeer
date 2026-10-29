# Codex Master Prompt — Aviation Pilot Aptitude Screening Platform (Windows, WPF, PostgreSQL)

Copy the full prompt below into Codex while the working directory is the repository root. Let Codex inspect the existing workspace first; if the repository is empty, it must create the solution from scratch.

---

## ROLE AND EXECUTION RULES

You are acting as a senior C#/.NET desktop engineer, WPF/XAML UI designer, test engineer, and technical lead. Build a complete, working Windows-native desktop application called **Aeroneer — Aviation Pilot Aptitude Screening Platform**, inspired by the general assessment concepts of COMPASS™ but not claiming to reproduce or be an official/validated COMPASS test.

This is an implementation task, not a request for a design proposal. Work directly in the current Codex workspace/repository: create and modify source files, configuration, database migrations, and documentation. Test projects may be scaffolded, but implementing and executing the full test suite is deferred to the later Windows validation stage. Do not merely print code snippets or a plan in chat.

### Non-negotiable execution rules

1. Start by inspecting the repository, current branch, operating system, installed SDKs, existing files, and Git status. Preserve useful existing work; never overwrite files blindly.
2. Treat this master specification as authoritative. Preserve it as `PROJECT_SPEC.md` in the repository.
3. This is an implementation task. Work directly in the repository; do not merely print snippets or a proposal.
4. Follow the project flow: **Understand → Implement the application and modules → Complete persistence, results, and scoring accuracy → Defer hardening and demo validation to a real Windows environment.**
5. During this implementation stage, prioritize creating the complete, coherent application and wiring its real user flows. Do **not** spend the limited session on full builds, test execution, runtime launches, Windows smoke testing, publishing, performance hardening, or demo validation. Those are explicitly deferred to a later Windows validation stage.
6. Do not claim that the application builds, tests pass, migrations work, the database connects, or the app runs. Those claims are out of scope until validation is actually performed later.
7. Keep code structured and testable, and avoid knowingly introducing inconsistent APIs or compile errors. Do not implement the full test suite during this stage; preserve the test requirements in the deferred validation documentation.
8. Implement real functionality, not fake successful results. Avoid dead buttons, misleading placeholder screens, hard-coded scores, and TODOs used as substitutes for required behavior. If a feature cannot be completed within the available session, record it accurately as incomplete in `IMPLEMENTATION_STATUS.md`.
9. If behavior is unspecified, choose a sensible default and document it in `DESIGN.md`; ask questions only if genuinely blocked.
10. The runtime target remains Windows 10/11 WPF with local PostgreSQL. On macOS, implement and edit the Windows-targeted project without claiming native WPF execution is possible.
11. Keep the runtime application offline-capable: no cloud API, remote database, telemetry, or web server requirement.
12. Use stable, compatible package versions. Do not add unused packages or unnecessarily complex frameworks.
13. Update `IMPLEMENTATION_STATUS.md` after each meaningful increment and before stopping, including completed work, incomplete requirements, changed files, blockers, and the exact next task.
14. Create a Git commit at a stable milestone when Git is available and the user’s existing changes are safe to commit. Never commit secrets or unrelated work. Do not make a commit if doing so would capture unrelated user changes.
15. On resumption, inspect `AGENTS.md`, `PROJECT_SPEC.md`, `IMPLEMENTATION_STATUS.md`, `DEFERRED_WINDOWS_VALIDATION.md`, Git status, and actual source files. Continue from the repository state; do not regenerate or restart completed work.
16. Never weaken or remove acceptance criteria to make progress look better. Clearly distinguish implemented, partially implemented, blocked, and not started.
17. At the end of an implementation session, summarize only the work actually performed and give the precise next action. Do not run the deferred validation checklist.

---

## 1. PRODUCT GOAL

Build a polished **Windows 10/11 native desktop app** for aspiring pilot candidates, featuring candidate registration, a dashboard, seven assessment modules, practice and assessment modes, configurable keyboard bindings, PostgreSQL persistence, results/history/analytics, and exportable reports.

### Authoritative requirement precedence

The attached/current product brief version 2.0 is authoritative for platform, module requirements, and input. Older prompt ideas may be reused for quality, scoring, reports, and architecture only when compatible with v2.0.

**Important conflict resolution:**
- Use a native WPF desktop application, not Blazor, browser UI, Electron, or a web site.
- Use **keyboard only** for every assessment and all navigation. Do not implement mouse movement controls, pointer lock, gamepad, joystick, rudder-pedal, browser APIs, JavaScript, or JS interop. Mouse clicks may be supported for ordinary WPF controls only if convenient, but no feature may require them; any “answer by clicking” behavior from an older prompt is superseded by keyboard-only requirements.
- The Control module must use the keyboard for all control axes. It must display the blue needle in a circular dial and the blue ball in the rectangular instrument, both tracking red center marks.
- Replace browser `localStorage` and any SQLite suggestion with **PostgreSQL** using EF Core and Npgsql. PostgreSQL must run locally on the same Windows machine (or on a local developer Docker network); no remote/cloud database.
- Replace Blazor routes with WPF navigation/views.
- All seven modules in the v2.0 brief are required: Introduction/Registration, Control, Slalom, Orientation, Mathematics, Memory, and Task Manager.
- Do not add gamepad, mouse-based tracking, touch controls, a web backend, authentication servers, or leaderboards as MVP requirements.

---

## 2. REQUIRED TECHNOLOGY STACK

Use compatible, stable releases only. Do not use previews/RCs. Verify package compatibility before pinning versions.

- **Target platform:** Windows 10/11 x64.
- **Framework:** .NET 10 LTS, target framework `net10.0-windows`.
- **Language:** C#; use current stable C# features compatible with .NET 10, nullable reference types enabled, implicit usings enabled.
- **Desktop UI:** WPF with XAML. All screens are WPF `Window`/`UserControl` views. UI language is XAML; application logic is C#.
- **Architecture:** MVVM and clean separation of App, Core, Infrastructure, and Tests.
- **Dependency injection/configuration:** `Microsoft.Extensions.Hosting`, dependency injection, configuration, and logging where helpful. Use `CommunityToolkit.Mvvm` only if it materially simplifies clean MVVM; otherwise implement small commands/interfaces without overengineering.
- **Database:** PostgreSQL local instance, **EF Core 10 + Npgsql EF Core provider 10** (stable compatible patch versions), code-first migrations, parameterized operations through EF/Npgsql. Use `Microsoft.EntityFrameworkCore.Design` and a local `dotnet-ef` tool manifest aligned to the EF major/minor version.
- **Local development DB:** Docker Compose with a pinned PostgreSQL major-version image (not a floating `latest` tag). Provide an option for a standard local PostgreSQL Windows installation too.
- **Tests:** xUnit for Core/domain/scoring/generator tests. Add practical Infrastructure/database integration tests if a test connection is configured.
- **Graphics/animation:** WPF `Canvas`, `Shape`, `Path`, `DrawingContext` or custom reusable drawing controls. Use an elapsed-time/fixed-step simulation loop for dynamic Control and Slalom visuals. No browser Canvas.
- **Charts:** custom WPF charts using `Canvas`, `Polyline`, `Path`, and `ItemsControl`; avoid a heavy charting dependency unless essential.
- **Export:** `System.Text.Json` for JSON report export. Provide a printable report using WPF `FlowDocument`/`PrintDialog` or a clear print-ready view; do not use browser print CSS.
- **No paid service dependencies.** Keep dependencies focused and document all non-framework packages.

Use compatible stable package versions. Pin exact versions via `Directory.Packages.props` if useful; align all EF Core packages and `dotnet-ef` versions. Never add a package without using it.

---

## 3. SOLUTION STRUCTURE

Create a maintainable solution with clean dependency direction. Adjust exact names only if the repository already has a sensible structure.

```text
Aeroneer.sln
Directory.Build.props
Directory.Packages.props              # recommended central package version management
.gitignore
.env.example
README.md
DESIGN.md
docker-compose.yml
scripts/
  setup-local.ps1                     # validate/setup local development configuration
  run-local.ps1                       # load local DB config and run app
  test.ps1                            # build and test
src/
  Aeroneer.App/                       # WPF UI, views, view models, assets, navigation
    App.xaml
    App.xaml.cs
    MainWindow.xaml
    MainWindow.xaml.cs
    Views/
    ViewModels/
    Resources/
    Controls/
    Assets/
    appsettings.json
  Aeroneer.Core/                      # no WPF/EF dependencies
    Models/
    Enums/
    Interfaces/
    Assessments/
    Scoring/
    Generators/
    Input/
    Validation/
  Aeroneer.Infrastructure/            # EF Core, PostgreSQL, repositories, migrations, export
    Data/
    Entities/
    Configurations/
    Migrations/
    Repositories/
    Services/
tests/
  Aeroneer.Core.Tests/
  Aeroneer.Infrastructure.Tests/      # only if practical; integration tests use env-configured DB
```

Dependencies must flow as follows:
- `Aeroneer.App` references `Aeroneer.Core` and `Aeroneer.Infrastructure`.
- `Aeroneer.Infrastructure` references `Aeroneer.Core`.
- `Aeroneer.Core` references neither WPF nor EF Core nor Infrastructure.
- Tests reference the projects they test.

Use MVVM: no scoring, randomization, persistence, or simulation rules directly inside XAML or view code-behind. Code-behind may wire WPF-only keyboard/focus/rendering events into a ViewModel/controller or a dedicated input adapter, but must remain thin.

Create shared model and service contracts for CandidateProfile, AssessmentAttempt, module result records, KeyBindings, assessment mode/status, and module identifiers. Use `Guid` IDs unless a strong reason supports another choice. Use UTC timestamps for persisted attempt start/end times.

---

## 4. APPLICATION SHELL AND VISUAL DESIGN

Create a polished, consistent dark aviation/instrument-panel interface. It must look like a desktop product rather than a default WPF template.

### Visual system

- Main background: deep navy/slate, e.g. `#101820`.
- Cards/panels: e.g. `#1B2935`.
- Primary accent: aviation amber, e.g. `#D9A441`.
- Secondary success accent: teal, e.g. `#62C6B5`.
- Target marks/alerts: red with sufficient contrast.
- Text: high contrast light grey/white and secondary blue-grey.
- Typography: Segoe UI for UI; Segoe UI Mono or another installed monospaced fallback for instrument numbers.
- Use reusable XAML resource dictionaries for colours, spacing, controls, typography, focus state, and common buttons/cards.
- Use coherent icons (vector/Path or bundled original SVG/PNG converted/embedded appropriately). Do not depend on fetching assets from the internet at runtime.
- Full-screen-friendly assessment view with minimal distractions, visible countdown/progress, active keyboard controls, accessible focus indicators, and clear pause/exit confirmation.
- Test screens should resize sensibly to common desktop resolutions. Tablet/responsive-web requirements do not apply; this is a Windows desktop application.

### Required screens/views

1. Candidate Registration / Introduction.
2. Home / Module Selection dashboard.
3. Shared Instructions screen or reusable module instruction flow.
4. Practice/Assessment mode and pre-run practice flow for each module.
5. Control assessment.
6. Slalom assessment.
7. Orientation assessment.
8. Mathematics assessment.
9. Memory assessment.
10. Task Manager assessment.
11. Results dashboard and detailed result view.
12. Assessment history.
13. Global keyboard settings screen.
14. About / Scoring explanation screen.
15. Application/data settings, including database status and clear-data action.

Navigation must be keyboard-operable. Use a consistent shell with page title, candidate display, status, navigation, and a clear exit/back action. Add a global mute toggle and optional sound cues only if this does not delay core requirements; sound defaults to off or has an accessible on/off setting.

The footer/About screen must include: “Educational tool; not an official COMPASS test. Scores are experimental and are not validated for pilot selection.”

---

## 5. CANDIDATE REGISTRATION / INTRODUCTION

Show candidate registration before entering the dashboard. Capture these fields from brief v2.0:

- Company/Institute
- Firstname
- Lastname
- Address
- Zipcode
- Place
- Country (dropdown from a bundled offline list)
- Birthdate
- Gender
- Telephone
- E-Mail

Implement clear validation messages. Unless the existing specification or test data dictates otherwise, treat Firstname, Lastname, Country, Birthdate, and E-Mail as required; other fields can be optional. Document this choice in `DESIGN.md` and clearly mark required fields.

Behavior:
- Initial focus is on the first field.
- Enter validates the current field and moves focus forward without unexpectedly submitting the whole form.
- Standard editing, including Backspace, works normally.
- Tab and Shift+Tab work.
- Continue remains disabled until required fields are valid.
- Country data is bundled; the app never fetches dropdown options remotely.
- Continue creates or updates the candidate record in PostgreSQL and opens the dashboard.
- Returning candidates should be handled simply: allow selecting an existing local candidate or starting a new candidate. Do not add passwords/cloud identity.
- Avoid logging personal data. Use parameterized queries/EF Core. Provide a clear-data action with confirmation and appropriate cascade deletion.

---

## 6. GLOBAL KEYBOARD INPUT AND SETTINGS

All modules and primary navigation must be fully operable by keyboard. Provide a global settings screen for key rebinding before tests.

### Key binding service

Implement `IInputBindingService` / `InputBindingService` in Core or an appropriate platform-independent layer. Store actions as stable identifiers and key values; never store display labels as the identity. Provide default bindings and allow users to rebind each supported action.

Default mappings (suggested; configurable):
- Control: Left/Right for horizontal correction and Up/Down for vertical correction.
- Alternative Control preset: A/D for horizontal and W/S for vertical.
- Slalom: Left/Right, with A/D as an optional preset.
- Orientation: number keys 1, 2, 3, 4 select answers.
- Common actions: Enter = submit/continue; Escape = pause/exit confirmation; Space = acknowledge/cancel an alert where specified.
- Task Manager navigation and editing uses normal focus navigation and Enter/Space; all alert actions are keyboard accessible.

Requirements:
- Show active bindings directly on each applicable instruction/test screen.
- Detect conflicts between actions that need to be simultaneous. Explain the conflict and prevent saving invalid mappings.
- Rebinding captures the next supported key and offers Cancel/Escape.
- Do not override normal text entry in TextBox controls. Use a dedicated active assessment input handler that is enabled only while a test is running and clear any held-key state on pause, focus loss, view change, or assessment end.
- Support continuous held-key input for smooth Control and Slalom movement, not just repeated KeyDown events.
- Keep keyboard handling deterministic and unit-testable where possible.

---

## 7. SHARED ASSESSMENT LIFECYCLE

Implement a reusable lifecycle/state model used by all modules, while allowing each module to have a specialized flow.

Supported modes:
- **Practice:** unlimited attempts, helpful hints, no pressure. Practice attempts can be saved for personal history but are never included in official assessment summaries.
- **Assessment:** timed/scored, fixed documented rules, maximum 3 started scored attempts per module per candidate.

Rules:
- A candidate must complete at least one practice trial for a module before starting that module’s scored assessment. Persist this flag in PostgreSQL.
- Show attempts remaining before starting an assessment.
- Increment/claim a scored attempt transactionally when it starts so repeated restarts cannot bypass the limit.
- Persist an attempt as InProgress at start and update it to Completed, Abandoned, or Invalid at the end. Only Completed scored attempts contribute to official best/overall scores; abandoned/invalid attempts still count toward the limit and are visible in history with an explanation.
- Escape during practice pauses or offers resume/restart; Escape during a scored test presents a confirmation and then marks the attempt Abandoned if the candidate exits.
- Use `Stopwatch` or monotonic elapsed-time measures for scoring timers. Do not assume `DispatcherTimer` fires precisely; timers may refresh the UI, but elapsed time must use an authoritative clock.
- Keep state transitions explicit and testable: Instructions → Practice → Practice Feedback → Ready for Assessment → Assessment → Results.
- Handle database disconnection gracefully and do not silently pretend results were saved. If a run cannot be persisted, show a clear message and provide a safe retry/export recovery route where feasible.

---

## 8. MODULE REQUIREMENTS

Implement all six assessment modules below plus Introduction. Every module needs keyboard instructions, at least one practice trial, scored assessment mode, result feedback, persistence, and a detail view.

### 8.1 CONTROL — Compensatory Tracking

Purpose: measure keyboard-driven correction of drift. The UI must show:
- A circular dial containing a blue needle and red target mark.
- A rectangular gauge containing a blue ball/indicator and red center mark.
- Clear instrument ticks/labels, timer, current deviation, time-on-target, and active key mapping.
- A compact live input indicator showing which configured keys are currently held.

Keyboard-only behavior:
- Use four configurable directional keys to apply input on two simulated axes; defaults are Left/Right and Up/Down. WASD can be selected as a preset.
- No mouse movement is read as assessment input. No pointer lock, gamepad, joystick, or rudder pedals.
- Keep both the blue needle and blue ball meaningfully animated and driven by the candidate’s keyboard corrections in accordance with the reference description. Explain the relationship between controls and displays in instructions.
- A held key applies a smooth force that ramps up; force decays on release. Clamp positions and velocity.
- Use a seedable deterministic pseudo-random turbulence generator so tests can reproduce simulation behavior.
- Use an elapsed-time/fixed-step loop (nominal 60 Hz) with delta-time accumulation. Simulation math stays outside UI code. The UI renders the simulation state only.
- Practice duration: 30 seconds, restartable. Assessment duration: 60 seconds. Start every run with a 3-2-1 countdown.
- If application focus is lost or input state becomes unreliable: pause practice safely; for assessment, pause and ask whether to resume or invalidate/abandon. Clear all held keys on pause.
- Increase difficulty in controlled stages (e.g., turbulence/instability adjustment every 20 seconds), document the exact rules, and do not use unbounded forces.

Metrics/scoring (all formulas must be implemented in Core and unit-tested):
- Time on target %, overall and per axis. Define a target tolerance, e.g. 8% of instrument radius/half-range.
- Mean absolute deviation per axis and combined, normalized to 0–1 and optionally displayed in pixels/units.
- RMS error.
- Correction count per axis (meaningful direction reversals, debounced to avoid counting keyboard repeat noise as separate intentional corrections).
- Smoothness score based on changes in motion/jerk, normalized so higher is better.
- Axis-balance score based on normalized X/Y error differences. The old mouse-vs-keyboard “coordination score” is NOT applicable; use horizontal-vs-vertical axes because the entire app is keyboard-only.
- Suggested overall Control score: 45% time-on-target + 25% normalized deviation score + 15% smoothness + 15% axis-balance. Define normalized deviation score so lower raw error maps to a higher score. Clamp all score components to 0–100 and document the formula on About/Scoring.
- Record a downsampled simulation trajectory every 100 ms for detailed charts/review, rather than saving every rendered frame.

Results should show overall score, time-on-target, mean deviation, RMS error, corrections, smoothness, separate X/Y metrics, deviation-over-time chart, and a trajectory path plot.

### 8.2 SLALOM — Path Tracking

Purpose: steer a blue delta/aircraft symbol laterally through gaps made by paired red and green markers scrolling from top to bottom.

- Use only configurable left/right keyboard actions (default Left/Right; A/D alternate preset).
- One practice run and **three scored runs of 90 seconds each** within a scored assessment attempt.
- Generate a deterministic/semi-random gate sequence with a configurable seed, minimum safe gap width, bounded lateral movement, and difficulty progression. Avoid impossible gates.
- The markers stream downward at a controlled rate; determine gate crossing when marker pairs meet the aircraft’s crossing line.
- Score distance from the center of the gap; centerline tracking gets more credit. Missing/colliding with a gate receives a documented penalty. Normalize gate outcomes to 0–100.
- Store per-run score, passed gates, missed/collided gates, average center deviation, and a trajectory/gate summary. Show live score/time and active keys.
- Display a feedback screen after each run and a combined three-run result at the end.

### 8.3 ORIENTATION — Instrument Interpretation

Purpose: interpret RBI (Relative Bearing Indicator), Artificial Horizon, and Compass, then identify the corresponding aircraft symbol among 1–4.

- Build an offline, bundled question bank with unambiguous, internally validated question/answer pairs. Include enough variety for repeatable assessments; do not rely on internet or remote images.
- Draw or compose instrument faces and four aircraft symbols with WPF primitives/vector paths. Keep instruments and symbols static while the current question is being answered.
- The updated Windows brief requires keyboard-only operation; answer with number keys 1–4. Do not require mouse clicks.
- Assessment timer: 10 minutes. Award 1 point for correct answers; incorrect answers have no penalty. An unanswered question is not correct. Clearly show question progress, timer, selected response, and answer feedback according to the chosen documented interaction policy.
- Practice mode should have a small sample set with explanations. In assessment mode, avoid revealing the correct answer before the scored session ends.
- Persist questions presented, selected answers, correct answers, and total points. Keep answer order/randomization deterministic when seeded in tests.

### 8.4 MATHEMATICS — Applied Mental Math

Purpose: quick mental arithmetic relevant to aviation and navigation: speed, distance, time, fuel consumption/remaining, unit conversion, ratios, and simple arithmetic.

- Use a verified bundled question bank and/or a tested generator with a seedable random source. Each generated question must have one unambiguous numeric answer. Avoid ambiguous rounding and unsupported assumptions.
- Example: “A car travels 23 km in 20 minutes. What is its average speed in km/hr?” Answer: `69`.
- No units in the answer field. Support integer/decimal answers and only allow tolerances when the question definition explicitly states one.
- Candidate types the answer and presses Enter or activates Continue from keyboard. Record answer correctness and response time per question.
- Suggested implementation default (document in `DESIGN.md` because v2.0 does not prescribe a count/duration): 10-minute assessment with up to 20 questions; end when time expires or the question bank is exhausted. Practice contains 5 questions with feedback.
- Score accuracy and speed transparently. Suggested score: 80% accuracy + 20% response-speed component scaled against a documented timeout per question. If no valid answers were attempted, do not produce NaN/infinity.
- Persist question text/type, candidate answer, correct answer, correctness, response time, total accuracy, and score. Keep immediate feedback in practice; defer full answer review until the end of a scored run.

### 8.5 MEMORY — Short-Term Recall

Purpose: recall Altitude (a), Speed (s), Heading (h), and Radio (r) after a limited display period.

Parameter generation/format rules:
- Altitude: 1,000–39,000 ft; use allowed increments of 100 or 500 and ensure generated values respect the chosen increment.
- Speed: 100–450 knots.
- Heading: 000–359 degrees, always render as 3 digits with leading zeroes.
- Radio: 118.000–136.975 MHz using 0.025 MHz increments where valid.
- Use a seedable generator so tests are reproducible.

Round flow:
1. Show a cockpit-styled “NEW DATA” panel with four values and a visible display countdown.
2. Display each round for a defined interval; rounds use 8, 7, 6, then 5 seconds as difficulty increases.
3. Hide values or partially blank them and ask the candidate to recall the missing parameters.
4. Use separate labeled inputs for Altitude, Speed, Heading, and Radio, with per-field validation. Support a compact value-plus-category parser (e.g. `240s`, `270h`, `124.800r`, and altitude with `a`) when input format permits. If using separate fields, accept the proper numeric value optionally followed by its matching category letter and reject a mismatched suffix. Write unit tests for compact parsing and leading-zero heading normalization.
5. Record recall time in milliseconds from reveal of the answer controls until submit, with a 15-second per-round recall timeout. Validate numeric ranges/format.
6. Show per-parameter correctness and correct values after each practice round; in assessment, provide the round’s result feedback after submission but do not reveal upcoming round values.

Rounds:
- Assessment: 10 rounds.
- Practice: 3 rounds.
- Rounds 1–3: 1 hidden parameter, 8-second display.
- Rounds 4–6: 2 hidden parameters, 7-second display.
- Rounds 7–8: 3 hidden parameters, 6-second display.
- Rounds 9–10: all 4 hidden parameters, 5-second display.
- The set of hidden parameters must be generated reproducibly and tracked correctly; visible parameters remain available for the round when applicable.
- Optional arithmetic distractor is not required. Only add it after the base feature is complete and stable.

Scoring (in Core, fully unit-tested):
- Accuracy percentage = correctly recalled fields / total fields requested.
- Average recall time in ms, excluding invalid/timeout data according to a documented rule.
- Per-round score = 80% accuracy + 20% speed, where speed is scaled against the 15-second recall timeout, clamped to 0–100.
- Difficulty-weighted final score 0–100; harder rounds must have explicit weights. Document formula and test it.
- Avoid divide-by-zero and invalid values for empty/abandoned attempts.
- Save a full per-round result breakdown, shown in the results UI.

### 8.6 TASK MANAGER — Dual-Task Management

Purpose: manage autopilot setting updates while responding to alerts that arrive concurrently.

- Show a “NEW INFORMATION” panel with Altitude, Heading, and Speed. Show an “AUTOPILOT SETTINGS” panel with editable/current Altitude, Heading, and Speed.
- New information arrives on a schedule and remains visible until the user updates settings or the update expires (define/document expiration behavior).
- Candidate uses only the keyboard to navigate, edit numeric settings, confirm/apply the update, and respond to/cancel an alert. Show active key hints and current values.
- Alerts should arrive concurrently with primary-task updates through a deterministic/seedable scheduler. Do not block the UI thread. Alerts must have a clear keyboard response, such as Space to acknowledge, with configurable mapping where suitable.
- Score primary task on correct transfer of all three values and update response time. Score secondary task on alert response accuracy and response time. Suggested overall Task Manager score: 60% primary task + 40% alert handling, with speed inside each component. Document the exact normalized formula.
- Suggested default (v2.0 does not specify duration): 30-second practice and 180-second assessment. Document this implementation decision.
- Persist event-level primary updates and alerts, correctness, response times, missed/expired events, and final metrics. Provide feedback on accuracy under dual load, not just action count.

---

## 9. POSTGRESQL DATA AND MIGRATIONS

PostgreSQL is mandatory. It runs locally; the desktop app must not connect to a cloud DB. Use EF Core + Npgsql and code-first migrations.

### Local setup

- Add `docker-compose.yml` for a pinned stable PostgreSQL major image, exposing only localhost (e.g. `127.0.0.1:5432:5432`) for development.
- Add `.env.example` and ensure `.env` is git-ignored. Do not commit live passwords or secrets. Document copy/rename instructions in README.
- Provide a PowerShell local run/setup script that can read `.env` values or a connection-string environment variable, validate required configuration, and start the app. Alternatively, support a standard locally installed PostgreSQL service without Docker.
- If Docker is unavailable, give explicit Windows steps for installing PostgreSQL and creating the database/user through pgAdmin or `psql`.
- Use `ConnectionStrings:Postgres` via environment variables/User Secrets/local ignored config. No hard-coded real credentials in source or committed appsettings.
- Provide health checks / useful errors for unreachable DB, wrong credentials, migrations not applied, and port conflicts. Do not crash without an actionable message.

### Data entities/tables

Create appropriate EF Core entities/configurations and migrations for at least:
- `Candidates`: registration fields, timestamps, country/gender strings or enums as appropriate.
- `AssessmentAttempts`: candidate FK, module type, mode, attempt number, status, started/finished timestamps, duration, score, accuracy, and any aggregate metrics.
- `CandidateModuleProgress`: practice-completed flags per module and any necessary module state, with unique candidate/module key.
- `KeyBindingSettings`: configurable key mappings, either candidate-scoped or one installation-level profile; choose and document. Prefer a per-candidate profile if simple.
- Detailed module results. Use normalized child tables for important repeated records (Memory rounds, Math question answers, Orientation answers, Slalom run/gate summary, Task Manager events) and/or PostgreSQL `jsonb` for structured simulation samples where this is more practical. Use typed EF conversions where appropriate; do not save all detail as an opaque string with no structure.

Recommended additional child records:
- `MemoryRoundResults`
- `MathQuestionResults`
- `OrientationAnswerResults`
- `SlalomRunResults`
- `TaskManagerEvents`
- Control trajectory samples in JSONB or a child table with a sensible cadence (100 ms)

Use appropriate indexes (candidate, module, mode, date), foreign keys, delete behavior, uniqueness constraints, precision for radio frequencies, and decimal/integer types appropriate for measurements. Store dates consistently; use UTC timestamps in the database and localize only for display. Use `jsonb` only for naturally variable module details, and validate serialization/deserialization.

### EF Core setup

- Implement `AppDbContext`, configurations, repositories, and a migration for the first schema.
- Use dependency injection and `IDbContextFactory<AppDbContext>` or a correctly scoped context strategy so a long-running WPF app does not keep one DbContext alive across the entire UI lifetime.
- Use asynchronous DB APIs for UI-facing persistence wherever possible.
- Do not use `EnsureCreated()`; use migrations.
- Provide a reliable migration command and/or a carefully designed first-run migration workflow. The README must give explicit migration commands. If the app applies migrations at startup, it must do so safely, report errors clearly, and still include versioned migrations in the repository.
- Add an initial migration and ensure the database creation path is executable from the repository's documented commands.
- Ensure attempt-limit checks use a transaction/unique or concurrency-safe approach so two starts cannot exceed the max attempt count.

### Data safety

- Do not send candidate data to external services or log PII.
- Make `Clear my data` require explicit confirmation and delete dependent data in a predictable transaction.
- Avoid using actual assessment results as fake demo data in production. Seed only question banks and default settings; if sample/demo history is provided, make it opt-in and clearly marked.

---

## 10. RESULTS DASHBOARD, HISTORY, SCORING EXPLANATIONS, EXPORT

Build a useful results dashboard with an empty state when no completed assessments exist.

### Summary
- Best completed scored Memory score.
- Best completed scored Control score.
- Best score per other module.
- Combined score with an explicit rule and coverage count. Suggested: average the best completed scored module scores that exist; display `X of 6 modules completed` and do not imply that missing modules scored zero.
- Number of scored attempts used / remaining by module.
- Practice results are visible in history but excluded from official summary numbers.

### Per-module detail
- Memory: accuracy %, mean/average recall time, per-round score bar chart, accuracy by difficulty, missed fields.
- Control: time-on-target %, mean deviation, RMS error, corrections, smoothness, X/Y metrics, deviation-over-time chart, trajectory path plot.
- Slalom: each of three runs, gates passed/missed, mean deviation from gap center, run score comparison.
- Orientation: answered/correct/incorrect, points, accuracy, time used.
- Mathematics: accuracy, average response time, question-type breakdown, scores.
- Task Manager: primary-task accuracy and response time, alert handling accuracy and response time, missed alerts, final dual-task score.

### History/reporting
- History table/list with timestamp, candidate, module, mode, status, and score. Filter by module/mode and open a detail view.
- Performance label (Excellent / Good / Average / Needs Practice) with explicitly documented thresholds, configurable if easy. Explain that these are demo heuristics, not validated selection cutoffs.
- Export a result/detail report to JSON using System.Text.Json. Export must include module results but avoid exposing unrelated candidate records.
- Provide a print-friendly report via WPF FlowDocument/PrintDialog or another offline-compatible, no-cloud approach. If a true PDF file export is implemented, use a maintained package with a compatible license; otherwise clearly document printing through “Microsoft Print to PDF”.
- Add a clear-data action with confirmation.
- Show immediate feedback after Memory rounds and Slalom runs, and module-level results after all assessments.
- Build all charts with responsive custom WPF drawing/controls, correct axes/labels/units, empty states, and no fake chart values.

---

## 11. QUALITY, TESTING, AND RELIABILITY

### Code quality

- Nullable reference types enabled. Prefer clear names, small services, explicit states, interfaces for external dependencies, and immutable result records where appropriate.
- No business logic in XAML or view code-behind.
- Add XML documentation to public Core APIs.
- No unexplained magic numbers: define named configuration constants/options for durations, ranges, thresholds, and scoring weights.
- Add centralized exception handling/logging that does not leak sensitive registration data.
- Avoid over-engineering (no generic plugin framework or CQRS/MediatR unless it materially helps).
- Do not include unfinished menu options or unused packages.

### Deferred test requirements (preserve for the later validation stage)

During the later validation stage, implement tests with deterministic seeds and meaningful assertions covering at minimum:

**Shared lifecycle/scoring**
- Attempt limit (3 scored starts per candidate/module), practice unlimited, practice required before assessment, and practice excluded from official best-score summaries.
- Combined score ignores missing modules instead of treating them as zeros.
- Invalid/abandoned attempts do not affect best score.

**Memory**
- Generated values obey altitude/speed/heading/radio ranges and increments.
- Heading zero padding/normalization.
- Compact input parser accepts matching suffix forms (e.g. `240s`) and rejects mismatched suffixes/out-of-range values.
- Number of hidden parameters and display interval by round.
- Accuracy, recall time, speed component, difficulty-weighted score, timeout, empty-input, and divide-by-zero behavior.
- Seeded generation is reproducible.

**Control**
- Time-on-target math, mean absolute deviation, RMS error, per-axis metrics, normalized deviation score, corrections, smoothness/axis-balance score, and total weighted score.
- Scores stay in 0–100 for edge cases.
- Simulation is deterministic with fixed seed, advances using delta/fixed-step time, clamps positions, and clears key state on pause.

**Slalom**
- Gate generator bounds and seed determinism.
- Center gap scores higher than an edge pass; collision/miss penalties work.
- Three-run aggregation and 90-second timing configuration.

**Orientation**
- Every bundled question maps to a valid answer 1–4; scoring awards only correct points and has no negative mark.
- Assessment timing/answer recording state.

**Mathematics**
- All seeded/generated questions have valid computed answers.
- Speed/distance/time/fuel formulas and numeric tolerance rules.
- Accuracy/speed scoring and no-answer edge cases.

**Task Manager**
- New settings are compared accurately; correct values score more than incorrect/expired values.
- Alerts are scored for acknowledgement and response time.
- Event scheduler determinism under a fixed seed.

**Persistence/configuration**
- Entity/configuration tests and serialization tests for `jsonb` payloads.
- If a PostgreSQL integration connection is available, test migrations, candidate/attempt persistence, module history, cascading clear-data behavior, and attempt concurrency. Use an environment variable such as `AERONEER_TEST_CONNECTION_STRING`; do not silently test PostgreSQL-specific code only against EF InMemory.

### Deferred build/test validation (do not execute during implementation stage)

Document the exact commands needed for later Windows validation, but do not run them during this implementation stage. The later validation stage must run `dotnet restore`, `dotnet build Aeroneer.sln -c Debug`, `dotnet test Aeroneer.sln -c Debug --no-build`, database migrations and persistence checks, and a real Windows 10/11 WPF smoke test. Record actual outputs and fix discovered failures then. Do not claim these checks have passed before they are executed.

---

## 12. DATABASE/APP STARTUP EXPERIENCE

- On launch, verify local database connectivity and that schema migrations are available/applied according to the chosen documented workflow.
- If PostgreSQL is not available, show a friendly, actionable native dialog/status screen: explain that the local database is not reachable and reference the exact README/PowerShell command. Do not show raw credentials.
- Avoid making the UI appear frozen during DB checks; use async startup/status updates.
- After connection, open candidate registration or recent-candidate selection.
- Avoid blocking the WPF dispatcher with scoring or simulation work. Run calculations off the UI thread where useful, then marshal view updates safely onto the dispatcher.
- Ensure the Control/Slalom loops stop and release state when leaving the screen or closing the app.
- The app should work offline once dependencies are installed, the local PostgreSQL service is running, and database migrations are applied.

---

## 13. REPOSITORY DOCUMENTATION AND DELIVERY

Create/update the following files:

### `README.md`
Must include:
- Project overview and clear disclaimer.
- Features and all seven modules.
- Architecture overview (Mermaid or ASCII).
- Software prerequisites and tested versions.
- Exact Windows 10/11 setup from scratch: install .NET SDK, install/use Docker Desktop or PostgreSQL locally, copy `.env.example`, create database, run migrations, build, test, and start app.
- PowerShell commands that work from the repo root; no vague “configure your database” instruction.
- Alternative instructions for a local PostgreSQL Windows service if Docker is not desired.
- How to run unit and integration tests.
- Keyboard defaults and rebinding.
- Scoring rules/weights for every module.
- Where data is stored and how to export/clear it.
- How to publish a self-contained Windows build.
- Known limitations and the fact that scores are experimental/not a validated selection tool.
- Screenshot placeholders may be included, but do not claim screenshots exist if none are added.

### `DESIGN.md`
Explain:
- Architecture and dependency boundaries.
- Why WPF/XAML and MVVM were chosen.
- PostgreSQL local-service choice and offline runtime constraints.
- Database schema/migration strategy.
- Keyboard input architecture and conflict handling.
- Each module’s state machine and scoring formulas.
- Defaults that were not explicitly prescribed in the brief (registration required fields, Mathematics duration/question cap, Task Manager duration, performance ratings, exact scoring weights, etc.).
- Trade-offs, privacy, and known limitations.

### `.gitignore` and secrets
Ignore bin/obj, `.env`, local secrets, logs, test output, and user-specific files. Commit only `.env.example`, never `.env` or real credentials.

### Publishing
Provide a Windows publish command for a self-contained x64 executable, e.g. `dotnet publish src/Aeroneer.App/Aeroneer.App.csproj -c Release -r win-x64 --self-contained true`. Validate actual command syntax against the target framework. Explain that PostgreSQL is a separate local prerequisite unless you provide an automated local DB installer (do not pretend the database is embedded in the executable).

---

## 14. IMPLEMENTATION ORDER — COMPLETE THE BUILD WORK FIRST

Use these phases as a dependency-ordered implementation checklist. The goal of this stage is to build the application, not to spend the available session running tests or polishing validation.

### Phase 1 — Understand and establish project context
- Inspect the repository, OS, SDKs, current branch, Git status, and existing code.
- Preserve useful work. If the repository is empty, scaffold the solution.
- Create/update `AGENTS.md`, `PROJECT_SPEC.md`, `IMPLEMENTATION_STATUS.md`, and `DEFERRED_WINDOWS_VALIDATION.md`.
- Create the solution/project structure and central build/package configuration appropriate for the target.
- Record environmental constraints, particularly that WPF runtime testing must happen on Windows.
- Do not run builds or tests.

### Phase 2 — Establish the application foundation
- Create Core models, enums, contracts, scoring/generator/input abstractions, and clear dependency boundaries.
- Create the WPF shell, navigation, theme, shared controls, keyboard-first interaction patterns, and global settings structure.
- Create Infrastructure structure for PostgreSQL/EF Core and configuration without spending the session on database troubleshooting.
- Add README.md and DESIGN.md as living project documentation.
- Do not build, run, publish, or execute tests.

### Phase 3 — Implement all assessment experiences
Implement each module's real user flow, domain behavior, keyboard interaction, practice mode, scored mode, feedback, and result model:
1. Memory
2. Mathematics
3. Control
4. Slalom
5. Orientation
6. Task Manager

You may change the order if repository dependencies make that safer, but keep all six modules in scope. Prefer complete vertical slices over many disconnected screens. Do not use fake scoring or fake successful persistence. Keep module logic outside XAML/code-behind and use deterministic generators where specified.

### Phase 4 — Integrate registration, lifecycle, and keyboard settings
- Complete candidate registration and validation.
- Implement candidate selection/re-entry.
- Wire shared practice/assessment lifecycle, practice prerequisites, attempt counting rules, status transitions, pause/exit behavior, and global key rebinding.
- Ensure navigation and assessment controls can be operated by keyboard.
- Connect the UI to real Core services rather than placeholder handlers.

### Phase 5 — Complete PostgreSQL persistence
- Implement EF Core entities, configurations, `AppDbContext`, repositories/services, migrations, indexes, constraints, relationships, and serialization for detailed module results.
- Implement candidate, attempt, module-progress, key-binding, and module-specific result persistence.
- Implement concurrency-safe scored-attempt claiming and data clearing.
- Add local PostgreSQL Docker Compose and Windows setup/run scripts, environment configuration, and actionable database error UI.
- Wire actual save/reload behavior into all modules. Do not pretend data was saved when persistence fails.
- The database is part of the product implementation; do not defer the data layer itself.

### Phase 6 — Results, history, scoring accuracy, and exports
- Implement score formulas in Core/domain services and keep them deterministic, bounded, and documented.
- Complete per-module results and detailed metrics, result dashboards, history, filtering, charts, combined-score coverage logic, performance labels, JSON export, and print-friendly reports.
- Ensure practice attempts are excluded from scored summaries and invalid/abandoned attempts are handled as specified.
- Ensure persisted results shown by the UI are loaded from PostgreSQL, not hard-coded demo values.
- Update About/Scoring, README.md, and DESIGN.md with the actual formulas and implementation choices.

### Phase 7 — Implementation completeness review (not runtime validation)
- Inspect the repository and compare implementation against every requirement in `PROJECT_SPEC.md`.
- Review the actual source and UI wiring to identify missing flows, fake handlers, TODO substitutes, inconsistent model contracts, incomplete persistence, and missing result metrics.
- Fix implementation gaps that can be addressed by source inspection.
- Update `IMPLEMENTATION_STATUS.md` with requirement-by-requirement status and exact unfinished items.
- Do not run `dotnet restore`, `dotnet build`, `dotnet test`, database migrations, the app, publishing, performance checks, or Windows smoke tests in this stage.
- Do not call the project complete if significant requirements remain incomplete. Report the true state and next implementation task.

### Deferred phase — Windows hardening, tests, build, and demo (later)
This phase is intentionally **not part of the current implementation run**. Perform it later in an actual Windows 10/11 environment:
- Restore/build Debug and Release.
- Implement and run the Core unit tests and practical PostgreSQL integration tests listed in Section 11.
- Start PostgreSQL, apply migrations, and validate persistence/reload, attempt limits, and clear-data behavior.
- Launch the real WPF app and smoke-test registration, keyboard navigation/rebinding, each module, result history, export, and offline behavior.
- Fix compile errors, runtime exceptions, binding issues, database/migration failures, UI defects, and accessibility problems.
- Publish a self-contained Windows x64 build if appropriate and verify the published app.
- Update `DEFERRED_WINDOWS_VALIDATION.md` with actual commands, outputs, fixes, and remaining limitations. Never mark a check passed without evidence.

---

## 15. ACCEPTANCE CHECKLIST FOR THIS IMPLEMENTATION STAGE

The current stage is complete only when the implementation is substantively present in source and the status is honestly documented. Build/test/runtime validation is deferred.

- [ ] A WPF Windows-native solution exists and is organized into App/Core/Infrastructure/Tests projects as appropriate.
- [ ] Project context files exist: `AGENTS.md`, `PROJECT_SPEC.md`, `IMPLEMENTATION_STATUS.md`, and `DEFERRED_WINDOWS_VALIDATION.md`.
- [ ] Candidate registration includes every required field and validation behavior.
- [ ] All six assessment modules have real instructions, practice/assessment flows, domain logic, keyboard input, feedback, and result models.
- [ ] Shared lifecycle rules, practice prerequisites, three-start scored attempt limit, pause/exit behavior, and configurable keyboard bindings are implemented.
- [ ] PostgreSQL entities, EF Core configurations, migrations, repositories/services, and local setup instructions are implemented and wired into the app.
- [ ] Module attempts and detailed results are persisted; the UI loads real persisted results.
- [ ] Scoring formulas and metrics are implemented in Core/domain services, bounded, and explained. No fake scores or fabricated saved results.
- [ ] Results dashboard, history, module details, charts, JSON export, print-friendly report, clear-data action, and empty/error states are implemented.
- [ ] `README.md` and `DESIGN.md` describe the actual code, architecture, setup, defaults, and scoring rules.
- [ ] `IMPLEMENTATION_STATUS.md` accurately records what is complete, partial, blocked, or not started.
- [ ] `DEFERRED_WINDOWS_VALIDATION.md` preserves the later build/test/database/runtime/publishing checklist.
- [ ] No secrets, real credentials, or unrelated user changes are committed.
- [ ] No claims are made that build, tests, migrations, runtime, or Windows smoke tests succeeded; these are deferred.

## 16. FINAL RESPONSE FOR THIS STAGE

At the end of the implementation stage, report:
1. The major application features actually implemented.
2. The important files/projects created or changed.
3. Any incomplete requirements and known source-level concerns.
4. The current contents of `IMPLEMENTATION_STATUS.md` in concise form.
5. The exact next implementation task.
6. A clear statement that build, test, migration, runtime, and Windows demo validation have intentionally been deferred.

Do not run the deferred validation checklist during this stage.
