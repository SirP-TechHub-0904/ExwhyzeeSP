using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ExwhyzeeSP.Services.WhatsApp
{
    public class WhatsAppCloudApiService : IWhatsAppCloudApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WhatsAppCloudApiService> _logger;

        public WhatsAppCloudApiService(HttpClient httpClient, IConfiguration configuration, ILogger<WhatsAppCloudApiService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        private (string ApiUrl, string AccessToken, string PhoneNumberId) GetCredentials()
        {
            var accessToken = Environment.GetEnvironmentVariable("WHATSAPP_ACCESS_TOKEN")
                ?? _configuration["WhatsApp:AccessToken"] ?? "";

            var phoneNumberId = Environment.GetEnvironmentVariable("WHATSAPP_PHONE_NUMBER_ID")
                ?? _configuration["WhatsApp:PhoneNumberId"] ?? "";

            var apiVersion = _configuration["WhatsApp:ApiVersion"] ?? "v20.0";

            var apiUrl = $"https://graph.facebook.com/{apiVersion}/{phoneNumberId}/messages";
            return (apiUrl, accessToken, phoneNumberId);
        }

        public async Task<bool> SendTextMessageAsync(string toPhoneNumber, string message)
        {
            var (apiUrl, accessToken, phoneNumberId) = GetCredentials();
            if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(phoneNumberId))
            {
                _logger.LogWarning("WhatsApp Cloud API credentials not configured. Mocking response for {Phone}: {Message}", toPhoneNumber, message);
                return false;
            }

            var payload = new
            {
                messaging_product = "whatsapp",
                recipient_type = "individual",
                to = CleanPhoneNumber(toPhoneNumber),
                type = "text",
                text = new { preview_url = true, body = message }
            };

            return await PostToWhatsAppApiAsync(apiUrl, accessToken, payload);
        }

        public async Task<bool> SendQuickReplyButtonsAsync(string toPhoneNumber, string headerText, string bodyText, List<(string Id, string Title)> buttons, string? footerText = null)
        {
            var (apiUrl, accessToken, phoneNumberId) = GetCredentials();
            if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(phoneNumberId))
            {
                _logger.LogWarning("WhatsApp Cloud API credentials not configured.");
                return false;
            }

            var buttonElements = new List<object>();
            foreach (var b in buttons)
            {
                buttonElements.Add(new
                {
                    type = "reply",
                    reply = new { id = b.Id, title = b.Title.Length > 20 ? b.Title.Substring(0, 20) : b.Title }
                });
            }

            var interactiveObject = new Dictionary<string, object>
            {
                ["type"] = "button",
                ["header"] = new { type = "text", text = headerText },
                ["body"] = new { text = bodyText },
                ["action"] = new { buttons = buttonElements }
            };

            if (!string.IsNullOrEmpty(footerText))
            {
                interactiveObject["footer"] = new { text = footerText };
            }

            var payload = new
            {
                messaging_product = "whatsapp",
                recipient_type = "individual",
                to = CleanPhoneNumber(toPhoneNumber),
                type = "interactive",
                interactive = interactiveObject
            };

            return await PostToWhatsAppApiAsync(apiUrl, accessToken, payload);
        }

        public async Task<bool> SendListMenuAsync(string toPhoneNumber, string headerText, string bodyText, string buttonText, List<(string Id, string Title, string Description)> sections, string? footerText = null)
        {
            var (apiUrl, accessToken, phoneNumberId) = GetCredentials();
            if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(phoneNumberId))
            {
                return false;
            }

            var rows = new List<object>();
            foreach (var s in sections)
            {
                rows.Add(new
                {
                    id = s.Id,
                    title = s.Title.Length > 24 ? s.Title.Substring(0, 24) : s.Title,
                    description = s.Description.Length > 72 ? s.Description.Substring(0, 72) : s.Description
                });
            }

            var interactiveObject = new Dictionary<string, object>
            {
                ["type"] = "list",
                ["header"] = new { type = "text", text = headerText },
                ["body"] = new { text = bodyText },
                ["action"] = new
                {
                    button = buttonText.Length > 20 ? buttonText.Substring(0, 20) : buttonText,
                    sections = new[]
                    {
                        new { title = "Available Services", rows = rows }
                    }
                }
            };

            if (!string.IsNullOrEmpty(footerText))
            {
                interactiveObject["footer"] = new { text = footerText };
            }

            var payload = new
            {
                messaging_product = "whatsapp",
                recipient_type = "individual",
                to = CleanPhoneNumber(toPhoneNumber),
                type = "interactive",
                interactive = interactiveObject
            };

            return await PostToWhatsAppApiAsync(apiUrl, accessToken, payload);
        }

        private async Task<bool> PostToWhatsAppApiAsync(string apiUrl, string accessToken, object payload)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Content = JsonContent.Create(payload);

                var response = await _httpClient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("WhatsApp message delivered successfully.");
                    return true;
                }
                else
                {
                    _logger.LogError("WhatsApp API error: {StatusCode} - {Body}", response.StatusCode, responseBody);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while sending WhatsApp message.");
                return false;
            }
        }

        private static string CleanPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return "";
            var cleaned = System.Text.RegularExpressions.Regex.Replace(phone, @"[^\d]", "");
            if (cleaned.StartsWith("0") && cleaned.Length == 11)
            {
                cleaned = "234" + cleaned.Substring(1);
            }
            return cleaned;
        }
    }
}
