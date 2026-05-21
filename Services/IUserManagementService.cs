using PediTrack.Models;
using PediTrack.Models.ViewModels;

namespace PediTrack.Services
{
    public interface IUserManagementService
    {
        Task<List<UserListViewModel>> GetAllUsersAsync();
        Task<AppUser?> GetUserByIdAsync(string id);
        Task<(bool Success, IEnumerable<string> Errors)> CreateUserAsync(CreateUserViewModel model);
        Task<(bool Success, IEnumerable<string> Errors)> UpdateUserAsync(EditUserViewModel model);
        Task<(bool Success, string Message)> DeleteUserAsync(string id);
        Task<(bool Success, string Message)> ToggleUserStatusAsync(string id);
        Task<AdminDashboardSummaryViewModel> GetAdminSummaryAsync();
    }
}
