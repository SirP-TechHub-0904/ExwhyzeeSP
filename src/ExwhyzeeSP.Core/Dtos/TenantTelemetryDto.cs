using System;

namespace ExwhyzeeSP.Core.Dtos
{
    public class TenantTelemetryReportDto
    {
        public string? ApiKey { get; set; }
        public string? SubDomain { get; set; }
        public int? TenantSchoolId { get; set; }

        public int TotalStudents { get; set; }
        public int MaleStudents { get; set; }
        public int FemaleStudents { get; set; }
        public int EarlyYearsStudents { get; set; }
        public int PrimaryStudents { get; set; }
        public int JuniorSecondaryStudents { get; set; }
        public int SeniorSecondaryStudents { get; set; }

        public int TotalStaff { get; set; }
        public int AcademicStaff { get; set; }
        public int NonAcademicStaff { get; set; }
        public int TotalClasses { get; set; }
        public int TotalSubjects { get; set; }

        public string ActiveSession { get; set; } = "2026/2027";
        public string ActiveTerm { get; set; } = "1st Term";
        public double DailyAttendanceRate { get; set; } = 94.0;

        public decimal TotalFeesBilled { get; set; }
        public decimal TotalFeesCollected { get; set; }
        public decimal TotalFeesPending { get; set; }

        public int TotalResultsPublished { get; set; }
        public int TotalPinsVerified { get; set; }
        public int CbtExamsConducted { get; set; }
        public int CbtSubmissions { get; set; }

        public double StorageUsedMb { get; set; }
        public long DatabaseSizeBytes { get; set; }
        public int ApiLatencyMs { get; set; } = 25;
        public string ClientVersion { get; set; } = "v2.5.0";
    }

    public class TenantCoordinationResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int SnapshotId { get; set; }
        public string SchoolName { get; set; } = string.Empty;
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    }
}
