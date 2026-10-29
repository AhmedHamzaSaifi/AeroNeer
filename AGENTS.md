# Aeroneer — Codex Repository Instructions

These instructions apply to all Codex work in this repository.

## Source of truth
- `PROJECT_SPEC.md` is the authoritative product specification.
- `IMPLEMENTATION_STATUS.md` is the authoritative record of current progress and next steps.
- `DEFERRED_WINDOWS_VALIDATION.md` is the authoritative checklist for the later Windows validation stage.
- `README.md` and `DESIGN.md` must reflect the actual implementation, not intended or imagined behavior.

## Current delivery strategy
Follow this order:
1. Understand the repository and requirements.
2. Implement the application and all module flows.
3. Complete PostgreSQL persistence, results, and scoring accuracy.
4. Defer test execution, builds, runtime launches, hardening, publishing, and demo validation to a real Windows 10/11 environment.

During the current implementation stage, do not run builds, tests, migrations, app launches, publishing, or Windows smoke tests. Do not claim that any of these have passed. Keep code testable and document validation steps for later.

## Coding rules
- Preserve existing user changes and inspect Git status before edits.
- Keep Core independent of WPF and EF Core.
- Keep business logic, scoring, generators, and persistence out of XAML and view code-behind.
- Use keyboard-first interaction for navigation and assessments.
- Use real service wiring and real persisted results; no fake scoring, fake successful saves, or dead buttons.
- Avoid unnecessary abstractions and unused dependencies.
- Do not commit secrets or unrelated changes.
- Update `IMPLEMENTATION_STATUS.md` after each meaningful milestone and before stopping.
- If interrupted, leave the repository in a recoverable state and record the exact next action.
