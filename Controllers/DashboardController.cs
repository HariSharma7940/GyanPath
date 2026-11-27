using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GyanPath.Data;
using GyanPath.Models;
using GyanPath.Models.ViewModels;

namespace GyanPath.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserRepository _userRepo;
        private readonly ProgressRepository _progressRepo;
        private readonly QuizRepository _quizRepo;
        private readonly JobRepository _jobRepo;

        public DashboardController(UserRepository userRepo, ProgressRepository progressRepo, QuizRepository quizRepo, JobRepository jobRepo)
        {
            _userRepo = userRepo;
            _progressRepo = progressRepo;
            _quizRepo = quizRepo;
            _jobRepo = jobRepo;
        }

        public IActionResult Index()
        {
            int? userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            User? user = _userRepo.GetById(userId.Value);
            if (user == null)
            {
                return NotFound();
            }

            List<Enrollment> enrollments = _progressRepo.GetUserEnrollments(userId.Value);
            List<Enrollment> activeEnrollments = enrollments.Where(e => !e.IsCompleted).ToList();
            List<Enrollment> completedRoadmaps = enrollments.Where(e => e.IsCompleted).ToList();

            List<QuizAttempt> recentQuizAttempts = _quizRepo.GetUserAttempts(userId.Value).Take(5).ToList();
            List<JobOpportunity> recommendedJobs = _jobRepo.GetRecommendationsForUser(userId.Value, user.PreferredDomain).Take(4).ToList();

            int totalCompletedLessons = _progressRepo.GetTotalCompletedLessonsCount(userId.Value);
            int totalQuizzesPassed = recentQuizAttempts.Count(q => q.Passed);

            var viewModel = new DashboardViewModel
            {
                User = user,
                ActiveEnrollments = activeEnrollments,
                CompletedRoadmaps = completedRoadmaps,
                RecentQuizAttempts = recentQuizAttempts,
                RecommendedJobs = recommendedJobs,
                TotalEnrolled = enrollments.Count,
                TotalCompletedLessons = totalCompletedLessons,
                TotalCompletedRoadmaps = completedRoadmaps.Count,
                TotalQuizzesPassed = totalQuizzesPassed
            };

            return View(viewModel);
        }

        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }
    }
}
