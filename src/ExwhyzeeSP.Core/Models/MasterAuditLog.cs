using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExwhyzeeSP.Core.Models
{
    /// <summary>
    /// Immutable system-wide audit and security log.
    /// Captures user activities, IP addresses, client devices, and authentication events.
    /// </summary>
    public class MasterAuditLog
    {
        [Key]
        public long Id { get; set; }

        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(150)]
        public string UserName { get; set; } = "Anonymous";

        [StringLength(100)]
        public string? UserRole { get; set; }

        [Required]
        [StringLength(60)]
        public string IpAddress { get; set; } = "127.0.0.1";

        [StringLength(500)]
        public string? UserAgent { get; set; }

        [StringLength(50)]
        public string Browser { get; set; } = "Unknown";

        [StringLength(50)]
        public string DeviceType { get; set; } = "Desktop"; // Desktop, Mobile, Tablet, Bot

        [StringLength(50)]
        public string OperatingSystem { get; set; } = "Unknown"; // Windows, macOS, Android, iOS, Linux

        [Required]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty; // Login, Logout, DelegatedSsoLaunch, DelegatedSsoAccept, TenantSuspended, etc.

        [StringLength(50)]
        public string Area { get; set; } = "Master"; // Master, Tenant, Api, Security

        [StringLength(100)]
        public string? TenantSubDomain { get; set; }

        public int? TenantSchoolId { get; set; }

        [StringLength(2000)]
        public string? Details { get; set; }

        [StringLength(30)]
        public string Severity { get; set; } = "Info"; // Info, Warning, Error, Critical
    }
}
