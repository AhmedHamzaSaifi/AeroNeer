# Codex Start Prompt — Aeroneer

Work in the currently open VS Code workspace and repository root.

1. Read `AGENTS.md` and `PROJECT_SPEC.md` fully enough to understand the project rules and requirements.
2. Inspect the current repository, Git status, operating system, installed SDKs, and existing code. Preserve useful work; do not overwrite files blindly.
3. Create or update `IMPLEMENTATION_STATUS.md` and `DEFERRED_WINDOWS_VALIDATION.md` before making substantial changes.
4. Follow the implementation order in `PROJECT_SPEC.md`: Understand → implement the application and all module flows → complete PostgreSQL persistence, results, and scoring accuracy → defer hardening/tests/build/runtime/demo to Windows later.
5. Start implementation immediately after inspection; do not return only a plan.
6. During this stage, do not run `dotnet restore`, `dotnet build`, `dotnet test`, database migrations, the app, publishing, or Windows smoke tests. Do not spend the session on hardening or demo validation. Keep code testable and preserve the later validation checklist.
7. Prioritize durable progress. Work in dependency-ordered increments, avoid disconnected placeholder screens, and update `IMPLEMENTATION_STATUS.md` at every meaningful checkpoint.
8. Before stopping, write the exact completed/incomplete status, changed files, blockers, and the single best next task into `IMPLEMENTATION_STATUS.md`. Do not claim validation that was not performed.
