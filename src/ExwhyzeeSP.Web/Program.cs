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
using ExwhyzeeSP.Data.MultiTenancy;

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
// 2. CONFIGURE MASTER & DYNAMIC TENANT DATABASES
// ==============================================================================
var masterConnectionString = Environment.GetEnvironmentVariable("MASTER_DB_CONNECTION")
    ?? builder.Configuration.GetConnectionString("MasterConnection");

if (string.IsNullOrWhiteSpace(masterConnectionString))
{
    throw new InvalidOperationException("CRITICAL CONFIGURATION ERROR: 'MASTER_DB_CONNECTION' is not configured in .env or configuration providers.");
}

// Master DB Context (For resolving school tenant domains and metadata)
builder.Services.AddDbContext<MasterDbContext>(options =>
    options.UseSqlServer(masterConnectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null);
    }));

// Multi-Tenant Service & Caching (Sub-millisecond host lookup)
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ITenantService, TenantService>();

// Dynamic Tenant Application DbContext (Switches SQL connection string dynamically per request)
builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
{
    var tenantService = serviceProvider.GetService<ITenantService>();
    var connStr = tenantService?.GetActiveConnectionString();

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

// HttpContextAccessor bridge
HttpHelper.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

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

// Dynamic Multi-Tenant Middleware (Executes on every request to bind school tenant)
app.UseTenantResolution();

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

