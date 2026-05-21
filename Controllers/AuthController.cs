using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PediTrack.Models;
using PediTrack.Models.ViewModels;

namespace PediTrack.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser>  _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration        _config;

        public AuthController(
            UserManager<AppUser>  userManager,
            SignInManager<AppUser> signInManager,
            IConfiguration        config)
        {
            _userManager   = userManager;
            _signInManager = signInManager;
            _config        = config;
        }

        // GET /Auth/Login
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectAfterLogin();

            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        // POST /Auth/Login
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                    ModelState.AddModelError(string.Empty, "Account locked after too many failed attempts. Try again in 15 minutes.");
                else
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            // Update last login timestamp
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            // Generate JWT and store in HttpOnly cookie
            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);

            var expiry = model.RememberMe
                ? DateTimeOffset.UtcNow.AddDays(7)
                : DateTimeOffset.UtcNow.AddHours(8);

            Response.Cookies.Append("PediTrack.Auth", token, new CookieOptions
            {
                HttpOnly = true,
                Secure   = false,               // set true when using HTTPS in production
                SameSite = SameSiteMode.Strict,
                Expires  = expiry
            });

            // Redirect
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectAfterLogin(roles.FirstOrDefault());
        }

        // POST /Auth/Logout
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("PediTrack.Auth");
            return RedirectToAction("Login");
        }

        // GET /Auth/AccessDenied
        public IActionResult AccessDenied()
            => View();

        // ── Private helpers ───────────────────────────────────────────────────
        private IActionResult RedirectAfterLogin(string? role = null)
        {
            if (role == null && User.Identity?.IsAuthenticated == true)
                role = User.FindFirst(ClaimTypes.Role)?.Value;

            return role == "Customer"
                ? RedirectToAction("Index", "CustomerDashboard")
                : RedirectToAction("Index", "Home");
        }

        private string GenerateJwtToken(AppUser user, IList<string> roles)
        {
            var jwt     = _config.GetSection("JwtSettings");
            var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["SecretKey"]!));
            var creds   = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry  = int.TryParse(jwt["ExpiryHours"], out int h) ? h : 8;

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub,   user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
                new(ClaimTypes.NameIdentifier,     user.Id),
                new(ClaimTypes.Name,               user.UserName!),
                new(ClaimTypes.Email,              user.Email!),
                new("displayName",                 user.DisplayName),
                new("participantId",               user.ParticipantId?.ToString() ?? ""),
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var token = new JwtSecurityToken(
                issuer:            jwt["Issuer"],
                audience:          jwt["Audience"],
                claims:            claims,
                expires:           DateTime.UtcNow.AddHours(expiry),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
