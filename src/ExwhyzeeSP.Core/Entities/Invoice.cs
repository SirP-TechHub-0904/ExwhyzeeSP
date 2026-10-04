using System;
using System.ComponentModel.DataAnnotations;
using ExwhyzeeSP.Core.Models;

namespace ExwhyzeeSP.Core.Entities
{
    public class Invoice
    {
        public Invoice()
        {
            this.InvoiceNumber = DateTime.UtcNow.ToString("yyyyMMdd") + Guid.NewGuid().ToString("N")[..4].ToUpper() + "INV";
            this.CreatedDate = DateTime.UtcNow;
            this.Amount = 0;
        }

        public int Id { get; set; }
        public string? ClassName { get; set; }
        public string? Title { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Amount { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public PaymentStatus Status { get; set; }

        [Display(Name = "Registration Number")]
        public string? RegistrationNumber { get; set; }
        public string? TransactionReference { get; set; }
        public int? EnrollmentId { get; set; }
        public Enrollment? Enrollment { get; set; }
        public int? SessionId { get; set; }
        public Session? Session { get; set; }
    }
}
