using System;
using System.ComponentModel.DataAnnotations;

namespace ExwhyzeeSP.Master.Models
{
    public class DeleteTenantViewModel
    {
        public int Id { get; set; }
        public string SchoolName { get; set; } = string.Empty;
        public string? SubDomain { get; set; }
        public string? HostDomain { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsActive { get; set; }
        public int TransactionCount { get; set; }
        public decimal TransactionVolume { get; set; }

        [Required(ErrorMessage = "Please type the required confirmation phrase.")]
        [Display(Name = "Confirmation Phrase")]
        public string ConfirmationText { get; set; } = string.Empty;

        [Required(ErrorMessage = "Administrator account password is required for security verification.")]
        [DataType(DataType.Password)]
        [Display(Name = "Admin Password")]
        public string AdminPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "A regulatory audit reason is required.")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Please enter a reason with at least 5 characters.")]
        [Display(Name = "Reason for Deletion")]
        public string Reason { get; set; } = string.Empty;

        [Range(typeof(bool), "true", "true", ErrorMessage = "You must acknowledge the consequences to proceed.")]
        [Display(Name = "I acknowledge that this action will permanently decommission this school and revoke all domain and portal access.")]
        public bool AcknowledgeConsequences { get; set; }
    }
}
