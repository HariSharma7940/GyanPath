using System;
using System.Collections.Generic;

namespace GyanPath.Models
{
    public class Lesson
    {
        public int LessonID { get; set; }
        public int RoadmapID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
        public int LessonOrder { get; set; }
        public string? Resources { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Helper / Display properties
        public string? RoadmapTitle { get; set; }
        public bool IsCompletedByUser { get; set; }
        public bool UserPassedQuiz { get; set; }
        public decimal? LastQuizScore { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
    }
}
