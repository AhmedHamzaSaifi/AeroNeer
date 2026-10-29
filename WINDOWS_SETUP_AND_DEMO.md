# Windows Setup and Demo Runbook — Aeroneer

This document provides exact, verified instructions to configure, launch, demonstrate, and verify Aeroneer on a local Windows 10/11 workstation.

---

## 1. Prerequisites

* **Operating System**: Windows 10 or 11 (x64).
* **.NET SDK**: .NET SDK `10.0.403` or compatible stable .NET 10 SDK (`dotnet --list-sdks`).
* **PostgreSQL**: PostgreSQL 17 or 18 running locally as a Windows service or via Docker container.
* **Working Tree**: Repository cloned and restored on the local disk.

---

## 2. Configuration & Safe Handling of Secrets

Aeroneer reads its PostgreSQL connection string via the standard .NET Generic Host configuration key `ConnectionStrings:Postgres` (mapped to the environment variable `ConnectionStrings__Postgres`).

1. Ensure `.env` exists at the repository root (copied from `.env.example`).
2. Verify `.env` is ignored by Git (`git status --ignored` confirms `.env` is never committed).
3. Confirm that `.env` contains:
   * `POSTGRES_DB=Aeroneer`
   * `POSTGRES_USER=postgres` (or your local user)
   * `POSTGRES_PASSWORD=<your-local-password>`
   * `ConnectionStrings__Postgres=Host=127.0.0.1;Port=5433;Database=Aeroneer;Username=postgres;Password=<your-local-password>`
4. To check configuration safely without printing secrets:
   ```powershell
   Get-Content .env | ForEach-Object { $k = $_.Split('=')[0].Trim(); if ($k -and !$k.StartsWith('#')) { Write-Host "Configured key: $k" } }
   ```

---

## 3. Database Initialization & Status Check

1. Verify that the local PostgreSQL service is running:
   ```powershell
   Get-Service *postgres*
   ```
2. Verify that the `Aeroneer` database and tables exist:
   ```powershell
   $pass = (Get-Content .env | Where-Object { $_.StartsWith('POSTGRES_PASSWORD=') }).Split('=', 2)[1].Trim()
   $env:PGPASSWORD = $pass
   & 'C:\Program Files\PostgreSQL\17\bin\psql.exe' -h 127.0.0.1 -p 5433 -U postgres -d Aeroneer -c '\dt'
   ```
   *Expected output: 4 tables (`__EFMigrationsHistory`, `Candidates`, `AssessmentAttempts`, `CandidateModuleProgress`).*
3. If setting up on a fresh machine or new database catalog, apply the schema migration:
   * The application automatically validates and applies pending migrations at startup via `await dbContext.Database.MigrateAsync()`.

---

## 4. Exact Application Launch Command

In PowerShell from the repository root:

```powershell
# 1. Load connection string into current process environment
Get-Content .env | ForEach-Object {
    $line = $_.Trim()
    if ($line -and !$line.StartsWith('#')) {
        $parts = $line.Split('=', 2)
        if ($parts.Length -eq 2) {
            [System.Environment]::SetEnvironmentVariable($parts[0].Trim(), $parts[1].Trim(), 'Process')
        }
    }
}

# 2. Build and run the WPF application
dotnet run --project src/Aeroneer.App/Aeroneer.App.csproj -c Debug --no-build
```

---

## 5. Shortest Reliable Demo Sequence

### Strongest Working Functionality
The current working demonstration focuses on the **Keyboard-First Candidate Registration Flow**, input validation, and local PostgreSQL persistence.

1. **Launch the Application**:
   * Execute the launch command above.
   * Observe that the window titled **Aeroneer | Candidate registration** opens centered on the screen.
2. **Keyboard Navigation & Validation**:
   * Notice that focus automatically starts in the **Company / institute** field.
   * Type a test value (e.g. `Flight Academy`).
   * Press <kbd>Enter</kbd> to observe keyboard traversal to **First name**.
   * Leave **First name** blank and press <kbd>Enter</kbd> to see validation feedback:
     * *Validation text indicates required fields (`First name is required`, `Last name is required`, etc.).*
     * The **Save candidate** button remains disabled while required fields are missing.
3. **Complete Synthetic Candidate Registration**:
   * Fill in valid synthetic test data:
     * **First name**: `Jane`
     * **Last name**: `Doe`
     * **Country**: Select `US` or `DE` from the drop-down.
     * **Birth date**: Select any valid date in the past.
     * **Email**: `jane.doe@flighttest.local`
4. **Persist the Candidate**:
   * Once valid inputs are entered, click or trigger **Save candidate**.
   * Observe the confirmation message:
     * *"Candidate profile saved to the local database."*
5. **Verify Persistence Survives Restart**:
   * Close the application.
   * Verify using `psql` that the candidate record exists in the local database:
     ```powershell
     $pass = (Get-Content .env | Where-Object { $_.StartsWith('POSTGRES_PASSWORD=') }).Split('=', 2)[1].Trim()
     $env:PGPASSWORD = $pass
     & 'C:\Program Files\PostgreSQL\17\bin\psql.exe' -h 127.0.0.1 -p 5433 -U postgres -d Aeroneer -c 'SELECT "Id", "FirstName", "LastName", "Email", "Country" FROM "Candidates" ORDER BY "CreatedAtUtc" DESC LIMIT 1;'
     ```
   * Relaunch the application; it starts cleanly without errors.

---

## 6. Known Limitations & Unverified Modules

* **Assessment Module Presentation Views**:
  * Core domain algorithms, schedules, question generators, and scoring models exist in `Aeroneer.Core` for all six modules (Control, Slalom, Orientation, Mathematics, Memory, Task Manager).
  * However, **interactive XAML presentation screens, timers, and assessment UI controls have not yet been implemented in `Aeroneer.App`**.
  * The current UI does not transition past the registration screen to a module selection dashboard.
* **Attempt History and Export UI**:
  * Result aggregation views, performance charts, and CSV/JSON export screens are not yet implemented in the GUI.

---

## 7. Troubleshooting

* **Modal Error: "Local database required"**:
  * Cause: `ConnectionStrings__Postgres` environment variable is not set in the running shell.
  * Solution: Re-run Step 1 of the launch command to load environment variables from `.env`.
* **Modal Error: "Database unavailable"**:
  * Cause: PostgreSQL service is stopped or port/password mismatch.
  * Solution: Check `Get-Service *postgres*`, verify port (5433 vs 5432), and ensure password matches `.env`.

