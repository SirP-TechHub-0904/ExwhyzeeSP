using System;
using System.ComponentModel.DataAnnotations;

namespace ExwhyzeeSP.Core.Entities
{
    public class FinanceInitializer
    {
        public FinanceInitializer()
        {
            this.InvoiceNumber = DateTime.UtcNow.ToString("yyyyMMdd") + Guid.NewGuid().ToString("N")[..4].ToUpper() + "INV";
        }

        public int Id { get; set; }
        public int EnrollmentId { get; set; }
        public Enrollment? Enrollment { get; set; }
        public int IncomeId { get; set; }
        public Income? Income { get; set; }
        public string InvoiceNumber { get; set; }
        public int PaymentAmountId { get; set; }
        public int SessionId { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public int Percent { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? UniqueId { get; set; }
        public TransactionStatus TransactionStatus { get; set; }

        [Display(Name = "Reference Id")]
        public string? ReferenceId { get; set; }

        [Display(Name = "Finance Source")]
        public FinanceSource FinanceSource { get; set; }

        [Display(Name = "Finance Type")]
        public FinanceType FinanceType { get; set; }
        public int PaymentTypeId { get; set; }
        public bool Payall { get; set; }
    }
}
