using System.ComponentModel.DataAnnotations;

namespace PediTrack.Models
{
    public class Visit
    {
        [Key]
        public int VisitId { get; set; }

        [Required]
        public int ParticipantId { get; set; }
        public Participant Participant { get; set; } = null!;

        [Required]
        public int StudyId { get; set; }
        public Study Study { get; set; } = null!;

        [Required, StringLength(100), Display(Name = "Visit Type")]
        public string VisitType { get; set; } = string.Empty; // Screening, Baseline, Follow-up, Final, Unscheduled

        [Required, Display(Name = "Scheduled Date")]
        [DataType(DataType.DateTime)]
        public DateTime ScheduledDate { get; set; }

        [Display(Name = "Completed Date")]
        [DataType(DataType.DateTime)]
        public DateTime? CompletedDate { get; set; }

        [Required, StringLength(30)]
        public string Status { get; set; } = "Scheduled"; // Scheduled, Completed, Missed, Cancelled, Rescheduled

        [StringLength(100), Display(Name = "Location")]
        public string? Location { get; set; }

        public int? AssignedStaffId { get; set; }
        public Investigator? AssignedStaff { get; set; }

        [StringLength(2000)]
        public string? Notes { get; set; }

        [Display(Name = "Visit Number")]
        public int? VisitNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
