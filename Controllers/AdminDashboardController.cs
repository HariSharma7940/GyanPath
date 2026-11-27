using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GyanPath.Data;
using GyanPath.Models.ViewModels;

namespace GyanPath.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly UserRepository _userRepo;
        private readonly RoadmapRepository _roadmapRepo;
        private readonly LessonRepository _lessonRepo;
        private readonly ProgressRepository _progressRepo;
        private readonly QuizRepository _quizRepo;
        private readonly JobRepository _jobRepo;

        public AdminDashboardController(
            UserRepository userRepo,
            RoadmapRepository roadmapRepo,
            LessonRepository lessonRepo,
            ProgressRepository progressRepo,
            QuizRepository quizRepo,
            JobRepository jobRepo)
        {
            _userRepo = userRepo;
            _roadmapRepo = roadmapRepo;
            _lessonRepo = lessonRepo;
            _progressRepo = progressRepo;
            _quizRepo = quizRepo;
            _jobRepo = jobRepo;
        }

        public IActionResult Index()
        {
            var users = _userRepo.GetAll();
            var roadmaps = _roadmapRepo.GetAll();
            var lessons = _lessonRepo.GetAll();
            var enrollments = _progressRepo.GetAllEnrollments(50);
            var quizAttempts = _quizRepo.GetAllAttempts(50);
            var jobs = _jobRepo.GetAll();

            var viewModel = new AdminDashboardViewModel
            {
                TotalUsers = users.Count,
                TotalRoadmaps = roadmaps.Count,
                TotalLessons = lessons.Count,
                TotalEnrollments = enrollments.Count,
                TotalQuizAttempts = quizAttempts.Count,
                TotalJobs = jobs.Count,
                RecentUsers = users.Take(5).ToList(),
                RecentEnrollments = enrollments.Take(5).ToList(),
                RecentQuizAttempts = quizAttempts.Take(5).ToList()
            };

            return View(viewModel);
        }
    }
}
