using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;

namespace ExwhyzeeSP.Services.Provisioning
{
    public class TenantMigrationStatus
    {
        public int TenantId { get; set; }
        public string SchoolName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool CanConnect { get; set; }
        public bool HasPendingMigrations { get; set; }
        public List<string> PendingMigrations { get; set; } = new();
        public List<string> AppliedMigrations { get; set; } = new();
        public string? ConnectionError { get; set; }

        public string StatusBadgeClass
        {
            get
            {
                if (!CanConnect) return "bg-danger text-white";
                if (HasPendingMigrations) return "bg-warning text-dark";
                return "bg-success text-white";
            }
        }

        public string StatusText
        {
            get
            {
                if (!CanConnect) return "Connection Failed";
                if (HasPendingMigrations) return $"{PendingMigrations.Count} Update(s) Pending";
                return "Up to Date";
            }
        }
    }

    public interface ITenantMigrationService
    {
        Task<TenantMigrationStatus> CheckStatusAsync(TenantSchool tenant);
        Task<Dictionary<int, TenantMigrationStatus>> CheckAllStatusesAsync(IEnumerable<TenantSchool> tenants);
        Task<ProvisioningResult> MigrateTenantDatabaseAsync(TenantSchool tenant);
    }

    public class TenantMigrationService : ITenantMigrationService
    {
        public async Task<TenantMigrationStatus> CheckStatusAsync(TenantSchool tenant)
        {
            var status = new TenantMigrationStatus
            {
                TenantId = tenant.Id,
                SchoolName = tenant.SchoolName,
                IsActive = tenant.IsActive
            };

            if (string.IsNullOrWhiteSpace(tenant.ConnectionString))
            {
                status.CanConnect = false;
                status.ConnectionError = "No connection string configured.";
                return status;
            }

            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer(tenant.ConnectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("ExwhyzeeSP.Data");
                    sqlOptions.CommandTimeout(10); // Fast timeout for status check
                });

                using var tenantContext = new ApplicationDbContext(optionsBuilder.Options);
                var pending = (await tenantContext.Database.GetPendingMigrationsAsync()).ToList();
                var applied = (await tenantContext.Database.GetAppliedMigrationsAsync()).ToList();

                status.CanConnect = true;
                status.PendingMigrations = pending;
                status.AppliedMigrations = applied;
                status.HasPendingMigrations = pending.Any();
            }
            catch (Exception ex)
            {
                status.CanConnect = false;
                status.ConnectionError = ex.Message;
            }

            return status;
        }

        public async Task<Dictionary<int, TenantMigrationStatus>> CheckAllStatusesAsync(IEnumerable<TenantSchool> tenants)
        {
            var result = new Dictionary<int, TenantMigrationStatus>();
            var tasks = tenants.Select(async t =>
            {
                var s = await CheckStatusAsync(t);
                return (t.Id, s);
            });

            var outcomes = await Task.WhenAll(tasks);
            foreach (var item in outcomes)
            {
                result[item.Id] = item.s;
            }

            return result;
        }

        public async Task<ProvisioningResult> MigrateTenantDatabaseAsync(TenantSchool tenant)
        {
            var result = new ProvisioningResult
            {
                DatabaseName = tenant.SchoolName,
                ConnectionString = tenant.ConnectionString
            };

            if (string.IsNullOrWhiteSpace(tenant.ConnectionString))
            {
                result.Success = false;
                result.Message = "No connection string configured for this tenant.";
                return result;
            }

            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer(tenant.ConnectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("ExwhyzeeSP.Data");
                    sqlOptions.CommandTimeout(180);
                });

                using var tenantContext = new ApplicationDbContext(optionsBuilder.Options);
                var pending = (await tenantContext.Database.GetPendingMigrationsAsync()).ToList();

                if (pending.Any())
                {
                    await tenantContext.Database.MigrateAsync();
                    result.Success = true;
                    result.Message = $"Applied {pending.Count} migration(s): {string.Join(", ", pending)}";
                }
                else
                {
                    result.Success = true;
                    result.Message = "Database is already up to date. No pending migrations.";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Migration failed: {ex.Message}";
            }

            return result;
        }
    }
}
