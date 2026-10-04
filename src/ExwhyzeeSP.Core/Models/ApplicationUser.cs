using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using ExwhyzeeSP.Core.Entities;

namespace ExwhyzeeSP.Core.Models
{
    /// <summary>
    /// Application User entity for school users (Admins, Staff, Students, Parents).
    /// Compatible with existing ASP.NET Identity password hashes.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Surname / Last Name")]
        public string? Surname { get; set; }

        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Other Name")]
        public string? OtherName { get; set; }

        [Display(Name = "Date Of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Date Registered")]
        public DateTime DateRegistered { get; set; } = DateTime.UtcNow;

        public EntityStatus Status { get; set; } = EntityStatus.Active;

        public DateTime? DataStatusChanged { get; set; }

        public string? Religion { get; set; }

        public string? Gender { get; set; }

        [Display(Name = "Contact Address")]
        public string? ContactAddress { get; set; }

        public string? City { get; set; }

        [Display(Name = "Phone Number")]
        public string? Phone { get; set; }

        [Display(Name = "Local Government Area")]
        public string? LocalGov { get; set; }

        [Display(Name = "State Of Origin")]
        public string? StateOfOrigin { get; set; }

        public string? Nationality { get; set; } = "Nigerian";

        public string? RegisteredBy { get; set; }

        public bool? IsLocked { get; set; } = false;

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public DateTime? LockoutEndDateUtc
        {
            get => LockoutEnd?.UtcDateTime;
            set => LockoutEnd = value.HasValue ? new DateTimeOffset(value.Value) : null;
        }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; } = new List<IdentityUserRole<string>>();

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; } = new List<IdentityUserClaim<string>>();

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; } = new List<IdentityUserLogin<string>>();

        /// <summary>
        /// Helper property to get full formatted name.
        /// </summary>
        public string FullName => $"{Surname} {FirstName} {OtherName}".Trim();
    }
}
