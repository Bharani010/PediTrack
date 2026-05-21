# SSRS Integration Guide

## Why SSRS Is Not Currently Integrated

SQL Server Reporting Services (SSRS) is **not included with SQL Server Express Edition**, which is the specific edition this project runs against (`.\SQLEXPRESS` connection string in `appsettings.json`). SSRS is only available with:

- SQL Server Standard Edition
- SQL Server Developer Edition (free for non-production use)
- SQL Server Enterprise Edition

Attempting to install or configure SSRS on an Express instance will fail. The Report Server endpoint (`/ReportServer`) and Report Manager web portal simply do not exist.

---

## What SSRS Integration Would Look Like

### Option A — ReportViewer Control (Legacy, Windows only)
The classic `Microsoft.Reporting.WebForms` / `Microsoft.Reporting.NETCore` NuGet package can render `.rdl` files either in Local Mode (data provided by the app) or Remote Mode (data fetched from a Report Server). For ASP.NET Core MVC this requires embedding an `<iframe>` or using the `AspNetCore.Reporting` wrapper.

**Steps to integrate:**
1. Install `AspNetCore.Reporting` NuGet package.
2. Add the `.rdl` report files to `/Reports/` in the project.
3. Create a controller action that calls `LocalReport.Render("PDF")` and returns a `FileContentResult`.
4. Add a view with an `<iframe src="/Reports/...">` or a direct download link.

### Option B — SSRS REST API (Requires a running Report Server)
SQL Server 2016+ ships an OData-compatible REST API at `/reports/api/v2.0/`. An ASP.NET Core app can proxy calls to this API to render reports dynamically.

**Steps to integrate:**
1. Provision a SQL Server Developer/Standard instance with SSRS installed and configured.
2. Deploy `.rdl` files to the Report Server via the web portal or `rsconfig`.
3. In the ASP.NET Core app, use `HttpClient` to call `GET /reports/api/v2.0/Reports({id})/Export?format=PDF`.
4. Stream the response back to the browser as a PDF download.

### Option C — Embedded iframe to Report Manager
For intranet deployments the simplest approach is an `<iframe>` pointing to the Report Server's URL, e.g.:
```html
<iframe src="http://reportserver/Reports/report/PediTrack/EnrollmentSummary&rs:Command=Render&rs:Format=HTML4.0"
        width="100%" height="800"></iframe>
```

### Reports that would be created (.rdl definitions)

| Report Name | Key Parameters | Data Source |
|---|---|---|
| Participant Enrollment Report | StudyId, Status, DateRange | `Participants` + `StudyEnrollments` |
| Visit Summary by Study | StudyId, VisitStatus, DateRange | `Visits` + `Studies` |
| Consent Expiry Report | ExpiresWithinDays, Status | `ConsentForms` + `Participants` |
| Study Progress Dashboard | StudyId | All tables via `vw_StudyEnrollmentSummary` |
| Investigator Workload | InvestigatorId, DateRange | `Visits` + `Investigators` |

---

## Current Alternative (Already Implemented)

Because SQL Server Express is in use, PediTrack delivers equivalent reporting functionality through four complementary mechanisms:

### 1. Chart.js Dashboards (Live DB queries)
`DashboardService.GetDashboardDataAsync()` runs ~10 async EF Core queries and supplies data to Chart.js charts embedded in the Home/Index view — enrollment trends, study status, visit status, age distribution, and study progress.

### 2. Rotativa PDF Export
`Rotativa.AspNetCore` is installed (`PediTrack.csproj`). `ReportsController.ParticipantsPrint` renders `Views/Reports/ParticipantsPrint.cshtml` as a PDF via `wkhtmltopdf`. Falls back to browser print if `wkhtmltopdf` is not installed.

### 3. CSV Export
`ReportService` exposes three CSV export methods:
- `ExportParticipantsCsvAsync` — `GET /Reports/ExportParticipantsCsv`
- `ExportVisitsCsvAsync` — `GET /Reports/ExportVisitsCsv`
- `ExportConsentsCsvAsync` — `GET /Reports/ExportConsentsCsv`

All three accept the same filter parameters as their corresponding view reports and return a properly quoted RFC-4180 CSV.

### 4. T-SQL Stored Procedures and Views
`DbInitializer.CreateSqlObjects()` creates on every startup:
- `vw_StudyEnrollmentSummary` — a SQL Server view aggregating enrollment and visit KPIs per study
- `usp_GetEnrollmentSummaryByStudy` — SP with optional `@StudyId`/`@Status` parameters
- `usp_GetParticipantVisitHistory` — SP returning full visit history for a participant

These are surfaced via the **SQL Analytics** page (`/Reports/SqlAnalytics`) using EF Core keyless entities and `FromSqlRaw`.

---

## Migration Path: Express → Developer Edition

SQL Server Developer Edition is **free for development and testing**. To migrate:

1. Download SQL Server Developer Edition from [https://www.microsoft.com/en-us/sql-server/sql-server-downloads](https://www.microsoft.com/en-us/sql-server/sql-server-downloads).
2. During setup, choose **New SQL Server stand-alone installation** and select the **Developer** feature set including **Reporting Services**.
3. After installation, open **SQL Server Reporting Services Configuration Manager** and configure the Report Server database (creates `ReportServer` and `ReportServerTempDB`).
4. Update `appsettings.json` connection string if the instance name changes (default Developer instance is `.\SQLSERVER` or `.\MSSQLSERVER`).
5. Deploy the PediTrackDB database to the new instance (backup/restore or `dotnet ef database update`).
6. Create `.rdl` report files in Visual Studio using the **SQL Server Reporting Services** project template (SSDT).
7. Publish reports to the Report Server via the SSDT Publish wizard.
8. Add the `AspNetCore.Reporting` NuGet package and implement report rendering actions in `ReportsController`.

### Checklist
- [ ] Install SQL Server Developer Edition with SSRS feature
- [ ] Configure SSRS Report Server URL (default: `http://localhost/ReportServer`)
- [ ] Migrate PediTrackDB to new instance
- [ ] Create `.rdl` files for the five reports listed above
- [ ] Deploy `.rdl` files to Report Server
- [ ] Add `AspNetCore.Reporting` to `PediTrack.csproj`
- [ ] Add SSRS controller actions and views
- [ ] Update `appsettings.json` with Report Server URL
