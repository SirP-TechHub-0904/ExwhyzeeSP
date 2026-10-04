using System;
using System.ComponentModel.DataAnnotations;

namespace ExwhyzeeSP.Master.Models
{
    public enum DatabaseProvisionMode
    {
        [Display(Name = "Existing Connection String (Paste)")]
        ExistingConnectionString = 1,

        [Display(Name = "Auto-Create Database & Dedicated SQL User")]
        AutoCreateDatabase = 2
    }

    public class CreateTenantViewModel
    {
        [Required(ErrorMessage = "School Name is required")]
        [StringLength(200)]
        [Display(Name = "School Name")]
        public string SchoolName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subdomain is required")]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9\-]+$", ErrorMessage = "Subdomain can only contain letters, numbers, and hyphens.")]
        [Display(Name = "Subdomain Prefix")]
        public string SubDomain { get; set; } = string.Empty;

        [StringLength(200)]
        [Display(Name = "Custom Domain (Optional)")]
        public string? HostDomain { get; set; }

        [Display(Name = "Database Setup Option")]
        public DatabaseProvisionMode ProvisionMode { get; set; } = DatabaseProvisionMode.AutoCreateDatabase;

        // Option A: Paste Connection String
        [Display(Name = "Database Connection String")]
        public string? ConnectionString { get; set; }

        // Option B: Auto-Provision Server Settings
        [Display(Name = "Database Server IP / Host")]
        public string ServerHost { get; set; } = "localhost";

        [Display(Name = "SQL Port")]
        public int? ServerPort { get; set; }

        [Display(Name = "SQL Admin User (e.g. sa)")]
        public string? AdminUser { get; set; } = "sa";

        [Display(Name = "SQL Admin Password")]
        public string? AdminPassword { get; set; }

        [Display(Name = "Use Windows Auth / Integrated Security")]
        public bool UseIntegratedSecurity { get; set; } = false;

        [Display(Name = "Custom Database Name (Leave blank to auto-generate)")]
        public string? CustomDatabaseName { get; set; }

        [Display(Name = "Custom DB Username (Leave blank to auto-generate)")]
        public string? CustomUsername { get; set; }

        [Display(Name = "Custom DB Password (Leave blank to auto-generate)")]
        public string? CustomPassword { get; set; }

        [Display(Name = "Run EF Core Migrations Immediately")]
        public bool RunMigrationsImmediately { get; set; } = true;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }
}
