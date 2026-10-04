using System;
using System.ComponentModel.DataAnnotations;

namespace ExwhyzeeSP.Core.Models
{
    public class MasterKnowledgeBaseItem
    {
        public long Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Display(Name = "Page / Guide Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Display(Name = "Target User Role")]
        public string TargetRole { get; set; } = "Admin"; // "Admin", "Staff", "Student"

        [Required]
        [MaxLength(80)]
        [Display(Name = "Area")]
        public string Area { get; set; } = "Admin"; // "Admin", "Staff", "Student", "CBTExam", "Content", "Financial", "Accomodation"

        [Required]
        [MaxLength(100)]
        [Display(Name = "Controller Name")]
        public string ControllerName { get; set; } = string.Empty; // e.g. "ResultManagement", "Panel", "ArchiveResult"

        [Required]
        [MaxLength(100)]
        [Display(Name = "Page / Action Name")]
        public string ActionName { get; set; } = "Index"; // e.g. "BatchResult", "EnterScore", "PrintResult"

        [Required]
        [MaxLength(250)]
        [Display(Name = "Full Route URL")]
        public string RouteUrl { get; set; } = string.Empty; // e.g. "/Admin/ResultManagement/BatchResult"

        [Required]
        [Display(Name = "What This Page Does")]
        public string WhatItDoes { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Step-by-Step Instructions & Guide")]
        public string Instructions { get; set; } = string.Empty;

        [MaxLength(80)]
        [Display(Name = "Badge / Category Tag")]
        public string? BadgeText { get; set; } // e.g. "Academics", "Grading", "Finance", "Homework"

        [Display(Name = "Display Sort Order")]
        public int SortOrder { get; set; } = 1;

        [Display(Name = "Published on Knowledge Base")]
        public bool IsPublished { get; set; } = true;

        [Display(Name = "Date Created")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
