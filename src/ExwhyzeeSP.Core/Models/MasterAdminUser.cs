using System;
using Microsoft.AspNetCore.Identity;

namespace ExwhyzeeSP.Core.Models
{
    /// <summary>
    /// Identity user model for SuperAdmins and platform managers accessing ExwhyzeeSP.Master.
    /// Completely isolated from individual school user databases.
    /// </summary>
    public class MasterAdminUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string RoleTitle { get; set; } = "SuperAdmin";
        public string? ProfileImageUrl { get; set; }
        public string TwoFactorChannel { get; set; } = "Email"; // "Email", "Authenticator", "None"
        public string? CurrentOtpHash { get; set; }
        public DateTime? OtpExpiryUtc { get; set; }
        public DateTime? LastLoginUtc { get; set; }
        public string? LastLoginIp { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public bool RequirePasswordChange { get; set; } = false;
        public string? SuspensionReason { get; set; }
        public DateTime? SuspendedAtUtc { get; set; }
    }
}

