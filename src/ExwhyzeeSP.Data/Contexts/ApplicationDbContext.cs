using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ExwhyzeeSP.Core.Entities;
using ExwhyzeeSP.Core.Entities.ResultArchive;
using ExwhyzeeSP.Core.Entities.UI;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.MultiTenancy;

namespace ExwhyzeeSP.Data.Contexts
{
    /// <summary>
    /// Dynamic Multi-Tenant Entity Framework Core DbContext for ExwhyzeeSP.Web.
    /// Resolves the dedicated school database connection string per HTTP request.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly ITenantService? _tenantService;

        public ApplicationDbContext()
        {
        }

        public static ApplicationDbContext Create() => new ApplicationDbContext();

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ITenantService? tenantService = null)
            : base(options)
        {
            _tenantService = tenantService;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connStr = _tenantService?.GetActiveConnectionString()
                    ?? Environment.GetEnvironmentVariable("DEFAULT_DB_CONNECTION")
                    ?? Environment.GetEnvironmentVariable("MASTER_DB_CONNECTION");

                if (!string.IsNullOrWhiteSpace(connStr))
                {
                    optionsBuilder.UseSqlServer(connStr, sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorNumbersToAdd: null);
                    });
                }
            }

            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));

            base.OnConfiguring(optionsBuilder);
        }

        // --- Core School Entity DbSets ---
        public DbSet<ClassLevel> ClassLevels { get; set; } = null!;
        public DbSet<EnrolledSubject> EnrolledSubjects { get; set; } = null!;
        public DbSet<Enrollment> Enrollments { get; set; } = null!;
        public DbSet<Session> Sessions { get; set; } = null!;
        public DbSet<Subject> Subjects { get; set; } = null!;
        public DbSet<PinCodeModel> PinCodeModels { get; set; } = null!;
        public DbSet<Property> Properties { get; set; } = null!;
        public DbSet<Setting> Settings { get; set; } = null!;
        public DbSet<Qualification> Qualifications { get; set; } = null!;
        public DbSet<States> States { get; set; } = null!;
        public DbSet<LocalGovs> LocalGovs { get; set; } = null!;
        public DbSet<StudentProfile> StudentProfiles { get; set; } = null!;
        public DbSet<StaffProfile> StaffProfiles { get; set; } = null!;
        public DbSet<PublishResult> PublishResults { get; set; } = null!;
        public DbSet<ImageModel> ImageModel { get; set; } = null!;
        public DbSet<Defaulter> Defaulters { get; set; } = null!;
        public DbSet<SmsReport> SmsReports { get; set; } = null!;
        public DbSet<SmsGroup> SmsGroups { get; set; } = null!;
        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<ContactUs> ContactUs { get; set; } = null!;
        public DbSet<MessageReply> MessageReply { get; set; } = null!;
        public DbSet<PostImage> PostImages { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<HallOfFame> HallOfFames { get; set; } = null!;
        public DbSet<ImageSlider> ImageSlider { get; set; } = null!;
        public DbSet<StudentData> StudentDatas { get; set; } = null!;
        public DbSet<Grading> Gradings { get; set; } = null!;
        public DbSet<GradingDetails> GradingDetails { get; set; } = null!;
        public DbSet<Syllable> syllables { get; set; } = null!;
        public DbSet<Attendance> Attendances { get; set; } = null!;
        public DbSet<Assignment> Assignments { get; set; } = null!;
        public DbSet<Quote> Quotes { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<Riddles> Riddles { get; set; } = null!;
        public DbSet<TimeTable> TimeTables { get; set; } = null!;
        public DbSet<AssignmentAnswer> AssignmentAnswers { get; set; } = null!;
        public DbSet<AttendanceDetail> AttendanceDetails { get; set; } = null!;
        public DbSet<BatchResult> BatchResults { get; set; } = null!;
        public DbSet<RecognitiveDomain> RecognitiveDomains { get; set; } = null!;
        public DbSet<PsychomotorDomain> PsychomotorDomains { get; set; } = null!;
        public DbSet<AffectiveDomain> AffectiveDomains { get; set; } = null!;
        public DbSet<SchoolFees> SchoolFees { get; set; } = null!;
        public DbSet<NewsLetter> NewsLetters { get; set; } = null!;
        public DbSet<SchoolAccount> SchoolAccounts { get; set; } = null!;
        public DbSet<WebsiteSettings> WebsiteSettings { get; set; } = null!;
        public DbSet<ImageGallery> ImageGallery { get; set; } = null!;
        public DbSet<ContentPage> ContentPages { get; set; } = null!;
        public DbSet<CategoryPage> CategoryPages { get; set; } = null!;
        public DbSet<ContentImage> ContentImages { get; set; } = null!;
        public DbSet<Help> Helps { get; set; } = null!;
        public DbSet<Ticket> Tickets { get; set; } = null!;
        public DbSet<Response> Responses { get; set; } = null!;
        public DbSet<WebsiteColor> WebsiteColors { get; set; } = null!;
        public DbSet<SettingsValue> SettingsValues { get; set; } = null!;
        public DbSet<Finance> Finances { get; set; } = null!;
        public DbSet<BankDetails> BankDetails { get; set; } = null!;
        public DbSet<Income> Incomes { get; set; } = null!;
        public DbSet<Expenditure> Expenditures { get; set; } = null!;
        public DbSet<ApprovedDevice> ApprovedDevices { get; set; } = null!;

        // --- Hostel Management ---
        public DbSet<Hostel> Hostels { get; set; } = null!;
        public DbSet<HostelAllotment> HostelAllotments { get; set; } = null!;
        public DbSet<HostelBed> HostelBeds { get; set; } = null!;
        public DbSet<HostelRoom> HostelRooms { get; set; } = null!;
        public DbSet<EnrolledHostel> EnrolledHostels { get; set; } = null!;
        public DbSet<EnrolledHostelBed> EnrolledHostelBeds { get; set; } = null!;
        public DbSet<EnrolledHostelRoom> EnrolledHostelRooms { get; set; } = null!;

        public DbSet<Documentary> Documentaries { get; set; } = null!;

        // --- Result Archives ---
        public DbSet<AffectiveDomainArchive> AffectiveDomainArchive { get; set; } = null!;
        public DbSet<RecognitiveDomainArchive> RecognitiveDomainArchive { get; set; } = null!;
        public DbSet<PsychomotorDomainArchive> PsychomotorDomainArchive { get; set; } = null!;
        public DbSet<NewsLetterArchive> NewsLetterArchive { get; set; } = null!;
        public DbSet<EnrolledSubjectArchive> EnrolledSubjectArchive { get; set; } = null!;
        public DbSet<SchoolFeesArchive> SchoolFeesArchive { get; set; } = null!;
        public DbSet<PrincipalArchive> PrincipalArchives { get; set; } = null!;
        public DbSet<ArchiveResult> ArchiveResults { get; set; } = null!;

        // --- Online Classes & Finance Initializers ---
        public DbSet<LiveClassOnline> LiveClassOnlines { get; set; } = null!;
        public DbSet<DataUserRequest> DataUserRequests { get; set; } = null!;
        public DbSet<OnlineCourseUpload> OnlineCourseUpload { get; set; } = null!;
        public DbSet<OnlineZoom> OnlineZooms { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<Tracker> Trackers { get; set; } = null!;
        public DbSet<FinanceInitializer> FinanceInitializers { get; set; } = null!;
        public DbSet<PaymentData> PaymentDatas { get; set; } = null!;
        public DbSet<LessonNote> LessonNotes { get; set; } = null!;

        // --- UI & Website Content ---
        public DbSet<SiteOverrideCSS> SiteOverrideCSSs { get; set; } = null!;
        public DbSet<SiteBreadCrumb> SiteBreadCrumbs { get; set; } = null!;
        public DbSet<SiteContact> SiteContacts { get; set; } = null!;
        public DbSet<SiteFooter> SiteFooters { get; set; } = null!;
        public DbSet<SiteFooterJS> SiteFooterJSs { get; set; } = null!;
        public DbSet<SiteGallery> SiteGalleries { get; set; } = null!;
        public DbSet<SiteGalleryList> SiteGalleryLists { get; set; } = null!;
        public DbSet<SiteHeader> SiteHeaders { get; set; } = null!;
        public DbSet<SiteHomeBody> SiteHomeBodys { get; set; } = null!;
        public DbSet<SitePageCategory> SitePageCategories { get; set; } = null!;
        public DbSet<SitePage> SitePages { get; set; } = null!;
        public DbSet<SiteSetting> SiteSettings { get; set; } = null!;
        public DbSet<SiteSlider> SiteSliders { get; set; } = null!;
        public DbSet<SiteNavHeader> SiteNavHeaders { get; set; } = null!;
        public DbSet<SiteNewsPost> SiteNewsPosts { get; set; } = null!;
        public DbSet<SiteNewsPostList> SiteNewsPostList { get; set; } = null!;
        public DbSet<SiteHomeBodyAfterNew> SiteHomeBodyAfterNews { get; set; } = null!;
        public DbSet<SiteSinglePost> SiteSinglePosts { get; set; } = null!;
        public DbSet<SuperSetting> SuperSettings { get; set; } = null!;
        public DbSet<Slider> Sliders { get; set; } = null!;
        public DbSet<WebPage> WebPages { get; set; } = null!;
        public DbSet<FAQ> FAQs { get; set; } = null!;
        public DbSet<Blog> Blogs { get; set; } = null!;
        public DbSet<BlogCategory> BlogCategories { get; set; } = null!;
        public DbSet<PageSection> PageSections { get; set; } = null!;
        public DbSet<PageSectionList> PageSectionLists { get; set; } = null!;
        public DbSet<Testimony> Testimonies { get; set; } = null!;
        public DbSet<PostModal> PostModal { get; set; } = null!;
        public DbSet<DataConfig> DataConfigs { get; set; } = null!;
        public DbSet<PageCategory> PageCategories { get; set; } = null!;
        public DbSet<TemplateType> TemplateTypes { get; set; } = null!;
        public DbSet<TemplateCategory> TemplateCategories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ---------------------------------------------------------
            // Legacy Identity 2.0 Database Schema Mappings (Zero-Drift)
            // ---------------------------------------------------------
            builder.Entity<IdentityRole>(b =>
            {
                b.ToTable("AspNetRoles");
                b.Ignore(r => r.NormalizedName);
                b.Ignore(r => r.ConcurrencyStamp);
            });

            builder.Entity<ApplicationUser>(b =>
            {
                b.ToTable("AspNetUsers");
                b.Ignore(u => u.NormalizedUserName);
                b.Ignore(u => u.NormalizedEmail);
                b.Ignore(u => u.ConcurrencyStamp);
                b.Property(u => u.LockoutEnd)
                    .HasColumnName("LockoutEndDateUtc")
                    .HasConversion(
                        v => v.HasValue ? v.Value.UtcDateTime : (DateTime?)null,
                        v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : null);
            });

            builder.Entity<IdentityUserRole<string>>(b =>
            {
                b.ToTable("AspNetUserRoles");
            });

            builder.Entity<IdentityUserClaim<string>>(b =>
            {
                b.ToTable("AspNetUserClaims");
            });

            builder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.ToTable("AspNetUserLogins");
            });

            builder.Ignore<IdentityUserToken<string>>();
            builder.Ignore<IdentityRoleClaim<string>>();

            // ---------------------------------------------------------
            // Legacy EF6 Navigation Shadow Column Names (Zero-Drift)
            // ---------------------------------------------------------
            builder.Entity<ClassLevel>()
                .HasOne(c => c.StaffProfile)
                .WithMany()
                .HasForeignKey("StaffProfile_Id");

            builder.Entity<PaymentData>()
                .HasOne(p => p.ApproveUser)
                .WithMany()
                .HasForeignKey("ApproveUser_Id");

            builder.Entity<AssignmentAnswer>()
                .HasOne(a => a.ClassLevel)
                .WithMany()
                .HasForeignKey("ClassLevel_Id");

            builder.Entity<AssignmentAnswer>()
                .HasOne(a => a.StudentProfile)
                .WithMany()
                .HasForeignKey("StudentProfile_Id");

            builder.Entity<AttendanceDetail>()
                .HasOne(a => a.StudentProfile)
                .WithMany()
                .HasForeignKey("StudentProfile_Id");

            builder.Entity<Defaulter>()
                .HasOne(d => d.StudentProfile)
                .WithMany()
                .HasForeignKey("StudentProfile_Id");

            builder.Entity<Enrollment>(b =>
            {
                b.Property(e => e.UserId).HasColumnName("User_Id");
                b.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId);
            });

            builder.Entity<Enrollment>()
                .HasOne(e => e.RecognitiveDomain)
                .WithMany()
                .HasForeignKey("RecognitiveDomain_Id");

            builder.Entity<MessageReply>()
                .HasOne(m => m.ContactUs)
                .WithMany()
                .HasForeignKey("ContactUs_Id");

            builder.Entity<Qualification>()
                .HasOne(q => q.StaffProfile)
                .WithMany()
                .HasForeignKey("StaffProfile_Id");

            builder.Entity<ContactUs>()
                .HasOne(c => c.user)
                .WithMany()
                .HasForeignKey("user_Id");

            // ---------------------------------------------------------
            // Table Name Parity with Live Database
            // ---------------------------------------------------------
            builder.Entity<ImageModel>().ToTable("ImageModels");
            builder.Entity<MessageReply>().ToTable("MessageReplies");
            builder.Entity<ImageSlider>().ToTable("ImageSliders");
            builder.Entity<Syllable>().ToTable("Syllables");
            builder.Entity<ImageGallery>().ToTable("ImageGalleries");
            builder.Entity<AffectiveDomainArchive>().ToTable("AffectiveDomainArchives");
            builder.Entity<RecognitiveDomainArchive>().ToTable("RecognitiveDomainArchives");
            builder.Entity<PsychomotorDomainArchive>().ToTable("PsychomotorDomainArchives");
            builder.Entity<NewsLetterArchive>().ToTable("NewsLetterArchives");
            builder.Entity<EnrolledSubjectArchive>().ToTable("EnrolledSubjectArchives");
            builder.Entity<SchoolFeesArchive>().ToTable("SchoolFeesArchives");
            builder.Entity<OnlineCourseUpload>().ToTable("OnlineCourseUploads");
            builder.Entity<SiteOverrideCSS>().ToTable("SiteOverrideCSSes");
            builder.Entity<SiteHomeBody>().ToTable("SiteHomeBodies");
            builder.Entity<SiteNewsPostList>().ToTable("SiteNewsPostLists");
            builder.Entity<PostModal>().ToTable("PostModals");
        }
    }
}
