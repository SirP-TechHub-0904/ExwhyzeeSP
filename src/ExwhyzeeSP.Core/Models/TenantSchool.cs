using System;
using System.ComponentModel.DataAnnotations;

namespace ExwhyzeeSP.Core.Models
{
    public enum TenantSuspensionMode
    {
        [Display(Name = "Active (Full Access)")]
        Active = 0,

        [Display(Name = "Full Suspension (Website & Portal Blocked)")]
        FullSuspension = 1,

        [Display(Name = "Portal & Dashboards Only (Public Website Active)")]
        PortalDashboardSuspensionOnly = 2,

        [Display(Name = "Temporary Maintenance Window")]
        MaintenanceMode = 3,

        [Display(Name = "Feature Restricted (Unpaid Modules Blocked)")]
        FeatureRestricted = 4
    }

    /// <summary>
    /// Represents a school tenant in the multi-tenant SaaS architecture.
    /// Stores routing, SQL connection, access control gates, and feature flags.
    /// </summary>
    public class TenantSchool
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Full name of the institution (e.g. Mercy Girls High School).
        /// </summary>
        [Required]
        [StringLength(200)]
        [Display(Name = "School Name")]
        public string SchoolName { get; set; } = string.Empty;

        /// <summary>
        /// Unique subdomain prefix (e.g. "mercygirls" -> mercygirls.exwhyzeesp.com). Blank if unknown.
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Subdomain / URL")]
        public string? SubDomain { get; set; }

        /// <summary>
        /// Optional custom domain (e.g. portal.mercygirls.edu.ng). Blank if unknown.
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Custom Domain / URL")]
        public string? HostDomain { get; set; }

        /// <summary>
        /// Dedicated SQL Server connection string for this school's database.
        /// </summary>
        [Required]
        [Display(Name = "Database Connection String")]
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Master Active status flag.
        /// </summary>
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Granular suspension mode (Full suspension, portal only, maintenance, or feature restricted).
        /// </summary>
        [Display(Name = "Access / Suspension Mode")]
        public TenantSuspensionMode SuspensionMode { get; set; } = TenantSuspensionMode.Active;

        /// <summary>
        /// Internal reason logged by Master Admin (e.g., "Overdue 2026 1st Term Subscription").
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Internal Suspension Reason")]
        public string? SuspensionReason { get; set; }

        /// <summary>
        /// Public / Student / Staff notice message displayed on lockout holding page.
        /// </summary>
        [StringLength(1000)]
        [Display(Name = "Lockout Public Notice Message")]
        public string? SuspensionNotice { get; set; }

        /// <summary>
        /// Grace period deadline before automated lockout triggers.
        /// </summary>
        [Display(Name = "Payment Grace Deadline")]
        public DateTime? GracePeriodDeadline { get; set; }

        // --- Feature Gatekeeper Flags ---
        [Display(Name = "Allow Result Checking & Verification PINs")]
        public bool AllowResultChecking { get; set; } = true;

        [Display(Name = "Allow Online CBT Examination Portal")]
        public bool AllowCbtExams { get; set; } = true;

        [Display(Name = "Allow Online Payment Gateways")]
        public bool AllowOnlinePayment { get; set; } = true;

        [Display(Name = "Allow Staff Portal & Grading Actions")]
        public bool AllowStaffPortal { get; set; } = true;

        [Display(Name = "Allow Student Dashboard & Results")]
        public bool AllowStudentPortal { get; set; } = true;

        [Display(Name = "Allow Online Admissions & Application Forms")]
        public bool AllowAdmissionPortal { get; set; } = true;

        [Display(Name = "Allow SMS Notification Dispatch")]
        public bool AllowSmsNotifications { get; set; } = true;

        [Display(Name = "Allow E-Learning & Digital Lesson Notes")]
        public bool AllowElearning { get; set; } = true;

        [Display(Name = "Date Registered")]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        [Display(Name = "Last Modified")]
        public DateTime? LastModified { get; set; }

        // --- Cloud Backup & Archive Tracking ---
        [Display(Name = "Last Backup Date")]
        public DateTime? LastBackupDate { get; set; }

        [StringLength(500)]
        [Display(Name = "Cloud Backup URL")]
        public string? LastBackupUrl { get; set; }

        [StringLength(50)]
        [Display(Name = "Last Backup Size")]
        public string? LastBackupSize { get; set; }

        // --- FinTech Soft Delete & Anonymization Audit ---
        [Display(Name = "Is Deleted")]
        public bool IsDeleted { get; set; } = false;

        [Display(Name = "Deleted At")]
        public DateTime? DeletedAt { get; set; }

        [StringLength(200)]
        [Display(Name = "Deleted By")]
        public string? DeletedBy { get; set; }

        [StringLength(500)]
        [Display(Name = "Deletion Reason")]
        public string? DeletionReason { get; set; }
    }
}
