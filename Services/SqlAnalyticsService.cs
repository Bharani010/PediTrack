using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PediTrack.Data;
using PediTrack.Models.ViewModels;

namespace PediTrack.Services
{
    public class SqlAnalyticsService : ISqlAnalyticsService
    {
        private readonly ApplicationDbContext _db;
        public SqlAnalyticsService(ApplicationDbContext db) => _db = db;

        // ── View query — no parameters needed ────────────────────────────────
        public async Task<List<StudyEnrollmentSummaryRow>> GetStudyEnrollmentSummaryAsync()
            => await _db.StudyEnrollmentSummary
                        .OrderBy(r => r.StudyCode)
                        .ToListAsync();

        // ── SP: usp_GetEnrollmentSummaryByStudy ──────────────────────────────
        public async Task<List<EnrollmentSummaryResult>> GetEnrollmentSummaryByStudyAsync(
            int? studyId = null, string? status = null)
        {
            var pStudyId = new SqlParameter("@StudyId", System.Data.SqlDbType.Int)
            {
                Value = studyId.HasValue ? (object)studyId.Value : DBNull.Value
            };
            var pStatus = new SqlParameter("@Status", System.Data.SqlDbType.NVarChar, 30)
            {
                Value = !string.IsNullOrWhiteSpace(status) ? (object)status : DBNull.Value
            };

            return await _db.EnrollmentSummaryResults
                            .FromSqlRaw("EXEC dbo.usp_GetEnrollmentSummaryByStudy @StudyId, @Status",
                                        pStudyId, pStatus)
                            .ToListAsync();
        }

        // ── SP: usp_GetParticipantVisitHistory ───────────────────────────────
        public async Task<List<ParticipantVisitHistoryRow>> GetParticipantVisitHistoryAsync(
            int participantId, int? studyId = null)
        {
            var pParticipantId = new SqlParameter("@ParticipantId", System.Data.SqlDbType.Int)
            {
                Value = participantId
            };
            var pStudyId = new SqlParameter("@StudyId", System.Data.SqlDbType.Int)
            {
                Value = studyId.HasValue ? (object)studyId.Value : DBNull.Value
            };

            return await _db.ParticipantVisitHistory
                            .FromSqlRaw("EXEC dbo.usp_GetParticipantVisitHistory @ParticipantId, @StudyId",
                                        pParticipantId, pStudyId)
                            .ToListAsync();
        }
    }
}
