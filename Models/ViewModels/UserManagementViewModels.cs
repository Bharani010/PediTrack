using System.ComponentModel.DataAnnotations;

namespace PediTrack.Models.ViewModels
{
    public class UserListViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? LinkedParticipant { get; set; }
        public int? ParticipantId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }

    public class CreateUserViewModel
    {
        [Required, EmailAddress, Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(200), Display(Name = "Display Name")]
        public string DisplayName { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), StringLength(100, MinimumLength = 8),
         Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Compare(nameof(Password)),
         Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required, Display(Name = "Role")]
        public string Role { get; set; } = "Customer";

        [Display(Name = "Linked Participant (required for Customer role)")]
        public int? ParticipantId { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required, EmailAddress, Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(200), Display(Name = "Display Name")]
        public string DisplayName { get; set; } = string.Empty;

        [Required, Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;

        [Display(Name = "Linked Participant")]
        public int? ParticipantId { get; set; }

        [Display(Name = "Account Active")]
        public bool IsActive { get; set; } = true;

        [DataType(DataType.Password),
         Display(Name = "New Password (leave blank to keep current)")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password), Compare(nameof(NewPassword)),
         Display(Name = "Confirm New Password")]
        public string? ConfirmNewPassword { get; set; }
    }

    public class AdminDashboardSummaryViewModel
    {
        public int TotalUsers { get; set; }
        public int AdminCount { get; set; }
        public int CustomerCount { get; set; }
        public int ActiveUsers { get; set; }
        public List<UserListViewModel> RecentUsers { get; set; } = new();
    }
}
