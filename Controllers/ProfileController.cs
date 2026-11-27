using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GyanPath.Data;
using GyanPath.Models;
using GyanPath.Models.ViewModels;

namespace GyanPath.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserRepository _userRepo;

        public ProfileController(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public IActionResult Index()
        {
            int? userId = GetCurrentUserId();
            if (!userId.HasValue) return RedirectToAction("Login", "Account");

            User? user = _userRepo.GetById(userId.Value);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpGet]
        public IActionResult Edit()
        {
            int? userId = GetCurrentUserId();
            if (!userId.HasValue) return RedirectToAction("Login", "Account");

            User? user = _userRepo.GetById(userId.Value);
            if (user == null) return NotFound();

            var viewModel = new ProfileViewModel
            {
                UserID = user.UserID,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                PreferredDomain = user.PreferredDomain,
                Role = user.Role
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProfileViewModel model)
        {
            int? userId = GetCurrentUserId();
            if (!userId.HasValue || userId.Value != model.UserID) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User? user = _userRepo.GetById(model.UserID);
            if (user == null) return NotFound();

            // Check if email changed and is taken
            if (!user.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase))
            {
                User? existing = _userRepo.GetByEmail(model.Email);
                if (existing != null && existing.UserID != model.UserID)
                {
                    ModelState.AddModelError("Email", "Email address is already in use by another account.");
                    return View(model);
                }
            }

            user.FullName = model.FullName.Trim();
            user.Email = model.Email.Trim().ToLower();
            user.Phone = model.Phone?.Trim();
            user.PreferredDomain = model.PreferredDomain;

            _userRepo.Update(user);

            // Handle password update if provided
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                if (string.IsNullOrEmpty(model.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Please enter your current password to set a new password.");
                    return View(model);
                }

                bool isCurrentValid = PasswordHasher.VerifyPassword(model.CurrentPassword, user.PasswordHash, user.PasswordSalt);
                if (!isCurrentValid)
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                    return View(model);
                }

                PasswordHasher.HashPassword(model.NewPassword, out string newHash, out string newSalt);
                _userRepo.UpdatePassword(user.UserID, newHash, newSalt);
            }

            TempData["Success"] = "Profile details updated successfully!";
            return RedirectToAction("Index");
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
