using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using GyanPath.Models;

namespace GyanPath.Data
{
    public class LessonRepository
    {
        private readonly DatabaseConnection _db;

        public LessonRepository(DatabaseConnection db)
        {
            _db = db;
        }

        public List<Lesson> GetByRoadmapId(int roadmapId, int? userId = null)
        {
            List<Lesson> lessons = new List<Lesson>();
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"SELECT l.LessonID, l.RoadmapID, l.Title, l.Description, l.Content, 
                                        l.VideoUrl, l.LessonOrder, l.Resources, l.CreatedAt, r.Title AS RoadmapTitle
                                 FROM Lessons l
                                 INNER JOIN Roadmaps r ON l.RoadmapID = r.RoadmapID
                                 WHERE l.RoadmapID = @RoadmapID
                                 ORDER BY l.LessonOrder ASC, l.LessonID ASC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RoadmapID", roadmapId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Lesson lesson = MapLesson(reader);
                            lessons.Add(lesson);
                        }
                    }
                }

                if (userId.HasValue)
                {
                    foreach (var lesson in lessons)
                    {
                        CheckUserCompletionAndQuizStatus(conn, lesson, userId.Value);
                    }
                }
            }
            return lessons;
        }

        public Lesson? GetById(int lessonId, int? userId = null)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"SELECT l.LessonID, l.RoadmapID, l.Title, l.Description, l.Content, 
                                        l.VideoUrl, l.LessonOrder, l.Resources, l.CreatedAt, r.Title AS RoadmapTitle
                                 FROM Lessons l
                                 INNER JOIN Roadmaps r ON l.RoadmapID = r.RoadmapID
                                 WHERE l.LessonID = @LessonID";

                Lesson? lesson = null;
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@LessonID", lessonId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lesson = MapLesson(reader);
                        }
                    }
                }

                if (lesson != null && userId.HasValue)
                {
                    CheckUserCompletionAndQuizStatus(conn, lesson, userId.Value);
                }

                return lesson;
            }
        }

        public List<Lesson> GetAll()
        {
            List<Lesson> lessons = new List<Lesson>();
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"SELECT l.LessonID, l.RoadmapID, l.Title, l.Description, l.Content, 
                                        l.VideoUrl, l.LessonOrder, l.Resources, l.CreatedAt, r.Title AS RoadmapTitle
                                 FROM Lessons l
                                 INNER JOIN Roadmaps r ON l.RoadmapID = r.RoadmapID
                                 ORDER BY r.Title ASC, l.LessonOrder ASC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lessons.Add(MapLesson(reader));
                        }
                    }
                }
            }
            return lessons;
        }

        public int Create(Lesson lesson)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"INSERT INTO Lessons (RoadmapID, Title, Description, Content, VideoUrl, LessonOrder, Resources, CreatedAt)
                                VALUES (@RoadmapID, @Title, @Description, @Content, @VideoUrl, @LessonOrder, @Resources, GETDATE());
                                SELECT SCOPE_IDENTITY();";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RoadmapID", lesson.RoadmapID);
                    cmd.Parameters.AddWithValue("@Title", lesson.Title);
                    cmd.Parameters.AddWithValue("@Description", lesson.Description);
                    cmd.Parameters.AddWithValue("@Content", lesson.Content);
                    cmd.Parameters.AddWithValue("@VideoUrl", (object?)lesson.VideoUrl ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LessonOrder", lesson.LessonOrder);
                    cmd.Parameters.AddWithValue("@Resources", (object?)lesson.Resources ?? DBNull.Value);

                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public bool Update(Lesson lesson)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"UPDATE Lessons
                                SET RoadmapID = @RoadmapID,
                                    Title = @Title,
                                    Description = @Description,
                                    Content = @Content,
                                    VideoUrl = @VideoUrl,
                                    LessonOrder = @LessonOrder,
                                    Resources = @Resources
                                WHERE LessonID = @LessonID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RoadmapID", lesson.RoadmapID);
                    cmd.Parameters.AddWithValue("@Title", lesson.Title);
                    cmd.Parameters.AddWithValue("@Description", lesson.Description);
                    cmd.Parameters.AddWithValue("@Content", lesson.Content);
                    cmd.Parameters.AddWithValue("@VideoUrl", (object?)lesson.VideoUrl ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LessonOrder", lesson.LessonOrder);
                    cmd.Parameters.AddWithValue("@Resources", (object?)lesson.Resources ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LessonID", lesson.LessonID);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Delete(int lessonId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "DELETE FROM Lessons WHERE LessonID = @LessonID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@LessonID", lessonId);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        private void CheckUserCompletionAndQuizStatus(SqlConnection conn, Lesson lesson, int userId)
        {
            string progressQuery = "SELECT IsCompleted FROM LessonProgress WHERE UserID = @UserID AND LessonID = @LessonID";
            using (SqlCommand cmd = new SqlCommand(progressQuery, conn))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@LessonID", lesson.LessonID);
                object result = cmd.ExecuteScalar();
                lesson.IsCompletedByUser = result != null && Convert.ToBoolean(result);
            }

            string quizQuery = "SELECT TOP 1 ScorePercentage, Passed FROM QuizAttempts WHERE UserID = @UserID AND LessonID = @LessonID ORDER BY AttemptedAt DESC";
            using (SqlCommand cmd = new SqlCommand(quizQuery, conn))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@LessonID", lesson.LessonID);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        lesson.LastQuizScore = Convert.ToDecimal(reader["ScorePercentage"]);
                        lesson.UserPassedQuiz = Convert.ToBoolean(reader["Passed"]);
                    }
                }
            }
        }

        private Lesson MapLesson(SqlDataReader reader)
        {
            return new Lesson
            {
                LessonID = Convert.ToInt32(reader["LessonID"]),
                RoadmapID = Convert.ToInt32(reader["RoadmapID"]),
                Title = reader["Title"].ToString() ?? string.Empty,
                Description = reader["Description"].ToString() ?? string.Empty,
                Content = reader["Content"].ToString() ?? string.Empty,
                VideoUrl = reader["VideoUrl"] != DBNull.Value ? reader["VideoUrl"].ToString() : null,
                LessonOrder = Convert.ToInt32(reader["LessonOrder"]),
                Resources = reader["Resources"] != DBNull.Value ? reader["Resources"].ToString() : null,
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                RoadmapTitle = reader["RoadmapTitle"].ToString() ?? string.Empty
            };
        }
    }
}
