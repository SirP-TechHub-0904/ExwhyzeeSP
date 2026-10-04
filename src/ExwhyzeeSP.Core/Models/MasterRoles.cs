namespace ExwhyzeeSP.Core.Models
{
    public static class MasterRoles
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string ContentManager = "ContentManager";
        public const string Accountant = "Accountant";
        public const string ReadOnly = "ReadOnly";

        public static readonly string[] All = { SuperAdmin, Admin, ContentManager, Accountant, ReadOnly };
        
        /// <summary>
        /// Roles that can be assigned to new or existing users. SuperAdmin is strictly reserved for the root account.
        /// </summary>
        public static readonly string[] AssignableRoles = { Admin, ContentManager, Accountant, ReadOnly };
    }
}
