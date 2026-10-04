using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ExwhyzeeSP.Services.Excel
{
    /// <summary>
    /// Service for processing Excel spreadsheets (.xlsx) using ClosedXML.
    /// Fast, cross-platform, and replaces legacy COM Interop and LinqToExcel.
    /// </summary>
    public interface IExcelService
    {
        /// <summary>
        /// Reads student score rows from an uploaded Excel file.
        /// </summary>
        Task<List<ExcelScoreRecord>> ReadScoresFromExcelAsync(IFormFile file);

        /// <summary>
        /// Generates an Excel broadsheet for a class level.
        /// </summary>
        byte[] GenerateClassBroadsheetExcel(string className, string sessionName, IEnumerable<ExcelBroadsheetRow> rows);
    }

    public class ExcelScoreRecord
    {
        public string RegistrationNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public decimal? TestScore { get; set; }
        public decimal? ExamScore { get; set; }
        public decimal? TestScore2 { get; set; }
        public decimal? Project { get; set; }
        public decimal? ClassExercise { get; set; }
        public decimal? Assessment { get; set; }
    }

    public class ExcelBroadsheetRow
    {
        public string RegNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public Dictionary<string, decimal?> SubjectScores { get; set; } = new();
        public decimal TotalScore { get; set; }
        public decimal AverageScore { get; set; }
        public int Position { get; set; }
    }
}
