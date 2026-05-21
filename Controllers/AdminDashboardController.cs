using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PediTrack.Data;
using PediTrack.Models.ViewModels;
using PediTrack.Services;

namespace PediTrack.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly IUserManagementService _userService;
        private readonly ApplicationDbContext   _db;

        public AdminDashboardController(IUserManagementService userService, ApplicationDbContext db)
        {
            _userService = userService;
            _db          = db;
        }

        // GET /AdminDashboard  — user management hub
        public async Task<IActionResult> Index()
        {
            var summary = await _userService.GetAdminSummaryAsync();
            return View(summary);
        }

        // GET /AdminDashboard/Users
        public async Task<IActionResult> Users()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        // GET /AdminDashboard/CreateUser
        public async Task<IActionResult> CreateUser()
        {
            await PopulateParticipantDropdown();
            return View(new CreateUserViewModel());
        }

        // POST /AdminDashboard/CreateUser
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateParticipantDropdown();
                return View(model);
            }

            var (success, errors) = await _userService.CreateUserAsync(model);
            if (!success)
            {
                foreach (var e in errors)
                    ModelState.AddModelError(string.Empty, e);
                await PopulateParticipantDropdown();
                return View(model);
            }

            TempData["Success"] = $"User {model.Email} created successfully.";
            return RedirectToAction(nameof(Users));
        }

        // GET /AdminDashboard/EditUser/id
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var userManager = HttpContext.RequestServices
                .GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<PediTrack.Models.AppUser>>();
            var roles = await userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id            = user.Id,
                Email         = user.Email ?? "",
                DisplayName   = user.DisplayName,
                Role          = roles.FirstOrDefault() ?? "Customer",
                ParticipantId = user.ParticipantId,
                IsActive      = user.IsActive
            };

            await PopulateParticipantDropdown(user.ParticipantId);
            return View(model);
        }

        // POST /AdminDashboard/EditUser/id
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            // Skip password validation if fields are blank
            if (string.IsNullOrWhiteSpace(model.NewPassword))
            {
                ModelState.Remove(nameof(model.NewPassword));
                ModelState.Remove(nameof(model.ConfirmNewPassword));
            }

            if (!ModelState.IsValid)
            {
                await PopulateParticipantDropdown(model.ParticipantId);
                return View(model);
            }

            var (success, errors) = await _userService.UpdateUserAsync(model);
            if (!success)
            {
                foreach (var e in errors)
                    ModelState.AddModelError(string.Empty, e);
                await PopulateParticipantDropdown(model.ParticipantId);
                return View(model);
            }

            TempData["Success"] = "User updated successfully.";
            return RedirectToAction(nameof(Users));
        }

        // POST /AdminDashboard/DeleteUser
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var (success, message) = await _userService.DeleteUserAsync(id);
            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction(nameof(Users));
        }

        // POST /AdminDashboard/ToggleUserStatus
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(string id)
        {
            var (success, message) = await _userService.ToggleUserStatusAsync(id);
            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction(nameof(Users));
        }

        // ── Helper ────────────────────────────────────────────────────────────
        private async Task PopulateParticipantDropdown(int? selectedId = null)
        {
            var participants = await _db.Participants
                .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
                .Select(p => new { p.ParticipantId, Display = p.FirstName + " " + p.LastName + " (" + p.MRN + ")" })
                .ToListAsync();

            ViewBag.Participants = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                participants, "ParticipantId", "Display", selectedId);
        }
    }
}
