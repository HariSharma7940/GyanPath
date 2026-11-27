using System.Collections.Generic;

namespace GyanPath.Models.ViewModels
{
    public class RoadmapDetailsViewModel
    {
        public Roadmap Roadmap { get; set; } = new Roadmap();
        public List<Lesson> Lessons { get; set; } = new List<Lesson>();
        public bool IsEnrolled { get; set; }
        public decimal CompletionPercentage { get; set; }
        public int CompletedLessonsCount { get; set; }
        public List<JobOpportunity> RelatedJobs { get; set; } = new List<JobOpportunity>();
    }

    public class QuizSubmissionModel
    {
        public int LessonID { get; set; }
        public Dictionary<int, int> Answers { get; set; } = new Dictionary<int, int>(); // QuestionID -> OptionID
    }

    public class QuizResultViewModel
    {
        public int LessonID { get; set; }
        public string LessonTitle { get; set; } = string.Empty;
        public int RoadmapID { get; set; }
        public string RoadmapTitle { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public decimal ScorePercentage { get; set; }
        public bool Passed { get; set; }
        public List<QuestionReviewItem> Reviews { get; set; } = new List<QuestionReviewItem>();
    }

    public class QuestionReviewItem
    {
        public int QuestionID { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public string SelectedOptionText { get; set; } = string.Empty;
        public string CorrectOptionText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}
