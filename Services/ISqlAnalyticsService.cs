using PediTrack.Models.ViewModels;

namespace PediTrack.Services
{
    public interface ISqlAnalyticsService
    {
        Task<List<StudyEnrollmentSummaryRow>> GetStudyEnrollmentSummaryAsync();
        Task<List<EnrollmentSummaryResult>> GetEnrollmentSummaryByStudyAsync(int? studyId = null, string? status = null);
        Task<List<ParticipantVisitHistoryRow>> GetParticipantVisitHistoryAsync(int participantId, int? studyId = null);
    }
}
