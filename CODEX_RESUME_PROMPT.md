# Codex Resume Prompt — Continue After Usage Limit Reset

Resume work in the currently open VS Code workspace. Do not restart the project.

1. Read `AGENTS.md`, `PROJECT_SPEC.md`, `IMPLEMENTATION_STATUS.md`, `DEFERRED_WINDOWS_VALIDATION.md`, and the relevant portions of `README.md` and `DESIGN.md`.
2. Inspect Git status and the actual source files associated with the last recorded task. Preserve all existing work and user changes.
3. Compare the status file with the code. If the status is stale or inaccurate, correct it based on source evidence before continuing.
4. Continue from the **exact next task** recorded in `IMPLEMENTATION_STATUS.md`. Do not regenerate completed files or begin a different phase just because it is easier.
5. Keep the intended order: understand → implement all assessment flows → complete PostgreSQL persistence and results/scoring accuracy → defer build, tests, runtime, hardening, and demo validation to Windows.
6. During this implementation stage, do not run `dotnet restore`, `dotnet build`, `dotnet test`, migrations, app launches, publishing, or smoke tests. Do not claim any of them passed.
7. Implement the next coherent increment, wire it into the existing architecture, and update `IMPLEMENTATION_STATUS.md` with changed files, completion state, blockers, and the next task.
8. If usage is nearly exhausted, stop at a recoverable checkpoint: update the status file first, state precisely what remains, and avoid starting a large change you cannot safely finish.
