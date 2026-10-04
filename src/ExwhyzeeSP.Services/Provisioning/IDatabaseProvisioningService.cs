using System;
using System.Threading.Tasks;

namespace ExwhyzeeSP.Services.Provisioning
{
    public class DatabaseServerConfig
    {
        public string ServerHost { get; set; } = "localhost";
        public int? Port { get; set; }
        public string? AdminUser { get; set; }
        public string? AdminPassword { get; set; }
        public bool UseIntegratedSecurity { get; set; } = false;

        public string GetAdminConnectionString()
        {
            var serverAddress = ServerHost;
            if (Port.HasValue && Port.Value > 0 && !serverAddress.Contains(",") && !serverAddress.Contains("\\"))
            {
                serverAddress = $"{serverAddress},{Port.Value}";
            }

            if (UseIntegratedSecurity)
            {
                return $"Server={serverAddress};Database=master;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;";
            }

            return $"Server={serverAddress};Database=master;User Id={AdminUser};Password={AdminPassword};TrustServerCertificate=True;MultipleActiveResultSets=true;";
        }
    }

    public class ProvisioningResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
    }

    public interface IDatabaseProvisioningService
    {
        string GenerateSecurePassword(int length = 16);
        string BuildConnectionString(string host, int? port, string databaseName, string username, string password, bool integratedSecurity = false);
        Task<bool> TestAdminConnectionAsync(DatabaseServerConfig config);
        Task<ProvisioningResult> ProvisionDatabaseAndUserAsync(DatabaseServerConfig serverConfig, string databaseName, string username, string password);
    }
}
