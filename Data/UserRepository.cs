using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using GyanPath.Models;

namespace GyanPath.Data
{
    public class UserRepository
    {
        private readonly DatabaseConnection _db;

        public UserRepository(DatabaseConnection db)
        {
            _db = db;
        }

        public User? GetByEmail(string email)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "SELECT UserID, FullName, Email, PasswordHash, PasswordSalt, Phone, PreferredDomain, Role, CreatedAt FROM Users WHERE Email = @Email";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email.Trim().ToLower());
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapUser(reader);
                        }
                    }
                }
            }
            return null;
        }

        public User? GetById(int userId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "SELECT UserID, FullName, Email, PasswordHash, PasswordSalt, Phone, PreferredDomain, Role, CreatedAt FROM Users WHERE UserID = @UserID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapUser(reader);
                        }
                    }
                }
            }
            return null;
        }

        public List<User> GetAll()
        {
            List<User> users = new List<User>();
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "SELECT UserID, FullName, Email, PasswordHash, PasswordSalt, Phone, PreferredDomain, Role, CreatedAt FROM Users ORDER BY CreatedAt DESC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(MapUser(reader));
                        }
                    }
                }
            }
            return users;
        }

        public int Create(User user)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"INSERT INTO Users (FullName, Email, PasswordHash, PasswordSalt, Phone, PreferredDomain, Role, CreatedAt)
                                VALUES (@FullName, @Email, @PasswordHash, @PasswordSalt, @Phone, @PreferredDomain, @Role, GETDATE());
                                SELECT SCOPE_IDENTITY();";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FullName", user.FullName);
                    cmd.Parameters.AddWithValue("@Email", user.Email.Trim().ToLower());
                    cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    cmd.Parameters.AddWithValue("@PasswordSalt", user.PasswordSalt);
                    cmd.Parameters.AddWithValue("@Phone", (object?)user.Phone ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PreferredDomain", user.PreferredDomain);
                    cmd.Parameters.AddWithValue("@Role", user.Role);

                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        public bool Update(User user)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"UPDATE Users 
                                SET FullName = @FullName, 
                                    Email = @Email, 
                                    Phone = @Phone, 
                                    PreferredDomain = @PreferredDomain, 
                                    Role = @Role 
                                WHERE UserID = @UserID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FullName", user.FullName);
                    cmd.Parameters.AddWithValue("@Email", user.Email.Trim().ToLower());
                    cmd.Parameters.AddWithValue("@Phone", (object?)user.Phone ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PreferredDomain", user.PreferredDomain);
                    cmd.Parameters.AddWithValue("@Role", user.Role);
                    cmd.Parameters.AddWithValue("@UserID", user.UserID);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdatePassword(int userId, string passwordHash, string passwordSalt)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "UPDATE Users SET PasswordHash = @Hash, PasswordSalt = @Salt WHERE UserID = @UserID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Hash", passwordHash);
                    cmd.Parameters.AddWithValue("@Salt", passwordSalt);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Delete(int userId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "DELETE FROM Users WHERE UserID = @UserID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        private User MapUser(SqlDataReader reader)
        {
            return new User
            {
                UserID = Convert.ToInt32(reader["UserID"]),
                FullName = reader["FullName"].ToString() ?? string.Empty,
                Email = reader["Email"].ToString() ?? string.Empty,
                PasswordHash = reader["PasswordHash"].ToString() ?? string.Empty,
                PasswordSalt = reader["PasswordSalt"].ToString() ?? string.Empty,
                Phone = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : null,
                PreferredDomain = reader["PreferredDomain"].ToString() ?? "Technology",
                Role = reader["Role"].ToString() ?? "Member",
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
            };
        }
    }
}
