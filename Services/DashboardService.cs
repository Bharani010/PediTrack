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
            var today = DateTime.Today;
            var vm = new DashboardViewModel();

            // ── Summary counts (all from DB) ────────────────────────────────────
            vm.TotalParticipants         = await _db.Participants.CountAsync();
            vm.ActiveParticipants        = await _db.Participants.CountAsync(p => p.Status == "Active");
            vm.TotalStudies              = await _db.Studies.CountAsync();
            vm.ActiveStudies             = await _db.Studies.CountAsync(s => s.Status == "Active" || s.Status == "Recruiting");
            vm.TotalVisits               = await _db.Visits.CountAsync();
            vm.UpcomingVisits            = await _db.Visits.CountAsync(v => v.Status == "Scheduled" && v.ScheduledDate >= today && v.ScheduledDate <= today.AddDays(14));
            vm.TotalEnrollments          = await _db.StudyEnrollments.CountAsync();
            vm.PendingConsents           = await _db.ConsentForms.CountAsync(c => c.Status == "Active" && !c.SignedByGuardian);
            vm.ExpiredConsents           = await _db.ConsentForms.CountAsync(c => c.Status == "Expired");
            vm.MissedVisits              = await _db.Visits.CountAsync(v => v.Status == "Missed");
            vm.ExpiringConsentsThisMonth = await _db.ConsentForms.CountAsync(c =>
                c.Status == "Active" &&
                c.ExpirationDate.HasValue &&
                c.ExpirationDate.Value >= today &&
                c.ExpirationDate.Value <= today.AddDays(30));

            // ── Enrollment trend — from first-ever enrollment to today ─────────
            // Shows up to 36 months of history so the chart always has real bars
            // regardless of whether seed data uses historical or relative dates.
            var firstDate = await _db.StudyEnrollments.MinAsync(e => (DateTime?)e.EnrollmentDate);
            DateTime trendStart;
            if (firstDate.HasValue)
            {
                var oldest = new DateTime(firstDate.Value.Year, firstDate.Value.Month, 1);
                var cap36  = new DateTime(today.Year, today.Month, 1).AddMonths(-35); // max 36 bars
                trendStart = oldest < cap36 ? cap36 : oldest;
            }
            else
            {
                trendStart = new DateTime(today.Year, today.Month, 1).AddMonths(-11);
            }

            for (var d = trendStart; d <= new DateTime(today.Year, today.Month, 1); d = d.AddMonths(1))
            {
                var monthEnd = d.AddMonths(1);
                vm.EnrollmentMonths.Add(d.ToString("MMM yy"));
                vm.EnrollmentCounts.Add(await _db.StudyEnrollments.CountAsync(e =>
                    e.EnrollmentDate >= d && e.EnrollmentDate < monthEnd));
            }

            // ── Study status doughnut ─────────────────────────────────────────
            var studyStatuses = await _db.Studies
                .GroupBy(s => s.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync();
            foreach (var s in studyStatuses)
            {
                vm.StudyStatusLabels.Add(s.Status);
                vm.StudyStatusCounts.Add(s.Count);
            }

            // ── Visit status doughnut ─────────────────────────────────────────
            var visitStatuses = await _db.Visits
                .GroupBy(v => v.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync();
            foreach (var v in visitStatuses)
            {
                vm.VisitStatusLabels.Add(v.Status);
                vm.VisitStatusCounts.Add(v.Count);
            }

            // ── Enrollments per active study (bar chart) ─────────────────────
            var studyEnrollCounts = await _db.Studies
                .Where(s => s.Status != "Closed")
                .Select(s => new
                {
                    s.StudyCode,
                    Count = s.StudyEnrollments.Count(e => e.Status == "Active")
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync();
            foreach (var row in studyEnrollCounts)
            {
                vm.StudyEnrollmentLabels.Add(row.StudyCode);
                vm.StudyEnrollmentCounts.Add(row.Count);
            }

            // ── Age distribution of active participants ───────────────────────
            var dobs = await _db.Participants
                .Where(p => p.Status == "Active")
                .Select(p => p.DateOfBirth)
                .ToListAsync();

            int a0_5 = 0, a6_10 = 0, a11_14 = 0, a15_17 = 0;
            foreach (var dob in dobs)
            {
                var age = (int)((today - dob).TotalDays / 365.25);
                if (age <= 5)       a0_5++;
                else if (age <= 10) a6_10++;
                else if (age <= 14) a11_14++;
                else                a15_17++;
            }
            vm.AgeGroupLabels = new List<string> { "0–5 yrs", "6–10 yrs", "11–14 yrs", "15–17 yrs" };
            vm.AgeGroupCounts = new List<int>    { a0_5, a6_10, a11_14, a15_17 };

            // ── Study progress (enrolled vs target) ──────────────────────────
            var studies = await _db.Studies
                .Where(s => s.Status == "Active" || s.Status == "Recruiting")
                .Select(s => new
                {
                    s.StudyCode,
                    s.StudyName,
                    s.Status,
                    s.MaxParticipants,
                    ActiveEnrolled = s.StudyEnrollments.Count(e => e.Status == "Active")
                })
                .OrderByDescending(s => s.ActiveEnrolled)
                .ToListAsync();

            foreach (var s in studies)
            {
                vm.StudyProgress.Add(new StudyProgressRow
                {
                    StudyCode       = s.StudyCode,
                    StudyName       = s.StudyName,
                    Status          = s.Status,
                    Enrolled        = s.ActiveEnrolled,
                    MaxParticipants = s.MaxParticipants ?? 0
                });
            }

            // ── Upcoming visits (next 14 days, max 8) ────────────────────────
            vm.UpcomingVisitList = await _db.Visits
                .Include(v => v.Participant)
                .Include(v => v.Study)
                .Where(v => v.Status == "Scheduled" && v.ScheduledDate >= today && v.ScheduledDate <= today.AddDays(14))
                .OrderBy(v => v.ScheduledDate)
                .Take(8)
                .ToListAsync();

            // ── Recent enrollments (last 6) ───────────────────────────────────
            vm.RecentEnrollments = await _db.StudyEnrollments
                .Include(se => se.Participant)
                .Include(se => se.Study)
                .OrderByDescending(se => se.EnrollmentDate)
                .Take(6)
                .ToListAsync();

            return vm;
        }
    }
}
