using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GyanPath.Data;
using GyanPath.Models;

namespace GyanPath.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminJobsController : Controller
    {
        private readonly JobRepository _jobRepo;

        public AdminJobsController(JobRepository jobRepo)
        {
            _jobRepo = jobRepo;
        }

        public IActionResult Index()
        {
            var jobs = _jobRepo.GetAll();
            return View(jobs);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new JobOpportunity());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(JobOpportunity job)
        {
            if (!ModelState.IsValid)
            {
                return View(job);
            }

            _jobRepo.Create(job);
            TempData["Success"] = $"Job Opportunity '{job.Title}' created successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            JobOpportunity? job = _jobRepo.GetById(id);
            if (job == null) return NotFound();
            return View(job);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(JobOpportunity job)
        {
            if (!ModelState.IsValid)
            {
                return View(job);
            }

            _jobRepo.Update(job);
            TempData["Success"] = $"Job Opportunity '{job.Title}' updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            JobOpportunity? job = _jobRepo.GetById(id);
            if (job != null)
            {
                _jobRepo.Delete(id);
                TempData["Success"] = $"Job Opportunity '{job.Title}' deleted successfully.";
            }
            return RedirectToAction("Index");
        }
    }
}
