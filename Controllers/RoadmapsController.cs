using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GyanPath.Data;
using GyanPath.Models;
using GyanPath.Models.ViewModels;

namespace GyanPath.Controllers
{
    public class RoadmapsController : Controller
    {
        private readonly RoadmapRepository _roadmapRepo;
        private readonly LessonRepository _lessonRepo;
        private readonly ProgressRepository _progressRepo;
        private readonly JobRepository _jobRepo;

        public RoadmapsController(RoadmapRepository roadmapRepo, LessonRepository lessonRepo, ProgressRepository progressRepo, JobRepository jobRepo)
        {
            _roadmapRepo = roadmapRepo;
            _lessonRepo = lessonRepo;
            _progressRepo = progressRepo;
            _jobRepo = jobRepo;
        }

        public IActionResult Index(string? search, string? domain, string? difficulty)
        {
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentDomain = domain ?? "All";
            ViewBag.CurrentDifficulty = difficulty ?? "All";

            List<Roadmap> roadmaps = _roadmapRepo.GetAll(search, domain, difficulty);
            return View(roadmaps);
        }

        public IActionResult Details(int id)
        {
            Roadmap? roadmap = _roadmapRepo.GetById(id);
            if (roadmap == null)
            {
                return NotFound();
            }

            int? userId = GetCurrentUserId();
            List<Lesson> lessons = _lessonRepo.GetByRoadmapId(id, userId);

            bool isEnrolled = false;
            decimal completionPercentage = 0m;
            int completedLessonsCount = 0;

            if (userId.HasValue)
            {
                isEnrolled = _progressRepo.IsEnrolled(userId.Value, id);
                completionPercentage = _progressRepo.GetRoadmapCompletionPercentage(userId.Value, id);
                completedLessonsCount = lessons.Count(l => l.IsCompletedByUser);
            }

            List<JobOpportunity> relatedJobs = _jobRepo.GetAll(null, roadmap.Domain, null).Take(4).ToList();

            var viewModel = new RoadmapDetailsViewModel
            {
                Roadmap = roadmap,
                Lessons = lessons,
                IsEnrolled = isEnrolled,
                CompletionPercentage = completionPercentage,
                CompletedLessonsCount = completedLessonsCount,
                RelatedJobs = relatedJobs
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Enroll(int id)
        {
            int? userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            Roadmap? roadmap = _roadmapRepo.GetById(id);
            if (roadmap == null)
            {
                return NotFound();
            }

            _progressRepo.Enroll(userId.Value, id);
            TempData["Success"] = $"Successfully enrolled in '{roadmap.Title}'!";
            return RedirectToAction("Details", new { id = id });
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
