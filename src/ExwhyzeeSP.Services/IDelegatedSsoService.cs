using System;

namespace ExwhyzeeSP.Services
{
    public class DelegatedSsoValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string MasterEmail { get; set; } = string.Empty;
        public string TargetSubDomain { get; set; } = string.Empty;
        public string RequestedRole { get; set; } = "SuperAdmin";
        public string Reason { get; set; } = string.Empty;
        public string Nonce { get; set; } = string.Empty;
        public DateTime IssuedAtUtc { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
    }

    public interface IDelegatedSsoService
    {
        string GenerateDelegatedToken(string masterEmail, string targetSubDomain, string requestedRole, string reason, int expirySeconds = 60);
        DelegatedSsoValidationResult ValidateDelegatedToken(string token, string? expectedSubdomain = null);
    }
}
