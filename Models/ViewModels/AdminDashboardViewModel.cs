using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GyanPath.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalRoadmaps { get; set; }
        public int TotalLessons { get; set; }
        public int TotalEnrollments { get; set; }
        public int TotalQuizAttempts { get; set; }
        public int TotalJobs { get; set; }

        public List<User> RecentUsers { get; set; } = new List<User>();
        public List<QuizAttempt> RecentQuizAttempts { get; set; } = new List<QuizAttempt>();
        public List<Enrollment> RecentEnrollments { get; set; } = new List<Enrollment>();
    }

    public class ProfileViewModel
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Preferred domain is required.")]
        public string PreferredDomain { get; set; } = "Technology";

        public string Role { get; set; } = "Member";

        [DataType(DataType.Password)]
        [Display(Name = "Current Password (only if changing password)")]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        [Display(Name = "New Password (optional)")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "New password and confirmation do not match.")]
        [Display(Name = "Confirm New Password")]
        public string? ConfirmNewPassword { get; set; }
    }
}
