using Microsoft.EntityFrameworkCore;
using PediTrack.Data;
using PediTrack.Models.ViewModels;

namespace PediTrack.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _db;
        public DashboardService(ApplicationDbContext db) => _db = db;

        public async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            var now = DateTime.Now;
            var vm = new DashboardViewModel();

            // Summary counts
            vm.TotalParticipants  = await _db.Participants.CountAsync();
            vm.ActiveParticipants = await _db.Participants.CountAsync(p => p.Status == "Active");
            vm.TotalStudies       = await _db.Studies.CountAsync();
            vm.ActiveStudies      = await _db.Studies.CountAsync(s => s.Status == "Active" || s.Status == "Recruiting");
            vm.TotalVisits        = await _db.Visits.CountAsync();
            vm.UpcomingVisits     = await _db.Visits.CountAsync(v => v.Status == "Scheduled" && v.ScheduledDate >= DateTime.Today && v.ScheduledDate <= DateTime.Today.AddDays(14));
            vm.TotalEnrollments   = await _db.StudyEnrollments.CountAsync();
            vm.PendingConsents    = await _db.ConsentForms.CountAsync(c => c.Status == "Active" && c.SignedByGuardian == false);
            vm.ExpiredConsents    = await _db.ConsentForms.CountAsync(c => c.Status == "Expired");
            vm.MissedVisits       = await _db.Visits.CountAsync(v => v.Status == "Missed");
            vm.ExpiringConsentsThisMonth = await _db.ConsentForms.CountAsync(c =>
                c.Status == "Active" &&
                c.ExpirationDate.HasValue &&
                c.ExpirationDate.Value >= DateTime.Today &&
                c.ExpirationDate.Value <= DateTime.Today.AddDays(30));

            // Enrollment trend — last 6 months
            for (int i = 5; i >= 0; i--)
            {
                var monthStart = new DateTime(now.Year, now.Month, 1).AddMonths(-i);
                var monthEnd   = monthStart.AddMonths(1);
                vm.EnrollmentMonths.Add(monthStart.ToString("MMM yyyy"));
                vm.EnrollmentCounts.Add(await _db.StudyEnrollments.CountAsync(e =>
                    e.EnrollmentDate >= monthStart && e.EnrollmentDate < monthEnd));
            }

            // Study status breakdown
            var studyStatuses = await _db.Studies
                .GroupBy(s => s.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();
            foreach (var s in studyStatuses)
            {
                vm.StudyStatusLabels.Add(s.Status);
                vm.StudyStatusCounts.Add(s.Count);
            }

            // Visit status breakdown
            var visitStatuses = await _db.Visits
                .GroupBy(v => v.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();
            foreach (var v in visitStatuses)
            {
                vm.VisitStatusLabels.Add(v.Status);
                vm.VisitStatusCounts.Add(v.Count);
            }

            // Recent participants (5)
            vm.RecentParticipants = await _db.Participants
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .ToListAsync();

            // Upcoming visits (7)
            vm.UpcomingVisitList = await _db.Visits
                .Include(v => v.Participant)
                .Include(v => v.Study)
                .Where(v => v.Status == "Scheduled" && v.ScheduledDate >= DateTime.Today)
                .OrderBy(v => v.ScheduledDate)
                .Take(7)
                .ToListAsync();

            // Recent enrollments (5)
            vm.RecentEnrollments = await _db.StudyEnrollments
                .Include(se => se.Participant)
                .Include(se => se.Study)
                .OrderByDescending(se => se.EnrollmentDate)
                .Take(5)
                .ToListAsync();

            return vm;
        }
    }
}
