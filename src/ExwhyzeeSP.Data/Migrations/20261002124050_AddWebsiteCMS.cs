using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExwhyzeeSP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWebsiteCMS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AffectiveDomainArchives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Attentiveness = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Honesty = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Neatness = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Punctuality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Relationship = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnrolmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffectiveDomainArchives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AffectiveDomains",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Attentiveness = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Honesty = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Neatness = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Punctuality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Relationship = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnrolmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffectiveDomains", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OtherName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateRegistered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DataStatusChanged = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Religion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocalGov = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateOfOrigin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegisteredBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsLocked = table.Column<bool>(type: "bit", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEndDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BatchResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentRegNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileId = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    EnrollmentId = table.Column<int>(type: "int", nullable: false),
                    AverageScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlogCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Publish = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoryPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuDescription = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShowInHome = table.Column<bool>(type: "bit", nullable: false),
                    ContentHome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Publish = table.Column<int>(type: "int", nullable: false),
                    MetaTage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaKeyword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RedirectUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryPages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FIleName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentImages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataConfigs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoginCSS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LayoutCSS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LayoutJavaScript = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DashboardCSS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MisDashboardCSS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MisJobBoardCSS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ElectionCustomCss = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MisLayoutJavaScript = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RedirectTohttpswww = table.Column<bool>(type: "bit", nullable: false),
                    RedirectTohttps = table.Column<bool>(type: "bit", nullable: false),
                    Configuration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LiveConfiguration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MailSender = table.Column<int>(type: "int", nullable: false),
                    OutlookServer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OutlookPort = table.Column<int>(type: "int", nullable: false),
                    OutlookUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OutlookSecurity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OutlookSenderEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OutlookSSLEnable = table.Column<bool>(type: "bit", nullable: false),
                    OutlookUseDefaultCredentials = table.Column<bool>(type: "bit", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoogleServer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GooglePort = table.Column<int>(type: "int", nullable: false),
                    GoogleUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoogleSecurity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoogleSenderEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoogleSSLEnable = table.Column<bool>(type: "bit", nullable: false),
                    GoogleUseDefaultCredentials = table.Column<bool>(type: "bit", nullable: false),
                    WebmailServer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebmailPort = table.Column<int>(type: "int", nullable: false),
                    WebmailUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebmailSecurity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebmailSenderEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebmailSSLEnable = table.Column<bool>(type: "bit", nullable: false),
                    WebmailUseDefaultCredentials = table.Column<bool>(type: "bit", nullable: false),
                    CCmails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BBmails = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataUserRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentsPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentsPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentsOccupation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormTeacher = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataUserRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Documentaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documentaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DIscription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GeneralEvent = table.Column<bool>(type: "bit", nullable: true),
                    IsFullDay = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Expenditures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenditures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FAQs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Show = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gradings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gradings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HallOfFames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HallOfFames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Helps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HelpUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Helps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hostels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HostelType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hostels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImageGalleries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageByte = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    GalleryAlt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentGallery = table.Column<bool>(type: "bit", nullable: false),
                    Show = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageGalleries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImageModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageContent = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImageSliders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    SliderAlt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TextOne = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TextTwo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TextThree = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentSlider = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageSliders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Incomes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incomes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewsLetterArchives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NextTResumptionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GenRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PTAMeetingDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PTAFee = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionId = table.Column<int>(type: "int", nullable: true),
                    NewsLetterId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsLetterArchives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShowModal = table.Column<bool>(type: "bit", nullable: false),
                    ShowMarque = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PageCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Publish = table.Column<bool>(type: "bit", nullable: false),
                    MenuSortOrder = table.Column<int>(type: "int", nullable: false),
                    HomeSortFrom = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostModals",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModalTime = table.Column<int>(type: "int", nullable: false),
                    ModalOccurance = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StopTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ShowOnlyImage = table.Column<bool>(type: "bit", nullable: false),
                    ShowTitle = table.Column<bool>(type: "bit", nullable: false),
                    ShowDescription = table.Column<bool>(type: "bit", nullable: false),
                    ShowImage = table.Column<bool>(type: "bit", nullable: false),
                    Publish = table.Column<bool>(type: "bit", nullable: false),
                    ButtonLink = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostModals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreviewContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatePosted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    WhoCanSeePost = table.Column<int>(type: "int", nullable: false),
                    PageType = table.Column<int>(type: "int", nullable: false),
                    PostedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: true),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrincipalArchives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrincipalName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionId = table.Column<int>(type: "int", nullable: true),
                    PrincipalId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrincipalArchives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitsPerSms = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UnitsInAccount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SmsUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SmsPassword = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PsychomotorDomainArchives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Drawing = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Club = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Painting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Handwriting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hobbies = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Speech = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sports = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnrolmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsychomotorDomainArchives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PsychomotorDomains",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Drawing = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Club = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Painting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Handwriting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hobbies = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Speech = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sports = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnrolmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsychomotorDomains", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Quotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateDisplayed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QuoteOfTheDay = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecognitiveDomainArchives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rememberance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Understanding = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Application = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Analyzing = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Evaluation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Creativity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnrolmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecognitiveDomainArchives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecognitiveDomains",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rememberance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Understanding = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Application = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Analyzing = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Evaluation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Creativity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnrolmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecognitiveDomains", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchoolAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcctPurpose = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcctName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcctNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchoolFeesArchives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmountDue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnrolmentId = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<int>(type: "int", nullable: true),
                    SchoolFeesId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolFeesArchives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Term = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolPrincipal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SessionYear = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ArchiveStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchoolName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolInitials = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolAddress2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageId = table.Column<int>(type: "int", nullable: true),
                    SmsUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SmsPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Passmark = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PromoteAll = table.Column<bool>(type: "bit", nullable: false),
                    PromotionByTrial = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShowPositionOnResult = table.Column<bool>(type: "bit", nullable: false),
                    ShowFeesOnResult = table.Column<bool>(type: "bit", nullable: false),
                    ShowAccctOnResult = table.Column<bool>(type: "bit", nullable: false),
                    ShowCumulativeResultForThirdTerm = table.Column<bool>(type: "bit", nullable: false),
                    EmailFrom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MailHost = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Port = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MailUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MailPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SslEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessmentScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExamScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TestScore2 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Project = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ClassExercise = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Assessment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScreeningVenue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScreeningTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScreeningDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebsiteLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PortalLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IskoollLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CBTLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPrimaryNursery = table.Column<bool>(type: "bit", nullable: false),
                    EnableBatchResultPrinting = table.Column<bool>(type: "bit", nullable: false),
                    PromotionByMathsAndEng = table.Column<bool>(type: "bit", nullable: false),
                    DefaultEnrollmentRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoogleAnalytics = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrintOutOption = table.Column<int>(type: "int", nullable: false),
                    AdmissionPinOption = table.Column<int>(type: "int", nullable: false),
                    NewsletterContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShowNewsletterPage = table.Column<bool>(type: "bit", nullable: false),
                    PinValidOption = table.Column<int>(type: "int", nullable: false),
                    PaymentCallBackUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZoomHostOne = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZoomHostOnePass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZoomHostTwo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZoomHostTwoPass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZoomHostThree = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZoomHostThreePass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnableZoom = table.Column<bool>(type: "bit", nullable: false),
                    EnableMultipleZoom = table.Column<bool>(type: "bit", nullable: false),
                    EnableCBT = table.Column<bool>(type: "bit", nullable: false),
                    EnablePreviewResult = table.Column<bool>(type: "bit", nullable: false),
                    EnableTestScore = table.Column<bool>(type: "bit", nullable: false),
                    EnableTestScore2 = table.Column<bool>(type: "bit", nullable: false),
                    EnableExamScore = table.Column<bool>(type: "bit", nullable: false),
                    EnableProject = table.Column<bool>(type: "bit", nullable: false),
                    EnableClassExercise = table.Column<bool>(type: "bit", nullable: false),
                    EnableAssessment = table.Column<bool>(type: "bit", nullable: false),
                    EnableHostel = table.Column<bool>(type: "bit", nullable: false),
                    EnableFinance = table.Column<bool>(type: "bit", nullable: false),
                    PaystackSecretKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaystackChargePercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FlutterwaveSecretKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FlutterwaveChargePercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DisableAllResultPrinting = table.Column<bool>(type: "bit", nullable: false),
                    DisableAllResultPrintingNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Qoute = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TopNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoogleMap = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShowAddressOneInTop = table.Column<bool>(type: "bit", nullable: false),
                    AddressOne = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressTwo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FacebookPage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstagramPage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwitterPage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TiktokPage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YoutubeChannel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailOne = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShowEmailOneInTop = table.Column<bool>(type: "bit", nullable: false),
                    ShowEmailOneInFooter = table.Column<bool>(type: "bit", nullable: false),
                    EmailTwo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShowEmailTwoInTop = table.Column<bool>(type: "bit", nullable: false),
                    ShowEmailTwoInFooter = table.Column<bool>(type: "bit", nullable: false),
                    EmailThree = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShowEmailThreeInTop = table.Column<bool>(type: "bit", nullable: false),
                    ShowEmailThreeInFooter = table.Column<bool>(type: "bit", nullable: false),
                    PhoneOne = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShowPhoneOneInTop = table.Column<bool>(type: "bit", nullable: false),
                    ShowPhoneOneInFooter = table.Column<bool>(type: "bit", nullable: false),
                    PhoneTwo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShowPhoneTwoInTop = table.Column<bool>(type: "bit", nullable: false),
                    ShowPhoneTwoInFooter = table.Column<bool>(type: "bit", nullable: false),
                    PhoneThree = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShowPhoneThreeInTop = table.Column<bool>(type: "bit", nullable: false),
                    ShowPhoneThreeInFooter = table.Column<bool>(type: "bit", nullable: false),
                    WorkingHour = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddFaqToHome = table.Column<bool>(type: "bit", nullable: false),
                    AddFaqToFooter = table.Column<bool>(type: "bit", nullable: false),
                    AddTestimonyToHome = table.Column<bool>(type: "bit", nullable: false),
                    AddTestimonyToFooter = table.Column<bool>(type: "bit", nullable: false),
                    AddPartnerToHome = table.Column<bool>(type: "bit", nullable: false),
                    DisableMainTopMenu = table.Column<bool>(type: "bit", nullable: false),
                    ShowContactUsMenu = table.Column<bool>(type: "bit", nullable: false),
                    ShowContactUsFooter = table.Column<bool>(type: "bit", nullable: false),
                    DefaultTitleBackgroundUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultTitleBackgroundKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddBlogToHome = table.Column<bool>(type: "bit", nullable: false),
                    AddBlogToMenu = table.Column<bool>(type: "bit", nullable: false),
                    AddBlogToFooter = table.Column<bool>(type: "bit", nullable: false),
                    BlogDisplayTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddCareerToMenu = table.Column<bool>(type: "bit", nullable: false),
                    AddCareerToFooter = table.Column<bool>(type: "bit", nullable: false),
                    ShowProductsInMenu = table.Column<bool>(type: "bit", nullable: false),
                    ShowProductsInFooter = table.Column<bool>(type: "bit", nullable: false),
                    ActivateProductsInHome = table.Column<bool>(type: "bit", nullable: false),
                    ShowThreeProducts = table.Column<bool>(type: "bit", nullable: false),
                    ShowSixProducts = table.Column<bool>(type: "bit", nullable: false),
                    ShowTwoProducts = table.Column<bool>(type: "bit", nullable: false),
                    CustomMenuTop = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UseCustomMenuTop = table.Column<bool>(type: "bit", nullable: false),
                    UseCategoryBlog = table.Column<bool>(type: "bit", nullable: false),
                    CareerDisplayTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignatureName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignatureTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultAttendanceStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DefaultAttendanceCloseTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EarlyMinute = table.Column<int>(type: "int", nullable: false),
                    OntimeMinute = table.Column<int>(type: "int", nullable: false),
                    LateMinute = table.Column<int>(type: "int", nullable: false),
                    VeryLateMinute = table.Column<int>(type: "int", nullable: false),
                    DefaultAttendanceStartTimeSaturday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReportCloseTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    JobCardNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobCardTitleBackgroundUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobCardTitleBackgroundKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignatureUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignatureKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnableBreakingNewsRibon = table.Column<bool>(type: "bit", nullable: false),
                    BreakingNewsRibonTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RibonPosition = table.Column<int>(type: "int", nullable: false),
                    RemitaTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RemitaInstruction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RemitaButton = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SettingsValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ValueData = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingsValues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteBreadCrumbs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Show = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteBreadCrumbs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteContacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Show = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteContacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteFooterJSs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Show = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteFooterJSs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteFooters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Show = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteFooters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteGalleries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UpperSection = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LowerSection = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Show = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteGalleries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteGalleryLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Show = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteGalleryLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Show = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteHeaders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteHomeBodies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Show = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteHomeBodies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteHomeBodyAfterNews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Show = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteHomeBodyAfterNews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteNavHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Show = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteNavHeaders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteNewsPostLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Show = table.Column<bool>(type: "bit", nullable: false),
                    link = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteNewsPostLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteNewsPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UpperSection = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LowerSection = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteNewsPosts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteOverrideCSSes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Show = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteOverrideCSSes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SitePageCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Show = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SitePageCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Show = table.Column<bool>(type: "bit", nullable: false),
                    EmailOne = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailTwo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailThree = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneOne = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneTwo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneThree = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressOne = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressTwo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressThree = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Host = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PX = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Receiver = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteSinglePosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Show = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSinglePosts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteSliders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Show = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSliders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sliders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YoutubeVideo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsVideo = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Show = table.Column<bool>(type: "bit", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MiniTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ButtonText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ButtonLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisableVideoButton = table.Column<bool>(type: "bit", nullable: false),
                    VideoLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VideoButtonText = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sliders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SmsGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Numbers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumbersCount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SmsReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SendTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderId = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateSent = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StateName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OtherNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastPrimarySchoolAttended = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Religion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameOfParents = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentsAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumberOfParents = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccupationOfParents = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PermanentHomeAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocalGov = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateOfOrigin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExamCenter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Disability = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmergencyContact = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExamScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdmissionNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DateOfAdmission = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AdmissionOfficer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SuperSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BucketName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserNipssArea = table.Column<bool>(type: "bit", nullable: false),
                    ActivateLogin = table.Column<bool>(type: "bit", nullable: false),
                    ActivateLoginInMenu = table.Column<bool>(type: "bit", nullable: false),
                    MaximumUsers = table.Column<int>(type: "int", nullable: false),
                    TemplateLayoutKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoginTemplateKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ColorTemplateKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SliderTemplateKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductTemplateKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BlogTemplateKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReloaderTemplateKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PageHeaderTemplateKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FooterTemplateKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebsiteTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyNameAbbreviation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyWebsiteLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DashboardTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyLogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyLogoKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UseNewMISDashboard = table.Column<bool>(type: "bit", nullable: false),
                    CompanyWhiteLogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyWhiteLogoKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyIconUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyIconKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReloaderIconUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReloaderIconKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JustWebsite = table.Column<bool>(type: "bit", nullable: false),
                    ActivateOnlyAuthorizedDevice = table.Column<bool>(type: "bit", nullable: false),
                    ActivateSMS = table.Column<bool>(type: "bit", nullable: false),
                    EmailTemplate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SMSTemplate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoginBackgroundImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoginBackgroundImageKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UseNormalLogoInLogin = table.Column<bool>(type: "bit", nullable: false),
                    UseWhiteLogoInLogin = table.Column<bool>(type: "bit", nullable: false),
                    LoginTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoginNoteTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoginNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoginNoteFooter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActivateComingSoon = table.Column<bool>(type: "bit", nullable: false),
                    ProductTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductTitleHome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShowMadeByJuray = table.Column<bool>(type: "bit", nullable: false),
                    ShowMadeByXYZ = table.Column<bool>(type: "bit", nullable: false),
                    ActivateProfilePortfolio = table.Column<bool>(type: "bit", nullable: false),
                    VerifyTokenFolio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PortfolioImageOneUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PortfolioImageOneKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PortfolioImageTwoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PortfolioImageTwoKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PortfolioBreacrumImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PortfolioBreacrumImageKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PortfolioTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PortfolioMiniTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PortfolioDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShowInMenuPortfolio = table.Column<bool>(type: "bit", nullable: false),
                    ShowInFooterPortfolio = table.Column<bool>(type: "bit", nullable: false),
                    SpecialMenuButtonText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecialMenuButtonLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecialMenuButtonActivate = table.Column<bool>(type: "bit", nullable: false),
                    ActivateWorkWithUsFooter = table.Column<bool>(type: "bit", nullable: false),
                    TitleWorkWithUsFooter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActivateDownloadOurAppFooter = table.Column<bool>(type: "bit", nullable: false),
                    ActivateSubscribeFormOnFooter = table.Column<bool>(type: "bit", nullable: false),
                    TitleDownloadOurAppFooter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionDownloadOurAppFooter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayStoreImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayStoreImageKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayStoreLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppStoreImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppStoreImageKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppStoreLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EncryptionNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EncryptionUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EncryptionKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaystackPublicKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaystackSecretKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaystackCallbackUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentModuleActive = table.Column<bool>(type: "bit", nullable: false),
                    CurrencySymbol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationPaymentNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuperSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TemplateCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Layout = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoginLayout = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TemplateName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TemplateColorPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UniqueKey = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Testimonies",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Show = table.Column<bool>(type: "bit", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Testimonies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Complain = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Closed = table.Column<bool>(type: "bit", nullable: false),
                    TicketNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    browser = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebsiteColors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebsiteSettingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebsiteColors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebsiteSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PortalUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebsiteUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebsiteLogo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    WebsiteIcon = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Layout = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Slider = table.Column<int>(type: "int", nullable: false),
                    AboutUs = table.Column<int>(type: "int", nullable: false),
                    EventAndNews = table.Column<int>(type: "int", nullable: false),
                    ContactUs = table.Column<int>(type: "int", nullable: false),
                    ShowBlogInHome = table.Column<bool>(type: "bit", nullable: false),
                    ShowHallOfFameInHome = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebsiteSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApprovedDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MacAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImelNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovedDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovedDevices_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactUs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    user_Id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    messageStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactUs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactUs_AspNetUsers_user_Id",
                        column: x => x.user_Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Riddles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorrectAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateAnswered = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TimeCountDown = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Riddles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Riddles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StaffProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Disability = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmergencyContact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaritalStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AboutMe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FavouriteFood = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BooksYouLike = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoviesYouLike = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TVShowsYouLike = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YourLikes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YourDisLikes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FavouriteColour = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffRegistrationId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfAppointment = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostHeld = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StudentProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Disability = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmergencyContact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentRegNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AboutMe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FavouriteFood = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BooksYouLike = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoviesYouLike = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TVShowsYouLike = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YourLikes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YourDisLikes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FavouriteColour = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastPrimarySchoolAttended = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentGuardianName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentGuardianAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentGuardianPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentGuardianOccupation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageId = table.Column<int>(type: "int", nullable: false),
                    IsUpdated = table.Column<bool>(type: "bit", nullable: false),
                    SecurityQuestion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Graduate = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Trackers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trackers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trackers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Blogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VideoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VideoKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Publish = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BlogCategoryId = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddToRibon = table.Column<bool>(type: "bit", nullable: false),
                    RibonCustomDisplayTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RibonSortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blogs_BlogCategories_BlogCategoryId",
                        column: x => x.BlogCategoryId,
                        principalTable: "BlogCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ContentPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    ShowInHome = table.Column<bool>(type: "bit", nullable: false),
                    ContentHome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryPageId = table.Column<int>(type: "int", nullable: false),
                    Publish = table.Column<int>(type: "int", nullable: false),
                    MetaTage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaKeyword = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentPages_CategoryPages_CategoryPageId",
                        column: x => x.CategoryPageId,
                        principalTable: "CategoryPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GradingDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GradingId = table.Column<int>(type: "int", nullable: false),
                    UpperLimit = table.Column<int>(type: "int", nullable: false),
                    LowerLimit = table.Column<int>(type: "int", nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradingDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GradingDetails_Gradings_GradingId",
                        column: x => x.GradingId,
                        principalTable: "Gradings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HostelRooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoomNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoOfStudent = table.Column<int>(type: "int", nullable: true),
                    HostelId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostelRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostelRooms_Hostels_HostelId",
                        column: x => x.HostelId,
                        principalTable: "Hostels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WebPages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Publish = table.Column<bool>(type: "bit", nullable: false),
                    SecurityPage = table.Column<bool>(type: "bit", nullable: false),
                    PageCategoryId = table.Column<long>(type: "bigint", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShowInMainTop = table.Column<bool>(type: "bit", nullable: false),
                    ShowInMenuDropDown = table.Column<bool>(type: "bit", nullable: false),
                    ShowInMainMenu = table.Column<bool>(type: "bit", nullable: false),
                    ShowInFooter = table.Column<bool>(type: "bit", nullable: false),
                    EnableDirectUrl = table.Column<bool>(type: "bit", nullable: false),
                    DirectUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeSortFrom = table.Column<int>(type: "int", nullable: false),
                    MainPageId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebPages_PageCategories_PageCategoryId",
                        column: x => x.PageCategoryId,
                        principalTable: "PageCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCommented = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageByte = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageContent = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PostId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostImages_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EnrolledHostels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HostelId = table.Column<int>(type: "int", nullable: true),
                    HostelType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrolledHostels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnrolledHostels_Hostels_HostelId",
                        column: x => x.HostelId,
                        principalTable: "Hostels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EnrolledHostels_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NewsLetters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NextTResumptionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GenRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PTAMeetingDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PTAFee = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsLetters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsLetters_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PinCodeModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PinNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Usage = table.Column<int>(type: "int", nullable: false),
                    EnrollmentId = table.Column<int>(type: "int", nullable: true),
                    BatchNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentPin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SessionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinCodeModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PinCodeModels_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SchoolFees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmountDue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnrolmentId = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolFees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchoolFees_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Syllables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionId = table.Column<int>(type: "int", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Syllables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Syllables_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SitePages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PageLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Show = table.Column<bool>(type: "bit", nullable: false),
                    MainPage = table.Column<bool>(type: "bit", nullable: false),
                    SubPage = table.Column<bool>(type: "bit", nullable: false),
                    IsNews = table.Column<bool>(type: "bit", nullable: false),
                    PreviewContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FooterPage = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    SitePageCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SitePages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SitePages_SitePageCategories_SitePageCategoryId",
                        column: x => x.SitePageCategoryId,
                        principalTable: "SitePageCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocalGovs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LGAName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatesId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalGovs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocalGovs_States_StatesId",
                        column: x => x.StatesId,
                        principalTable: "States",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TemplateTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrackKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TemplateCategoryId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateTypes_TemplateCategories_TemplateCategoryId",
                        column: x => x.TemplateCategoryId,
                        principalTable: "TemplateCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Responses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepliedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TicketId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Responses_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageReplies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReplyMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    ContactUs_Id = table.Column<int>(type: "int", nullable: true),
                    userId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReplyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ContactUsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageReplies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageReplies_AspNetUsers_userId",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MessageReplies_ContactUs_ContactUsId",
                        column: x => x.ContactUsId,
                        principalTable: "ContactUs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MessageReplies_ContactUs_ContactUs_Id",
                        column: x => x.ContactUs_Id,
                        principalTable: "ContactUs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClassLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    StaffProfile_Id = table.Column<int>(type: "int", nullable: true),
                    SessionId = table.Column<int>(type: "int", nullable: true),
                    Passmark = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PromotionByTrial = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AccessmentScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExamScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TestScore2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Project = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ClassExercise = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Assessment = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ShowPositionOnClassResult = table.Column<bool>(type: "bit", nullable: false),
                    SchoolFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SortByOrder = table.Column<bool>(type: "bit", nullable: false),
                    SubjectSettings = table.Column<bool>(type: "bit", nullable: false),
                    ShowClass = table.Column<bool>(type: "bit", nullable: false),
                    ShowAverageOverPositionInClass = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassLevels_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClassLevels_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClassLevels_StaffProfiles_StaffProfile_Id",
                        column: x => x.StaffProfile_Id,
                        principalTable: "StaffProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Qualifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameOfInstitution = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YearObtained = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CertificateObtained = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StaffId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffProfile_Id = table.Column<int>(type: "int", nullable: true),
                    StaffProfileId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Qualifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Qualifications_StaffProfiles_StaffProfileId",
                        column: x => x.StaffProfileId,
                        principalTable: "StaffProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Qualifications_StaffProfiles_StaffProfile_Id",
                        column: x => x.StaffProfile_Id,
                        principalTable: "StaffProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Defaulters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileId = table.Column<int>(type: "int", nullable: false),
                    StudentProfile_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Defaulters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Defaulters_StudentProfiles_StudentProfile_Id",
                        column: x => x.StudentProfile_Id,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PaymentDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentProfileId = table.Column<int>(type: "int", nullable: true),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FinanceSource = table.Column<int>(type: "int", nullable: false),
                    UniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApproveId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApproveUser_Id = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentDatas_AspNetUsers_ApproveUser_Id",
                        column: x => x.ApproveUser_Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaymentDatas_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentDatas_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HostelBeds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BedNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HostelRoomId = table.Column<int>(type: "int", nullable: true),
                    HostelId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostelBeds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostelBeds_HostelRooms_HostelRoomId",
                        column: x => x.HostelRoomId,
                        principalTable: "HostelRooms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HostelBeds_Hostels_HostelId",
                        column: x => x.HostelId,
                        principalTable: "Hostels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PageSections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VideoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VideoKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondImageKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YoutubeUrlLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MiniTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Qoute = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ButtonText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ButtonLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DirectUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShowInHome = table.Column<bool>(type: "bit", nullable: false),
                    DisableButton = table.Column<bool>(type: "bit", nullable: false),
                    Disable = table.Column<bool>(type: "bit", nullable: false),
                    FixedAfterFooter = table.Column<bool>(type: "bit", nullable: false),
                    HomePageSortOrder = table.Column<int>(type: "int", nullable: false),
                    HomeSortFrom = table.Column<int>(type: "int", nullable: false),
                    PageSortOrder = table.Column<int>(type: "int", nullable: false),
                    BlogSectionPosition = table.Column<int>(type: "int", nullable: false),
                    ShowSectionInBlog = table.Column<bool>(type: "bit", nullable: false),
                    WebPageId = table.Column<long>(type: "bigint", nullable: true),
                    TemplateKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddToRibon = table.Column<bool>(type: "bit", nullable: false),
                    RibonCustomDisplayTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RibonSortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageSections_WebPages_WebPageId",
                        column: x => x.WebPageId,
                        principalTable: "WebPages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EnrolledHostelRooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HostelRoomId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoomNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoOfStudent = table.Column<int>(type: "int", nullable: true),
                    EnrolledHostelId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrolledHostelRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnrolledHostelRooms_EnrolledHostels_EnrolledHostelId",
                        column: x => x.EnrolledHostelId,
                        principalTable: "EnrolledHostels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EnrolledHostelRooms_HostelRooms_HostelRoomId",
                        column: x => x.HostelRoomId,
                        principalTable: "HostelRooms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ArchiveResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<int>(type: "int", nullable: true),
                    ClassLevelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchiveResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArchiveResults_ClassLevels_ClassLevelId",
                        column: x => x.ClassLevelId,
                        principalTable: "ClassLevels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ArchiveResults_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassLevelId = table.Column<int>(type: "int", nullable: true),
                    StudentProfileId = table.Column<int>(type: "int", nullable: false),
                    User_Id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    AverageScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ArchiveAverageScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CummulativeAverageScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EnrollmentRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnrollmentRemark1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnrollmentRemark2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextResumption = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnableLiveClass = table.Column<bool>(type: "bit", nullable: false),
                    Printed = table.Column<bool>(type: "bit", nullable: false),
                    RecognitiveDomain_Id = table.Column<int>(type: "int", nullable: true),
                    AmountRequiredToPay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enrollments_AspNetUsers_User_Id",
                        column: x => x.User_Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Enrollments_ClassLevels_ClassLevelId",
                        column: x => x.ClassLevelId,
                        principalTable: "ClassLevels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Enrollments_RecognitiveDomains_RecognitiveDomain_Id",
                        column: x => x.RecognitiveDomain_Id,
                        principalTable: "RecognitiveDomains",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Enrollments_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Enrollments_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PublishResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    ClassLevelId = table.Column<int>(type: "int", nullable: false),
                    PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PublishResults_ClassLevels_ClassLevelId",
                        column: x => x.ClassLevelId,
                        principalTable: "ClassLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublishResults_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClassLevelId = table.Column<int>(type: "int", nullable: false),
                    ExamScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TestScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TestScore2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Project = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ClassExercise = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Assessment = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PassMark = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RequiresPass = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ShowSubject = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subjects_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Subjects_ClassLevels_ClassLevelId",
                        column: x => x.ClassLevelId,
                        principalTable: "ClassLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeTables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassLevelId = table.Column<int>(type: "int", nullable: true),
                    Monday = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    M6_7 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    M7_8 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    M8_9 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    M9_10 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    M10_11 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    M11_12 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    M12_13 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    M13_14 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    M14_15 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    M15_16 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    M16_17 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    M17_18 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tuesday = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    T6_7 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    T7_8 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    T8_9 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    T9_10 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    T10_11 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    T11_12 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    T12_13 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    T13_14 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    T14_15 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    T15_16 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    T16_17 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    T17_18 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Wednessday = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    W6_7 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    W7_8 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    W8_9 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    W9_10 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    W10_11 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    W11_12 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    W12_13 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    W13_14 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    W14_15 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    W15_16 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    W16_17 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    W17_18 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Thursday = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Th6_7 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Th7_8 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Th8_9 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Th9_10 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Th10_11 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Th11_12 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Th12_13 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Th13_14 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Th14_15 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Th15_16 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Th16_17 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Th17_18 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Friday = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    F6_7 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    F7_8 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    F8_9 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    F9_10 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    F10_11 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    F11_12 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    F12_13 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    F13_14 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    F14_15 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    F15_16 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    F16_17 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    F17_18 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time6_7 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time7_8 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time8_9 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time9_10 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time10_11 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time11_12 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time12_13 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time13_14 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time14_15 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time15_16 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time16_17 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time17_18 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeTables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeTables_ClassLevels_ClassLevelId",
                        column: x => x.ClassLevelId,
                        principalTable: "ClassLevels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PageSectionLists",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VideoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VideoKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IconText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MiniTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoreDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ButtonText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ButtonLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DirectUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Disable = table.Column<bool>(type: "bit", nullable: false),
                    PageSectionId = table.Column<long>(type: "bigint", nullable: true),
                    AddToRibon = table.Column<bool>(type: "bit", nullable: false),
                    RibonCustomDisplayTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RibonSortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageSectionLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageSectionLists_PageSections_PageSectionId",
                        column: x => x.PageSectionId,
                        principalTable: "PageSections",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EnrolledHostelBeds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HostelBedId = table.Column<int>(type: "int", nullable: true),
                    BedNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnrolledHostelRoomId = table.Column<int>(type: "int", nullable: true),
                    EnrolledHostelId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrolledHostelBeds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnrolledHostelBeds_EnrolledHostelRooms_EnrolledHostelRoomId",
                        column: x => x.EnrolledHostelRoomId,
                        principalTable: "EnrolledHostelRooms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EnrolledHostelBeds_EnrolledHostels_EnrolledHostelId",
                        column: x => x.EnrolledHostelId,
                        principalTable: "EnrolledHostels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EnrolledHostelBeds_HostelBeds_HostelBedId",
                        column: x => x.HostelBedId,
                        principalTable: "HostelBeds",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ClassLevelId = table.Column<int>(type: "int", nullable: true),
                    EnrollmentId = table.Column<int>(type: "int", nullable: true),
                    SessionId = table.Column<int>(type: "int", nullable: true),
                    Updated = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendances_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Attendances_ClassLevels_ClassLevelId",
                        column: x => x.ClassLevelId,
                        principalTable: "ClassLevels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Attendances_Enrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Attendances_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FinanceInitializers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnrollmentId = table.Column<int>(type: "int", nullable: false),
                    IncomeId = table.Column<int>(type: "int", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentAmountId = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Percent = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionStatus = table.Column<int>(type: "int", nullable: false),
                    ReferenceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinanceSource = table.Column<int>(type: "int", nullable: false),
                    FinanceType = table.Column<int>(type: "int", nullable: false),
                    PaymentTypeId = table.Column<int>(type: "int", nullable: false),
                    Payall = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceInitializers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinanceInitializers_Enrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FinanceInitializers_Incomes_IncomeId",
                        column: x => x.IncomeId,
                        principalTable: "Incomes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Finances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdminNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinanceType = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TellerNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UniqueIdCheck = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinanceSource = table.Column<int>(type: "int", nullable: false),
                    TransactionStatus = table.Column<int>(type: "int", nullable: false),
                    AllocationStatus = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnrollmentId = table.Column<int>(type: "int", nullable: true),
                    SessionId = table.Column<int>(type: "int", nullable: true),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PaymentTypeId = table.Column<int>(type: "int", nullable: false),
                    IncomeId = table.Column<int>(type: "int", nullable: false),
                    Payall = table.Column<bool>(type: "bit", nullable: false),
                    Skip = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Finances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Finances_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Finances_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Finances_Enrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Finances_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassLevelId = table.Column<int>(type: "int", nullable: true),
                    SessionId = table.Column<int>(type: "int", nullable: true),
                    SubjectId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateSubmitionEnds = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignments_ClassLevels_ClassLevelId",
                        column: x => x.ClassLevelId,
                        principalTable: "ClassLevels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assignments_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assignments_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EnrolledSubjectArchives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TestScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ExamScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TestScore2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Project = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ClassExercise = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Assessment = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalCA = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EnrollmentId = table.Column<int>(type: "int", nullable: false),
                    IsOffered = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    EnrollmentSubjectId = table.Column<int>(type: "int", nullable: true),
                    SubjectId = table.Column<int>(type: "int", nullable: true),
                    GradingOption = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrolledSubjectArchives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnrolledSubjectArchives_Enrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnrolledSubjectArchives_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EnrolledSubjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectId = table.Column<int>(type: "int", nullable: true),
                    TestScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ExamScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TestScore2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Project = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ClassExercise = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Assessment = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalCA = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EnrollmentId = table.Column<int>(type: "int", nullable: false),
                    IsOffered = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    GradingOption = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrolledSubjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnrolledSubjects_Enrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnrolledSubjects_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LessonNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectId = table.Column<int>(type: "int", nullable: true),
                    SessionId = table.Column<int>(type: "int", nullable: true),
                    StaffProfileId = table.Column<int>(type: "int", nullable: true),
                    Topic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastEdited = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonNotes_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LessonNotes_StaffProfiles_StaffProfileId",
                        column: x => x.StaffProfileId,
                        principalTable: "StaffProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LessonNotes_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LiveClassOnlines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassLevelId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: true),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UrlLive = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClassDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TeacherName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LiveStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveClassOnlines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LiveClassOnlines_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LiveClassOnlines_ClassLevels_ClassLevelId",
                        column: x => x.ClassLevelId,
                        principalTable: "ClassLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LiveClassOnlines_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LiveClassOnlines_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OnlineCourseUploads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Topic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassLevelId = table.Column<int>(type: "int", nullable: true),
                    SessionId = table.Column<int>(type: "int", nullable: true),
                    SubjectId = table.Column<int>(type: "int", nullable: true),
                    UploadType = table.Column<int>(type: "int", nullable: false),
                    Upload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnlineCourseUploads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnlineCourseUploads_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OnlineCourseUploads_ClassLevels_ClassLevelId",
                        column: x => x.ClassLevelId,
                        principalTable: "ClassLevels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OnlineCourseUploads_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OnlineCourseUploads_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OnlineZooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HostEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MeetingId = table.Column<long>(type: "bigint", nullable: true),
                    MeetingUUId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassLevelId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: true),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeetingType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnlineZooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnlineZooms_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OnlineZooms_ClassLevels_ClassLevelId",
                        column: x => x.ClassLevelId,
                        principalTable: "ClassLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OnlineZooms_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OnlineZooms_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HostelAllotments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    HostelBedId = table.Column<int>(type: "int", nullable: true),
                    HostelRoomId = table.Column<int>(type: "int", nullable: true),
                    HostelId = table.Column<int>(type: "int", nullable: true),
                    SessionId = table.Column<int>(type: "int", nullable: true),
                    AllotedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostelAllotments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostelAllotments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HostelAllotments_EnrolledHostelBeds_HostelBedId",
                        column: x => x.HostelBedId,
                        principalTable: "EnrolledHostelBeds",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HostelAllotments_EnrolledHostelRooms_HostelRoomId",
                        column: x => x.HostelRoomId,
                        principalTable: "EnrolledHostelRooms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HostelAllotments_EnrolledHostels_HostelId",
                        column: x => x.HostelId,
                        principalTable: "EnrolledHostels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HostelAllotments_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AttendanceDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    StudentId = table.Column<int>(type: "int", nullable: true),
                    StudentProfile_Id = table.Column<int>(type: "int", nullable: true),
                    SessionId = table.Column<int>(type: "int", nullable: true),
                    IsPresent = table.Column<bool>(type: "bit", nullable: false),
                    EnrollmentId = table.Column<int>(type: "int", nullable: true),
                    AttendanceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceDetails_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttendanceDetails_Attendances_AttendanceId",
                        column: x => x.AttendanceId,
                        principalTable: "Attendances",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttendanceDetails_Enrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttendanceDetails_StudentProfiles_StudentProfile_Id",
                        column: x => x.StudentProfile_Id,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AssignmentAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignmentId = table.Column<int>(type: "int", nullable: true),
                    DateAnswered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    StudentId = table.Column<int>(type: "int", nullable: true),
                    StudentProfile_Id = table.Column<int>(type: "int", nullable: true),
                    ClassId = table.Column<int>(type: "int", nullable: true),
                    ClassLevel_Id = table.Column<int>(type: "int", nullable: true),
                    EnrollementId = table.Column<int>(type: "int", nullable: true),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Assessed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignmentAnswers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssignmentAnswers_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssignmentAnswers_ClassLevels_ClassLevel_Id",
                        column: x => x.ClassLevel_Id,
                        principalTable: "ClassLevels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssignmentAnswers_Enrollments_EnrollementId",
                        column: x => x.EnrollementId,
                        principalTable: "Enrollments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssignmentAnswers_StudentProfiles_StudentProfile_Id",
                        column: x => x.StudentProfile_Id,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovedDevices_UserId",
                table: "ApprovedDevices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchiveResults_ClassLevelId",
                table: "ArchiveResults",
                column: "ClassLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchiveResults_SessionId",
                table: "ArchiveResults",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentAnswers_AssignmentId",
                table: "AssignmentAnswers",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentAnswers_ClassLevel_Id",
                table: "AssignmentAnswers",
                column: "ClassLevel_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentAnswers_EnrollementId",
                table: "AssignmentAnswers",
                column: "EnrollementId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentAnswers_StudentProfile_Id",
                table: "AssignmentAnswers",
                column: "StudentProfile_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentAnswers_UserId",
                table: "AssignmentAnswers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_ClassLevelId",
                table: "Assignments",
                column: "ClassLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_SessionId",
                table: "Assignments",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_SubjectId",
                table: "Assignments",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceDetails_AttendanceId",
                table: "AttendanceDetails",
                column: "AttendanceId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceDetails_EnrollmentId",
                table: "AttendanceDetails",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceDetails_StudentProfile_Id",
                table: "AttendanceDetails",
                column: "StudentProfile_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceDetails_UserId",
                table: "AttendanceDetails",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_ClassLevelId",
                table: "Attendances",
                column: "ClassLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_EnrollmentId",
                table: "Attendances",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_SessionId",
                table: "Attendances",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_UserId",
                table: "Attendances",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_BlogCategoryId",
                table: "Blogs",
                column: "BlogCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevels_SessionId",
                table: "ClassLevels",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevels_StaffProfile_Id",
                table: "ClassLevels",
                column: "StaffProfile_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevels_UserId",
                table: "ClassLevels",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                table: "Comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactUs_user_Id",
                table: "ContactUs",
                column: "user_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ContentPages_CategoryPageId",
                table: "ContentPages",
                column: "CategoryPageId");

            migrationBuilder.CreateIndex(
                name: "IX_Defaulters_StudentProfile_Id",
                table: "Defaulters",
                column: "StudentProfile_Id");

            migrationBuilder.CreateIndex(
                name: "IX_EnrolledHostelBeds_EnrolledHostelId",
                table: "EnrolledHostelBeds",
                column: "EnrolledHostelId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrolledHostelBeds_EnrolledHostelRoomId",
                table: "EnrolledHostelBeds",
                column: "EnrolledHostelRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrolledHostelBeds_HostelBedId",
                table: "EnrolledHostelBeds",
                column: "HostelBedId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrolledHostelRooms_EnrolledHostelId",
                table: "EnrolledHostelRooms",
                column: "EnrolledHostelId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrolledHostelRooms_HostelRoomId",
                table: "EnrolledHostelRooms",
                column: "HostelRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrolledHostels_HostelId",
                table: "EnrolledHostels",
                column: "HostelId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrolledHostels_SessionId",
                table: "EnrolledHostels",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrolledSubjectArchives_EnrollmentId",
                table: "EnrolledSubjectArchives",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrolledSubjectArchives_SubjectId",
                table: "EnrolledSubjectArchives",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrolledSubjects_EnrollmentId",
                table: "EnrolledSubjects",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrolledSubjects_SubjectId",
                table: "EnrolledSubjects",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_ClassLevelId",
                table: "Enrollments",
                column: "ClassLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_RecognitiveDomain_Id",
                table: "Enrollments",
                column: "RecognitiveDomain_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_SessionId",
                table: "Enrollments",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentProfileId",
                table: "Enrollments",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_User_Id",
                table: "Enrollments",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_FinanceInitializers_EnrollmentId",
                table: "FinanceInitializers",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FinanceInitializers_IncomeId",
                table: "FinanceInitializers",
                column: "IncomeId");

            migrationBuilder.CreateIndex(
                name: "IX_Finances_ApprovedById",
                table: "Finances",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Finances_EnrollmentId",
                table: "Finances",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Finances_SessionId",
                table: "Finances",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Finances_UserId",
                table: "Finances",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GradingDetails_GradingId",
                table: "GradingDetails",
                column: "GradingId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelAllotments_HostelBedId",
                table: "HostelAllotments",
                column: "HostelBedId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelAllotments_HostelId",
                table: "HostelAllotments",
                column: "HostelId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelAllotments_HostelRoomId",
                table: "HostelAllotments",
                column: "HostelRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelAllotments_SessionId",
                table: "HostelAllotments",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelAllotments_UserId",
                table: "HostelAllotments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelBeds_HostelId",
                table: "HostelBeds",
                column: "HostelId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelBeds_HostelRoomId",
                table: "HostelBeds",
                column: "HostelRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_HostelRooms_HostelId",
                table: "HostelRooms",
                column: "HostelId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonNotes_SessionId",
                table: "LessonNotes",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonNotes_StaffProfileId",
                table: "LessonNotes",
                column: "StaffProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonNotes_SubjectId",
                table: "LessonNotes",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_LiveClassOnlines_ClassLevelId",
                table: "LiveClassOnlines",
                column: "ClassLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_LiveClassOnlines_SessionId",
                table: "LiveClassOnlines",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_LiveClassOnlines_SubjectId",
                table: "LiveClassOnlines",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_LiveClassOnlines_UserId",
                table: "LiveClassOnlines",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalGovs_StatesId",
                table: "LocalGovs",
                column: "StatesId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReplies_ContactUs_Id",
                table: "MessageReplies",
                column: "ContactUs_Id");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReplies_ContactUsId",
                table: "MessageReplies",
                column: "ContactUsId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReplies_userId",
                table: "MessageReplies",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsLetters_SessionId",
                table: "NewsLetters",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_OnlineCourseUploads_ClassLevelId",
                table: "OnlineCourseUploads",
                column: "ClassLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_OnlineCourseUploads_SessionId",
                table: "OnlineCourseUploads",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_OnlineCourseUploads_SubjectId",
                table: "OnlineCourseUploads",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_OnlineCourseUploads_UserId",
                table: "OnlineCourseUploads",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OnlineZooms_ClassLevelId",
                table: "OnlineZooms",
                column: "ClassLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_OnlineZooms_SessionId",
                table: "OnlineZooms",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_OnlineZooms_SubjectId",
                table: "OnlineZooms",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_OnlineZooms_UserId",
                table: "OnlineZooms",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PageSectionLists_PageSectionId",
                table: "PageSectionLists",
                column: "PageSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_PageSections_WebPageId",
                table: "PageSections",
                column: "WebPageId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDatas_ApproveUser_Id",
                table: "PaymentDatas",
                column: "ApproveUser_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDatas_SessionId",
                table: "PaymentDatas",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDatas_StudentProfileId",
                table: "PaymentDatas",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PinCodeModels_SessionId",
                table: "PinCodeModels",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_PostImages_PostId",
                table: "PostImages",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishResults_ClassLevelId",
                table: "PublishResults",
                column: "ClassLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishResults_SessionId",
                table: "PublishResults",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Qualifications_StaffProfile_Id",
                table: "Qualifications",
                column: "StaffProfile_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Qualifications_StaffProfileId",
                table: "Qualifications",
                column: "StaffProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Responses_TicketId",
                table: "Responses",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Riddles_UserId",
                table: "Riddles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolFees_SessionId",
                table: "SchoolFees",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_SitePages_SitePageCategoryId",
                table: "SitePages",
                column: "SitePageCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffProfiles_UserId",
                table: "StaffProfiles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentProfiles_UserId",
                table: "StudentProfiles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_ClassLevelId",
                table: "Subjects",
                column: "ClassLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_UserId",
                table: "Subjects",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Syllables_SessionId",
                table: "Syllables",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateTypes_TemplateCategoryId",
                table: "TemplateTypes",
                column: "TemplateCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeTables_ClassLevelId",
                table: "TimeTables",
                column: "ClassLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Trackers_UserId",
                table: "Trackers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WebPages_PageCategoryId",
                table: "WebPages",
                column: "PageCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AffectiveDomainArchives");

            migrationBuilder.DropTable(
                name: "AffectiveDomains");

            migrationBuilder.DropTable(
                name: "ApprovedDevices");

            migrationBuilder.DropTable(
                name: "ArchiveResults");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AssignmentAnswers");

            migrationBuilder.DropTable(
                name: "AttendanceDetails");

            migrationBuilder.DropTable(
                name: "BankDetails");

            migrationBuilder.DropTable(
                name: "BatchResults");

            migrationBuilder.DropTable(
                name: "Blogs");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "ContentImages");

            migrationBuilder.DropTable(
                name: "ContentPages");

            migrationBuilder.DropTable(
                name: "DataConfigs");

            migrationBuilder.DropTable(
                name: "DataUserRequests");

            migrationBuilder.DropTable(
                name: "Defaulters");

            migrationBuilder.DropTable(
                name: "Documentaries");

            migrationBuilder.DropTable(
                name: "EnrolledSubjectArchives");

            migrationBuilder.DropTable(
                name: "EnrolledSubjects");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Expenditures");

            migrationBuilder.DropTable(
                name: "FAQs");

            migrationBuilder.DropTable(
                name: "FinanceInitializers");

            migrationBuilder.DropTable(
                name: "Finances");

            migrationBuilder.DropTable(
                name: "GradingDetails");

            migrationBuilder.DropTable(
                name: "HallOfFames");

            migrationBuilder.DropTable(
                name: "Helps");

            migrationBuilder.DropTable(
                name: "HostelAllotments");

            migrationBuilder.DropTable(
                name: "ImageGalleries");

            migrationBuilder.DropTable(
                name: "ImageModels");

            migrationBuilder.DropTable(
                name: "ImageSliders");

            migrationBuilder.DropTable(
                name: "LessonNotes");

            migrationBuilder.DropTable(
                name: "LiveClassOnlines");

            migrationBuilder.DropTable(
                name: "LocalGovs");

            migrationBuilder.DropTable(
                name: "MessageReplies");

            migrationBuilder.DropTable(
                name: "NewsLetterArchives");

            migrationBuilder.DropTable(
                name: "NewsLetters");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "OnlineCourseUploads");

            migrationBuilder.DropTable(
                name: "OnlineZooms");

            migrationBuilder.DropTable(
                name: "PageSectionLists");

            migrationBuilder.DropTable(
                name: "PaymentDatas");

            migrationBuilder.DropTable(
                name: "PinCodeModels");

            migrationBuilder.DropTable(
                name: "PostImages");

            migrationBuilder.DropTable(
                name: "PostModals");

            migrationBuilder.DropTable(
                name: "PrincipalArchives");

            migrationBuilder.DropTable(
                name: "Properties");

            migrationBuilder.DropTable(
                name: "PsychomotorDomainArchives");

            migrationBuilder.DropTable(
                name: "PsychomotorDomains");

            migrationBuilder.DropTable(
                name: "PublishResults");

            migrationBuilder.DropTable(
                name: "Qualifications");

            migrationBuilder.DropTable(
                name: "Quotes");

            migrationBuilder.DropTable(
                name: "RecognitiveDomainArchives");

            migrationBuilder.DropTable(
                name: "Responses");

            migrationBuilder.DropTable(
                name: "Riddles");

            migrationBuilder.DropTable(
                name: "SchoolAccounts");

            migrationBuilder.DropTable(
                name: "SchoolFees");

            migrationBuilder.DropTable(
                name: "SchoolFeesArchives");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "SettingsValues");

            migrationBuilder.DropTable(
                name: "SiteBreadCrumbs");

            migrationBuilder.DropTable(
                name: "SiteContacts");

            migrationBuilder.DropTable(
                name: "SiteFooterJSs");

            migrationBuilder.DropTable(
                name: "SiteFooters");

            migrationBuilder.DropTable(
                name: "SiteGalleries");

            migrationBuilder.DropTable(
                name: "SiteGalleryLists");

            migrationBuilder.DropTable(
                name: "SiteHeaders");

            migrationBuilder.DropTable(
                name: "SiteHomeBodies");

            migrationBuilder.DropTable(
                name: "SiteHomeBodyAfterNews");

            migrationBuilder.DropTable(
                name: "SiteNavHeaders");

            migrationBuilder.DropTable(
                name: "SiteNewsPostLists");

            migrationBuilder.DropTable(
                name: "SiteNewsPosts");

            migrationBuilder.DropTable(
                name: "SiteOverrideCSSes");

            migrationBuilder.DropTable(
                name: "SitePages");

            migrationBuilder.DropTable(
                name: "SiteSettings");

            migrationBuilder.DropTable(
                name: "SiteSinglePosts");

            migrationBuilder.DropTable(
                name: "SiteSliders");

            migrationBuilder.DropTable(
                name: "Sliders");

            migrationBuilder.DropTable(
                name: "SmsGroups");

            migrationBuilder.DropTable(
                name: "SmsReports");

            migrationBuilder.DropTable(
                name: "StudentDatas");

            migrationBuilder.DropTable(
                name: "SuperSettings");

            migrationBuilder.DropTable(
                name: "Syllables");

            migrationBuilder.DropTable(
                name: "TemplateTypes");

            migrationBuilder.DropTable(
                name: "Testimonies");

            migrationBuilder.DropTable(
                name: "TimeTables");

            migrationBuilder.DropTable(
                name: "Trackers");

            migrationBuilder.DropTable(
                name: "WebsiteColors");

            migrationBuilder.DropTable(
                name: "WebsiteSettings");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "Attendances");

            migrationBuilder.DropTable(
                name: "BlogCategories");

            migrationBuilder.DropTable(
                name: "CategoryPages");

            migrationBuilder.DropTable(
                name: "Incomes");

            migrationBuilder.DropTable(
                name: "Gradings");

            migrationBuilder.DropTable(
                name: "EnrolledHostelBeds");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropTable(
                name: "ContactUs");

            migrationBuilder.DropTable(
                name: "PageSections");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "SitePageCategories");

            migrationBuilder.DropTable(
                name: "TemplateCategories");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.DropTable(
                name: "EnrolledHostelRooms");

            migrationBuilder.DropTable(
                name: "HostelBeds");

            migrationBuilder.DropTable(
                name: "WebPages");

            migrationBuilder.DropTable(
                name: "ClassLevels");

            migrationBuilder.DropTable(
                name: "RecognitiveDomains");

            migrationBuilder.DropTable(
                name: "StudentProfiles");

            migrationBuilder.DropTable(
                name: "EnrolledHostels");

            migrationBuilder.DropTable(
                name: "HostelRooms");

            migrationBuilder.DropTable(
                name: "PageCategories");

            migrationBuilder.DropTable(
                name: "StaffProfiles");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Hostels");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
