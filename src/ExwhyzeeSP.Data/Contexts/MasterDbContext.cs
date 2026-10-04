using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ExwhyzeeSP.Core.Models;

namespace ExwhyzeeSP.Data.Contexts
{
    /// <summary>
    /// Master Entity Framework DbContext used by ExwhyzeeSP.Master and the Tenant Resolution Engine.
    /// Stores tenant school metadata, domains, and connection string mappings.
    /// </summary>
    public class MasterDbContext : IdentityDbContext<MasterAdminUser>
    {
        public MasterDbContext(DbContextOptions<MasterDbContext> options)
            : base(options)
        {
        }

        public DbSet<TenantSchool> TenantSchools { get; set; } = null!;
        public DbSet<Testimony> Testimonies { get; set; } = null!;
        public DbSet<MasterCmsPage> MasterCmsPages { get; set; } = null!;
        public DbSet<Blog> Blogs { get; set; } = null!;
        public DbSet<BlogCategory> BlogCategories { get; set; } = null!;
        public DbSet<MasterPricingPlan> PricingPlans { get; set; } = null!;
        public DbSet<MasterKnowledgeBaseItem> KnowledgeBaseItems { get; set; } = null!;
        public DbSet<MasterSchoolTransaction> SchoolTransactions { get; set; } = null!;
        public DbSet<MasterTenantMetricSnapshot> TenantMetricSnapshots { get; set; } = null!;
        public DbSet<MasterAuditLog> AuditLogs { get; set; } = null!;
        public DbSet<MasterEmailLog> EmailLogs { get; set; } = null!;
        public DbSet<MarketingContact> MarketingContacts { get; set; } = null!;
        public DbSet<MarketingTemplate> MarketingTemplates { get; set; } = null!;
        public DbSet<MarketingCampaign> MarketingCampaigns { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<MasterCmsPage>()
                .HasIndex(p => p.Slug)
                .IsUnique();

            // Configure unique indexes for subdomains and custom domains
            builder.Entity<TenantSchool>()
                .HasIndex(t => t.SubDomain)
                .IsUnique();

            builder.Entity<TenantSchool>()
                .Property(t => t.SchoolName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Entity<TenantSchool>()
                .Property(t => t.SubDomain)
                .IsRequired()
                .HasMaxLength(100);

            builder.Entity<TenantSchool>()
                .Property(t => t.ConnectionString)
                .IsRequired();

            // Soft-delete global query filter
            builder.Entity<TenantSchool>()
                .HasQueryFilter(t => !t.IsDeleted);
        }
    }
}
