using Microsoft.AspNetCore.Mvc;
using PediTrack.Models;
using PediTrack.Services;

namespace PediTrack.Controllers
{
    public class StudiesController : Controller
    {
        private readonly IStudyService _service;

        public StudiesController(IStudyService service) => _service = service;

        public async Task<IActionResult> Index(string? status)
        {
            ViewBag.Status = status ?? "All";
            var studies = await _service.GetAllAsync(status);
            return View(studies);
        }

        public async Task<IActionResult> Details(int id)
        {
            var study = await _service.GetByIdAsync(id);
            if (study == null) return NotFound();
            return View(study);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Investigators = await _service.GetAllInvestigatorsAsync();
            return View(new Study { StartDate = DateTime.Today });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Study study)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Investigators = await _service.GetAllInvestigatorsAsync();
                return View(study);
            }
            await _service.CreateAsync(study);
            TempData["Success"] = $"Study '{study.StudyName}' created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var study = await _service.GetByIdAsync(id);
            if (study == null) return NotFound();
            ViewBag.Investigators = await _service.GetAllInvestigatorsAsync();
            return View(study);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Study study)
        {
            if (id != study.StudyId) return BadRequest();
            if (!ModelState.IsValid)
            {
                ViewBag.Investigators = await _service.GetAllInvestigatorsAsync();
                return View(study);
            }
            await _service.UpdateAsync(study);
            TempData["Success"] = "Study updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var study = await _service.GetByIdAsync(id);
            if (study == null) return NotFound();
            return View(study);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            TempData["Success"] = "Study removed.";
            return RedirectToAction(nameof(Index));
        }
    }
}
