using Microsoft.EntityFrameworkCore;
using PediTrack.Models;

namespace PediTrack.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Participant> Participants { get; set; }
        public DbSet<Study> Studies { get; set; }
        public DbSet<StudyEnrollment> StudyEnrollments { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<ConsentForm> ConsentForms { get; set; }
        public DbSet<Investigator> Investigators { get; set; }
        public DbSet<DataDictionaryEntry> DataDictionaryEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Participant
            modelBuilder.Entity<Participant>(e =>
            {
                e.HasIndex(p => p.MRN).IsUnique();
                e.Property(p => p.Status).HasDefaultValue("Active");
                e.Ignore(p => p.FullName);
                e.Ignore(p => p.Age);
            });

            // Study → Investigator (PI)
            modelBuilder.Entity<Study>(e =>
            {
                e.HasIndex(s => s.StudyCode).IsUnique();
                e.HasOne(s => s.PrincipalInvestigator)
                 .WithMany(i => i.Studies)
                 .HasForeignKey(s => s.PrincipalInvestigatorId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // StudyEnrollment — composite natural uniqueness
            modelBuilder.Entity<StudyEnrollment>(e =>
            {
                e.HasIndex(se => new { se.ParticipantId, se.StudyId }).IsUnique();
                e.HasOne(se => se.Participant)
                 .WithMany(p => p.StudyEnrollments)
                 .HasForeignKey(se => se.ParticipantId)
                 .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(se => se.Study)
                 .WithMany(s => s.StudyEnrollments)
                 .HasForeignKey(se => se.StudyId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // Visit
            modelBuilder.Entity<Visit>(e =>
            {
                e.HasOne(v => v.Participant)
                 .WithMany(p => p.Visits)
                 .HasForeignKey(v => v.ParticipantId)
                 .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(v => v.Study)
                 .WithMany(s => s.Visits)
                 .HasForeignKey(v => v.StudyId)
                 .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(v => v.AssignedStaff)
                 .WithMany(i => i.AssignedVisits)
                 .HasForeignKey(v => v.AssignedStaffId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // ConsentForm
            modelBuilder.Entity<ConsentForm>(e =>
            {
                e.HasOne(c => c.Participant)
                 .WithMany(p => p.ConsentForms)
                 .HasForeignKey(c => c.ParticipantId)
                 .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(c => c.Study)
                 .WithMany(s => s.ConsentForms)
                 .HasForeignKey(c => c.StudyId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // Investigator FullName is computed
            modelBuilder.Entity<Investigator>()
                .Ignore(i => i.FullName);
        }
    }
}
