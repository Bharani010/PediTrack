using PediTrack.Models;

namespace PediTrack.Services
{
    public interface IVisitService
    {
        Task<List<Visit>> GetAllAsync(int? participantId = null, int? studyId = null, string? status = null);
        Task<Visit?> GetByIdAsync(int id);
        Task<Visit> CreateAsync(Visit visit);
        Task<Visit> UpdateAsync(Visit visit);
        Task<bool> DeleteAsync(int id);
        Task<List<Visit>> GetUpcomingAsync(int days = 14);
    }
}
