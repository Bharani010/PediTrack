using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

            try
            {
                vm.Summary   = await _sqlAnalytics.GetStudyEnrollmentSummaryAsync();
                vm.SpResults = await _sqlAnalytics.GetEnrollmentSummaryByStudyAsync(studyId, studyStatus);
            }
            catch
            {
                // SQL objects may not exist (e.g. first run before view/SPs were created, or Express edition).
                TempData["Warning"] = "SQL Analytics objects are not yet available. The database view and stored procedures will be created on the next application restart.";
            }

            ViewBag.Studies = await _db.Studies.OrderBy(s => s.StudyName).ToListAsync();
            return View(vm);
        }
    }
}
