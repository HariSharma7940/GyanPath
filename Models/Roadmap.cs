using System;
using System.Collections.Generic;

namespace GyanPath.Models
{
    public class Roadmap
    {
        public int RoadmapID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Domain { get; set; } = "Technology"; // Technology, Design, Business, Creative Arts
        public string Description { get; set; } = string.Empty;
        public string Difficulty { get; set; } = "Beginner"; // Beginner, Intermediate, Advanced
        public string EstimatedDuration { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public string? LearningObjectives { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation / helper properties
        public List<Lesson> Lessons { get; set; } = new List<Lesson>();
        public int LessonCount { get; set; }
        public decimal UserProgressPercentage { get; set; }
        public bool IsEnrolled { get; set; }
    }
}
