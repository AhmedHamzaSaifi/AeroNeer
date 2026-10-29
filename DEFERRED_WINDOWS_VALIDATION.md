# Deferred Windows Validation, Testing, Hardening, and Demo Plan

This document intentionally preserves validation work for a later stage in a real Windows 10/11 environment. Do not execute these steps during the implementation-first stage.

## Preconditions
- The source implementation is substantially complete.
- PostgreSQL local setup and EF Core migrations source are present.
- `IMPLEMENTATION_STATUS.md` lists remaining incomplete requirements.
- Use a clean working tree or clearly understand all existing changes before validation.
- Current inspection (2026-10-29): development host is macOS arm64, `dotnet` is not installed, and the workspace has no `.git` metadata. This is an environment observation, not a validation result.

## Stage A — Restore and build
- [ ] Install/use a compatible stable .NET SDK.
- [ ] Run `dotnet --info`.
- [ ] Run `dotnet restore`.
- [ ] Run `dotnet build Aeroneer.sln -c Debug`.
- [ ] Fix compile errors and repeat until successful.
- [ ] Run `dotnet build Aeroneer.sln -c Release`.
- [ ] Record exact commands and outcomes.

## Stage B — Tests
- [ ] Implement Core unit tests for lifecycle, attempt limits, practice prerequisites, score aggregation, and invalid/abandoned attempts.
- [ ] Test Memory generators, ranges/increments, parsing, hidden-field schedule, accuracy/speed/difficulty scoring, timeouts, edge cases, and deterministic seeds.
- [ ] Test Control simulation determinism, clamping, held-key state, timing metrics, deviation, RMS, corrections, smoothness, axis balance, and weighted score.
- [ ] Test Slalom gate determinism, bounds, center-versus-edge scoring, penalties, and three-run aggregation.
- [ ] Test Orientation question validity, deterministic ordering, answer capture, timing, and scoring.
- [ ] Test Mathematics answer generation, formulas, numeric tolerances, speed/accuracy scoring, and empty-answer edge cases.
- [ ] Test Task Manager setting transfers, alert handling, response times, event scheduling determinism, and scoring.
- [ ] Test configuration, serialization, and important EF mappings.
- [ ] Run `dotnet test Aeroneer.sln -c Debug`.
- [ ] Run PostgreSQL integration tests when a test connection is configured; do not substitute EF InMemory for PostgreSQL-specific behavior.
- [ ] Record actual results and fix failures.

## Stage C — PostgreSQL integration
- [ ] Start local PostgreSQL using Docker Compose or a locally installed PostgreSQL service.
- [ ] Configure credentials locally; never commit secrets.
- [ ] Apply EF Core migrations using the documented command.
- [ ] Verify candidate create/update/reload.
- [ ] Verify attempt start, completion, abandonment, and detailed module-result persistence/reload.
- [ ] Verify the three-start attempt limit under repeated and concurrent starts.
- [ ] Verify practice prerequisite and practice exclusion from official summaries.
- [ ] Verify history, filters, clear-data transaction/cascade, and error handling.
- [ ] Verify JSON serialization/deserialization for structured detail payloads.

## Stage D — Windows runtime smoke test
- [ ] Launch the WPF application on Windows 10/11.
- [ ] Test registration and returning-candidate flow.
- [ ] Test keyboard-only navigation, focus, rebinding, conflict detection, pause, focus loss, and exit confirmation.
- [ ] Complete a practice trial and scored assessment for every module.
- [ ] Check Control/Slalom animation timing and lifecycle cleanup.
- [ ] Check result metrics, charts, history, combined scores, JSON export, and print flow.
- [ ] Test empty states, database-disconnected state, invalid configuration, and recovery paths.
- [ ] Verify offline runtime after dependencies are installed.
- [ ] Record screenshots only if actually captured; do not use fake/demo values as real results.

## Stage E — Hardening and publishing
- [ ] Review logs for accidental PII and secrets.
- [ ] Review accessibility, keyboard focus, resizing, responsiveness, and UI binding errors.
- [ ] Review data deletion, transaction boundaries, attempt concurrency, and error recovery.
- [ ] Publish self-contained Windows x64 build if appropriate.
- [ ] Verify published app against a local PostgreSQL prerequisite.
- [ ] Document actual tested Windows version, .NET SDK, PostgreSQL version, commands, results, and known limitations.

## Final reporting rule
Every check must be marked `Passed`, `Failed`, `Blocked`, or `Not run`, with evidence. Never mark a check passed without executing it and observing the result.

## Current validation state
- Restore/build/tests, migrations, runtime, publishing, and Windows smoke checks: Not run by design.
- Blocker for local compilation: .NET SDK unavailable in the current macOS workspace. Perform the deferred checks on a real Windows 10/11 environment with a compatible stable .NET SDK.
