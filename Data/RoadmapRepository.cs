using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using GyanPath.Models;

namespace GyanPath.Data
{
    public class RoadmapRepository
    {
        private readonly DatabaseConnection _db;

        public RoadmapRepository(DatabaseConnection db)
        {
            _db = db;
        }

        public List<Roadmap> GetAll(string? search = null, string? domain = null, string? difficulty = null)
        {
            List<Roadmap> roadmaps = new List<Roadmap>();
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"SELECT r.RoadmapID, r.Title, r.Domain, r.Description, r.Difficulty, 
                                        r.EstimatedDuration, r.ThumbnailUrl, r.LearningObjectives, r.CreatedAt,
                                        (SELECT COUNT(*) FROM Lessons l WHERE l.RoadmapID = r.RoadmapID) AS LessonCount
                                 FROM Roadmaps r
                                 WHERE 1=1";

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query += " AND (r.Title LIKE @Search OR r.Description LIKE @Search)";
                }
                if (!string.IsNullOrWhiteSpace(domain) && !string.Equals(domain.Trim(), "All", StringComparison.OrdinalIgnoreCase))
                {
                    query += " AND r.Domain = @Domain";
                }
                if (!string.IsNullOrWhiteSpace(difficulty) && !string.Equals(difficulty.Trim(), "All", StringComparison.OrdinalIgnoreCase))
                {
                    query += " AND r.Difficulty = @Difficulty";
                }

                query += " ORDER BY r.CreatedAt DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        cmd.Parameters.AddWithValue("@Search", $"%{search.Trim()}%");
                    }
                    if (!string.IsNullOrWhiteSpace(domain) && !string.Equals(domain.Trim(), "All", StringComparison.OrdinalIgnoreCase))
                    {
                        cmd.Parameters.AddWithValue("@Domain", domain.Trim());
                    }
                    if (!string.IsNullOrWhiteSpace(difficulty) && !string.Equals(difficulty.Trim(), "All", StringComparison.OrdinalIgnoreCase))
                    {
                        cmd.Parameters.AddWithValue("@Difficulty", difficulty.Trim());
                    }

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            roadmaps.Add(MapRoadmap(reader));
                        }
                    }
                }
            }
            return roadmaps;
        }

        public Roadmap? GetById(int roadmapId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"SELECT r.RoadmapID, r.Title, r.Domain, r.Description, r.Difficulty, 
                                        r.EstimatedDuration, r.ThumbnailUrl, r.LearningObjectives, r.CreatedAt,
                                        (SELECT COUNT(*) FROM Lessons l WHERE l.RoadmapID = r.RoadmapID) AS LessonCount
                                 FROM Roadmaps r
                                 WHERE r.RoadmapID = @RoadmapID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RoadmapID", roadmapId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapRoadmap(reader);
                        }
                    }
                }
            }
            return null;
        }

        public List<Roadmap> GetByDomain(string domain)
        {
            return GetAll(null, domain, null);
        }

        public int Create(Roadmap roadmap)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"INSERT INTO Roadmaps (Title, Domain, Description, Difficulty, EstimatedDuration, ThumbnailUrl, LearningObjectives, CreatedAt)
                                VALUES (@Title, @Domain, @Description, @Difficulty, @EstimatedDuration, @ThumbnailUrl, @LearningObjectives, GETDATE());
                                SELECT SCOPE_IDENTITY();";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", roadmap.Title);
                    cmd.Parameters.AddWithValue("@Domain", roadmap.Domain);
                    cmd.Parameters.AddWithValue("@Description", roadmap.Description);
                    cmd.Parameters.AddWithValue("@Difficulty", roadmap.Difficulty);
                    cmd.Parameters.AddWithValue("@EstimatedDuration", roadmap.EstimatedDuration);
                    cmd.Parameters.AddWithValue("@ThumbnailUrl", (object?)roadmap.ThumbnailUrl ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LearningObjectives", (object?)roadmap.LearningObjectives ?? DBNull.Value);

                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public bool Update(Roadmap roadmap)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"UPDATE Roadmaps
                                SET Title = @Title,
                                    Domain = @Domain,
                                    Description = @Description,
                                    Difficulty = @Difficulty,
                                    EstimatedDuration = @EstimatedDuration,
                                    ThumbnailUrl = @ThumbnailUrl,
                                    LearningObjectives = @LearningObjectives
                                WHERE RoadmapID = @RoadmapID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", roadmap.Title);
                    cmd.Parameters.AddWithValue("@Domain", roadmap.Domain);
                    cmd.Parameters.AddWithValue("@Description", roadmap.Description);
                    cmd.Parameters.AddWithValue("@Difficulty", roadmap.Difficulty);
                    cmd.Parameters.AddWithValue("@EstimatedDuration", roadmap.EstimatedDuration);
                    cmd.Parameters.AddWithValue("@ThumbnailUrl", (object?)roadmap.ThumbnailUrl ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LearningObjectives", (object?)roadmap.LearningObjectives ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@RoadmapID", roadmap.RoadmapID);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Delete(int roadmapId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "DELETE FROM Roadmaps WHERE RoadmapID = @RoadmapID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RoadmapID", roadmapId);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        private Roadmap MapRoadmap(SqlDataReader reader)
        {
            return new Roadmap
            {
                RoadmapID = Convert.ToInt32(reader["RoadmapID"]),
                Title = reader["Title"].ToString() ?? string.Empty,
                Domain = reader["Domain"].ToString() ?? "Technology",
                Description = reader["Description"].ToString() ?? string.Empty,
                Difficulty = reader["Difficulty"].ToString() ?? "Beginner",
                EstimatedDuration = reader["EstimatedDuration"].ToString() ?? string.Empty,
                ThumbnailUrl = reader["ThumbnailUrl"] != DBNull.Value ? reader["ThumbnailUrl"].ToString() : null,
                LearningObjectives = reader["LearningObjectives"] != DBNull.Value ? reader["LearningObjectives"].ToString() : null,
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                LessonCount = reader["LessonCount"] != DBNull.Value ? Convert.ToInt32(reader["LessonCount"]) : 0
            };
        }
    }
}
