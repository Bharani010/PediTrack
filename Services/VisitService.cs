using Microsoft.EntityFrameworkCore;
using PediTrack.Data;
using PediTrack.Models;

namespace PediTrack.Services
{
    public class VisitService : IVisitService
    {
        private readonly ApplicationDbContext _db;
        public VisitService(ApplicationDbContext db) => _db = db;

        public async Task<List<Visit>> GetAllAsync(int? participantId = null, int? studyId = null, string? status = null)
        {
            var query = _db.Visits
                .Include(v => v.Participant)
                .Include(v => v.Study)
                .Include(v => v.AssignedStaff)
                .AsQueryable();

            if (participantId.HasValue) query = query.Where(v => v.ParticipantId == participantId);
            if (studyId.HasValue)       query = query.Where(v => v.StudyId == studyId);
            if (!string.IsNullOrWhiteSpace(status) && status != "All")
                query = query.Where(v => v.Status == status);

            return await query.OrderByDescending(v => v.ScheduledDate).ToListAsync();
        }

        public async Task<Visit?> GetByIdAsync(int id) =>
            await _db.Visits
                .Include(v => v.Participant)
                .Include(v => v.Study)
                .Include(v => v.AssignedStaff)
                .FirstOrDefaultAsync(v => v.VisitId == id);

        public async Task<Visit> CreateAsync(Visit visit)
        {
            visit.CreatedAt = DateTime.UtcNow;
            visit.UpdatedAt = DateTime.UtcNow;
            _db.Visits.Add(visit);
            await _db.SaveChangesAsync();
            return visit;
        }

        public async Task<Visit> UpdateAsync(Visit visit)
        {
            var existing = await _db.Visits.FindAsync(visit.VisitId)
                           ?? throw new InvalidOperationException("Visit not found.");
            existing.ParticipantId   = visit.ParticipantId;
            existing.StudyId         = visit.StudyId;
            existing.VisitType       = visit.VisitType;
            existing.VisitNumber     = visit.VisitNumber;
            existing.ScheduledDate   = visit.ScheduledDate;
            existing.CompletedDate   = visit.CompletedDate;
            existing.Status          = visit.Status;
            existing.Location        = visit.Location;
            existing.AssignedStaffId = visit.AssignedStaffId;
            existing.Notes           = visit.Notes;
            existing.UpdatedAt       = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var visit = await _db.Visits.FindAsync(id);
            if (visit == null) return false;
            _db.Visits.Remove(visit);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<Visit>> GetUpcomingAsync(int days = 14) =>
            await _db.Visits
                .Include(v => v.Participant)
                .Include(v => v.Study)
                .Where(v => v.Status == "Scheduled" && v.ScheduledDate >= DateTime.Today && v.ScheduledDate <= DateTime.Today.AddDays(days))
                .OrderBy(v => v.ScheduledDate)
                .ToListAsync();
    }
}
