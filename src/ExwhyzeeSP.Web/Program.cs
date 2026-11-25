using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DotNetEnv;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Data.Identity;

// ==============================================================================
// 1. LOAD ENVIRONMENT VARIABLES (.env)
// ==============================================================================
var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (!File.Exists(envPath))
{
    envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env");
}
if (File.Exists(envPath))
{
    Env.Load(envPath);
}

var builder = WebApplication.CreateBuilder(args);

// ==============================================================================
// 2. CONFIGURE SCHOOL DATABASE FROM .env
// ==============================================================================
var defaultConnectionString = Environment.GetEnvironmentVariable("DEFAULT_DB_CONNECTION")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(defaultConnectionString))
{
    throw new InvalidOperationException("CRITICAL CONFIGURATION ERROR: 'DEFAULT_DB_CONNECTION' is not configured in .env or configuration providers.");
}

// Multi-Tenant Services (.env & host routing)
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ExwhyzeeSP.Data.MultiTenancy.ITenantService, ExwhyzeeSP.Data.MultiTenancy.TenantService>();

builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
{
    var tenantService = serviceProvider.GetService<ExwhyzeeSP.Data.MultiTenancy.ITenantService>();
    var connStr = ExwhyzeeSP.Data.MultiTenancy.TenantContext.CurrentConnectionString
        ?? tenantService?.GetActiveConnectionString();

    if (string.IsNullOrWhiteSpace(connStr))
    {
        connStr = Environment.GetEnvironmentVariable("DEFAULT_DB_CONNECTION")
            ?? builder.Configuration.GetConnectionString("DefaultConnection");
    }

    if (!string.IsNullOrWhiteSpace(connStr))
    {
        options.UseSqlServer(connStr, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null);
        });
    }
});


builder.Services.AddHttpContextAccessor();

// ==============================================================================
// 3. CONFIGURE IDENTITY (School Users: Staff, Students, Admins, Parents)
// ==============================================================================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 4;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddUserStore<LegacyUserStore>()
.AddRoleStore<LegacyRoleStore>()
.AddUserManager<ApplicationUserManager>()
.AddSignInManager<ApplicationSignInManager>()
.AddRoleManager<ApplicationRoleManager>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/LogOff";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.Name = "ExwhyzeeSP_School_Auth";
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
});

// Session Support (For 2FA verification and legacy session state)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ==============================================================================
// 4. ADD MVC CONTROLLERS & RAZOR PAGES
// ==============================================================================
builder.Services.AddScoped<ExwhyzeeSP.Services.ISettingsService, ExwhyzeeSP.Services.SettingsService>();
builder.Services.AddScoped<ExwhyzeeSP.Services.IStorageService, ExwhyzeeSP.Services.StorageService>();
builder.Services.AddScoped<ExwhyzeeSP.Services.IDelegatedSsoService, ExwhyzeeSP.Services.DelegatedSsoService>();
builder.Services.AddScoped<ExwhyzeeSP.Services.IAuditLogService, ExwhyzeeSP.Services.AuditLogService>();
builder.Services.AddScoped<ExwhyzeeSP.Services.IEmailLoggingService, ExwhyzeeSP.Services.EmailLoggingService>();
builder.Services.AddTransient<Microsoft.AspNetCore.Mvc.ViewFeatures.IHtmlGenerator, ExwhyzeeSP.Web.Services.CustomHtmlGenerator>();
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Maintain PascalCase (e.g. Value, Text, Id, Name) for SelectList and DTOs to match legacy jQuery AJAX scripts
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    })
    .AddRazorRuntimeCompilation();

builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

var app = builder.Build();

// Safely apply missing Website CMS columns/tables on startup without touching existing data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        await ExwhyzeeSP.Data.Seed.WebsiteCmsSchemaInitializer.EnsureAppliedAsync(dbContext);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<Microsoft.Extensions.Logging.ILogger<Program>>();
        logger.LogError(ex, "An error occurred while ensuring Website CMS schema.");
    }
}

// HttpContextAccessor bridge
HttpHelper.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

// Support reverse proxy headers (Nginx / Cloudflare SSL termination)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | 
                       Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto | 
                       Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedHost
});

// Dynamic Multi-Tenant Host Resolution (Resolves dedicated school database per request from .env)
app.UseMiddleware<ExwhyzeeSP.Data.MultiTenancy.TenantResolutionMiddleware>();

// ==============================================================================
// 5. CONFIGURE HTTP PIPELINE
// ==============================================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// 17 Area Routing
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Legacy shortcuts for XYZ Account area (e.g. /XYZ/Login, /Account/Login)
app.MapControllerRoute(
    name: "xyz_shortcut",
    pattern: "XYZ/{action=Login}/{id?}",
    defaults: new { area = "Account", controller = "XYZ" });

app.MapControllerRoute(
    name: "account_shortcut",
    pattern: "Account/{action=Login}/{id?}",
    defaults: new { area = "Account", controller = "XYZ" });

// Route shortcuts for Area Controllers called directly by AJAX/scripts without area prefix
app.MapControllerRoute(
    name: "events_control_shortcut",
    pattern: "EventsControl/{action=Index}/{id?}",
    defaults: new { area = "Staff", controller = "EventsControl" });

app.MapControllerRoute(
    name: "user_managers_shortcut",
    pattern: "UserManagers/{action=Index}/{id?}",
    defaults: new { area = "Admin", controller = "UserManagers" });

app.MapControllerRoute(
    name: "class_levels_shortcut",
    pattern: "ClassLevels/{action=Index}/{id?}",
    defaults: new { area = "Admin", controller = "ClassLevels" });

app.MapControllerRoute(
    name: "staff_profile_shortcut",
    pattern: "StaffProfile/{action=Index}/{id?}",
    defaults: new { area = "Admin", controller = "StaffProfile" });

app.MapControllerRoute(
    name: "payment_shortcut",
    pattern: "Payment/{action=Index}/{id?}",
    defaults: new { area = "Financial", controller = "Payment" });

app.MapControllerRoute(
    name: "cbt_subject_shortcut",
    pattern: "CBTSubject/{action=Index}/{id?}",
    defaults: new { area = "CBTExam", controller = "CBTSubject" });

app.MapControllerRoute(
    name: "staff_panel_shortcut",
    pattern: "panel/{action=Index}/{id?}",
    defaults: new { area = "Staff", controller = "Panel" });

// Default Root Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

