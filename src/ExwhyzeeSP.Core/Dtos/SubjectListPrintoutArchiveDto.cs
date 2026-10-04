using ExwhyzeeSP.Core.Entities;
using ExwhyzeeSP.Core.Entities.ResultArchive;
using ExwhyzeeSP.Core.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExwhyzeeSP.Core.Dtos
{
    public class SubjectListPrintoutArchiveDto
    {
        public int Id { get; set; }

        public int? SubjectId { get; set; }
        public string Subject { get; set; }

        [Display(Name = " Test Score:")]
        public decimal? TestScore { get; set; }

        [Display(Name = "Exam Score:")]
        public decimal? ExamScore { get; set; }

        [Display(Name = "2nd Test Score:")]
        public decimal? TestScore2 { get; set; }

        [Display(Name = "Project:")]
        public decimal? Project { get; set; }

        [Display(Name = "Class Exercise:")]
        public decimal? ClassExercise { get; set; }

        [Display(Name = "Assessment:")]
        public decimal? Assessment { get; set; }

        [Display(Name = "Total CA:")]
        public decimal? TotalCA { get; set; }

        [Display(Name = "Total Score:")]
        public decimal? TotalScore { get; set; }

        public decimal? HighestTotalScore { get; set; }
        public decimal? LowestTotalScore { get; set; }

        public int EnrollmentId { get; set; }
        public Enrollment? Enrollments { get; set; }
        public bool IsOffered { get; set; }

        [Display(Name = "Grading Option")]
        public GradingOption GradingOption { get; set; }

        [Display(Name = "Grade:")]
        public string Grade => GradingEvaluator.GetGrade(this.TotalScore, this.GradingOption);

        [Display(Name = "Remark:")]
        public string Remark => GradingEvaluator.GetRemark(this.TotalScore, this.GradingOption);

        public decimal? FirstTermScore { get; set; }
        public decimal? SecondTermScore { get; set; }
        public decimal? ThirdTermScore { get; set; }
        public decimal? CummulativeScore { get; set; }
        public decimal? CummulativeAverageScore { get; set; }

        [Display(Name = "Cummulative Grade:")]
        public string CummulativeGrade => GradingEvaluator.GetGrade(this.CummulativeScore, this.GradingOption);

        [Display(Name = "Cummulative Remark:")]
        public string CummulativeRemark => GradingEvaluator.GetRemark(this.CummulativeScore, this.GradingOption);

        public int Position { get; set; }
        public string SubjectPosition { get; set; }
        public decimal? ClassAverage { get; set; }
        public int SortOrder { get; set; }
    }
}