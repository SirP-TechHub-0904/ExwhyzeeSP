using System;
using System.Collections.Generic;
using ExwhyzeeSP.Core.Models;

namespace ExwhyzeeSP.Master.Models
{
    public class TopSchoolItem
    {
        public int Id { get; set; }
        public string SchoolName { get; set; } = string.Empty;
        public string SubDomain { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public int StaffCount { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class TopSchoolRevenueItem
    {
        public int SchoolId { get; set; }
        public string SchoolName { get; set; } = string.Empty;
        public string SubDomain { get; set; } = string.Empty;
        public decimal TotalPaid { get; set; }
        public decimal TotalBilled { get; set; }
        public int InvoicesCount { get; set; }
        public DateTime LastPaymentDate { get; set; }
    }

    public class TopAcademicSchoolItem
    {
        public int SchoolId { get; set; }
        public string SchoolName { get; set; } = string.Empty;
        public string SubDomain { get; set; } = string.Empty;
        public int TotalStudents { get; set; }
        public int AcademicStaff { get; set; }
        public int NonAcademicStaff { get; set; }
        public double AttendanceRate { get; set; }
        public int PinsVerified { get; set; }
        public int CbtExams { get; set; }
    }

    public class CategoryDistributionItem
    {
        public string CategoryName { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class DashboardTabViewModel
    {
        public string ActiveTab { get; set; } = "schools";
        public string DateRange { get; set; } = "year";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // ================= TAB 1: SCHOOLS =================
        public int TotalSchools { get; set; }
        public int ActiveSchools { get; set; }
        public int InactiveSchools { get; set; }
        public int PendingUpdatesCount { get; set; }
        public List<TopSchoolItem> TopSchoolsByStudents { get; set; } = new();
        public List<TenantSchool> RecentSchools { get; set; } = new();
        public List<TenantSchool> InactiveSchoolList { get; set; } = new();

        // ================= TAB 2: FINANCE =================
        public decimal TotalRevenue { get; set; }
        public decimal TotalPendingRevenue { get; set; }
        public int PaidInvoicesCount { get; set; }
        public int PendingInvoicesCount { get; set; }
        public int OverdueInvoicesCount { get; set; }
        public int ActivePlansCount { get; set; }
        public List<TopSchoolRevenueItem> TopRevenueSchools { get; set; } = new();
        public List<MasterSchoolTransaction> RecentTransactions { get; set; } = new();
        public List<MasterSchoolTransaction> PendingTransactions { get; set; } = new();

        // ================= TAB 3: ACADEMICS =================
        public int TotalStudents { get; set; }
        public int TotalStaff { get; set; }
        public double AvgAttendance { get; set; }
        public int TotalPinsVerified { get; set; }
        public int TotalCbtExams { get; set; }
        public List<TopAcademicSchoolItem> TopAttendanceSchools { get; set; } = new();
        public List<TopAcademicSchoolItem> TopStaffSchools { get; set; } = new();
        public List<TopAcademicSchoolItem> TopAssessmentSchools { get; set; } = new();

        // ================= TAB 4: MARKETING =================
        public int TotalContacts { get; set; }
        public int TotalCampaigns { get; set; }
        public int TotalDelivered { get; set; }
        public int TotalTemplates { get; set; }
        public List<MarketingCampaign> RecentCampaigns { get; set; } = new();
        public List<MarketingContact> RecentContacts { get; set; } = new();
        public List<CategoryDistributionItem> ContactCategories { get; set; } = new();

        // ================= TAB 5: CONTENT & PLATFORM =================
        public int TotalKb { get; set; }
        public int TotalBlogs { get; set; }
        public int TotalTestimonies { get; set; }
        public int TotalAuditLogs { get; set; }
        public List<MasterKnowledgeBaseItem> RecentKb { get; set; } = new();
        public List<Blog> RecentBlogs { get; set; } = new();
        public List<MasterAuditLog> RecentAudits { get; set; } = new();

        // ================= CHART DATA (Multi-View Switchable) =================
        public List<string> ChartLabels { get; set; } = new();
        public List<decimal> ChartValues1 { get; set; } = new();
        public List<decimal> ChartValues2 { get; set; } = new();
        public string ChartTitle { get; set; } = string.Empty;
        public string ChartSeriesName1 { get; set; } = string.Empty;
        public string ChartSeriesName2 { get; set; } = string.Empty;
        public List<string> DonutLabels { get; set; } = new();
        public List<decimal> DonutValues { get; set; } = new();
    }
}
