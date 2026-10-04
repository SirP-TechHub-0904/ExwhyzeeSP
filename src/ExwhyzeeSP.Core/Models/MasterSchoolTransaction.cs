using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExwhyzeeSP.Core.Models
{
    public enum SchoolBillingType
    {
        [Display(Name = "Termly School Portal Subscription")]
        TermlySubscription = 1,

        [Display(Name = "Annual School Portal License")]
        AnnualLicense = 2,

        [Display(Name = "Initial Deployment & Setup Fee")]
        SetupAndOnboarding = 3,

        [Display(Name = "Scratch Card / Result Processing PINs")]
        ResultPINs = 4,

        [Display(Name = "Custom Domain & SSL Setup")]
        DomainAndSSL = 5,

        [Display(Name = "Staff CBT & Portal Training")]
        Training = 6,

        [Display(Name = "Custom Feature Development")]
        CustomDevelopment = 7,

        [Display(Name = "Other Educational Services")]
        Other = 8
    }

    public enum SchoolPaymentStatus
    {
        [Display(Name = "Pending Payment")]
        Pending = 1,

        [Display(Name = "Paid / Confirmed")]
        Paid = 2,

        [Display(Name = "Overdue")]
        Overdue = 3,

        [Display(Name = "Partially Paid")]
        PartiallyPaid = 4,

        [Display(Name = "Cancelled / Waived")]
        Cancelled = 5
    }

    public enum SchoolPaymentChannel
    {
        [Display(Name = "Direct Bank Transfer / Wire")]
        BankTransfer = 1,

        [Display(Name = "Paystack Online")]
        Paystack = 2,

        [Display(Name = "Flutterwave Online")]
        Flutterwave = 3,

        [Display(Name = "Cash Payment")]
        Cash = 4,

        [Display(Name = "POS / Card Payment")]
        POS = 5,

        [Display(Name = "Bank Draft / Cheque")]
        Cheque = 6
    }

    /// <summary>
    /// Master financial ledger recording all school billing invoices, payments, renewal subscriptions, and PIN orders.
    /// </summary>
    public class MasterSchoolTransaction
    {
        public long Id { get; set; }

        [Required]
        public int TenantSchoolId { get; set; }

        [ForeignKey("TenantSchoolId")]
        public virtual TenantSchool? TenantSchool { get; set; }

        [Required]
        [StringLength(50)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public SchoolBillingType BillingType { get; set; } = SchoolBillingType.TermlySubscription;

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999999.99)]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999999.99)]
        public decimal AmountPaid { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BalanceDue => Amount - AmountPaid;

        public SchoolPaymentStatus Status { get; set; } = SchoolPaymentStatus.Pending;

        public SchoolPaymentChannel PaymentChannel { get; set; } = SchoolPaymentChannel.BankTransfer;

        [StringLength(100)]
        public string? BankReference { get; set; }

        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(14);

        public DateTime? PaidDate { get; set; }

        [StringLength(500)]
        public string? ProofDocumentUrl { get; set; }

        [StringLength(100)]
        public string? ReceiptNumber { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(150)]
        public string? RecordedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
