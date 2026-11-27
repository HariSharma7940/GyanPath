using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GyanPath.Data;
using GyanPath.Models;

namespace GyanPath.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminRoadmapsController : Controller
    {
        private readonly RoadmapRepository _roadmapRepo;

        public AdminRoadmapsController(RoadmapRepository roadmapRepo)
        {
            _roadmapRepo = roadmapRepo;
        }

        public IActionResult Index()
        {
            var roadmaps = _roadmapRepo.GetAll();
            return View(roadmaps);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Roadmap());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Roadmap roadmap)
        {
            if (!ModelState.IsValid)
            {
                return View(roadmap);
            }

            _roadmapRepo.Create(roadmap);
            TempData["Success"] = $"Roadmap '{roadmap.Title}' created successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Roadmap? roadmap = _roadmapRepo.GetById(id);
            if (roadmap == null) return NotFound();
            return View(roadmap);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Roadmap roadmap)
        {
            if (!ModelState.IsValid)
            {
                return View(roadmap);
            }

            _roadmapRepo.Update(roadmap);
            TempData["Success"] = $"Roadmap '{roadmap.Title}' updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            Roadmap? roadmap = _roadmapRepo.GetById(id);
            if (roadmap != null)
            {
                _roadmapRepo.Delete(id);
                TempData["Success"] = $"Roadmap '{roadmap.Title}' deleted successfully.";
            }
            return RedirectToAction("Index");
        }
    }
}
