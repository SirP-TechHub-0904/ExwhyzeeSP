using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace ExwhyzeeSP.Services.Provisioning
{
    public class DatabaseProvisioningService : IDatabaseProvisioningService
    {
        public string GenerateSecurePassword(int length = 16)
        {
            const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            const string lower = "abcdefghijkmnopqrstuvwxyz";
            const string digits = "23456789";
            const string specials = "!@#$%^&*";
            const string all = upper + lower + digits + specials;

            var bytes = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);

            var result = new StringBuilder(length);
            // Ensure at least one of each category
            result.Append(upper[bytes[0] % upper.Length]);
            result.Append(lower[bytes[1] % lower.Length]);
            result.Append(digits[bytes[2] % digits.Length]);
            result.Append(specials[bytes[3] % specials.Length]);

            for (int i = 4; i < length; i++)
            {
                result.Append(all[bytes[i] % all.Length]);
            }

            return result.ToString();
        }

        public string BuildConnectionString(string host, int? port, string databaseName, string username, string password, bool integratedSecurity = false)
        {
            var serverAddress = host;
            if (port.HasValue && port.Value > 0 && !serverAddress.Contains(",") && !serverAddress.Contains("\\"))
            {
                serverAddress = $"{serverAddress},{port.Value}";
            }

            if (integratedSecurity)
            {
                return $"Server={serverAddress};Database={databaseName};Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;";
            }

            return $"Server={serverAddress};Database={databaseName};User Id={username};Password={password};TrustServerCertificate=True;MultipleActiveResultSets=true;";
        }

        public async Task<bool> TestAdminConnectionAsync(DatabaseServerConfig config)
        {
            try
            {
                var connStr = config.GetAdminConnectionString();
                using var conn = new SqlConnection(connStr);
                await conn.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ProvisioningResult> ProvisionDatabaseAndUserAsync(
            DatabaseServerConfig serverConfig,
            string databaseName,
            string username,
            string password)
        {
            var result = new ProvisioningResult
            {
                DatabaseName = databaseName,
                Username = username,
                Password = password
            };

            // Sanitize identifier names (only alphanumeric and underscore)
            var cleanDbName = Regex.Replace(databaseName.Trim(), @"[^a-zA-Z0-9_]", "_");
            var cleanUsername = Regex.Replace(username.Trim(), @"[^a-zA-Z0-9_]", "_");

            if (string.IsNullOrWhiteSpace(cleanDbName))
            {
                result.Success = false;
                result.Message = "Invalid database name.";
                return result;
            }

            if (!serverConfig.UseIntegratedSecurity && string.IsNullOrWhiteSpace(cleanUsername))
            {
                result.Success = false;
                result.Message = "Invalid username.";
                return result;
            }

            try
            {
                var adminConnStr = serverConfig.GetAdminConnectionString();
                using var conn = new SqlConnection(adminConnStr);
                await conn.OpenAsync();

                // 1. Create Database if not exists
                var createDbSql = $@"
                    IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = @dbName)
                    BEGIN
                        CREATE DATABASE [{cleanDbName}];
                    END";

                using (var cmd = new SqlCommand(createDbSql, conn))
                {
                    cmd.Parameters.AddWithValue("@dbName", cleanDbName);
                    await cmd.ExecuteNonQueryAsync();
                }

                // 2. If not using integrated security, create SQL Server Login and Database User
                if (!serverConfig.UseIntegratedSecurity)
                {
                    // Escape single quotes for SQL password literal
                    var escapedPassword = password.Replace("'", "''");

                    // Create Login at Server level
                    var createLoginSql = $@"
                        IF NOT EXISTS (SELECT name FROM sys.server_principals WHERE name = @loginName)
                        BEGIN
                            CREATE LOGIN [{cleanUsername}] WITH PASSWORD = '{escapedPassword}', CHECK_POLICY = OFF;
                        END
                        ELSE
                        BEGIN
                            ALTER LOGIN [{cleanUsername}] WITH PASSWORD = '{escapedPassword}';
                        END";

                    using (var cmd = new SqlCommand(createLoginSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@loginName", cleanUsername);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    // Create User in the target Database and assign db_owner role
                    var createUserSql = $@"
                        USE [{cleanDbName}];
                        IF NOT EXISTS (SELECT name FROM sys.database_principals WHERE name = @userName)
                        BEGIN
                            CREATE USER [{cleanUsername}] FOR LOGIN [{cleanUsername}];
                        END;
                        ALTER ROLE [db_owner] ADD MEMBER [{cleanUsername}];";

                    using (var cmd = new SqlCommand(createUserSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@userName", cleanUsername);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                // 3. Build the final dedicated connection string
                result.ConnectionString = BuildConnectionString(
                    serverConfig.ServerHost,
                    serverConfig.Port,
                    cleanDbName,
                    cleanUsername,
                    password,
                    serverConfig.UseIntegratedSecurity);

                result.Success = true;
                result.Message = $"Database [{cleanDbName}] and dedicated user [{cleanUsername}] provisioned successfully!";
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Database provisioning error: {ex.Message}";
                return result;
            }
        }
    }
}
