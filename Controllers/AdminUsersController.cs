using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GyanPath.Data;
using GyanPath.Models;

namespace GyanPath.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : Controller
    {
        private readonly UserRepository _userRepo;

        public AdminUsersController(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public IActionResult Index()
        {
            var users = _userRepo.GetAll();
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new User());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                ModelState.AddModelError("Password", "Password must be at least 6 characters.");
            }

            if (_userRepo.GetByEmail(user.Email) != null)
            {
                ModelState.AddModelError("Email", "User with this email already exists.");
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            PasswordHasher.HashPassword(password, out string hash, out string salt);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            user.CreatedAt = DateTime.Now;

            _userRepo.Create(user);
            TempData["Success"] = $"User '{user.FullName}' created successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            User? user = _userRepo.GetById(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User user, string? newPassword)
        {
            User? existingUser = _userRepo.GetById(user.UserID);
            if (existingUser == null) return NotFound();

            if (!existingUser.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase))
            {
                User? duplicate = _userRepo.GetByEmail(user.Email);
                if (duplicate != null && duplicate.UserID != user.UserID)
                {
                    ModelState.AddModelError("Email", "Email address is already in use by another user.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            existingUser.Phone = user.Phone;
            existingUser.PreferredDomain = user.PreferredDomain;
            existingUser.Role = user.Role;

            _userRepo.Update(existingUser);

            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                PasswordHasher.HashPassword(newPassword, out string hash, out string salt);
                _userRepo.UpdatePassword(existingUser.UserID, hash, salt);
            }

            TempData["Success"] = $"User '{user.FullName}' updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            User? user = _userRepo.GetById(id);
            if (user != null)
            {
                _userRepo.Delete(id);
                TempData["Success"] = $"User '{user.FullName}' deleted successfully.";
            }
            return RedirectToAction("Index");
        }
    }
}
