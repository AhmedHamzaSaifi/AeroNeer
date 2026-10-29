# Aeroneer

Aeroneer is a Windows 10/11 WPF aviation aptitude practice platform. It is an educational tool, not an official COMPASS test. Scores are experimental and are not validated for pilot selection.

## Current implementation

The repository currently contains a .NET 10 solution with Core, WPF App, and PostgreSQL Infrastructure projects. Core contains deterministic domain slices for all six assessment modules, shared attempt/input contracts, and registration validation. The WPF app has a registration form that saves a candidate to a local PostgreSQL database. The initial EF migration currently covers candidate profiles, attempt records, and module practice progress.

The dashboard, candidate re-entry, assessment screens, module persistence, results/history/export, keyboard settings UI, and complete setup scripts are not yet implemented. This is not a complete or validated application.

## Architecture

```text
Aeroneer.App (WPF / MVVM) -> Aeroneer.Core
Aeroneer.App (WPF / MVVM) -> Aeroneer.Infrastructure (EF Core / PostgreSQL) -> Aeroneer.Core
```

Core does not reference WPF or EF Core. App uses short-lived `IDbContextFactory<AppDbContext>` contexts. The app applies checked-in migrations at startup and displays an actionable database error if the local database cannot be reached.

## Prerequisites

- Windows 10/11 x64 for WPF runtime use.
- Stable .NET 10 SDK.
- Docker Desktop with Compose, or a local PostgreSQL 17 installation.
- PostgreSQL is a separate local service; it is not embedded in Aeroneer.

The current development workspace was inspected on macOS arm64 without the .NET SDK. Windows compatibility and all runtime behavior remain unverified.

## Local PostgreSQL

From the repository root in PowerShell:

```powershell
Copy-Item .env.example .env
# Edit .env and replace the example password with a local-only password.
docker compose up -d postgres
$env:ConnectionStrings__Postgres = "Host=127.0.0.1;Port=5432;Database=aeroneer;Username=aeroneer;Password=YOUR_LOCAL_PASSWORD"
```

The Compose port is bound to localhost. For a standard PostgreSQL installation, create a local database/user and set `ConnectionStrings__Postgres` to the equivalent localhost connection string. Never commit `.env` or real credentials.

EF Core migrations are applied by the app at startup. When the complete implementation is ready, the expected Windows checks are:

```powershell
dotnet restore
dotnet build Aeroneer.sln -c Debug
dotnet test Aeroneer.sln -c Debug --no-build
dotnet run --project src/Aeroneer.App/Aeroneer.App.csproj
```

Tests and build/runtime checks are deliberately deferred. There is not yet a test project or a complete app flow. Do not treat these commands as passed validation.

## Modules and scoring source

Core source currently contains Memory, Mathematics, Control, Slalom, Orientation, and Task Manager domain behavior. Implemented formulas/defaults are recorded in [DESIGN.md](DESIGN.md). All modules still require WPF interaction, full attempt/result persistence, and focused tests before they meet product requirements.

## Data and privacy

Candidate profiles and initial lifecycle rows are stored in the local PostgreSQL database. Candidate data is not sent to external services. The current schema does not yet implement all module-detail tables or the clear-data UI. Do not use production candidate data until the complete deletion/recovery paths are implemented and validated.

## Keyboard input

Core defines stable action IDs, arrow/number defaults, an alternative WASD movement preset, and simultaneous-action conflict checks. The WPF rebinding and active-assessment input adapter are not implemented yet.

## Publishing

A Windows x64 self-contained publish command is expected to be:

```powershell
dotnet publish src/Aeroneer.App/Aeroneer.App.csproj -c Release -r win-x64 --self-contained true
```

This command has not been run. PostgreSQL remains a separate local prerequisite.

## Limitations

The application is at an early implementation stage. Builds, tests, migrations, database connections, WPF runtime, publishing, performance, and Windows smoke checks have not been run. The scoring logic is experimental and must not be used for pilot selection.
