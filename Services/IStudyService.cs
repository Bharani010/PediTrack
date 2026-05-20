using PediTrack.Models;

namespace PediTrack.Services
{
    public interface IStudyService
    {
        Task<List<Study>> GetAllAsync(string? status = null);
        Task<Study?> GetByIdAsync(int id);
        Task<Study> CreateAsync(Study study);
        Task<Study> UpdateAsync(Study study);
        Task<bool> DeleteAsync(int id);
        Task<List<Investigator>> GetAllInvestigatorsAsync();
    }
}
