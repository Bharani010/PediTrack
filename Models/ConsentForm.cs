using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PediTrack.Models
{
    public class ConsentForm
    {
        [Key]
        public int ConsentFormId { get; set; }

        [Required]
        public int ParticipantId { get; set; }
        [ValidateNever]
        public Participant Participant { get; set; } = null!;

        [Required]
        public int StudyId { get; set; }
        [ValidateNever]
        public Study Study { get; set; } = null!;

        [Required, Display(Name = "Consent Date")]
        [DataType(DataType.Date)]
        public DateTime ConsentDate { get; set; }

        [Display(Name = "Expiration Date")]
        [DataType(DataType.Date)]
        public DateTime? ExpirationDate { get; set; }

        [Required, StringLength(20), Display(Name = "Form Version")]
        public string Version { get; set; } = "1.0";

        [Display(Name = "Signed by Guardian?")]
        public bool SignedByGuardian { get; set; }

        [StringLength(200), Display(Name = "Witness Name")]
        public string? WitnessName { get; set; }

        [Required, StringLength(30)]
        public string Status { get; set; } = "Active"; // Active, Expired, Revoked, Superseded

        [StringLength(500)]
        public string? Notes { get; set; }

        [Display(Name = "Re-consent Required?")]
        public bool ReconsentRequired { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
