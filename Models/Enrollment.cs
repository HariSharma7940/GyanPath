using System;

namespace GyanPath.Models
{
    public class Enrollment
    {
        public int EnrollmentID { get; set; }
        public int UserID { get; set; }
        public int RoadmapID { get; set; }
        public DateTime EnrolledAt { get; set; } = DateTime.Now;
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Joined properties
        public string? UserFullName { get; set; }
        public string? UserEmail { get; set; }
        public string? RoadmapTitle { get; set; }
        public string? RoadmapDomain { get; set; }
        public string? ThumbnailUrl { get; set; }
        public decimal ProgressPercentage { get; set; }
    }
}
