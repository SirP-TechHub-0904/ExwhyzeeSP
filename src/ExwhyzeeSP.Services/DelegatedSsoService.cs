using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ExwhyzeeSP.Services
{
    /// <summary>
    /// Cryptographically signs and validates short-lived, single-use delegated SSO tokens
    /// allowing Master SuperAdmins and Admins to seamlessly access tenant school instances.
    /// </summary>
    public class DelegatedSsoService : IDelegatedSsoService
    {
        private const string Issuer = "ExwhyzeeSP.Master";
        private const string Audience = "ExwhyzeeSP.Tenant";

        private byte[] GetSigningKey()
        {
            var envKey = Environment.GetEnvironmentVariable("MASTER_INTEROP_SIGNING_KEY");
            if (!string.IsNullOrWhiteSpace(envKey) && envKey.Length >= 32)
            {
                return Encoding.UTF8.GetBytes(envKey);
            }

            // Fallback deterministic derivation from Master DB connection or platform salt
            var seed = Environment.GetEnvironmentVariable("MASTER_DB_CONNECTION") ?? "ExwhyzeeSP_Master_Secret_Salt_Key_2026";
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(seed + "_SSO_INTEROP_SECRET"));
        }

        public string GenerateDelegatedToken(string masterEmail, string targetSubDomain, string requestedRole, string reason, int expirySeconds = 60)
        {
            var keyBytes = GetSigningKey();
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;
            var expires = now.AddSeconds(expirySeconds);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, masterEmail),
                new Claim(JwtRegisteredClaimNames.Email, masterEmail),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("target_subdomain", targetSubDomain?.ToLowerInvariant() ?? string.Empty),
                new Claim("requested_role", requestedRole ?? "SuperAdmin"),
                new Claim("reason", reason ?? "Master Platform Support Inspection"),
                new Claim("is_delegated", "true")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = Issuer,
                Audience = Audience,
                IssuedAt = now,
                NotBefore = now.AddSeconds(-5), // 5 seconds clock skew grace
                Expires = expires,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public DelegatedSsoValidationResult ValidateDelegatedToken(string token, string? expectedSubdomain = null)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return new DelegatedSsoValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Delegated SSO token is missing."
                };
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = GetSigningKey();
            var securityKey = new SymmetricSecurityKey(keyBytes);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Issuer,
                ValidateAudience = true,
                ValidAudience = Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(10)
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                if (validatedToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return new DelegatedSsoValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "Invalid cryptographic algorithm on token."
                    };
                }

                var email = principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value 
                         ?? principal.FindFirst(ClaimTypes.Email)?.Value 
                         ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value 
                         ?? string.Empty;

                var targetSub = principal.FindFirst("target_subdomain")?.Value ?? string.Empty;
                var role = principal.FindFirst("requested_role")?.Value ?? "SuperAdmin";
                var reason = principal.FindFirst("reason")?.Value ?? string.Empty;
                var nonce = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(expectedSubdomain) &&
                    !string.IsNullOrWhiteSpace(targetSub) &&
                    !string.Equals(targetSub, expectedSubdomain, StringComparison.OrdinalIgnoreCase))
                {
                    return new DelegatedSsoValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = $"Target subdomain mismatch (token targets '{targetSub}', active tenant is '{expectedSubdomain}')."
                    };
                }

                return new DelegatedSsoValidationResult
                {
                    IsValid = true,
                    MasterEmail = email,
                    TargetSubDomain = targetSub,
                    RequestedRole = role,
                    Reason = reason,
                    Nonce = nonce,
                    IssuedAtUtc = jwtToken.ValidFrom,
                    ExpiresAtUtc = jwtToken.ValidTo
                };
            }
            catch (SecurityTokenExpiredException)
            {
                return new DelegatedSsoValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Delegated login token has expired (tokens are valid for 60 seconds). Please launch again from Master."
                };
            }
            catch (Exception ex)
            {
                return new DelegatedSsoValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Token verification failed: {ex.Message}"
                };
            }
        }
    }
}
