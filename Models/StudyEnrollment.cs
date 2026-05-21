using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PediTrack.Models
{
    public class StudyEnrollment
    {
        [Key]
        public int EnrollmentId { get; set; }

        [Required]
        public int ParticipantId { get; set; }
        [ValidateNever]
        public Participant Participant { get; set; } = null!;

        [Required]
        public int StudyId { get; set; }
        [ValidateNever]
        public Study Study { get; set; } = null!;

        [Required, Display(Name = "Enrollment Date")]
        [DataType(DataType.Date)]
        public DateTime EnrollmentDate { get; set; } = DateTime.Today;

        [Required, StringLength(30)]
        public string Status { get; set; } = "Enrolled"; // Enrolled, Active, Completed, Withdrawn, Screen Failed

        [Display(Name = "Withdrawal Date")]
        [DataType(DataType.Date)]
        public DateTime? WithdrawalDate { get; set; }

        [StringLength(500), Display(Name = "Withdrawal Reason")]
        public string? WithdrawalReason { get; set; }

        [StringLength(50), Display(Name = "Subject ID")]
        public string? SubjectId { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
