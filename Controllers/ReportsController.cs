using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PediTrack.Data;
using PediTrack.Models.ViewModels;
using PediTrack.Services;

namespace PediTrack.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly IReportService        _reportService;
        private readonly ISqlAnalyticsService  _sqlAnalytics;
        private readonly ApplicationDbContext  _db;

        public ReportsController(
            IReportService reportService,
            ISqlAnalyticsService sqlAnalytics,
            ApplicationDbContext db)
        {
            _reportService = reportService;
            _sqlAnalytics  = sqlAnalytics;
            _db            = db;
        }

        // ── Hub ──────────────────────────────────────────────────────────────
        public IActionResult Index() => View();

        // ── Participant Report ───────────────────────────────────────────────
        public async Task<IActionResult> Participants(int? studyId, string? status, DateTime? from, DateTime? to)
        {
            var vm = await _reportService.GetParticipantReportAsync(studyId, status, from, to);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ExportParticipantsCsv(int? studyId, string? status, DateTime? from, DateTime? to)
        {
            var csv      = await _reportService.ExportParticipantsCsvAsync(studyId, status, from, to);
            var fileName = $"participants-{DateTime.Today:yyyy-MM-dd}.csv";
            return File(Encoding.UTF8.GetBytes(csv), "text/csv", fileName);
        }

        // ── Visit Report ─────────────────────────────────────────────────────
        public async Task<IActionResult> Visits(int? studyId, string? status, DateTime? from, DateTime? to)
        {
            var vm = await _reportService.GetVisitReportAsync(studyId, status, from, to);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ExportVisitsCsv(int? studyId, string? status, DateTime? from, DateTime? to)
        {
            var csv      = await _reportService.ExportVisitsCsvAsync(studyId, status, from, to);
            var fileName = $"visits-{DateTime.Today:yyyy-MM-dd}.csv";
            return File(Encoding.UTF8.GetBytes(csv), "text/csv", fileName);
        }

        // ── Consent Report ───────────────────────────────────────────────────
        public async Task<IActionResult> Consents(int? studyId, string? status)
        {
            var vm = await _reportService.GetConsentReportAsync(studyId, status);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ExportConsentsCsv(int? studyId, string? status)
        {
            var csv      = await _reportService.ExportConsentsCsvAsync(studyId, status);
            var fileName = $"consents-{DateTime.Today:yyyy-MM-dd}.csv";
            return File(Encoding.UTF8.GetBytes(csv), "text/csv", fileName);
        }

        // ── Print (PDF/browser) ──────────────────────────────────────────────
        public async Task<IActionResult> ParticipantsPrint(int? studyId, string? status, DateTime? from, DateTime? to)
        {
            var rows = await _reportService.GetParticipantRowsAsync(studyId, status, from, to);
            return View(rows);
        }

        // ── SQL Analytics — view + stored procedure results ──────────────────
        public async Task<IActionResult> SqlAnalytics(int? studyId, string? studyStatus)
        {
            var vm = new SqlAnalyticsViewModel
            {
                FilterStudyId     = studyId,
                FilterStudyStatus = studyStatus
            };

            // Check whether the view exists; create all SQL objects if they are missing.
            bool objectsReady = await EnsureSqlObjectsAsync();

            if (objectsReady)
            {
                try
                {
                    vm.Summary   = await _sqlAnalytics.GetStudyEnrollmentSummaryAsync();
                    vm.SpResults = await _sqlAnalytics.GetEnrollmentSummaryByStudyAsync(studyId, studyStatus);
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Query failed: {ex.Message}";
                }
            }

            ViewBag.Studies = await _db.Studies.OrderBy(s => s.StudyName).ToListAsync();
            return View(vm);
        }

        // Returns true when the SQL view exists (creating it first if it was absent).
        private async Task<bool> EnsureSqlObjectsAsync()
        {
            try
            {
                // Single lightweight check against sys.objects
                var conn = _db.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                    await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText =
                    "SELECT COUNT(*) FROM sys.objects " +
                    "WHERE name = 'vw_StudyEnrollmentSummary' AND type = 'V'";
                var count = (int)(await cmd.ExecuteScalarAsync())!;

                if (count == 0)
                {
                    // Objects missing — create them now (this also runs on first startup)
                    DbInitializer.CreateSqlObjects(_db);
                }

                return true;
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"SQL object setup failed: {ex.Message}";
                return false;
            }
        }
    }
}
