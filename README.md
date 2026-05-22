# PediTrack — Pediatric Clinical Research Management System

> A full-stack ASP.NET Core 10 MVC web application for managing pediatric clinical trial participants, study enrollments, visit schedules, and consent documentation at a fictional children's research hospital (Meridian Children's Hospital, Houston TX).

![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-10.0-512BD4?logo=dotnet)
![SQL Server](https://img.shields.io/badge/SQL_Server-Express-CC2927?logo=microsoftsqlserver)
![C#](https://img.shields.io/badge/C%23-13-239120?logo=csharp)
![EF Core](https://img.shields.io/badge/EF_Core-10-512BD4)
![xUnit](https://img.shields.io/badge/xUnit-35_tests-green)

---

## Overview

PediTrack is a research data management platform built to demonstrate real-world clinical research IT competencies: multi-role authentication, relational data modeling, T-SQL analytics, PDF/CSV reporting, and a guardian-facing portal for study participants.

The system supports two distinct user experiences:

- **Admin / Research Staff** — full CRUD across all 8 modules, an analytics dashboard with live charts, and user management
- **Guardians (Customer role)** — a scoped read-only portal showing their child's profile, study enrollments, visit schedule, and consent form status

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET Core 10 MVC + Razor Runtime Compilation |
| Language | C# 13 |
| ORM | Entity Framework Core 10 (code-first, `EnsureCreated`) |
| Database | SQL Server Express (`.\SQLEXPRESS`) |
| Auth | ASP.NET Core Identity + JWT Bearer (HttpOnly cookie) |
| Frontend | Bootstrap 5, Chart.js, Font Awesome 6 |
| PDF Export | Rotativa.AspNetCore (wkhtmltopdf) |
| Testing | xUnit + EF Core InMemory (35 tests) |

---

## Features

### 🔐 Authentication & Authorization
- JWT tokens stored in **HttpOnly cookies** — no localStorage exposure
- Role-based access: `Admin` and `Customer`
- 15-minute account lockout after 5 failed attempts
- RememberMe extends session from 8 hours to 7 days
- Automatic redirect to login on 401; Access Denied page on 403

### 🏥 Admin Portal
| Module | Capabilities |
|--------|-------------|
| **Participants** | Full CRUD — MRN, demographics, guardian info, enrollment status |
| **Studies** | Manage IRB-approved studies with PI assignment, phase, capacity tracking |
| **Visits** | Schedule and track visits by type (Baseline, Follow-up, Annual…), status, and assigned staff |
| **Consent Forms** | Version-controlled consent tracking with expiration alerts and re-consent flags |
| **Investigators** | PI and study staff directory |
| **Dashboard** | Chart.js: enrollment trends, visit completion rates, consent status breakdown |
| **Reports** | Filtered participant/visit/consent reports with CSV export and PDF print |
| **SQL Analytics** | Live data from a SQL Server view + 2 stored procedures with optional parameter filtering |
| **Data Dictionary** | Table/column metadata with allowed values and business rules |
| **User Management** | Create/edit/deactivate admin and guardian accounts; link guardians to participants |

### 👨‍👩‍👧 Guardian Portal (Customer Role)
- Scoped to the linked participant via `participantId` JWT claim
- My Child's Profile — demographics, MRN, guardian info
- My Studies — enrollment status, subject ID, PI, sponsor
- My Visits — upcoming visits with countdown, past visit history
- Consent Forms — active/expired status, re-consent alerts, version info

### 📊 T-SQL Analytics
Three SQL Server objects created automatically on startup:

| Object | Type | Purpose |
|--------|------|---------|
| `vw_StudyEnrollmentSummary` | View | Per-study aggregation: enrollment counts, visit completion rates, enrollment % of capacity |
| `usp_GetEnrollmentSummaryByStudy` | Stored Procedure | Optional `@StudyId` / `@Status` filters; `CASE`, `GROUP BY`, `ORDER BY` |
| `usp_GetParticipantVisitHistory` | Stored Procedure | Full visit timeline per participant with enrollment status join |

---

## Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server Express (`.\SQLEXPRESS`) — [download](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- (Optional) [wkhtmltopdf](https://wkhtmltopdf.org/downloads.html) for PDF export via Rotativa

### Run locally

```powershell
git clone https://github.com/Bharani010/PediTrack.git
cd PediTrack
dotnet run
```

Open **http://localhost:5000** — you'll land on the login page.

The app calls `EnsureCreated()` + `DbInitializer.Seed()` on first startup, which:
1. Creates the `PediTrackDB` database and all tables
2. Seeds 4 studies, 12 participants, 18 visits, 10 consent forms, 6 investigators
3. Creates the SQL view and 2 stored procedures
4. Seeds Identity roles (`Admin`, `Customer`) and demo user accounts

> **To reset to a clean state**: drop `PediTrackDB` in SSMS (or via `sqlcmd`) and restart the app.

### Hot reload (Razor changes without restart)

```powershell
dotnet watch run
```

---

## Demo Credentials

| Role | Email | Password |
|------|-------|----------|
| Admin | `admin@peditrack.com` | `Admin@PediTrack2024!` |
| Guardian (Ethan Brooks) | `p.brooks@email.com` | `Guardian@PediTrack2024!` |
| Guardian (Sofia Martinez) | `c.martinez@email.com` | `Guardian@PediTrack2024!` |
| Guardian (Aaliyah Washington) | `r.wash@email.com` | `Guardian@PediTrack2024!` |

---

## Architecture

```
PediTrack/
├── Controllers/          # Thin controllers — validate, call service, redirect
│   ├── AuthController            # Login / logout / JWT cookie
│   ├── AdminDashboardController  # Admin summary + user management
│   ├── CustomerDashboardController # Guardian portal (scoped by participantId claim)
│   ├── HomeController            # Main dashboard (charts)
│   ├── ParticipantsController
│   ├── StudiesController
│   ├── VisitsController
│   ├── ConsentFormsController
│   ├── InvestigatorsController
│   ├── ReportsController         # CSV export, PDF print, SQL Analytics
│   └── DataDictionaryController
│
├── Services/             # All business logic & DB queries
│   ├── ParticipantService / IParticipantService
│   ├── StudyService / IStudyService
│   ├── VisitService / IVisitService
│   ├── DashboardService / IDashboardService   # ~10 async queries → Chart.js data
│   ├── ReportService / IReportService         # Filtered reports + CSV builder
│   ├── SqlAnalyticsService / ISqlAnalyticsService  # View + SP queries via FromSqlRaw
│   └── UserManagementService / IUserManagementService
│
├── Models/
│   ├── AppUser.cs          # IdentityUser + DisplayName, ParticipantId, IsActive
│   ├── Participant.cs
│   ├── Study.cs
│   ├── Visit.cs
│   ├── ConsentForm.cs
│   ├── Investigator.cs
│   ├── StudyEnrollment.cs
│   ├── DataDictionaryEntry.cs
│   └── ViewModels/         # Dashboard, Report, SqlAnalytics view models
│
├── Data/
│   ├── ApplicationDbContext.cs   # IdentityDbContext<AppUser>; all EF relationships
│   └── DbInitializer.cs          # Seed data + CreateSqlObjects (view + SPs)
│
├── Views/
│   ├── Shared/
│   │   ├── _Layout.cshtml         # Admin shell (orange sidebar, chart imports)
│   │   └── _CustomerLayout.cshtml # Guardian shell (teal sidebar)
│   ├── Auth/                # Login, AccessDenied
│   ├── AdminDashboard/      # Summary, Users CRUD
│   ├── CustomerDashboard/   # Index, MyProfile, MyStudies, MyVisits, MyConsents
│   ├── Home/                # Analytics dashboard (Chart.js)
│   ├── Reports/             # Index, Participants, Visits, Consents, SqlAnalytics, Print
│   └── [Participants|Studies|Visits|ConsentForms|Investigators|DataDictionary]/
│
└── wwwroot/css/site.css    # All design tokens and component styles
```

### Key data relationships

```
Participant ──< StudyEnrollment >── Study
Participant ──< Visit >──────────── Study
Participant ──< ConsentForm >─────── Study
Study ──── Investigator (PI, SetNull on delete)
Visit.AssignedStaffId ──── Investigator (SetNull)
AppUser.ParticipantId ──── Participant (SetNull, optional)
```

### JWT flow

```
POST /Auth/Login
  → validate credentials + IsActive
  → GenerateJwtToken (claims: sub, email, role, displayName, participantId)
  → set PediTrack.Auth HttpOnly cookie (8h / 7d RememberMe)

All requests
  → JwtBearerEvents.OnMessageReceived reads cookie → populates User principal
  → [Authorize(Roles="Admin")] / [Authorize(Roles="Customer")] enforced per controller
  → 401 → redirect /Auth/Login?returnUrl=...
  → 403 → redirect /Auth/AccessDenied
```

---

## Running Tests

```powershell
cd PediTrack.Tests
dotnet test
```

35 tests covering:
- **ParticipantService** — CRUD, filtering, status transitions
- **ReportService** — participant/visit/consent report queries with filters
- **DashboardService** — chart data aggregations

All tests use **EF Core InMemory** provider — no SQL Server required.

---

## Configuration

`appsettings.json` key sections:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=PediTrackDB;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "SecretKey": "<32+ char secret>",
    "Issuer": "PediTrack",
    "Audience": "PediTrackUsers",
    "ExpiryHours": 8
  }
}
```

> For production: set `Secure = true` on the cookie (requires HTTPS), rotate the JWT secret key, and move it to environment variables or Azure Key Vault.

---

## Built With

- [ASP.NET Core 10](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [ASP.NET Core Identity](https://docs.microsoft.com/aspnet/core/security/authentication/identity)
- [Chart.js](https://www.chartjs.org/)
- [Bootstrap 5](https://getbootstrap.com/)
- [Font Awesome 6](https://fontawesome.com/)
- [Rotativa.AspNetCore](https://github.com/webprofusion/Rotativa.AspNetCore)
