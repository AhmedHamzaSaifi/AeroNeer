# Design Notes

## Architecture and boundaries

The solution is divided into `Aeroneer.Core`, `Aeroneer.Infrastructure`, and `Aeroneer.App`. Core owns candidate/attempt contracts, registration validation, keyboard action identities, deterministic generators, simulation rules, and scoring. Infrastructure owns EF Core entities, PostgreSQL mapping, migrations, repositories, and lifecycle persistence. The WPF app owns XAML and MVVM presentation. Core has no WPF or EF Core dependency.

The app creates `IDbContextFactory<AppDbContext>` and uses a fresh context per repository/lifecycle operation. Local PostgreSQL is required; there is no cloud service or remote API. Migrations are checked in and the app attempts to apply them during startup, reporting an actionable generic error without displaying database credentials.

## Current implementation boundary

The current UI contains only candidate registration and persistence. It does not yet navigate to a dashboard or expose existing-candidate selection. Candidate, attempt, and module-progress persistence exist; detailed module-result entities and storage are incomplete. Assessment domain slices below are not yet connected to WPF flows or persistence.

## Registration choices

First name, last name, country, birth date, and e-mail are required. Other brief fields are optional. Birth dates in the future are rejected; e-mail uses the framework validation attribute. Country choices are generated from installed .NET globalization region data, without network access. The current form displays every required field and starts focus on the first field, Company/Institute. Pressing Enter in a text box moves focus to the next control and does not submit.

## Shared lifecycle and input

Attempts use `Guid` identifiers, UTC timestamps, explicit Practice/Assessment mode, and InProgress/Completed/Abandoned/Invalid status. Practice completion is tracked per candidate/module. Scored starts require that practice flag and are limited to three; Infrastructure uses a serializable transaction and a filtered unique index to claim an attempt. Abandoned and invalid scored rows remain counted. The store contract and implementation exist, but no module UI currently invokes them.

Input bindings use stable `InputAction` IDs and key-name strings. Suggested defaults use arrow keys, D1-D4, Enter, Escape, and Space; WASD is an alternate movement preset. Conflict detection checks duplicate keys among actions in a simultaneous group. WPF key capture, text-box protection, and held-key adapters remain unimplemented.

## Module defaults and formulas

- **Memory:** Seeded values use altitude 1,000-39,000 ft in 100- or 500-ft increments, speed 100-450 kt, heading 000-359, and radio 118.000-136.975 MHz in 0.025 MHz steps. Assessment round schedules are 1 hidden field/8 seconds (rounds 1-3), 2/7 seconds (4-6), 3/6 seconds (7-8), and 4/5 seconds (9-10). Round score is 80% field accuracy + 20% speed against a 15-second recall timeout; final score is weighted by requested hidden-field count. This is domain logic only; round UI and result persistence are incomplete.
- **Mathematics:** Questions are seeded across arithmetic, speed/distance/time, fuel, conversion, and ratio. Tolerances are explicit and currently zero for generated questions. Scoring uses 80% accuracy and 20% response speed against a 30-second per-question timeout. The intended assessment cap is 20 questions/10 minutes; the Core generator supports a requested count, but timed UI enforcement is not implemented.
- **Control:** Fixed simulation step is 1/60 second, with samples every 100 ms and position clamped to normalized [-1, 1]. Difficulty increases every 20 seconds to a maximum turbulence multiplier of 3. Target tolerance is 8% per axis. Overall score is 45% time-on-target, 25% normalized mean-deviation score, 15% smoothness, and 15% axis balance. Smoothness currently uses bounded second-difference motion change, not a jerk estimate. The simulation/rendering lifecycle and chart are incomplete.
- **Slalom:** Three scored runs of 90 seconds are defined. Seeded gates use a 0.36-0.62 normalized gap and bounded center transitions. A passed gate receives `100 * (1 - 0.65 * normalized center deviation)`; missed/collided gates receive zero. Run scores are the mean of scheduled gate scores; assessment score is the mean of exactly three runs. Scrolling simulation and feedback UI remain incomplete.
- **Orientation:** Questions are seeded and combine compass heading, RBI relative bearing, and artificial-horizon pitch/bank into four symbol options. Correct answers award one point; incorrect and unanswered questions receive no negative mark. Assessment duration is ten minutes. Instrument/symbol WPF rendering and answer-session persistence are incomplete.
- **Task Manager:** Seeded primary updates and alerts are scheduled independently, so they can overlap. New updates expire after 20 seconds; alerts after 10 seconds. Overall score weights primary task 60% and alerts 40%; within each component, correctness is 70% and speed is 30%. Practice/assessment durations are 30/180 seconds. The WPF event loop and persistence are incomplete.

These are experimental practice heuristics, not validated selection thresholds. Results must never be presented as a validated measure of pilot suitability.

## Database scope

The initial migration creates `Candidates`, `AssessmentAttempts`, and `CandidateModuleProgress`. Candidate deletion cascades to lifecycle rows. Scored attempt numbers have a filtered unique index per candidate/module/mode. Key-binding settings, detailed module results, history queries, exports, and transactional clear-data workflow still need implementation.

## Deferred work

Build, tests, migrations execution, database integration, WPF runtime, Windows smoke testing, hardening, and publishing are intentionally deferred to Windows 10/11. See [DEFERRED_WINDOWS_VALIDATION.md](DEFERRED_WINDOWS_VALIDATION.md).