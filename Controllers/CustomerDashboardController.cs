using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PediTrack.Data;
using PediTrack.Models.ViewModels;

namespace PediTrack.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerDashboardController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CustomerDashboardController(ApplicationDbContext db) => _db = db;

        // GET /CustomerDashboard
        public async Task<IActionResult> Index()
        {
            var participantIdClaim = User.FindFirst("participantId")?.Value;
            if (!int.TryParse(participantIdClaim, out int participantId) || participantId == 0)
                return View("AccountNotLinked");

            var participant = await _db.Participants
                .FirstOrDefaultAsync(p => p.ParticipantId == participantId);

            if (participant == null)
                return View("AccountNotLinked");

            var enrollments = await _db.StudyEnrollments
                .Include(se => se.Study)
                .Where(se => se.ParticipantId == participantId)
                .OrderByDescending(se => se.EnrollmentDate)
                .ToListAsync();

            var allVisits = await _db.Visits
                .Include(v => v.Study)
                .Include(v => v.AssignedStaff)
                .Where(v => v.ParticipantId == participantId)
                .ToListAsync();

            var consentForms = await _db.ConsentForms
                .Include(c => c.Study)
                .Where(c => c.ParticipantId == participantId)
                .OrderByDescending(c => c.ConsentDate)
                .ToListAsync();

            var today = DateTime.Today;
            var vm = new CustomerDashboardViewModel
            {
                Participant      = participant,
                Enrollments      = enrollments,
                UpcomingVisits   = allVisits
                    .Where(v => v.ScheduledDate.Date >= today && v.Status == "Scheduled")
                    .OrderBy(v => v.ScheduledDate)
                    .Take(5)
                    .ToList(),
                RecentVisits     = allVisits
                    .Where(v => v.Status == "Completed")
                    .OrderByDescending(v => v.CompletedDate)
                    .Take(5)
                    .ToList(),
                ConsentForms     = consentForms,
                TotalVisits      = allVisits.Count,
                CompletedVisits  = allVisits.Count(v => v.Status == "Completed"),
                MissedVisits     = allVisits.Count(v => v.Status == "Missed"),
                ScheduledVisits  = allVisits.Count(v => v.Status == "Scheduled"),
            };

            return View(vm);
        }

        // GET /CustomerDashboard/MyProfile
        public async Task<IActionResult> MyProfile()
        {
            var participantId = GetParticipantId();
            if (participantId == null) return View("AccountNotLinked");

            var participant = await _db.Participants
                .FirstOrDefaultAsync(p => p.ParticipantId == participantId);

            if (participant == null) return View("AccountNotLinked");
            return View(participant);
        }

        // GET /CustomerDashboard/MyStudies
        public async Task<IActionResult> MyStudies()
        {
            var participantId = GetParticipantId();
            if (participantId == null) return View("AccountNotLinked");

            var enrollments = await _db.StudyEnrollments
                .Include(se => se.Study).ThenInclude(s => s.PrincipalInvestigator)
                .Where(se => se.ParticipantId == participantId)
                .OrderByDescending(se => se.EnrollmentDate)
                .ToListAsync();

            return View(enrollments);
        }

        // GET /CustomerDashboard/MyVisits
        public async Task<IActionResult> MyVisits()
        {
            var participantId = GetParticipantId();
            if (participantId == null) return View("AccountNotLinked");

            var visits = await _db.Visits
                .Include(v => v.Study)
                .Include(v => v.AssignedStaff)
                .Where(v => v.ParticipantId == participantId)
                .OrderByDescending(v => v.ScheduledDate)
                .ToListAsync();

            return View(visits);
        }

        // GET /CustomerDashboard/MyConsents
        public async Task<IActionResult> MyConsents()
        {
            var participantId = GetParticipantId();
            if (participantId == null) return View("AccountNotLinked");

            var consents = await _db.ConsentForms
                .Include(c => c.Study)
                .Where(c => c.ParticipantId == participantId)
                .OrderByDescending(c => c.ConsentDate)
                .ToListAsync();

            return View(consents);
        }

        // ── Helper ────────────────────────────────────────────────────────────
        private int? GetParticipantId()
        {
            var claim = User.FindFirst("participantId")?.Value;
            return int.TryParse(claim, out int id) && id > 0 ? id : null;
        }
    }
}
