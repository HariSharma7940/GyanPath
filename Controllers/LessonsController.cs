using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GyanPath.Data;
using GyanPath.Models;

namespace GyanPath.Controllers
{
    public class LessonsController : Controller
    {
        private readonly LessonRepository _lessonRepo;
        private readonly ProgressRepository _progressRepo;
        private readonly RoadmapRepository _roadmapRepo;
        private readonly QuizRepository _quizRepo;

        public LessonsController(LessonRepository lessonRepo, ProgressRepository progressRepo, RoadmapRepository roadmapRepo, QuizRepository quizRepo)
        {
            _lessonRepo = lessonRepo;
            _progressRepo = progressRepo;
            _roadmapRepo = roadmapRepo;
            _quizRepo = quizRepo;
        }

        public IActionResult Details(int id)
        {
            int? userId = GetCurrentUserId();
            Lesson? lesson = _lessonRepo.GetById(id, userId);
            if (lesson == null)
            {
                return NotFound();
            }

            Roadmap? roadmap = _roadmapRepo.GetById(lesson.RoadmapID);
            ViewBag.Roadmap = roadmap;

            // Get Next & Previous Lessons
            List<Lesson> siblingLessons = _lessonRepo.GetByRoadmapId(lesson.RoadmapID, userId);
            int currentIndex = siblingLessons.FindIndex(l => l.LessonID == lesson.LessonID);
            ViewBag.PrevLesson = currentIndex > 0 ? siblingLessons[currentIndex - 1] : null;
            ViewBag.NextLesson = currentIndex >= 0 && currentIndex < siblingLessons.Count - 1 ? siblingLessons[currentIndex + 1] : null;

            // Check if quiz exists for this lesson
            var questions = _quizRepo.GetQuestionsByLessonId(id);
            ViewBag.HasQuiz = questions.Count > 0;
            ViewBag.QuestionCount = questions.Count;

            return View(lesson);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Complete(int id)
        {
            int? userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            Lesson? lesson = _lessonRepo.GetById(id);
            if (lesson == null)
            {
                return NotFound();
            }

            _progressRepo.MarkLessonCompleted(userId.Value, id);
            TempData["Success"] = $"Lesson '{lesson.Title}' marked as completed!";

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
