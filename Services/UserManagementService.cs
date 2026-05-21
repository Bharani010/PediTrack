using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PediTrack.Data;
using PediTrack.Models;
using PediTrack.Models.ViewModels;

namespace PediTrack.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<AppUser>  _userManager;
        private readonly ApplicationDbContext  _db;

        public UserManagementService(UserManager<AppUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db          = db;
        }

        // ── List all users with role and linked participant ───────────────────
        public async Task<List<UserListViewModel>> GetAllUsersAsync()
        {
            var users = await _db.Users
                .Include(u => u.Participant)
                .OrderBy(u => u.CreatedAt)
                .ToListAsync();

            var result = new List<UserListViewModel>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new UserListViewModel
                {
                    Id               = user.Id,
                    Email            = user.Email ?? "",
                    DisplayName      = user.DisplayName,
                    Role             = roles.FirstOrDefault() ?? "—",
                    LinkedParticipant = user.Participant != null
                                          ? $"{user.Participant.FirstName} {user.Participant.LastName} ({user.Participant.MRN})"
                                          : null,
                    ParticipantId    = user.ParticipantId,
                    IsActive         = user.IsActive,
                    CreatedAt        = user.CreatedAt,
                    LastLoginAt      = user.LastLoginAt
                });
            }
            return result;
        }

        // ── Get single user ───────────────────────────────────────────────────
        public async Task<AppUser?> GetUserByIdAsync(string id)
            => await _db.Users.Include(u => u.Participant).FirstOrDefaultAsync(u => u.Id == id);

        // ── Create user ───────────────────────────────────────────────────────
        public async Task<(bool Success, IEnumerable<string> Errors)> CreateUserAsync(CreateUserViewModel model)
        {
            var user = new AppUser
            {
                UserName      = model.Email,
                Email         = model.Email,
                DisplayName   = model.DisplayName,
                ParticipantId = model.ParticipantId,
                IsActive      = true,
                EmailConfirmed = true,
                CreatedAt     = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description));

            await _userManager.AddToRoleAsync(user, model.Role);
            return (true, Enumerable.Empty<string>());
        }

        // ── Update user ───────────────────────────────────────────────────────
        public async Task<(bool Success, IEnumerable<string> Errors)> UpdateUserAsync(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return (false, new[] { "User not found." });

            user.Email         = model.Email;
            user.UserName      = model.Email;
            user.DisplayName   = model.DisplayName;
            user.ParticipantId = model.ParticipantId;
            user.IsActive      = model.IsActive;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return (false, updateResult.Errors.Select(e => e.Description));

            // Update role
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(model.Role))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            // Optional password reset
            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                var token  = await _userManager.GeneratePasswordResetTokenAsync(user);
                var pwResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                if (!pwResult.Succeeded)
                    return (false, pwResult.Errors.Select(e => e.Description));
            }

            return (true, Enumerable.Empty<string>());
        }

        // ── Delete user ───────────────────────────────────────────────────────
        public async Task<(bool Success, string Message)> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return (false, "User not found.");

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded
                ? (true,  $"User {user.Email} deleted.")
                : (false, string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        // ── Toggle active/inactive ────────────────────────────────────────────
        public async Task<(bool Success, string Message)> ToggleUserStatusAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return (false, "User not found.");

            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);
            return (true, user.IsActive ? "Account activated." : "Account deactivated.");
        }

        // ── Admin dashboard summary ───────────────────────────────────────────
        public async Task<AdminDashboardSummaryViewModel> GetAdminSummaryAsync()
        {
            var allUsers = await GetAllUsersAsync();
            return new AdminDashboardSummaryViewModel
            {
                TotalUsers    = allUsers.Count,
                AdminCount    = allUsers.Count(u => u.Role == "Admin"),
                CustomerCount = allUsers.Count(u => u.Role == "Customer"),
                ActiveUsers   = allUsers.Count(u => u.IsActive),
                RecentUsers   = allUsers.OrderByDescending(u => u.CreatedAt).Take(5).ToList()
            };
        }
    }
}
