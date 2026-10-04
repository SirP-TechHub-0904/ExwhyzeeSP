using System;
using ExwhyzeeSP.Core.Entities;

namespace ExwhyzeeSP.Core.Helpers
{
    /// <summary>
    /// Evaluates academic grades and remarks dynamically from scores based on school grading options.
    /// Pure in-memory calculation with 0 database roundtrips.
    /// </summary>
    public static class GradingEvaluator
    {
        public static string GetGrade(decimal? score, GradingOption option)
        {
            if (!score.HasValue || score.Value <= 0)
                return "-";

            var s = score.Value;

            switch (option)
            {
                case GradingOption.SSS:
                    if (s >= 75) return "A1";
                    if (s >= 70) return "B2";
                    if (s >= 65) return "B3";
                    if (s >= 60) return "C4";
                    if (s >= 55) return "C5";
                    if (s >= 50) return "C6";
                    if (s >= 45) return "D7";
                    if (s >= 40) return "E8";
                    return "F9";

                case GradingOption.JSS:
                    if (s >= 70) return "A";
                    if (s >= 60) return "C";
                    if (s >= 50) return "P";
                    return "F";

                case GradingOption.PRI:
                case GradingOption.NUR:
                case GradingOption.PG:
                case GradingOption.PRE:
                    if (s >= 80) return "A";
                    if (s >= 70) return "B";
                    if (s >= 60) return "C";
                    if (s >= 50) return "D";
                    return "E";

                default:
                    if (s >= 70) return "A";
                    if (s >= 60) return "B";
                    if (s >= 50) return "C";
                    if (s >= 45) return "D";
                    if (s >= 40) return "E";
                    return "F";
            }
        }

        public static string GetRemark(decimal? score, GradingOption option)
        {
            if (!score.HasValue || score.Value <= 0)
                return "-";

            var s = score.Value;

            switch (option)
            {
                case GradingOption.SSS:
                    if (s >= 75) return "Excellent";
                    if (s >= 70) return "Very Good";
                    if (s >= 65) return "Good";
                    if (s >= 60) return "Credit";
                    if (s >= 55) return "Credit";
                    if (s >= 50) return "Credit";
                    if (s >= 45) return "Pass";
                    if (s >= 40) return "Pass";
                    return "Fail";

                case GradingOption.JSS:
                    if (s >= 70) return "Distinction";
                    if (s >= 60) return "Credit";
                    if (s >= 50) return "Pass";
                    return "Fail";

                case GradingOption.PRI:
                case GradingOption.NUR:
                case GradingOption.PG:
                case GradingOption.PRE:
                    if (s >= 80) return "Excellent";
                    if (s >= 70) return "Very Good";
                    if (s >= 60) return "Good";
                    if (s >= 50) return "Fair";
                    return "Poor";

                default:
                    if (s >= 70) return "Excellent";
                    if (s >= 60) return "Very Good";
                    if (s >= 50) return "Good";
                    if (s >= 45) return "Pass";
                    return "Fail";
            }
        }
    }
}
