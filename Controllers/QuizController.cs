using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GyanPath.Data;
using GyanPath.Models;
using GyanPath.Models.ViewModels;

namespace GyanPath.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private readonly QuizRepository _quizRepo;
        private readonly LessonRepository _lessonRepo;
        private readonly ProgressRepository _progressRepo;
        private readonly RoadmapRepository _roadmapRepo;

        public QuizController(QuizRepository quizRepo, LessonRepository lessonRepo, ProgressRepository progressRepo, RoadmapRepository roadmapRepo)
        {
            _quizRepo = quizRepo;
            _lessonRepo = lessonRepo;
            _progressRepo = progressRepo;
            _roadmapRepo = roadmapRepo;
        }

        [HttpGet]
        public IActionResult Start(int id)
        {
            int? userId = GetCurrentUserId();
            Lesson? lesson = _lessonRepo.GetById(id, userId);
            if (lesson == null)
            {
                return NotFound();
            }

            List<Question> questions = _quizRepo.GetQuestionsByLessonId(id);
            if (questions.Count == 0)
            {
                TempData["Error"] = "No quiz questions available for this lesson yet.";
                return RedirectToAction("Details", "Lessons", new { id = id });
            }

            lesson.Questions = questions;
            ViewBag.Roadmap = _roadmapRepo.GetById(lesson.RoadmapID);

            return View(lesson);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit(IFormCollection form)
        {
            int? userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!int.TryParse(form["LessonID"], out int lessonId))
            {
                return BadRequest("Invalid Lesson ID");
            }

            Lesson? lesson = _lessonRepo.GetById(lessonId);
            if (lesson == null)
            {
                return NotFound();
            }

            List<Question> questions = _quizRepo.GetQuestionsByLessonId(lessonId);
            if (questions.Count == 0)
            {
                return BadRequest("No questions found for this quiz.");
            }

            int correctAnswers = 0;
            List<QuestionReviewItem> reviewItems = new List<QuestionReviewItem>();

            foreach (var q in questions)
            {
                string selectedStr = form[$"question_{q.QuestionID}"].ToString();
                int.TryParse(selectedStr, out int selectedOptionId);

                var correctOption = q.Options.FirstOrDefault(o => o.IsCorrect);
                var selectedOption = q.Options.FirstOrDefault(o => o.OptionID == selectedOptionId);

                bool isCorrect = selectedOption != null && selectedOption.IsCorrect;
                if (isCorrect)
                {
                    correctAnswers++;
                }

                reviewItems.Add(new QuestionReviewItem
                {
                    QuestionID = q.QuestionID,
                    QuestionText = q.QuestionText,
                    Explanation = q.Explanation ?? "No explanation provided.",
                    SelectedOptionText = selectedOption != null ? selectedOption.OptionText : "Not Answered",
                    CorrectOptionText = correctOption != null ? correctOption.OptionText : "N/A",
                    IsCorrect = isCorrect
                });
            }

            int totalQuestions = questions.Count;
            decimal scorePercentage = Math.Round((correctAnswers * 100.0m) / totalQuestions, 2);
            bool passed = scorePercentage >= 60.0m;

            // Save Quiz Attempt to SQL Server via ADO.NET
            QuizAttempt attempt = new QuizAttempt
            {
                UserID = userId.Value,
                LessonID = lessonId,
                ScorePercentage = scorePercentage,
                Passed = passed,
                TotalQuestions = totalQuestions,
                CorrectAnswers = correctAnswers,
                AttemptedAt = DateTime.Now
            };

            int attemptId = _quizRepo.SaveQuizAttempt(attempt);

            // If user passed, mark lesson as completed!
            if (passed)
            {
                _progressRepo.MarkLessonCompleted(userId.Value, lessonId);
            }

            TempData["Success"] = passed ? "Congratulations! You passed the quiz!" : "Quiz completed. Review your answers below.";

            // Pass review data via TempData / Redirect
            HttpContext.Session.SetString($"QuizReview_{attemptId}", System.Text.Json.JsonSerializer.Serialize(reviewItems));

            return RedirectToAction("Result", new { id = attemptId, lessonId = lessonId, score = scorePercentage, passed = passed, correct = correctAnswers, total = totalQuestions });
        }

        [HttpGet]
        public IActionResult Result(int id, int lessonId, decimal score, bool passed, int correct, int total)
        {
            Lesson? lesson = _lessonRepo.GetById(lessonId);
            if (lesson == null)
            {
                return NotFound();
            }

            Roadmap? roadmap = _roadmapRepo.GetById(lesson.RoadmapID);

            List<QuestionReviewItem> reviews = new List<QuestionReviewItem>();
            string? reviewJson = HttpContext.Session.GetString($"QuizReview_{id}");
            if (!string.IsNullOrEmpty(reviewJson))
            {
                reviews = System.Text.Json.JsonSerializer.Deserialize<List<QuestionReviewItem>>(reviewJson) ?? new List<QuestionReviewItem>();
            }

            var viewModel = new QuizResultViewModel
            {
                LessonID = lessonId,
                LessonTitle = lesson.Title,
                RoadmapID = lesson.RoadmapID,
                RoadmapTitle = roadmap?.Title ?? "Roadmap",
                TotalQuestions = total,
                CorrectAnswers = correct,
                ScorePercentage = score,
                Passed = passed,
                Reviews = reviews
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
