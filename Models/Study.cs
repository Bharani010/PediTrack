using System.ComponentModel.DataAnnotations;

namespace PediTrack.Models
{
    public class Study
    {
        [Key]
        public int StudyId { get; set; }

        [Required, StringLength(20), Display(Name = "Study Code")]
        public string StudyCode { get; set; } = string.Empty;

        [Required, StringLength(300), Display(Name = "Study Name")]
        public string StudyName { get; set; } = string.Empty;

        [StringLength(50), Display(Name = "IRB Number")]
        public string? IRBNumber { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required, Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Required, StringLength(30), Display(Name = "Status")]
        public string Status { get; set; } = "Active"; // Active, Recruiting, Closed, Suspended

        [Display(Name = "Max Participants")]
        [Range(1, 10000)]
        public int? MaxParticipants { get; set; }

        [StringLength(100), Display(Name = "Sponsor")]
        public string? Sponsor { get; set; }

        [StringLength(100), Display(Name = "Study Phase")]
        public string? Phase { get; set; } // Phase I, II, III, Observational, etc.

        public int? PrincipalInvestigatorId { get; set; }
        public Investigator? PrincipalInvestigator { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<StudyEnrollment> StudyEnrollments { get; set; } = new List<StudyEnrollment>();
        public ICollection<Visit> Visits { get; set; } = new List<Visit>();
        public ICollection<ConsentForm> ConsentForms { get; set; } = new List<ConsentForm>();
    }
}
