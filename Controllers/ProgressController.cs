using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GyanPath.Data;

namespace GyanPath.Controllers
{
    [Authorize]
    public class ProgressController : Controller
    {
        private readonly ProgressRepository _progressRepo;
        private readonly QuizRepository _quizRepo;

        public ProgressController(ProgressRepository progressRepo, QuizRepository quizRepo)
        {
            _progressRepo = progressRepo;
            _quizRepo = quizRepo;
        }

        public IActionResult Index()
        {
            int? userId = GetCurrentUserId();
            if (!userId.HasValue) return RedirectToAction("Login", "Account");

            var enrollments = _progressRepo.GetUserEnrollments(userId.Value);
            var quizAttempts = _quizRepo.GetUserAttempts(userId.Value);

            ViewBag.Enrollments = enrollments;
            ViewBag.QuizAttempts = quizAttempts;

            return View();
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
