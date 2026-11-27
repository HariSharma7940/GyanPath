using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using GyanPath.Data;
using GyanPath.Models;

namespace GyanPath.Controllers
{
    public class JobsController : Controller
    {
        private readonly JobRepository _jobRepo;

        public JobsController(JobRepository jobRepo)
        {
            _jobRepo = jobRepo;
        }

        public IActionResult Index(string? search, string? domain, string? jobType)
        {
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentDomain = domain ?? "All";
            ViewBag.CurrentJobType = jobType ?? "All";

            int? userId = GetCurrentUserId();
            string userDomain = User.FindFirst("PreferredDomain")?.Value ?? "Technology";

            List<JobOpportunity> jobs;
            if (userId.HasValue)
            {
                jobs = _jobRepo.GetRecommendationsForUser(userId.Value, userDomain);
                if (!string.IsNullOrWhiteSpace(search))
                {
                    jobs = jobs.Where(j => j.Title.Contains(search, StringComparison.OrdinalIgnoreCase) || 
                                           j.Company.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                           j.RequiredSkills.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(domain) && domain != "All")
                {
                    jobs = jobs.Where(j => j.Domain.Equals(domain, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(jobType) && jobType != "All")
                {
                    jobs = jobs.Where(j => j.JobType.Equals(jobType, StringComparison.OrdinalIgnoreCase)).ToList();
                }
            }
            else
            {
                jobs = _jobRepo.GetAll(search, domain, jobType);
            }

            return View(jobs);
        }

        public IActionResult Details(int id)
        {
            JobOpportunity? job = _jobRepo.GetById(id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
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
