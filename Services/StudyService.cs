using Microsoft.EntityFrameworkCore;
using PediTrack.Data;
using PediTrack.Models;

namespace PediTrack.Services
{
    public class StudyService : IStudyService
    {
        private readonly ApplicationDbContext _db;
        public StudyService(ApplicationDbContext db) => _db = db;

        public async Task<List<Study>> GetAllAsync(string? status = null)
        {
            var query = _db.Studies.Include(s => s.PrincipalInvestigator).AsQueryable();
            if (!string.IsNullOrWhiteSpace(status) && status != "All")
                query = query.Where(s => s.Status == status);
            return await query.OrderByDescending(s => s.StartDate).ToListAsync();
        }

        public async Task<Study?> GetByIdAsync(int id) =>
            await _db.Studies
                .Include(s => s.PrincipalInvestigator)
                .Include(s => s.StudyEnrollments).ThenInclude(se => se.Participant)
                .Include(s => s.Visits)
                .Include(s => s.ConsentForms)
                .FirstOrDefaultAsync(s => s.StudyId == id);

        public async Task<Study> CreateAsync(Study study)
        {
            study.CreatedAt = DateTime.UtcNow;
            study.UpdatedAt = DateTime.UtcNow;
            _db.Studies.Add(study);
            await _db.SaveChangesAsync();
            return study;
        }

        public async Task<Study> UpdateAsync(Study study)
        {
            study.UpdatedAt = DateTime.UtcNow;
            _db.Studies.Update(study);
            await _db.SaveChangesAsync();
            return study;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var study = await _db.Studies.FindAsync(id);
            if (study == null) return false;
            _db.Studies.Remove(study);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<Investigator>> GetAllInvestigatorsAsync() =>
            await _db.Investigators.Where(i => i.Status == "Active").OrderBy(i => i.LastName).ToListAsync();
    }
}
