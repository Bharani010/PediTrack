using System.Text;
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

        // ── Participant Report ───────────────────────────────────────────────
        public async Task<ReportViewModel> GetParticipantReportAsync(int? studyId, string? status, DateTime? from, DateTime? to)
        {
            var query = _db.Participants.AsQueryable();
            if (!string.IsNullOrWhiteSpace(status) && status != "All") query = query.Where(p => p.Status == status);
            if (from.HasValue) query = query.Where(p => p.EnrollmentDate >= from);
            if (to.HasValue)   query = query.Where(p => p.EnrollmentDate <= to);
            if (studyId.HasValue) query = query.Where(p => p.StudyEnrollments.Any(se => se.StudyId == studyId));

            var participants = await query.OrderBy(p => p.LastName).ToListAsync();

            return new ReportViewModel
            {
                ReportTitle      = "Participant Status Report",
                Participants     = participants,
                TotalRecords     = participants.Count,
                StatusBreakdown  = participants.GroupBy(p => p.Status).ToDictionary(g => g.Key, g => g.Count()),
                StudyId          = studyId,
                Status           = status,
                DateFrom         = from,
                DateTo           = to,
                AvailableStudies = await _db.Studies.OrderBy(s => s.StudyName).ToListAsync()
            };
        }

        // ── Visit Report ─────────────────────────────────────────────────────
        public async Task<ReportViewModel> GetVisitReportAsync(int? studyId, string? status, DateTime? from, DateTime? to)
        {
            var query = _db.Visits.Include(v => v.Participant).Include(v => v.Study).AsQueryable();
            if (!string.IsNullOrWhiteSpace(status) && status != "All") query = query.Where(v => v.Status == status);
            if (from.HasValue)    query = query.Where(v => v.ScheduledDate >= from);
            if (to.HasValue)      query = query.Where(v => v.ScheduledDate <= to);
            if (studyId.HasValue) query = query.Where(v => v.StudyId == studyId);

            var visits = await query.OrderByDescending(v => v.ScheduledDate).ToListAsync();

            return new ReportViewModel
            {
                ReportTitle      = "Visit Activity Report",
                Visits           = visits,
                TotalRecords     = visits.Count,
                StatusBreakdown  = visits.GroupBy(v => v.Status).ToDictionary(g => g.Key, g => g.Count()),
                StudyId          = studyId,
                Status           = status,
                DateFrom         = from,
                DateTo           = to,
                AvailableStudies = await _db.Studies.OrderBy(s => s.StudyName).ToListAsync()
            };
        }

        // ── Consent Report ───────────────────────────────────────────────────
        public async Task<ReportViewModel> GetConsentReportAsync(int? studyId, string? status)
        {
            var query = _db.ConsentForms.Include(c => c.Participant).Include(c => c.Study).AsQueryable();
            if (!string.IsNullOrWhiteSpace(status) && status != "All") query = query.Where(c => c.Status == status);
            if (studyId.HasValue) query = query.Where(c => c.StudyId == studyId);

            var consents = await query.OrderBy(c => c.ExpirationDate).ToListAsync();

            return new ReportViewModel
            {
                ReportTitle      = "Consent Form Status Report",
                ConsentForms     = consents,
                TotalRecords     = consents.Count,
                StatusBreakdown  = consents.GroupBy(c => c.Status).ToDictionary(g => g.Key, g => g.Count()),
                StudyId          = studyId,
                Status           = status,
                AvailableStudies = await _db.Studies.OrderBy(s => s.StudyName).ToListAsync()
            };
        }

        // ── Participant rows (for print view) ────────────────────────────────
        public async Task<List<ParticipantReportRow>> GetParticipantRowsAsync(int? studyId, string? status, DateTime? from, DateTime? to)
        {
            var query = _db.Participants
                .Include(p => p.StudyEnrollments).ThenInclude(se => se.Study)
                .Include(p => p.Visits)
                .Include(p => p.ConsentForms)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) && status != "All") query = query.Where(p => p.Status == status);
            if (from.HasValue)    query = query.Where(p => p.EnrollmentDate >= from);
            if (to.HasValue)      query = query.Where(p => p.EnrollmentDate <= to);
            if (studyId.HasValue) query = query.Where(p => p.StudyEnrollments.Any(se => se.StudyId == studyId));

            var participants = await query.OrderBy(p => p.LastName).ToListAsync();

            return participants.Select(p =>
            {
                var enrollment      = studyId.HasValue
                    ? p.StudyEnrollments.FirstOrDefault(se => se.StudyId == studyId)
                    : p.StudyEnrollments.FirstOrDefault();
                var consent         = p.ConsentForms.OrderByDescending(c => c.ConsentDate).FirstOrDefault();
                var completedVisits = p.Visits.Count(v => v.Status == "Completed");

                return new ParticipantReportRow
                {
                    MRN             = p.MRN,
                    FullName        = $"{p.FirstName} {p.LastName}",
                    Age             = (int)((DateTime.Today - p.DateOfBirth).TotalDays / 365.25),
                    Gender          = p.Gender,
                    Status          = p.Status,
                    StudyName       = enrollment?.Study?.StudyName ?? "—",
                    EnrollmentDate  = enrollment?.EnrollmentDate ?? p.EnrollmentDate,
                    TotalVisits     = p.Visits.Count,
                    CompletedVisits = completedVisits,
                    ConsentStatus   = consent?.Status ?? "None"
                };
            }).ToList();
        }

        // ── CSV Exports ──────────────────────────────────────────────────────

        public async Task<string> ExportParticipantsCsvAsync(int? studyId, string? status, DateTime? from, DateTime? to)
        {
            var rows = await GetParticipantRowsAsync(studyId, status, from, to);
            var sb   = new StringBuilder();
            sb.AppendLine("MRN,Full Name,Age,Gender,Participant Status,Study,Enrollment Date,Total Visits,Completed Visits,Consent Status");
            foreach (var r in rows)
                sb.AppendLine($"{r.MRN},{Q(r.FullName)},{r.Age},{r.Gender},{r.Status},{Q(r.StudyName)},{r.EnrollmentDate:yyyy-MM-dd},{r.TotalVisits},{r.CompletedVisits},{r.ConsentStatus}");
            return sb.ToString();
        }

        public async Task<string> ExportVisitsCsvAsync(int? studyId, string? status, DateTime? from, DateTime? to)
        {
            var query = _db.Visits.Include(v => v.Participant).Include(v => v.Study).Include(v => v.AssignedStaff).AsQueryable();
            if (!string.IsNullOrWhiteSpace(status) && status != "All") query = query.Where(v => v.Status == status);
            if (from.HasValue)    query = query.Where(v => v.ScheduledDate >= from);
            if (to.HasValue)      query = query.Where(v => v.ScheduledDate <= to);
            if (studyId.HasValue) query = query.Where(v => v.StudyId == studyId);

            var visits = await query.OrderByDescending(v => v.ScheduledDate).ToListAsync();
            var sb = new StringBuilder();
            sb.AppendLine("Visit ID,Participant,Study,Visit Type,Visit Number,Scheduled Date,Completed Date,Status,Location,Assigned Staff,Notes");
            foreach (var v in visits)
                sb.AppendLine($"{v.VisitId},{Q(v.Participant?.FullName ?? "")},{v.Study?.StudyCode},{v.VisitType},{v.VisitNumber},{v.ScheduledDate:yyyy-MM-dd},{(v.CompletedDate.HasValue ? v.CompletedDate.Value.ToString("yyyy-MM-dd") : "")},{v.Status},{Q(v.Location ?? "")},{Q(v.AssignedStaff?.FullName ?? "")},{Q(v.Notes ?? "")}");
            return sb.ToString();
        }

        public async Task<string> ExportConsentsCsvAsync(int? studyId, string? status)
        {
            var query = _db.ConsentForms.Include(c => c.Participant).Include(c => c.Study).AsQueryable();
            if (!string.IsNullOrWhiteSpace(status) && status != "All") query = query.Where(c => c.Status == status);
            if (studyId.HasValue) query = query.Where(c => c.StudyId == studyId);

            var consents = await query.OrderBy(c => c.ExpirationDate).ToListAsync();
            var sb = new StringBuilder();
            sb.AppendLine("Consent ID,Participant,Study,Version,Consent Date,Expiration Date,Signed By Guardian,Witness,Status,Re-consent Required,Notes");
            foreach (var c in consents)
                sb.AppendLine($"{c.ConsentFormId},{Q(c.Participant?.FullName ?? "")},{c.Study?.StudyCode},v{c.Version},{c.ConsentDate:yyyy-MM-dd},{(c.ExpirationDate.HasValue ? c.ExpirationDate.Value.ToString("yyyy-MM-dd") : "")},{(c.SignedByGuardian ? "Yes" : "No")},{Q(c.WitnessName ?? "")},{c.Status},{(c.ReconsentRequired ? "Yes" : "No")},{Q(c.Notes ?? "")}");
            return sb.ToString();
        }

        // CSV quote helper — wraps field in quotes if it contains a comma or quote
        private static string Q(string value)
        {
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
                return $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
        }
    }
}
