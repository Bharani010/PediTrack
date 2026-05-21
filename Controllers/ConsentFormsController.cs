using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PediTrack.Data;
using PediTrack.Models;

namespace PediTrack.Controllers
{
    public class ConsentFormsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ConsentFormsController(ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Index(string? status)
        {
            ViewBag.Status = status ?? "All";
            var query = _db.ConsentForms
                .Include(c => c.Participant)
                .Include(c => c.Study)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) && status != "All")
                query = query.Where(c => c.Status == status);

            return View(await query.OrderBy(c => c.ExpirationDate).ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var consent = await _db.ConsentForms
                .Include(c => c.Participant)
                .Include(c => c.Study)
                .FirstOrDefaultAsync(c => c.ConsentFormId == id);
            if (consent == null) return NotFound();
            return View(consent);
        }

        public async Task<IActionResult> Create(int? participantId)
        {
            await PopulateDropdowns(participantId);
            return View(new ConsentForm
            {
                ConsentDate = DateTime.Today,
                ExpirationDate = DateTime.Today.AddYears(2),
                ParticipantId = participantId ?? 0
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConsentForm consent)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(consent.ParticipantId);
                return View(consent);
            }
            _db.ConsentForms.Add(consent);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Consent form recorded successfully.";
            return consent.ParticipantId > 0
                ? RedirectToAction("Details", "Participants", new { id = consent.ParticipantId })
                : RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var consent = await _db.ConsentForms
                .Include(c => c.Participant)
                .Include(c => c.Study)
                .FirstOrDefaultAsync(c => c.ConsentFormId == id);
            if (consent == null) return NotFound();
            await PopulateDropdowns(consent.ParticipantId);
            return View(consent);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ConsentForm consent)
        {
            if (id != consent.ConsentFormId) return BadRequest();
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(consent.ParticipantId);
                return View(consent);
            }

            // Fetch existing record and update only scalar fields
            var existing = await _db.ConsentForms.FindAsync(id);
            if (existing == null) return NotFound();
            existing.ParticipantId      = consent.ParticipantId;
            existing.StudyId            = consent.StudyId;
            existing.ConsentDate        = consent.ConsentDate;
            existing.ExpirationDate     = consent.ExpirationDate;
            existing.Version            = consent.Version;
            existing.SignedByGuardian   = consent.SignedByGuardian;
            existing.WitnessName        = consent.WitnessName;
            existing.Status             = consent.Status;
            existing.ReconsentRequired  = consent.ReconsentRequired;
            existing.Notes              = consent.Notes;

            await _db.SaveChangesAsync();
            TempData["Success"] = "Consent form updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var c = await _db.ConsentForms.Include(x => x.Participant).Include(x => x.Study).FirstOrDefaultAsync(x => x.ConsentFormId == id);
            if (c == null) return NotFound();
            return View(c);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var c = await _db.ConsentForms.FindAsync(id);
            if (c != null) { _db.ConsentForms.Remove(c); await _db.SaveChangesAsync(); }
            TempData["Success"] = "Consent form removed.";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdowns(int? selectedParticipantId = null)
        {
            ViewBag.Participants = new SelectList(
                await _db.Participants.OrderBy(p => p.LastName).ToListAsync(),
                "ParticipantId", "FullName", selectedParticipantId);
            ViewBag.Studies = new SelectList(
                await _db.Studies.OrderBy(s => s.StudyName).ToListAsync(),
                "StudyId", "StudyName");
        }
    }
}
