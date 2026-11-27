using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GyanPath.Data;
using GyanPath.Models;

namespace GyanPath.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminLessonsController : Controller
    {
        private readonly LessonRepository _lessonRepo;
        private readonly RoadmapRepository _roadmapRepo;

        public AdminLessonsController(LessonRepository lessonRepo, RoadmapRepository roadmapRepo)
        {
            _lessonRepo = lessonRepo;
            _roadmapRepo = roadmapRepo;
        }

        public IActionResult Index()
        {
            var lessons = _lessonRepo.GetAll();
            return View(lessons);
        }

        [HttpGet]
        public IActionResult Create(int? roadmapId)
        {
            PopulateRoadmapsDropDownList(roadmapId);
            return View(new Lesson { RoadmapID = roadmapId ?? 0, LessonOrder = 1 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Lesson lesson)
        {
            if (!ModelState.IsValid)
            {
                PopulateRoadmapsDropDownList(lesson.RoadmapID);
                return View(lesson);
            }

            _lessonRepo.Create(lesson);
            TempData["Success"] = $"Lesson '{lesson.Title}' created successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Lesson? lesson = _lessonRepo.GetById(id);
            if (lesson == null) return NotFound();
            PopulateRoadmapsDropDownList(lesson.RoadmapID);
            return View(lesson);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Lesson lesson)
        {
            if (!ModelState.IsValid)
            {
                PopulateRoadmapsDropDownList(lesson.RoadmapID);
                return View(lesson);
            }

            _lessonRepo.Update(lesson);
            TempData["Success"] = $"Lesson '{lesson.Title}' updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            Lesson? lesson = _lessonRepo.GetById(id);
            if (lesson != null)
            {
                _lessonRepo.Delete(id);
                TempData["Success"] = $"Lesson '{lesson.Title}' deleted successfully.";
            }
            return RedirectToAction("Index");
        }

        private void PopulateRoadmapsDropDownList(object? selectedRoadmap = null)
        {
            var roadmaps = _roadmapRepo.GetAll();
            ViewBag.RoadmapID = new SelectList(roadmaps, "RoadmapID", "Title", selectedRoadmap);
        }
    }
}
