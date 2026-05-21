using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PediTrack.Models
{
    public class AppUser : IdentityUser
    {
        [StringLength(200)]
        public string DisplayName { get; set; } = string.Empty;

        // For Customer role: link to the participant they represent (guardian)
        public int? ParticipantId { get; set; }
        public Participant? Participant { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
    }
}
