using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExwhyzeeSP.Core.Dtos.WhatsApp
{
    public class WhatsAppWebhookPayload
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("entry")]
        public List<WhatsAppEntry>? Entry { get; set; }
    }

    public class WhatsAppEntry
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("changes")]
        public List<WhatsAppChange>? Changes { get; set; }
    }

    public class WhatsAppChange
    {
        [JsonPropertyName("value")]
        public WhatsAppValue? Value { get; set; }

        [JsonPropertyName("field")]
        public string? Field { get; set; }
    }

    public class WhatsAppValue
    {
        [JsonPropertyName("messaging_product")]
        public string? MessagingProduct { get; set; }

        [JsonPropertyName("metadata")]
        public WhatsAppMetadata? Metadata { get; set; }

        [JsonPropertyName("contacts")]
        public List<WhatsAppContact>? Contacts { get; set; }

        [JsonPropertyName("messages")]
        public List<WhatsAppIncomingMessage>? Messages { get; set; }

        [JsonPropertyName("statuses")]
        public List<WhatsAppStatus>? Statuses { get; set; }
    }

    public class WhatsAppMetadata
    {
        [JsonPropertyName("display_phone_number")]
        public string? DisplayPhoneNumber { get; set; }

        [JsonPropertyName("phone_number_id")]
        public string? PhoneNumberId { get; set; }
    }

    public class WhatsAppContact
    {
        [JsonPropertyName("profile")]
        public WhatsAppProfile? Profile { get; set; }

        [JsonPropertyName("wa_id")]
        public string? WaId { get; set; }
    }

    public class WhatsAppProfile
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class WhatsAppIncomingMessage
    {
        [JsonPropertyName("from")]
        public string? From { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("timestamp")]
        public string? Timestamp { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("text")]
        public WhatsAppTextMessage? Text { get; set; }

        [JsonPropertyName("interactive")]
        public WhatsAppInteractiveMessage? Interactive { get; set; }

        [JsonPropertyName("button")]
        public WhatsAppButtonReply? Button { get; set; }
    }

    public class WhatsAppTextMessage
    {
        [JsonPropertyName("body")]
        public string? Body { get; set; }
    }

    public class WhatsAppInteractiveMessage
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("button_reply")]
        public WhatsAppButtonReply? ButtonReply { get; set; }

        [JsonPropertyName("list_reply")]
        public WhatsAppListReply? ListReply { get; set; }
    }

    public class WhatsAppButtonReply
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }
    }

    public class WhatsAppListReply
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    public class WhatsAppStatus
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("timestamp")]
        public string? Timestamp { get; set; }

        [JsonPropertyName("recipient_id")]
        public string? RecipientId { get; set; }
    }
}
