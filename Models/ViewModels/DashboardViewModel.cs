using System.Collections.Generic;

namespace GyanPath.Models.ViewModels
{
    public class DashboardViewModel
    {
        public User User { get; set; } = new User();
        public List<Enrollment> ActiveEnrollments { get; set; } = new List<Enrollment>();
        public List<Enrollment> CompletedRoadmaps { get; set; } = new List<Enrollment>();
        public List<QuizAttempt> RecentQuizAttempts { get; set; } = new List<QuizAttempt>();
        public List<JobOpportunity> RecommendedJobs { get; set; } = new List<JobOpportunity>();
        
        public int TotalEnrolled { get; set; }
        public int TotalCompletedLessons { get; set; }
        public int TotalCompletedRoadmaps { get; set; }
        public int TotalQuizzesPassed { get; set; }
    }
}
