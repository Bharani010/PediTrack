using System.ComponentModel.DataAnnotations;

namespace PediTrack.Models
{
    public class Investigator
    {
        [Key]
        public int InvestigatorId { get; set; }

        [Required, StringLength(100), Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(100), Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [StringLength(200)]
        public string FullName => $"{Title} {FirstName} {LastName}".Trim();

        [StringLength(50)]
        public string? Title { get; set; } = "Dr."; // Dr., Prof., Mr., Ms.

        [Required, StringLength(50)]
        public string Role { get; set; } = "Investigator"; // PI, Co-I, Study Coordinator, Research Staff

        [Required, StringLength(254)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        [Phone]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? Department { get; set; }

        [StringLength(200)]
        public string? Institution { get; set; } = "Meridian Children's Hospital";

        [Required, StringLength(20)]
        public string Status { get; set; } = "Active";

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Study> Studies { get; set; } = new List<Study>();
        public ICollection<Visit> AssignedVisits { get; set; } = new List<Visit>();
    }
}
