using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;

namespace ExwhyzeeSP.Master.Controllers
{
    /// <summary>
    /// Master Statistical Intelligence & Analytics Controller.
    /// Provides multi-school comparisons, demographic breakdowns, financial liquidity,
    /// CBT/result verification trends, and predictive growth projections.
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin},{MasterRoles.Accountant}")]
    public class AnalyticsController : Controller
    {
        private readonly MasterDbContext _context;

        public AnalyticsController(MasterDbContext context)
        {
            _context = context;
        }

        [HttpGet("/Analytics")]
        public async Task<IActionResult> Index()
        {
            var schools = await _context.TenantSchools.AsNoTracking().ToListAsync();
            var snapshots = await _context.TenantMetricSnapshots
                .AsNoTracking()
                .Include(s => s.TenantSchool)
                .OrderByDescending(s => s.RecordedAtUtc)
                .ToListAsync();

            // Latest snapshot per school
            var latestSnapshots = snapshots
                .GroupBy(s => s.TenantSchoolId)
                .Select(g => g.First())
                .ToList();

            ViewBag.TotalSchools = schools.Count;
            ViewBag.ActiveSchools = schools.Count(s => s.IsActive);
            ViewBag.TotalStudents = latestSnapshots.Sum(s => s.TotalStudents);
            ViewBag.TotalStaff = latestSnapshots.Sum(s => s.TotalStaff);
            ViewBag.TotalMaleStudents = latestSnapshots.Sum(s => s.MaleStudents);
            ViewBag.TotalFemaleStudents = latestSnapshots.Sum(s => s.FemaleStudents);
            ViewBag.TotalFeesBilled = latestSnapshots.Sum(s => s.TotalFeesBilled);
            ViewBag.TotalFeesCollected = latestSnapshots.Sum(s => s.TotalFeesCollected);
            ViewBag.TotalFeesPending = latestSnapshots.Sum(s => s.TotalFeesPending);
            ViewBag.TotalPinsVerified = latestSnapshots.Sum(s => s.TotalPinsVerified);
            ViewBag.TotalCbtExams = latestSnapshots.Sum(s => s.CbtExamsConducted);
            ViewBag.AvgAttendanceRate = latestSnapshots.Any() ? Math.Round(latestSnapshots.Average(s => s.DailyAttendanceRate), 1) : 0;
            ViewBag.HealthyNodes = latestSnapshots.Count(s => s.HealthStatus == TelemetryHealthStatus.Healthy);
            ViewBag.Schools = schools;
            ViewBag.LatestSnapshots = latestSnapshots;

            return View(latestSnapshots);
        }

        [HttpGet("/Analytics/SchoolComparison")]
        public async Task<IActionResult> SchoolComparison()
        {
            var snapshots = await _context.TenantMetricSnapshots
                .AsNoTracking()
                .Include(s => s.TenantSchool)
                .OrderByDescending(s => s.RecordedAtUtc)
                .ToListAsync();

            var latest = snapshots.GroupBy(s => s.TenantSchoolId).Select(g => g.First()).ToList();
            return View(latest);
        }

        [HttpGet("/Analytics/Demographics")]
        public async Task<IActionResult> Demographics()
        {
            var snapshots = await _context.TenantMetricSnapshots
                .AsNoTracking()
                .Include(s => s.TenantSchool)
                .OrderByDescending(s => s.RecordedAtUtc)
                .ToListAsync();

            var latest = snapshots.GroupBy(s => s.TenantSchoolId).Select(g => g.First()).ToList();
            return View(latest);
        }

        [HttpGet("/Analytics/Projections")]
        public async Task<IActionResult> Projections()
        {
            var snapshots = await _context.TenantMetricSnapshots
                .AsNoTracking()
                .Include(s => s.TenantSchool)
                .OrderByDescending(s => s.RecordedAtUtc)
                .ToListAsync();

            var latest = snapshots.GroupBy(s => s.TenantSchoolId).Select(g => g.First()).ToList();
            return View(latest);
        }

        [HttpGet("/Analytics/TelemetryLive")]
        public async Task<IActionResult> TelemetryLive()
        {
            var snapshots = await _context.TenantMetricSnapshots
                .AsNoTracking()
                .Include(s => s.TenantSchool)
                .OrderByDescending(s => s.RecordedAtUtc)
                .Take(50)
                .ToListAsync();

            return View(snapshots);
        }
    }
}
