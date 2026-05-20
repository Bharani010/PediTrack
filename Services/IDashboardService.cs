using PediTrack.Models.ViewModels;

namespace PediTrack.Services
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardDataAsync();
    }
}
