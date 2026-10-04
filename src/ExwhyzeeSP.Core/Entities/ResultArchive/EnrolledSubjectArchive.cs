using System;
using System.ComponentModel.DataAnnotations;
using ExwhyzeeSP.Core.Entities;
using ExwhyzeeSP.Core.Helpers;

namespace ExwhyzeeSP.Core.Entities.ResultArchive
{
    public class EnrolledSubjectArchive
    {
        public int Id { get; set; }

        public string? SubjectName { get; set; }

        [Display(Name = "Test Score:")]
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

        public int EnrollmentId { get; set; }
        public Enrollment? Enrollments { get; set; }

        public bool IsOffered { get; set; } = true;

        public int SortOrder { get; set; }

        public int? EnrollmentSubjectId { get; set; }

        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        [Display(Name = "Grading Option")]
        public GradingOption GradingOption { get; set; }

        [Display(Name = "Grade:")]
        public string Grade => GradingEvaluator.GetGrade(this.TotalScore, this.GradingOption);

        [Display(Name = "Remark:")]
        public string Remark => GradingEvaluator.GetRemark(this.TotalScore, this.GradingOption);
    }
}
