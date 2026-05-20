using PediTrack.Models.ViewModels;

namespace PediTrack.Services
{
    public interface IReportService
    {
        Task<ReportViewModel> GetParticipantReportAsync(int? studyId, string? status, DateTime? from, DateTime? to);
        Task<ReportViewModel> GetVisitReportAsync(int? studyId, string? status, DateTime? from, DateTime? to);
        Task<ReportViewModel> GetConsentReportAsync(int? studyId, string? status);
        Task<List<ParticipantReportRow>> GetParticipantRowsAsync(int? studyId, string? status, DateTime? from, DateTime? to);
    }
}
