using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;

namespace ExwhyzeeSP.Services.Excel
{
    /// <summary>
    /// Implements Excel reading and generation using ClosedXML for .NET 9.
    /// Replaces legacy Microsoft.Office.Interop.Excel and LinqToExcel.
    /// </summary>
    public class ExcelService : IExcelService
    {
        public async Task<List<ExcelScoreRecord>> ReadScoresFromExcelAsync(IFormFile file)
        {
            var records = new List<ExcelScoreRecord>();

            if (file == null || file.Length == 0)
                return records;

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var workbook = new XLWorkbook(memoryStream);
            var worksheet = workbook.Worksheet(1); // First worksheet

            var rows = worksheet.RangeUsed().RowsUsed();
            bool isHeader = true;

            foreach (var row in rows)
            {
                if (isHeader)
                {
                    isHeader = false;
                    continue; // Skip header row
                }

                var regNo = row.Cell(1).GetString()?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(regNo))
                    continue;

                var name = row.Cell(2).GetString()?.Trim() ?? string.Empty;

                decimal? ParseCell(int col)
                {
                    var val = row.Cell(col).GetString()?.Trim();
                    if (decimal.TryParse(val, out decimal d)) return d;
                    return null;
                }

                records.Add(new ExcelScoreRecord
                {
                    RegistrationNumber = regNo,
                    StudentName = name,
                    TestScore = ParseCell(3),
                    ExamScore = ParseCell(4),
                    TestScore2 = ParseCell(5),
                    Project = ParseCell(6),
                    ClassExercise = ParseCell(7),
                    Assessment = ParseCell(8)
                });
            }

            return records;
        }

        public byte[] GenerateClassBroadsheetExcel(string className, string sessionName, IEnumerable<ExcelBroadsheetRow> rows)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Broadsheet");

            // Title Header
            worksheet.Cell(1, 1).Value = $"{className} Broadsheet - {sessionName}";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 14;

            // Table Headers
            worksheet.Cell(3, 1).Value = "Reg Number";
            worksheet.Cell(3, 2).Value = "Full Name";
            worksheet.Cell(3, 3).Value = "Total Score";
            worksheet.Cell(3, 4).Value = "Average";
            worksheet.Cell(3, 5).Value = "Position";

            var headerRange = worksheet.Range(3, 1, 3, 5);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1a5276");
            headerRange.Style.Font.FontColor = XLColor.White;

            int currentRow = 4;
            foreach (var r in rows)
            {
                worksheet.Cell(currentRow, 1).Value = r.RegNumber;
                worksheet.Cell(currentRow, 2).Value = r.FullName;
                worksheet.Cell(currentRow, 3).Value = r.TotalScore;
                worksheet.Cell(currentRow, 4).Value = r.AverageScore;
                worksheet.Cell(currentRow, 5).Value = r.Position;
                currentRow++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
