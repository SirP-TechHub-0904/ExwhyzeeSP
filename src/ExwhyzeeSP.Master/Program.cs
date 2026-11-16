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
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Data.Seed;

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
// 2. CONFIGURE DATABASE CONNECTION (Master DB)
// ==============================================================================
var masterConnectionString = Environment.GetEnvironmentVariable("MASTER_DB_CONNECTION")
    ?? builder.Configuration.GetConnectionString("MasterConnection");

if (string.IsNullOrWhiteSpace(masterConnectionString))
{
    throw new InvalidOperationException("CRITICAL CONFIGURATION ERROR: 'MASTER_DB_CONNECTION' is not configured in .env or configuration providers.");
}

builder.Services.AddDbContext<MasterDbContext>(options =>
    options.UseSqlServer(masterConnectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null);
    }));

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<ExwhyzeeSP.Services.WhatsApp.IWhatsAppCloudApiService, ExwhyzeeSP.Services.WhatsApp.WhatsAppCloudApiService>();

builder.Services.AddIdentity<ExwhyzeeSP.Core.Models.MasterAdminUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<MasterDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.Name = "ExwhyzeeSP_Master_Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

builder.Services.AddAuthorization();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ExwhyzeeSP.Services.Provisioning.IDatabaseProvisioningService, ExwhyzeeSP.Services.Provisioning.DatabaseProvisioningService>();
builder.Services.AddScoped<ExwhyzeeSP.Services.Provisioning.ITenantMigrationService, ExwhyzeeSP.Services.Provisioning.TenantMigrationService>();
builder.Services.AddScoped<ExwhyzeeSP.Services.IDelegatedSsoService, ExwhyzeeSP.Services.DelegatedSsoService>();
builder.Services.AddScoped<ExwhyzeeSP.Services.IAuditLogService, ExwhyzeeSP.Services.AuditLogService>();
builder.Services.AddScoped<ExwhyzeeSP.Services.IEmailLoggingService, ExwhyzeeSP.Services.EmailLoggingService>();
builder.Services.AddScoped<ExwhyzeeSP.Services.IMarketingService, ExwhyzeeSP.Services.MarketingService>();

var app = builder.Build();

// ==============================================================================
// 4. SEED MASTER DATABASE (Local Safe Setup)
// ==============================================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<MasterDbContext>();
        await MasterSeedData.InitializeAsync(context, services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<Microsoft.Extensions.Logging.ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the Master Database.");
    }
}

// ==============================================================================
// 5. CONFIGURE HTTP PIPELINE
// ==============================================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

var contentTypeProvider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
contentTypeProvider.Mappings[".woff"] = "font/woff";
contentTypeProvider.Mappings[".woff2"] = "font/woff2";
contentTypeProvider.Mappings[".ttf"] = "font/ttf";
contentTypeProvider.Mappings[".eot"] = "application/vnd.ms-fontobject";
contentTypeProvider.Mappings[".svg"] = "image/svg+xml";
contentTypeProvider.Mappings[".otf"] = "font/otf";
contentTypeProvider.Mappings[".webp"] = "image/webp";

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = contentTypeProvider
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
