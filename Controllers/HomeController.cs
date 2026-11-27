using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GyanPath.Data;
using GyanPath.Models;

namespace GyanPath.Controllers
{
    public class HomeController : Controller
    {
        private readonly RoadmapRepository _roadmapRepo;
        private readonly JobRepository _jobRepo;

        public HomeController(RoadmapRepository roadmapRepo, JobRepository jobRepo)
        {
            _roadmapRepo = roadmapRepo;
            _jobRepo = jobRepo;
        }

        public IActionResult Index()
        {
            ViewBag.FeaturedRoadmaps = _roadmapRepo.GetAll(null, null, null).Take(3).ToList();
            ViewBag.FeaturedJobs = _jobRepo.GetAll().Take(3).ToList();
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
