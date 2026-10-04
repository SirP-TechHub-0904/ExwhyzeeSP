using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExwhyzeeSP.Data.Migrations.MasterDb
{
    /// <inheritdoc />
    public partial class AddMasterFeature3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowAdmissionPortal",
                table: "TenantSchools",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowCbtExams",
                table: "TenantSchools",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowElearning",
                table: "TenantSchools",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowOnlinePayment",
                table: "TenantSchools",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowResultChecking",
                table: "TenantSchools",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowSmsNotifications",
                table: "TenantSchools",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowStaffPortal",
                table: "TenantSchools",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowStudentPortal",
                table: "TenantSchools",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "GracePeriodDeadline",
                table: "TenantSchools",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SuspensionMode",
                table: "TenantSchools",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SuspensionNotice",
                table: "TenantSchools",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuspensionReason",
                table: "TenantSchools",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimestampUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    UserRole = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Browser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeviceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OperatingSystem = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Area = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TenantSubDomain = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TenantSchoolId = table.Column<int>(type: "int", nullable: true),
                    Details = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Severity = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecipientEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RecipientName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    SenderEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    BodyHtml = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BodyText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailCategory = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    TriggeredBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    TenantSubDomain = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketingContacts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SchoolName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RoleTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Tag = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsSubscribed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastEmailedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketingContacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketingTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PreheaderText = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    BodyHtml = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketingTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchoolTransactions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantSchoolId = table.Column<int>(type: "int", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BillingType = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaymentChannel = table.Column<int>(type: "int", nullable: false),
                    BankReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProofDocumentUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReceiptNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RecordedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchoolTransactions_TenantSchools_TenantSchoolId",
                        column: x => x.TenantSchoolId,
                        principalTable: "TenantSchools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantMetricSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantSchoolId = table.Column<int>(type: "int", nullable: false),
                    SnapshotDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalStudents = table.Column<int>(type: "int", nullable: false),
                    MaleStudents = table.Column<int>(type: "int", nullable: false),
                    FemaleStudents = table.Column<int>(type: "int", nullable: false),
                    EarlyYearsStudents = table.Column<int>(type: "int", nullable: false),
                    PrimaryStudents = table.Column<int>(type: "int", nullable: false),
                    JuniorSecondaryStudents = table.Column<int>(type: "int", nullable: false),
                    SeniorSecondaryStudents = table.Column<int>(type: "int", nullable: false),
                    TotalStaff = table.Column<int>(type: "int", nullable: false),
                    AcademicStaff = table.Column<int>(type: "int", nullable: false),
                    NonAcademicStaff = table.Column<int>(type: "int", nullable: false),
                    TotalClasses = table.Column<int>(type: "int", nullable: false),
                    TotalSubjects = table.Column<int>(type: "int", nullable: false),
                    ActiveSession = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ActiveTerm = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DailyAttendanceRate = table.Column<double>(type: "float", nullable: false),
                    TotalFeesBilled = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalFeesCollected = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalFeesPending = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalResultsPublished = table.Column<int>(type: "int", nullable: false),
                    TotalPinsVerified = table.Column<int>(type: "int", nullable: false),
                    CbtExamsConducted = table.Column<int>(type: "int", nullable: false),
                    CbtSubmissions = table.Column<int>(type: "int", nullable: false),
                    StorageUsedMb = table.Column<double>(type: "float", nullable: false),
                    DatabaseSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    ApiLatencyMs = table.Column<int>(type: "int", nullable: false),
                    HealthStatus = table.Column<int>(type: "int", nullable: false),
                    SyncSource = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ClientVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RecordedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantMetricSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantMetricSnapshots_TenantSchools_TenantSchoolId",
                        column: x => x.TenantSchoolId,
                        principalTable: "TenantSchools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketingCampaigns",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SenderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SenderEmail = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    TargetTag = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MarketingTemplateId = table.Column<int>(type: "int", nullable: true),
                    BodyHtml = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalRecipients = table.Column<int>(type: "int", nullable: false),
                    DeliveredCount = table.Column<int>(type: "int", nullable: false),
                    FailedCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionStartedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExecutionCompletedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ExecutionSummary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketingCampaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketingCampaigns_MarketingTemplates_MarketingTemplateId",
                        column: x => x.MarketingTemplateId,
                        principalTable: "MarketingTemplates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketingCampaigns_MarketingTemplateId",
                table: "MarketingCampaigns",
                column: "MarketingTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolTransactions_TenantSchoolId",
                table: "SchoolTransactions",
                column: "TenantSchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantMetricSnapshots_TenantSchoolId",
                table: "TenantMetricSnapshots",
                column: "TenantSchoolId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "EmailLogs");

            migrationBuilder.DropTable(
                name: "MarketingCampaigns");

            migrationBuilder.DropTable(
                name: "MarketingContacts");

            migrationBuilder.DropTable(
                name: "SchoolTransactions");

            migrationBuilder.DropTable(
                name: "TenantMetricSnapshots");

            migrationBuilder.DropTable(
                name: "MarketingTemplates");

            migrationBuilder.DropColumn(
                name: "AllowAdmissionPortal",
                table: "TenantSchools");

            migrationBuilder.DropColumn(
                name: "AllowCbtExams",
                table: "TenantSchools");

            migrationBuilder.DropColumn(
                name: "AllowElearning",
                table: "TenantSchools");

            migrationBuilder.DropColumn(
                name: "AllowOnlinePayment",
                table: "TenantSchools");

            migrationBuilder.DropColumn(
                name: "AllowResultChecking",
                table: "TenantSchools");

            migrationBuilder.DropColumn(
                name: "AllowSmsNotifications",
                table: "TenantSchools");

            migrationBuilder.DropColumn(
                name: "AllowStaffPortal",
                table: "TenantSchools");

            migrationBuilder.DropColumn(
                name: "AllowStudentPortal",
                table: "TenantSchools");

            migrationBuilder.DropColumn(
                name: "GracePeriodDeadline",
                table: "TenantSchools");

            migrationBuilder.DropColumn(
                name: "SuspensionMode",
                table: "TenantSchools");

            migrationBuilder.DropColumn(
                name: "SuspensionNotice",
                table: "TenantSchools");

            migrationBuilder.DropColumn(
                name: "SuspensionReason",
                table: "TenantSchools");
        }
    }
}
