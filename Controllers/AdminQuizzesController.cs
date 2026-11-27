using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GyanPath.Data;
using GyanPath.Models;

namespace GyanPath.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminQuizzesController : Controller
    {
        private readonly QuizRepository _quizRepo;
        private readonly LessonRepository _lessonRepo;

        public AdminQuizzesController(QuizRepository quizRepo, LessonRepository lessonRepo)
        {
            _quizRepo = quizRepo;
            _lessonRepo = lessonRepo;
        }

        public IActionResult Index()
        {
            var questions = _quizRepo.GetAllQuestions();
            return View(questions);
        }

        [HttpGet]
        public IActionResult Create(int? lessonId)
        {
            PopulateLessonsDropDownList(lessonId);
            Question question = new Question
            {
                LessonID = lessonId ?? 0,
                Options = new List<QuizOption>
                {
                    new QuizOption { OptionText = "", IsCorrect = true },
                    new QuizOption { OptionText = "", IsCorrect = false },
                    new QuizOption { OptionText = "", IsCorrect = false },
                    new QuizOption { OptionText = "", IsCorrect = false }
                }
            };
            return View(question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Question question, string[] optionTexts, int correctOptionIndex)
        {
            if (string.IsNullOrWhiteSpace(question.QuestionText))
            {
                ModelState.AddModelError("QuestionText", "Question Text is required.");
            }
            if (question.LessonID <= 0)
            {
                ModelState.AddModelError("LessonID", "Please select a valid lesson.");
            }

            question.Options = new List<QuizOption>();
            for (int i = 0; i < optionTexts.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(optionTexts[i]))
                {
                    question.Options.Add(new QuizOption
                    {
                        OptionText = optionTexts[i].Trim(),
                        IsCorrect = (i == correctOptionIndex)
                    });
                }
            }

            if (question.Options.Count < 2)
            {
                ModelState.AddModelError("", "Please provide at least 2 option choices.");
            }

            if (!ModelState.IsValid)
            {
                PopulateLessonsDropDownList(question.LessonID);
                return View(question);
            }

            _quizRepo.CreateQuestion(question);
            TempData["Success"] = "Quiz question created successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Question? question = _quizRepo.GetQuestionById(id);
            if (question == null) return NotFound();
            PopulateLessonsDropDownList(question.LessonID);
            return View(question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Question question, string[] optionTexts, int correctOptionIndex)
        {
            if (string.IsNullOrWhiteSpace(question.QuestionText))
            {
                ModelState.AddModelError("QuestionText", "Question Text is required.");
            }

            question.Options = new List<QuizOption>();
            for (int i = 0; i < optionTexts.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(optionTexts[i]))
                {
                    question.Options.Add(new QuizOption
                    {
                        QuestionID = question.QuestionID,
                        OptionText = optionTexts[i].Trim(),
                        IsCorrect = (i == correctOptionIndex)
                    });
                }
            }

            if (!ModelState.IsValid)
            {
                PopulateLessonsDropDownList(question.LessonID);
                return View(question);
            }

            _quizRepo.UpdateQuestion(question);
            TempData["Success"] = "Quiz question updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            Question? q = _quizRepo.GetQuestionById(id);
            if (q != null)
            {
                _quizRepo.DeleteQuestion(id);
                TempData["Success"] = "Quiz question deleted successfully.";
            }
            return RedirectToAction("Index");
        }

        private void PopulateLessonsDropDownList(object? selectedLesson = null)
        {
            var lessons = _lessonRepo.GetAll();
            ViewBag.LessonID = new SelectList(lessons, "LessonID", "Title", selectedLesson);
        }
    }
}
