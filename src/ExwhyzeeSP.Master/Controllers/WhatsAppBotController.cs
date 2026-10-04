using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ExwhyzeeSP.Core.Dtos.WhatsApp;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Services.WhatsApp;

namespace ExwhyzeeSP.Master.Controllers
{
    [ApiController]
    [Route("api/whatsapp")]
    public class WhatsAppBotController : ControllerBase
    {
        private readonly MasterDbContext _context;
        private readonly IWhatsAppCloudApiService _whatsAppService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WhatsAppBotController> _logger;

        public WhatsAppBotController(
            MasterDbContext context,
            IWhatsAppCloudApiService whatsAppService,
            IConfiguration configuration,
            ILogger<WhatsAppBotController> logger)
        {
            _context = context;
            _whatsAppService = whatsAppService;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// 1. Meta Webhook Verification Handshake
        /// Meta calls this endpoint when configuring the Webhook in developers.facebook.com
        /// </summary>
        [HttpGet("webhook")]
        public IActionResult VerifyWebhook(
            [FromQuery(Name = "hub.mode")] string? mode,
            [FromQuery(Name = "hub.verify_token")] string? token,
            [FromQuery(Name = "hub.challenge")] string? challenge)
        {
            var configuredToken = Environment.GetEnvironmentVariable("WHATSAPP_VERIFY_TOKEN")
                ?? _configuration["WhatsApp:VerifyToken"]
                ?? "ExwhyzeeSP_Secret_Verify_Token_2026";

            if (mode == "subscribe" && token == configuredToken)
            {
                _logger.LogInformation("Meta WhatsApp Webhook verified successfully.");
                return Ok(challenge);
            }

            _logger.LogWarning("Meta WhatsApp Webhook verification failed. Received Token: {Token}", token);
            return Forbid();
        }

        /// <summary>
        /// 2. Incoming Messages Webhook
        /// Meta sends events here whenever a user sends a message, clicks a button, or selects from a list.
        /// </summary>
        [HttpPost("webhook")]
        public async Task<IActionResult> ReceiveWebhook([FromBody] WhatsAppWebhookPayload payload)
        {
            try
            {
                var change = payload?.Entry?.FirstOrDefault()?.Changes?.FirstOrDefault()?.Value;
                var message = change?.Messages?.FirstOrDefault();

                if (message == null)
                {
                    // Acknowledge delivery receipts, read statuses, etc.
                    return Ok();
                }

                var fromNumber = message.From;
                if (string.IsNullOrEmpty(fromNumber)) return Ok();

                var senderProfile = change?.Contacts?.FirstOrDefault()?.Profile?.Name ?? "Educator / Parent";

                // Extract user input text or button selection ID
                string userInput = "";
                if (message.Type == "text" && message.Text != null)
                {
                    userInput = message.Text.Body?.Trim() ?? "";
                }
                else if (message.Type == "interactive" && message.Interactive != null)
                {
                    if (message.Interactive.ButtonReply != null)
                    {
                        userInput = message.Interactive.ButtonReply.Id ?? message.Interactive.ButtonReply.Title ?? "";
                    }
                    else if (message.Interactive.ListReply != null)
                    {
                        userInput = message.Interactive.ListReply.Id ?? message.Interactive.ListReply.Title ?? "";
                    }
                }
                else if (message.Type == "button" && message.Button != null)
                {
                    userInput = message.Button.Id ?? message.Button.Title ?? "";
                }

                _logger.LogInformation("Incoming WhatsApp message from {From} ({Name}): {Input}", fromNumber, senderProfile, userInput);

                await ProcessBotResponseAsync(fromNumber, senderProfile, userInput);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing WhatsApp webhook event.");
            }

            return Ok();
        }

        private async Task ProcessBotResponseAsync(string fromNumber, string senderName, string input)
        {
            var lower = input.ToLowerInvariant().Trim();

            // Match Main Actions
            if (lower == "menu" || lower == "hi" || lower == "hello" || lower == "start" || lower == "help" || string.IsNullOrEmpty(lower))
            {
                var buttons = new List<(string Id, string Title)>
                {
                    ("btn_demo", "Schedule Live Demo"),
                    ("btn_pricing", "View Pricing Plans"),
                    ("btn_guides", "Operating Guides")
                };

                await _whatsAppService.SendQuickReplyButtonsAsync(
                    fromNumber,
                    "Exwhyzee SP Portal",
                    $"Hello {senderName}! Welcome to Exwhyzee SP Cloud School Management.\n\nAutomate your school's Admissions, CBT Exams, Error-Free Report Cards, and Fee Billing.\n\nHow can we assist your institution today?",
                    buttons,
                    "Exwhyzee Technologies • Support: 08165680904"
                );
                return;
            }

            if (lower == "btn_demo" || lower.Contains("demo"))
            {
                var reply = $"Dear {senderName},\n\nTo schedule a personalized live virtual walkthrough of Exwhyzee SP with a deployment engineer, please visit:\nhttps://exwhyzeesp.com/Demo\n\nOr reply directly here with:\n• School Name\n• Location/State\n• Estimated Student Population\n\nOur team will contact you promptly!";
                await _whatsAppService.SendTextMessageAsync(fromNumber, reply);
                return;
            }

            if (lower == "btn_pricing" || lower.Contains("pricing") || lower.Contains("fees") || lower.Contains("cost"))
            {
                var plans = await _context.PricingPlans.Where(p => p.IsActive).OrderBy(p => p.SortOrder).ToListAsync();
                var text = $"Exwhyzee SP Flexible Billing Models:\n\n";
                foreach (var p in plans)
                {
                    text += $"• *{p.Name}*: {p.PriceText} {p.BillingCycle}\n  _{p.Subtitle}_\n\n";
                }
                text += "Explore all plans in detail:\nhttps://exwhyzeesp.com/Pricing";
                await _whatsAppService.SendTextMessageAsync(fromNumber, text);
                return;
            }

            if (lower == "btn_guides" || lower.Contains("guide") || lower.Contains("kb") || lower.Contains("manual"))
            {
                var sections = new List<(string Id, string Title, string Description)>
                {
                    ("kb_admin", "Administrator Manual", "Sessions, broadsheets, PIN generation"),
                    ("kb_staff", "Teacher & Staff Guide", "Scoresheet entry, lesson notes, CBT"),
                    ("kb_student", "Student & Parent Guide", "Result checking, CBT tests, notes")
                };

                await _whatsAppService.SendListMenuAsync(
                    fromNumber,
                    "Knowledge Base Directory",
                    "Browse step-by-step operating instructions for every portal role:",
                    "Select Guide",
                    sections,
                    "Online documentation: exwhyzeesp.com/KnowledgeBase"
                );
                return;
            }

            if (lower == "kb_admin" || lower == "kb_staff" || lower == "kb_student")
            {
                var role = lower == "kb_admin" ? "Admin" : lower == "kb_staff" ? "Staff" : "Student";
                var items = await _context.KnowledgeBaseItems
                    .Where(k => k.IsPublished && k.TargetRole == role)
                    .OrderBy(k => k.SortOrder)
                    .Take(5)
                    .ToListAsync();

                var text = $"*{role} Operating Guides:*\n\n";
                foreach (var item in items)
                {
                    text += $"* {item.Title}*\nRoute: `{item.RouteUrl}`\n{item.WhatItDoes}\n\n";
                }
                text += $"Full directory: https://exwhyzeesp.com/KnowledgeBase?role={role}";
                await _whatsAppService.SendTextMessageAsync(fromNumber, text);
                return;
            }

            // Keyword Search in Knowledge Base
            var matchedGuide = await _context.KnowledgeBaseItems
                .Where(k => k.IsPublished && (
                    k.Title.Contains(input) ||
                    k.WhatItDoes.Contains(input) ||
                    k.RouteUrl.Contains(input)))
                .FirstOrDefaultAsync();

            if (matchedGuide != null)
            {
                var reply = $"*{matchedGuide.Title}* ({matchedGuide.TargetRole} Module)\n\n" +
                            $"*Route:* `{matchedGuide.RouteUrl}`\n\n" +
                            $"*What This Page Does:*\n{matchedGuide.WhatItDoes}\n\n" +
                            $"*Instructions:*\n{matchedGuide.Instructions}\n\n" +
                            $"View online: https://exwhyzeesp.com/KnowledgeBase#{matchedGuide.Title.ToLower().Replace(" ", "-")}";

                await _whatsAppService.SendTextMessageAsync(fromNumber, reply);
                return;
            }

            // Default Fallback
            var fallback = $"Thank you for your message, {senderName}!\n\n" +
                           $"I couldn't find an exact match for \"{input}\".\n\n" +
                           $"Reply with:\n" +
                           $"• *1* - Schedule Live Demo\n" +
                           $"• *2* - View Pricing Plans\n" +
                           $"• *3* - Knowledge Base Guides\n" +
                           $"• *4* - Speak with Technical Support (08165680904)";

            await _whatsAppService.SendTextMessageAsync(fromNumber, fallback);
        }
    }
}
