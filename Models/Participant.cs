using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PediTrack.Models
{
    public class Participant
    {
        [Key]
        public int ParticipantId { get; set; }

        [Required, StringLength(20), Display(Name = "Medical Record #")]
        public string MRN { get; set; } = string.Empty;

        [Required, StringLength(100), Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(100), Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [NotMapped, Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";

        [Required, Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [NotMapped, Display(Name = "Age")]
        public int Age => (int)((DateTime.Today - DateOfBirth).TotalDays / 365.25);

        [Required, StringLength(20)]
        public string Gender { get; set; } = string.Empty;

        [StringLength(200), Display(Name = "Guardian Name")]
        public string? GuardianName { get; set; }

        [StringLength(20), Display(Name = "Guardian Phone")]
        [Phone]
        public string? GuardianPhone { get; set; }

        [StringLength(254), Display(Name = "Guardian Email")]
        [EmailAddress]
        public string? GuardianEmail { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [Required, Display(Name = "Enrollment Date")]
        [DataType(DataType.Date)]
        public DateTime EnrollmentDate { get; set; } = DateTime.Today;

        [Required, StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Withdrawn, Completed, Pending

        [StringLength(1000)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<StudyEnrollment> StudyEnrollments { get; set; } = new List<StudyEnrollment>();
        public ICollection<Visit> Visits { get; set; } = new List<Visit>();
        public ICollection<ConsentForm> ConsentForms { get; set; } = new List<ConsentForm>();
    }
}
