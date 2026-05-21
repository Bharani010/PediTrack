using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PediTrack.Data;
using PediTrack.Models;

namespace PediTrack.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DataDictionaryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DataDictionaryController(ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Index(string? table)
        {
            var tables = await _db.DataDictionaryEntries
                .Select(e => e.TableName)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            ViewBag.Tables = tables;
            ViewBag.SelectedTable = table ?? "All";

            var query = _db.DataDictionaryEntries.AsQueryable();
            if (!string.IsNullOrWhiteSpace(table) && table != "All")
                query = query.Where(e => e.TableName == table);

            return View(await query.OrderBy(e => e.TableName).ThenBy(e => e.ColumnName).ToListAsync());
        }

        public IActionResult Create()
        {
            PopulateTableList();
            return View(new DataDictionaryEntry { LastUpdated = DateTime.Today });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DataDictionaryEntry entry)
        {
            if (!ModelState.IsValid) { PopulateTableList(); return View(entry); }
            entry.LastUpdated = DateTime.UtcNow;
            _db.DataDictionaryEntries.Add(entry);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Data dictionary entry added.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var entry = await _db.DataDictionaryEntries.FindAsync(id);
            if (entry == null) return NotFound();
            PopulateTableList();
            return View(entry);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DataDictionaryEntry entry)
        {
            if (id != entry.EntryId) return BadRequest();
            if (!ModelState.IsValid) { PopulateTableList(); return View(entry); }
            entry.LastUpdated = DateTime.UtcNow;
            _db.DataDictionaryEntries.Update(entry);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Entry updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var entry = await _db.DataDictionaryEntries.FindAsync(id);
            if (entry != null) { _db.DataDictionaryEntries.Remove(entry); await _db.SaveChangesAsync(); }
            TempData["Success"] = "Entry removed.";
            return RedirectToAction(nameof(Index));
        }

        private void PopulateTableList()
        {
            ViewBag.KnownTables = new[]
            {
                "Participants", "Studies", "StudyEnrollments",
                "Visits", "ConsentForms", "Investigators", "DataDictionaryEntries"
            };
        }
    }
}
