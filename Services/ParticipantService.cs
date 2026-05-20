using Microsoft.EntityFrameworkCore;
using PediTrack.Data;
using PediTrack.Models;

namespace PediTrack.Services
{
    public class ParticipantService : IParticipantService
    {
        private readonly ApplicationDbContext _db;

        public ParticipantService(ApplicationDbContext db) => _db = db;

        public async Task<List<Participant>> GetAllAsync(string? search = null, string? status = null)
        {
            var query = _db.Participants.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(p =>
                    p.FirstName.ToLower().Contains(term) ||
                    p.LastName.ToLower().Contains(term) ||
                    p.MRN.ToLower().Contains(term) ||
                    (p.GuardianName != null && p.GuardianName.ToLower().Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(status) && status != "All")
                query = query.Where(p => p.Status == status);

            return await query.OrderBy(p => p.LastName).ThenBy(p => p.FirstName).ToListAsync();
        }

        public async Task<Participant?> GetByIdAsync(int id) =>
            await _db.Participants
                .Include(p => p.StudyEnrollments).ThenInclude(se => se.Study)
                .Include(p => p.Visits).ThenInclude(v => v.Study)
                .Include(p => p.ConsentForms).ThenInclude(c => c.Study)
                .FirstOrDefaultAsync(p => p.ParticipantId == id);

        public async Task<Participant> CreateAsync(Participant participant)
        {
            participant.CreatedAt = DateTime.UtcNow;
            participant.UpdatedAt = DateTime.UtcNow;
            _db.Participants.Add(participant);
            await _db.SaveChangesAsync();
            return participant;
        }

        public async Task<Participant> UpdateAsync(Participant participant)
        {
            participant.UpdatedAt = DateTime.UtcNow;
            _db.Participants.Update(participant);
            await _db.SaveChangesAsync();
            return participant;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var participant = await _db.Participants.FindAsync(id);
            if (participant == null) return false;
            _db.Participants.Remove(participant);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MRNExistsAsync(string mrn, int? excludeId = null)
        {
            var query = _db.Participants.Where(p => p.MRN == mrn);
            if (excludeId.HasValue) query = query.Where(p => p.ParticipantId != excludeId.Value);
            return await query.AnyAsync();
        }

        public async Task<List<Participant>> GetByStudyAsync(int studyId) =>
            await _db.Participants
                .Where(p => p.StudyEnrollments.Any(se => se.StudyId == studyId))
                .OrderBy(p => p.LastName)
                .ToListAsync();
    }
}
