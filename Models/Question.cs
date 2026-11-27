using System;
using System.Collections.Generic;

namespace GyanPath.Models
{
    public class Question
    {
        public int QuestionID { get; set; }
        public int LessonID { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string? Explanation { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Display properties
        public string? LessonTitle { get; set; }
        public List<QuizOption> Options { get; set; } = new List<QuizOption>();
    }

    public class QuizOption
    {
        public int OptionID { get; set; }
        public int QuestionID { get; set; }
        public string OptionText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}
