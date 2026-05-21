# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```powershell
# Run the application (port 5000 by default)
dotnet run

# Build only
dotnet build

# Run with watch (hot reload via Razor Runtime Compilation)
dotnet watch run

# Apply EF Core migrations (if switching from EnsureCreated to migrations)
dotnet ef migrations add <MigrationName>
dotnet ef database update

# Drop and re-seed the database (delete PediTrackDB in SSMS, then restart app)
# App calls db.Database.EnsureCreated() + DbInitializer.Seed() on every startup
```

## Database

- **SQL Server Express** via `.\SQLEXPRESS`, database name `PediTrackDB`.
- **No EF Migrations** — `Program.cs` calls `db.Database.EnsureCreated()` then `DbInitializer.Seed()` on startup. Schema is code-first from models; to reset, drop the DB in SSMS and restart.
- **Seed guard**: each entity type checks `if (!db.X.Any())` before inserting, so seeds are idempotent.
- `FullName` and `Age` on `Participant`, and `FullName` on `Investigator`, are `[NotMapped]` / `Ignore()`d computed properties — never map them to columns.

## Architecture

### Layer structure
```
Controllers/   — thin: validate ModelState, call service, set TempData, redirect
Services/      — all business logic and DB queries (interface + implementation pairs)
Models/        — EF entity classes; ViewModels/ for Dashboard and Reports
Data/          — ApplicationDbContext (relationships/indexes) + DbInitializer (seed)
Views/         — Razor views per controller; _Layout.cshtml is the single shell
wwwroot/css/   — site.css holds all custom design tokens and component styles
```

### Key relationships (ApplicationDbContext)
- `Participant` ← (cascade) → `Visit`, `ConsentForm`, `StudyEnrollment`
- `Study` → `Investigator` (PI, SetNull on delete); `Study` ← (Restrict) → `Visit`, `ConsentForm`, `StudyEnrollment`
- `StudyEnrollment` has a unique composite index on `(ParticipantId, StudyId)`
- `Visit.AssignedStaffId` → `Investigator` (SetNull on delete)

### Service pattern
Every domain area has `I{X}Service` / `{X}Service`. Controllers inject the interface. `DashboardService` is the most complex — it runs ~10 async DB queries and builds chart-ready `List<string>` / `List<int>` for Chart.js. `ReportService` exposes three report queries plus `GetParticipantRowsAsync` for the print view.

### Views conventions
- All pages use `_Layout.cshtml` which provides: sidebar nav, topbar, TempData flash messages (auto-dismissed after 5s), Chart.js, Bootstrap 5, jQuery.
- CSS design tokens live in `--primary` (orange), `--accent2`, `--success`, `--danger`, `--warning`, `--text-muted`, etc. Use these, not raw colours.
- Badge classes follow status values: `badge-active`, `badge-completed`, `badge-withdrawn`, `badge-scheduled`, `badge-missed`, `badge-cancelled`, `badge-expired`, `badge-recruiting`, `badge-closed`.
- Form pages use `.form-section` / `.form-section-title` / `.form-grid-2` / `.form-grid-3` / `.form-group` / `.form-label` / `.required`.
- Detail pages use `.detail-grid` (two-column), `.info-grid`, `.info-item` for label/value pairs.
- Card components: `.card` > `.card-header` + `.card-body` + `.card-footer`.

### No authentication (yet)
The app currently has no ASP.NET Core Identity or any auth middleware. All routes are publicly accessible. When adding roles, the planned split is **Admin** (full CRUD + user management) and **Staff/User** (read + limited write).

## Current status of modules
All 8 controllers have complete view sets (Index/Details/Create/Edit/Delete):
`Participants`, `Studies`, `Visits`, `ConsentForms`, `Investigators`, `Reports`, `DataDictionary`, `Home` (dashboard only).

## Rotativa (PDF)
`Rotativa.AspNetCore` is installed for print/PDF reports. Setup is wrapped in a try/catch in `Program.cs` — if `wkhtmltopdf` is not installed the app still runs and falls back to browser print. The print view is `Views/Reports/ParticipantsPrint.cshtml`.
