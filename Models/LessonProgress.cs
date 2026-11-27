using System;

namespace GyanPath.Models
{
    public class LessonProgress
    {
        public int ProgressID { get; set; }
        public int UserID { get; set; }
        public int LessonID { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }

        public string? LessonTitle { get; set; }
        public int RoadmapID { get; set; }
    }

    public class QuizAttempt
    {
        public int AttemptID { get; set; }
        public int UserID { get; set; }
        public int LessonID { get; set; }
        public decimal ScorePercentage { get; set; }
        public bool Passed { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public DateTime AttemptedAt { get; set; } = DateTime.Now;

        // Joined properties
        public string? LessonTitle { get; set; }
        public string? UserFullName { get; set; }
        public string? RoadmapTitle { get; set; }
    }
}
