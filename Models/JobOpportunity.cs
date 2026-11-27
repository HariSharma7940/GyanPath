using System;

namespace GyanPath.Models
{
    public class JobOpportunity
    {
        public int JobID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Domain { get; set; } = "Technology"; // Technology, Design, Business, Creative Arts
        public string Location { get; set; } = string.Empty;
        public string JobType { get; set; } = "Full-time"; // Full-time, Internship, Part-time, Remote
        public string Description { get; set; } = string.Empty;
        public string RequiredSkills { get; set; } = string.Empty;
        public string ApplicationUrl { get; set; } = string.Empty;
        public DateTime PostedDate { get; set; } = DateTime.Now;

        // Recommendation score / reason for user UI display
        public bool IsRecommended { get; set; }
        public string? RecommendationReason { get; set; }
    }
}
