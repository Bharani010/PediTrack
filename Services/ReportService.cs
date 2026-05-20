using Microsoft.EntityFrameworkCore;
using PediTrack.Data;
using PediTrack.Models;
using PediTrack.Models.ViewModels;

namespace PediTrack.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _db;
        public ReportService(ApplicationDbContext db) => _db = db;

        public async Task<ReportViewModel> GetParticipantReportAsync(int? studyId, string? status, DateTime? from, DateTime? to)
        {
            var query = _db.Participants.AsQueryable();
            if (!string.IsNullOrWhiteSpace(status) && status != "All") query = query.Where(p => p.Status == status);
            if (from.HasValue) query = query.Where(p => p.EnrollmentDate >= from);
            if (to.HasValue)   query = query.Where(p => p.EnrollmentDate <= to);
            if (studyId.HasValue) query = query.Where(p => p.StudyEnrollments.Any(se => se.StudyId == studyId));

            var participants = await query.OrderBy(p => p.LastName).ToListAsync();

            var statusBreakdown = participants
                .GroupBy(p => p.Status)
                .ToDictionary(g => g.Key, g => g.Count());

            return new ReportViewModel
            {
                ReportTitle = "Participant Status Report",
                Participants = participants,
                TotalRecords = participants.Count,
                StatusBreakdown = statusBreakdown,
                StudyId = studyId,
                Status = status,
                DateFrom = from,
                DateTo = to,
                AvailableStudies = await _db.Studies.OrderBy(s => s.StudyName).ToListAsync()
            };
        }

        public async Task<ReportViewModel> GetVisitReportAsync(int? studyId, string? status, DateTime? from, DateTime? to)
        {
            var query = _db.Visits
                .Include(v => v.Participant)
                .Include(v => v.Study)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) && status != "All") query = query.Where(v => v.Status == status);
            if (from.HasValue) query = query.Where(v => v.ScheduledDate >= from);
            if (to.HasValue)   query = query.Where(v => v.ScheduledDate <= to);
            if (studyId.HasValue) query = query.Where(v => v.StudyId == studyId);

            var visits = await query.OrderByDescending(v => v.ScheduledDate).ToListAsync();

            return new ReportViewModel
            {
                ReportTitle = "Visit Activity Report",
                Visits = visits,
                TotalRecords = visits.Count,
                StatusBreakdown = visits.GroupBy(v => v.Status).ToDictionary(g => g.Key, g => g.Count()),
                StudyId = studyId,
                Status = status,
                DateFrom = from,
                DateTo = to,
                AvailableStudies = await _db.Studies.OrderBy(s => s.StudyName).ToListAsync()
            };
        }

        public async Task<ReportViewModel> GetConsentReportAsync(int? studyId, string? status)
        {
            var query = _db.ConsentForms
                .Include(c => c.Participant)
                .Include(c => c.Study)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) && status != "All") query = query.Where(c => c.Status == status);
            if (studyId.HasValue) query = query.Where(c => c.StudyId == studyId);

            var consents = await query.OrderBy(c => c.ExpirationDate).ToListAsync();

            return new ReportViewModel
            {
                ReportTitle = "Consent Form Status Report",
                ConsentForms = consents,
                TotalRecords = consents.Count,
                StatusBreakdown = consents.GroupBy(c => c.Status).ToDictionary(g => g.Key, g => g.Count()),
                StudyId = studyId,
                Status = status,
                AvailableStudies = await _db.Studies.OrderBy(s => s.StudyName).ToListAsync()
            };
        }

        public async Task<List<ParticipantReportRow>> GetParticipantRowsAsync(int? studyId, string? status, DateTime? from, DateTime? to)
        {
            var query = _db.Participants
                .Include(p => p.StudyEnrollments).ThenInclude(se => se.Study)
                .Include(p => p.Visits)
                .Include(p => p.ConsentForms)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) && status != "All") query = query.Where(p => p.Status == status);
            if (from.HasValue) query = query.Where(p => p.EnrollmentDate >= from);
            if (to.HasValue)   query = query.Where(p => p.EnrollmentDate <= to);
            if (studyId.HasValue) query = query.Where(p => p.StudyEnrollments.Any(se => se.StudyId == studyId));

            var participants = await query.OrderBy(p => p.LastName).ToListAsync();

            return participants.Select(p =>
            {
                var enrollment = studyId.HasValue
                    ? p.StudyEnrollments.FirstOrDefault(se => se.StudyId == studyId)
                    : p.StudyEnrollments.FirstOrDefault();
                var consent = p.ConsentForms.OrderByDescending(c => c.ConsentDate).FirstOrDefault();
                var totalVisits = p.Visits.Count;
                var completedVisits = p.Visits.Count(v => v.Status == "Completed");

                return new ParticipantReportRow
                {
                    MRN = p.MRN,
                    FullName = $"{p.FirstName} {p.LastName}",
                    Age = (int)((DateTime.Today - p.DateOfBirth).TotalDays / 365.25),
                    Gender = p.Gender,
                    Status = p.Status,
                    StudyName = enrollment?.Study?.StudyName ?? "—",
                    EnrollmentDate = enrollment?.EnrollmentDate ?? p.EnrollmentDate,
                    TotalVisits = totalVisits,
                    CompletedVisits = completedVisits,
                    ConsentStatus = consent?.Status ?? "None"
                };
            }).ToList();
        }
    }
}
