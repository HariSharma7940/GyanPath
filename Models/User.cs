using System;

namespace GyanPath.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string PreferredDomain { get; set; } = "Technology";
        public string Role { get; set; } = "Member"; // 'Member' or 'Admin'
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
