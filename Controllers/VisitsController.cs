using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PediTrack.Data;
using PediTrack.Models;
using PediTrack.Services;

namespace PediTrack.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VisitsController : Controller
    {
        private readonly IVisitService _service;
        private readonly ApplicationDbContext _db;

        public VisitsController(IVisitService service, ApplicationDbContext db)
        {
            _service = service;
            _db = db;
        }

        public async Task<IActionResult> Index(int? participantId, int? studyId, string? status)
        {
            ViewBag.Status = status ?? "All";
            ViewBag.FilterParticipantId = participantId;
            ViewBag.FilterStudyId = studyId;
            var visits = await _service.GetAllAsync(participantId, studyId, status);
            return View(visits);
        }

        public async Task<IActionResult> Details(int id)
        {
            var visit = await _service.GetByIdAsync(id);
            if (visit == null) return NotFound();
            return View(visit);
        }

        public async Task<IActionResult> Create(int? participantId)
        {
            await PopulateDropdowns(participantId);
            var visit = new Visit
            {
                ScheduledDate = DateTime.Today.AddDays(1),
                ParticipantId = participantId ?? 0
            };
            return View(visit);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Visit visit)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(visit.ParticipantId);
                return View(visit);
            }
            await _service.CreateAsync(visit);
            TempData["Success"] = "Visit scheduled successfully.";
            return visit.ParticipantId > 0
                ? RedirectToAction("Details", "Participants", new { id = visit.ParticipantId })
                : RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var visit = await _service.GetByIdAsync(id);
            if (visit == null) return NotFound();
            await PopulateDropdowns(visit.ParticipantId, visit.VisitType, visit.Location);
            return View(visit);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Visit visit)
        {
            if (id != visit.VisitId) return BadRequest();
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(visit.ParticipantId, visit.VisitType, visit.Location);
                return View(visit);
            }
            await _service.UpdateAsync(visit);
            TempData["Success"] = "Visit updated.";
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var visit = await _service.GetByIdAsync(id);
            if (visit == null) return NotFound();
            return View(visit);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            TempData["Success"] = "Visit removed.";
            return RedirectToAction(nameof(Index));
        }

        // AJAX: Mark visit complete
        [HttpPost]
        public async Task<IActionResult> MarkComplete(int id)
        {
            var visit = await _service.GetByIdAsync(id);
            if (visit == null) return NotFound();
            visit.Status = "Completed";
            visit.CompletedDate = DateTime.Now;
            await _service.UpdateAsync(visit);
            return Json(new { success = true });
        }

        private async Task PopulateDropdowns(int? selectedParticipantId = null, string? selectedVisitType = null, string? selectedLocation = null)
        {
            // Show ALL participants and studies so existing records with any status always appear in dropdowns
            ViewBag.Participants = new SelectList(
                await _db.Participants.OrderBy(p => p.LastName).ThenBy(p => p.FirstName).ToListAsync(),
                "ParticipantId", "FullName", selectedParticipantId);

            ViewBag.Studies = new SelectList(
                await _db.Studies.OrderBy(s => s.StudyName).ToListAsync(),
                "StudyId", "StudyName");

            ViewBag.Staff = new SelectList(
                await _db.Investigators.Where(i => i.Status == "Active").OrderBy(i => i.LastName).ToListAsync(),
                "InvestigatorId", "FullName");

            var visitTypes = new[] { "Screening", "Baseline", "Follow-up", "Annual", "Final", "Unscheduled" };
            ViewBag.VisitTypes = new SelectList(visitTypes, selectedVisitType);

            ViewBag.Statuses = new SelectList(new[] { "Scheduled", "Completed", "Missed", "Cancelled", "Rescheduled" });

            var locations = new[] {
                "Clinic A", "Clinic B", "Cardiology Lab", "Neuro Unit",
                "Oncology Ward", "Surgery Suite", "Infusion Center",
                "Outpatient / General", "Telehealth / Remote"
            };
            // If existing location isn't in the list, add it so editing never silently changes the value
            if (!string.IsNullOrWhiteSpace(selectedLocation) && !locations.Contains(selectedLocation))
                locations = locations.Append(selectedLocation).ToArray();
            ViewBag.Locations = new SelectList(locations, selectedLocation);
        }
    }
}
