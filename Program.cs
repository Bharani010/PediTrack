using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PediTrack.Data;
using PediTrack.Models;
using PediTrack.Services;

var builder = WebApplication.CreateBuilder(args);

// ── MVC + Razor Runtime Compilation ─────────────────────────────────────────
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

// ── Entity Framework ─────────────────────────────────────────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── ASP.NET Core Identity ─────────────────────────────────────────────────────
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    // Password rules
    options.Password.RequiredLength         = 8;
    options.Password.RequireDigit           = true;
    options.Password.RequireLowercase       = true;
    options.Password.RequireUppercase       = true;
    options.Password.RequireNonAlphanumeric = true;

    // Lockout
    options.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;

    // User
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ── JWT Authentication — token stored in HttpOnly cookie ─────────────────────
var jwtSection = builder.Configuration.GetSection("JwtSettings");
var jwtKey     = Encoding.UTF8.GetBytes(jwtSection["SecretKey"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultForbidScheme       = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey         = new SymmetricSecurityKey(jwtKey),
        ValidateIssuer           = true,
        ValidIssuer              = jwtSection["Issuer"],
        ValidateAudience         = true,
        ValidAudience            = jwtSection["Audience"],
        ValidateLifetime         = true,
        ClockSkew                = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        // Read JWT from HttpOnly cookie instead of Authorization header
        OnMessageReceived = ctx =>
        {
            ctx.Token = ctx.Request.Cookies["PediTrack.Auth"];
            return Task.CompletedTask;
        },

        // Redirect to login page on 401 (MVC browsers expect a redirect, not a 401 JSON)
        OnChallenge = ctx =>
        {
            ctx.HandleResponse();
            var returnUrl = Uri.EscapeDataString(ctx.Request.Path + ctx.Request.QueryString);
            ctx.Response.Redirect($"/Auth/Login?returnUrl={returnUrl}");
            return Task.CompletedTask;
        },

        // Redirect to access-denied page on 403
        OnForbidden = ctx =>
        {
            ctx.Response.Redirect("/Auth/AccessDenied");
            return Task.CompletedTask;
        }
    };
});

// ── Application Services ──────────────────────────────────────────────────────
builder.Services.AddScoped<IParticipantService,    ParticipantService>();
builder.Services.AddScoped<IStudyService,          StudyService>();
builder.Services.AddScoped<IVisitService,          VisitService>();
builder.Services.AddScoped<IReportService,         ReportService>();
builder.Services.AddScoped<IDashboardService,      DashboardService>();
builder.Services.AddScoped<ISqlAnalyticsService,   SqlAnalyticsService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();

// ── Build ─────────────────────────────────────────────────────────────────────
var app = builder.Build();

// ── Database seed ─────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db          = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    db.Database.EnsureCreated();
    DbInitializer.Seed(db);
    await DbInitializer.SeedUsersAsync(db, userManager, roleManager);
}

// ── Middleware pipeline ───────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();   // must be BEFORE UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");   // landing page = login

// ── Rotativa (optional PDF export) ───────────────────────────────────────────
try { Rotativa.AspNetCore.RotativaConfiguration.Setup(app.Environment.WebRootPath); }
catch { /* wkhtmltopdf not installed — print via browser instead */ }

app.Run();
