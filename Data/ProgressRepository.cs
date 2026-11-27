using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using GyanPath.Models;

namespace GyanPath.Data
{
    public class ProgressRepository
    {
        private readonly DatabaseConnection _db;

        public ProgressRepository(DatabaseConnection db)
        {
            _db = db;
        }

        public bool Enroll(int userId, int roadmapId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"IF NOT EXISTS (SELECT 1 FROM Enrollments WHERE UserID = @UserID AND RoadmapID = @RoadmapID)
                                 BEGIN
                                     INSERT INTO Enrollments (UserID, RoadmapID, EnrolledAt, IsCompleted)
                                     VALUES (@UserID, @RoadmapID, GETDATE(), 0);
                                 END";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@RoadmapID", roadmapId);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool IsEnrolled(int userId, int roadmapId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "SELECT COUNT(*) FROM Enrollments WHERE UserID = @UserID AND RoadmapID = @RoadmapID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@RoadmapID", roadmapId);
                    conn.Open();
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }

        public List<Enrollment> GetUserEnrollments(int userId)
        {
            List<Enrollment> enrollments = new List<Enrollment>();
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"SELECT e.EnrollmentID, e.UserID, e.RoadmapID, e.EnrolledAt, e.IsCompleted, e.CompletedAt,
                                        r.Title AS RoadmapTitle, r.Domain AS RoadmapDomain, r.ThumbnailUrl,
                                        u.FullName AS UserFullName, u.Email AS UserEmail
                                 FROM Enrollments e
                                 INNER JOIN Roadmaps r ON e.RoadmapID = r.RoadmapID
                                 INNER JOIN Users u ON e.UserID = u.UserID
                                 WHERE e.UserID = @UserID
                                 ORDER BY e.EnrolledAt DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            enrollments.Add(MapEnrollment(reader));
                        }
                    }
                }

                foreach (var e in enrollments)
                {
                    e.ProgressPercentage = GetRoadmapCompletionPercentage(conn, userId, e.RoadmapID);
                }
            }
            return enrollments;
        }

        public List<Enrollment> GetAllEnrollments(int limit = 100)
        {
            List<Enrollment> enrollments = new List<Enrollment>();
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = $@"SELECT TOP ({limit}) e.EnrollmentID, e.UserID, e.RoadmapID, e.EnrolledAt, e.IsCompleted, e.CompletedAt,
                                        r.Title AS RoadmapTitle, r.Domain AS RoadmapDomain, r.ThumbnailUrl,
                                        u.FullName AS UserFullName, u.Email AS UserEmail
                                 FROM Enrollments e
                                 INNER JOIN Roadmaps r ON e.RoadmapID = r.RoadmapID
                                 INNER JOIN Users u ON e.UserID = u.UserID
                                 ORDER BY e.EnrolledAt DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            enrollments.Add(MapEnrollment(reader));
                        }
                    }
                }

                foreach (var e in enrollments)
                {
                    e.ProgressPercentage = GetRoadmapCompletionPercentage(conn, e.UserID, e.RoadmapID);
                }
            }
            return enrollments;
        }

        public bool MarkLessonCompleted(int userId, int lessonId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                string query = @"IF EXISTS (SELECT 1 FROM LessonProgress WHERE UserID = @UserID AND LessonID = @LessonID)
                                 BEGIN
                                     UPDATE LessonProgress SET IsCompleted = 1, CompletedAt = GETDATE()
                                     WHERE UserID = @UserID AND LessonID = @LessonID;
                                 END
                                 ELSE
                                 BEGIN
                                     INSERT INTO LessonProgress (UserID, LessonID, IsCompleted, CompletedAt)
                                     VALUES (@UserID, @LessonID, 1, GETDATE());
                                 END";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@LessonID", lessonId);
                    cmd.ExecuteNonQuery();
                }

                // Check if roadmap is now 100% complete
                string roadmapIdQuery = "SELECT RoadmapID FROM Lessons WHERE LessonID = @LessonID";
                int roadmapId = 0;
                using (SqlCommand cmd = new SqlCommand(roadmapIdQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@LessonID", lessonId);
                    object rId = cmd.ExecuteScalar();
                    if (rId != null) roadmapId = Convert.ToInt32(rId);
                }

                if (roadmapId > 0)
                {
                    decimal pct = GetRoadmapCompletionPercentage(conn, userId, roadmapId);
                    if (pct >= 100.00m)
                    {
                        string markRoadmapQuery = @"UPDATE Enrollments 
                                                    SET IsCompleted = 1, CompletedAt = GETDATE() 
                                                    WHERE UserID = @UserID AND RoadmapID = @RoadmapID";
                        using (SqlCommand cmd = new SqlCommand(markRoadmapQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@UserID", userId);
                            cmd.Parameters.AddWithValue("@RoadmapID", roadmapId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                return true;
            }
        }

        public decimal GetRoadmapCompletionPercentage(int userId, int roadmapId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                return GetRoadmapCompletionPercentage(conn, userId, roadmapId);
            }
        }

        private decimal GetRoadmapCompletionPercentage(SqlConnection conn, int userId, int roadmapId)
        {
            string totalLessonsQuery = "SELECT COUNT(*) FROM Lessons WHERE RoadmapID = @RoadmapID";
            int totalLessons = 0;
            using (SqlCommand cmd = new SqlCommand(totalLessonsQuery, conn))
            {
                cmd.Parameters.AddWithValue("@RoadmapID", roadmapId);
                totalLessons = (int)cmd.ExecuteScalar();
            }

            if (totalLessons == 0) return 0m;

            string completedLessonsQuery = @"SELECT COUNT(DISTINCT lp.LessonID)
                                            FROM LessonProgress lp
                                            INNER JOIN Lessons l ON lp.LessonID = l.LessonID
                                            WHERE lp.UserID = @UserID AND l.RoadmapID = @RoadmapID AND lp.IsCompleted = 1";
            int completedLessons = 0;
            using (SqlCommand cmd = new SqlCommand(completedLessonsQuery, conn))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@RoadmapID", roadmapId);
                completedLessons = (int)cmd.ExecuteScalar();
            }

            return Math.Round((completedLessons * 100.0m) / totalLessons, 2);
        }

        public int GetTotalCompletedLessonsCount(int userId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "SELECT COUNT(*) FROM LessonProgress WHERE UserID = @UserID AND IsCompleted = 1";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    conn.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        public int GetTotalCompletedRoadmapsCount(int userId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "SELECT COUNT(*) FROM Enrollments WHERE UserID = @UserID AND IsCompleted = 1";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    conn.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        private Enrollment MapEnrollment(SqlDataReader reader)
        {
            return new Enrollment
            {
                EnrollmentID = Convert.ToInt32(reader["EnrollmentID"]),
                UserID = Convert.ToInt32(reader["UserID"]),
                RoadmapID = Convert.ToInt32(reader["RoadmapID"]),
                EnrolledAt = Convert.ToDateTime(reader["EnrolledAt"]),
                IsCompleted = Convert.ToBoolean(reader["IsCompleted"]),
                CompletedAt = reader["CompletedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CompletedAt"]) : null,
                RoadmapTitle = reader["RoadmapTitle"].ToString() ?? string.Empty,
                RoadmapDomain = reader["RoadmapDomain"].ToString() ?? string.Empty,
                ThumbnailUrl = reader["ThumbnailUrl"] != DBNull.Value ? reader["ThumbnailUrl"].ToString() : null,
                UserFullName = reader["UserFullName"].ToString() ?? string.Empty,
                UserEmail = reader["UserEmail"].ToString() ?? string.Empty
            };
        }
    }
}
