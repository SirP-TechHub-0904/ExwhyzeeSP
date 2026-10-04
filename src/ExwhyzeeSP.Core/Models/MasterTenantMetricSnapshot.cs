using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExwhyzeeSP.Core.Models
{
    public enum TelemetryHealthStatus
    {
        Healthy = 1,
        Warning = 2,
        Degraded = 3,
        Offline = 4
    }

    /// <summary>
    /// Stores statistical telemetry and analytical snapshots collected from tenant school instances
    /// via the Tenant Coordination API or direct database synchronizers.
    /// </summary>
    public class MasterTenantMetricSnapshot
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TenantSchoolId { get; set; }

        [ForeignKey(nameof(TenantSchoolId))]
        public virtual TenantSchool? TenantSchool { get; set; }

        public DateTime SnapshotDate { get; set; } = DateTime.UtcNow;

        // Student Demographics
        public int TotalStudents { get; set; }
        public int MaleStudents { get; set; }
        public int FemaleStudents { get; set; }
        public int EarlyYearsStudents { get; set; }
        public int PrimaryStudents { get; set; }
        public int JuniorSecondaryStudents { get; set; }
        public int SeniorSecondaryStudents { get; set; }

        // Staffing & Structure
        public int TotalStaff { get; set; }
        public int AcademicStaff { get; set; }
        public int NonAcademicStaff { get; set; }
        public int TotalClasses { get; set; }
        public int TotalSubjects { get; set; }

        // Academic Calendar
        [StringLength(50)]
        public string ActiveSession { get; set; } = "2026/2027";

        [StringLength(50)]
        public string ActiveTerm { get; set; } = "1st Term";

        public double DailyAttendanceRate { get; set; } = 92.5; // Percentage e.g. 95.2

        // Financial Metrics
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalFeesBilled { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalFeesCollected { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalFeesPending { get; set; }

        public double FeeCollectionPercentage => TotalFeesBilled > 0 ? (double)(TotalFeesCollected / TotalFeesBilled * 100) : 0;

        // Examination & CBT & PIN Verification
        public int TotalResultsPublished { get; set; }
        public int TotalPinsVerified { get; set; }
        public int CbtExamsConducted { get; set; }
        public int CbtSubmissions { get; set; }

        // Infrastructure & System Telemetry
        public double StorageUsedMb { get; set; }
        public long DatabaseSizeBytes { get; set; }
        public int ApiLatencyMs { get; set; } = 24;
        public TelemetryHealthStatus HealthStatus { get; set; } = TelemetryHealthStatus.Healthy;

        [StringLength(100)]
        public string SyncSource { get; set; } = "TenantCoordinationApi";

        [StringLength(50)]
        public string ClientVersion { get; set; } = "v2.5.0";

        public DateTime RecordedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
