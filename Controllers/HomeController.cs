using Microsoft.AspNetCore.Mvc;
using PediTrack.Services;

namespace PediTrack.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDashboardService _dashboard;

        public HomeController(IDashboardService dashboard) => _dashboard = dashboard;

        public async Task<IActionResult> Index()
        {
            var vm = await _dashboard.GetDashboardDataAsync();
            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
