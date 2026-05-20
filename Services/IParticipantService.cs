using PediTrack.Models;

namespace PediTrack.Services
{
    public interface IParticipantService
    {
        Task<List<Participant>> GetAllAsync(string? search = null, string? status = null);
        Task<Participant?> GetByIdAsync(int id);
        Task<Participant> CreateAsync(Participant participant);
        Task<Participant> UpdateAsync(Participant participant);
        Task<bool> DeleteAsync(int id);
        Task<bool> MRNExistsAsync(string mrn, int? excludeId = null);
        Task<List<Participant>> GetByStudyAsync(int studyId);
    }
}
