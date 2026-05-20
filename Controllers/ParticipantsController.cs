using Microsoft.AspNetCore.Mvc;
using PediTrack.Models;
using PediTrack.Services;

namespace PediTrack.Controllers
{
    public class ParticipantsController : Controller
    {
        private readonly IParticipantService _service;
        private readonly IStudyService _studyService;

        public ParticipantsController(IParticipantService service, IStudyService studyService)
        {
            _service = service;
            _studyService = studyService;
        }

        // GET: /Participants
        public async Task<IActionResult> Index(string? search, string? status)
        {
            ViewBag.Search = search;
            ViewBag.Status = status ?? "All";
            var participants = await _service.GetAllAsync(search, status);
            return View(participants);
        }

        // GET: /Participants/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var participant = await _service.GetByIdAsync(id);
            if (participant == null) return NotFound();
            return View(participant);
        }

        // GET: /Participants/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Studies = await _studyService.GetAllAsync("Active");
            return View(new Participant { EnrollmentDate = DateTime.Today });
        }

        // POST: /Participants/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Participant participant)
        {
            if (await _service.MRNExistsAsync(participant.MRN))
                ModelState.AddModelError("MRN", "This MRN already exists in the system.");

            if (!ModelState.IsValid)
            {
                ViewBag.Studies = await _studyService.GetAllAsync("Active");
                return View(participant);
            }

            await _service.CreateAsync(participant);
            TempData["Success"] = $"Participant {participant.FirstName} {participant.LastName} added successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Participants/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var participant = await _service.GetByIdAsync(id);
            if (participant == null) return NotFound();
            ViewBag.Studies = await _studyService.GetAllAsync("Active");
            return View(participant);
        }

        // POST: /Participants/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Participant participant)
        {
            if (id != participant.ParticipantId) return BadRequest();

            if (await _service.MRNExistsAsync(participant.MRN, participant.ParticipantId))
                ModelState.AddModelError("MRN", "This MRN is already assigned to another participant.");

            if (!ModelState.IsValid)
            {
                ViewBag.Studies = await _studyService.GetAllAsync("Active");
                return View(participant);
            }

            await _service.UpdateAsync(participant);
            TempData["Success"] = "Participant record updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: /Participants/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var participant = await _service.GetByIdAsync(id);
            if (participant == null) return NotFound();
            return View(participant);
        }

        // POST: /Participants/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            TempData["Success"] = "Participant removed from the system.";
            return RedirectToAction(nameof(Index));
        }
    }
}
