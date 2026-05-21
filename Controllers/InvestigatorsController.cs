using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PediTrack.Data;
using PediTrack.Models;

namespace PediTrack.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InvestigatorsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public InvestigatorsController(ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var investigators = await _db.Investigators
                .Include(i => i.Studies)
                .OrderBy(i => i.LastName)
                .ToListAsync();
            return View(investigators);
        }

        public async Task<IActionResult> Details(int id)
        {
            var inv = await _db.Investigators
                .Include(i => i.Studies)
                .Include(i => i.AssignedVisits).ThenInclude(v => v.Participant)
                .FirstOrDefaultAsync(i => i.InvestigatorId == id);
            if (inv == null) return NotFound();
            return View(inv);
        }

        public IActionResult Create() => View(new Investigator());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Investigator investigator)
        {
            if (!ModelState.IsValid) return View(investigator);
            _db.Investigators.Add(investigator);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"{investigator.Title} {investigator.FirstName} {investigator.LastName} added.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var inv = await _db.Investigators.FindAsync(id);
            if (inv == null) return NotFound();
            return View(inv);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Investigator investigator)
        {
            if (id != investigator.InvestigatorId) return BadRequest();
            if (!ModelState.IsValid) return View(investigator);

            // Fetch existing to preserve navigation props, then update scalar fields
            var existing = await _db.Investigators.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Title        = investigator.Title;
            existing.FirstName    = investigator.FirstName;
            existing.LastName     = investigator.LastName;
            existing.Role         = investigator.Role;
            existing.Email        = investigator.Email;
            existing.Phone        = investigator.Phone;
            existing.Department   = investigator.Department;
            existing.Institution  = investigator.Institution;
            existing.Status       = investigator.Status;
            existing.Notes        = investigator.Notes;

            await _db.SaveChangesAsync();
            TempData["Success"] = $"{existing.Title} {existing.FirstName} {existing.LastName} updated.";
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var inv = await _db.Investigators.FindAsync(id);
            if (inv == null) return NotFound();
            return View(inv);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inv = await _db.Investigators.FindAsync(id);
            if (inv != null) { _db.Investigators.Remove(inv); await _db.SaveChangesAsync(); }
            TempData["Success"] = "Investigator removed.";
            return RedirectToAction(nameof(Index));
        }
    }
}
