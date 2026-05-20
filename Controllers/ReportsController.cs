using Microsoft.AspNetCore.Mvc;
using PediTrack.Services;

namespace PediTrack.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService) => _reportService = reportService;

        // Reports hub page
        public IActionResult Index() => View();

        // Participant Report
        public async Task<IActionResult> Participants(int? studyId, string? status, DateTime? from, DateTime? to)
        {
            var vm = await _reportService.GetParticipantReportAsync(studyId, status, from, to);
            return View(vm);
        }

        // Visit Report
        public async Task<IActionResult> Visits(int? studyId, string? status, DateTime? from, DateTime? to)
        {
            var vm = await _reportService.GetVisitReportAsync(studyId, status, from, to);
            return View(vm);
        }

        // Consent Report
        public async Task<IActionResult> Consents(int? studyId, string? status)
        {
            var vm = await _reportService.GetConsentReportAsync(studyId, status);
            return View(vm);
        }

        // PDF Print version of Participant Report
        public async Task<IActionResult> ParticipantsPrint(int? studyId, string? status, DateTime? from, DateTime? to)
        {
            var rows = await _reportService.GetParticipantRowsAsync(studyId, status, from, to);
            return View(rows);
        }
    }
}
