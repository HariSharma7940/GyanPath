using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using GyanPath.Models;

namespace GyanPath.Data
{
    public class QuizRepository
    {
        private readonly DatabaseConnection _db;

        public QuizRepository(DatabaseConnection db)
        {
            _db = db;
        }

        public List<Question> GetQuestionsByLessonId(int lessonId)
        {
            List<Question> questions = new List<Question>();
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"SELECT q.QuestionID, q.LessonID, q.QuestionText, q.Explanation, q.CreatedAt, l.Title AS LessonTitle
                                 FROM Questions q
                                 INNER JOIN Lessons l ON q.LessonID = l.LessonID
                                 WHERE q.LessonID = @LessonID
                                 ORDER BY q.QuestionID ASC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@LessonID", lessonId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            questions.Add(MapQuestion(reader));
                        }
                    }
                }

                foreach (var q in questions)
                {
                    q.Options = GetOptionsForQuestion(conn, q.QuestionID);
                }
            }
            return questions;
        }

        public Question? GetQuestionById(int questionId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"SELECT q.QuestionID, q.LessonID, q.QuestionText, q.Explanation, q.CreatedAt, l.Title AS LessonTitle
                                 FROM Questions q
                                 INNER JOIN Lessons l ON q.LessonID = l.LessonID
                                 WHERE q.QuestionID = @QuestionID";
                Question? question = null;
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@QuestionID", questionId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            question = MapQuestion(reader);
                        }
                    }
                }

                if (question != null)
                {
                    question.Options = GetOptionsForQuestion(conn, question.QuestionID);
                }

                return question;
            }
        }

        public List<Question> GetAllQuestions()
        {
            List<Question> questions = new List<Question>();
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"SELECT q.QuestionID, q.LessonID, q.QuestionText, q.Explanation, q.CreatedAt, l.Title AS LessonTitle
                                 FROM Questions q
                                 INNER JOIN Lessons l ON q.LessonID = l.LessonID
                                 ORDER BY l.Title ASC, q.QuestionID ASC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            questions.Add(MapQuestion(reader));
                        }
                    }
                }

                foreach (var q in questions)
                {
                    q.Options = GetOptionsForQuestion(conn, q.QuestionID);
                }
            }
            return questions;
        }

        public int CreateQuestion(Question question)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string qQuery = @"INSERT INTO Questions (LessonID, QuestionText, Explanation, CreatedAt)
                                         VALUES (@LessonID, @QuestionText, @Explanation, GETDATE());
                                         SELECT SCOPE_IDENTITY();";
                        int questionId;
                        using (SqlCommand cmd = new SqlCommand(qQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@LessonID", question.LessonID);
                            cmd.Parameters.AddWithValue("@QuestionText", question.QuestionText);
                            cmd.Parameters.AddWithValue("@Explanation", (object?)question.Explanation ?? DBNull.Value);
                            questionId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        foreach (var opt in question.Options)
                        {
                            string oQuery = @"INSERT INTO QuizOptions (QuestionID, OptionText, IsCorrect)
                                             VALUES (@QuestionID, @OptionText, @IsCorrect);";
                            using (SqlCommand oCmd = new SqlCommand(oQuery, conn, transaction))
                            {
                                oCmd.Parameters.AddWithValue("@QuestionID", questionId);
                                oCmd.Parameters.AddWithValue("@OptionText", opt.OptionText);
                                oCmd.Parameters.AddWithValue("@IsCorrect", opt.IsCorrect);
                                oCmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return questionId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public bool UpdateQuestion(Question question)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string qQuery = @"UPDATE Questions
                                         SET LessonID = @LessonID,
                                             QuestionText = @QuestionText,
                                             Explanation = @Explanation
                                         WHERE QuestionID = @QuestionID";
                        using (SqlCommand cmd = new SqlCommand(qQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@LessonID", question.LessonID);
                            cmd.Parameters.AddWithValue("@QuestionText", question.QuestionText);
                            cmd.Parameters.AddWithValue("@Explanation", (object?)question.Explanation ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@QuestionID", question.QuestionID);
                            cmd.ExecuteNonQuery();
                        }

                        // Replace options
                        string delQuery = "DELETE FROM QuizOptions WHERE QuestionID = @QuestionID";
                        using (SqlCommand delCmd = new SqlCommand(delQuery, conn, transaction))
                        {
                            delCmd.Parameters.AddWithValue("@QuestionID", question.QuestionID);
                            delCmd.ExecuteNonQuery();
                        }

                        foreach (var opt in question.Options)
                        {
                            string oQuery = @"INSERT INTO QuizOptions (QuestionID, OptionText, IsCorrect)
                                             VALUES (@QuestionID, @OptionText, @IsCorrect);";
                            using (SqlCommand oCmd = new SqlCommand(oQuery, conn, transaction))
                            {
                                oCmd.Parameters.AddWithValue("@QuestionID", question.QuestionID);
                                oCmd.Parameters.AddWithValue("@OptionText", opt.OptionText);
                                oCmd.Parameters.AddWithValue("@IsCorrect", opt.IsCorrect);
                                oCmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public bool DeleteQuestion(int questionId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "DELETE FROM Questions WHERE QuestionID = @QuestionID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@QuestionID", questionId);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public int SaveQuizAttempt(QuizAttempt attempt)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"INSERT INTO QuizAttempts (UserID, LessonID, ScorePercentage, Passed, TotalQuestions, CorrectAnswers, AttemptedAt)
                                VALUES (@UserID, @LessonID, @ScorePercentage, @Passed, @TotalQuestions, @CorrectAnswers, GETDATE());
                                SELECT SCOPE_IDENTITY();";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", attempt.UserID);
                    cmd.Parameters.AddWithValue("@LessonID", attempt.LessonID);
                    cmd.Parameters.AddWithValue("@ScorePercentage", attempt.ScorePercentage);
                    cmd.Parameters.AddWithValue("@Passed", attempt.Passed);
                    cmd.Parameters.AddWithValue("@TotalQuestions", attempt.TotalQuestions);
                    cmd.Parameters.AddWithValue("@CorrectAnswers", attempt.CorrectAnswers);

                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public List<QuizAttempt> GetUserAttempts(int userId)
        {
            List<QuizAttempt> attempts = new List<QuizAttempt>();
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"SELECT qa.AttemptID, qa.UserID, qa.LessonID, qa.ScorePercentage, qa.Passed, 
                                        qa.TotalQuestions, qa.CorrectAnswers, qa.AttemptedAt, 
                                        l.Title AS LessonTitle, u.FullName AS UserFullName, r.Title AS RoadmapTitle
                                 FROM QuizAttempts qa
                                 INNER JOIN Lessons l ON qa.LessonID = l.LessonID
                                 INNER JOIN Roadmaps r ON l.RoadmapID = r.RoadmapID
                                 INNER JOIN Users u ON qa.UserID = u.UserID
                                 WHERE qa.UserID = @UserID
                                 ORDER BY qa.AttemptedAt DESC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            attempts.Add(MapQuizAttempt(reader));
                        }
                    }
                }
            }
            return attempts;
        }

        public List<QuizAttempt> GetAllAttempts(int limit = 100)
        {
            List<QuizAttempt> attempts = new List<QuizAttempt>();
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = $@"SELECT TOP ({limit}) qa.AttemptID, qa.UserID, qa.LessonID, qa.ScorePercentage, qa.Passed, 
                                        qa.TotalQuestions, qa.CorrectAnswers, qa.AttemptedAt, 
                                        l.Title AS LessonTitle, u.FullName AS UserFullName, r.Title AS RoadmapTitle
                                 FROM QuizAttempts qa
                                 INNER JOIN Lessons l ON qa.LessonID = l.LessonID
                                 INNER JOIN Roadmaps r ON l.RoadmapID = r.RoadmapID
                                 INNER JOIN Users u ON qa.UserID = u.UserID
                                 ORDER BY qa.AttemptedAt DESC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            attempts.Add(MapQuizAttempt(reader));
                        }
                    }
                }
            }
            return attempts;
        }

        private List<QuizOption> GetOptionsForQuestion(SqlConnection conn, int questionId)
        {
            List<QuizOption> options = new List<QuizOption>();
            string query = "SELECT OptionID, QuestionID, OptionText, IsCorrect FROM QuizOptions WHERE QuestionID = @QuestionID ORDER BY OptionID ASC";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@QuestionID", questionId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        options.Add(new QuizOption
                        {
                            OptionID = Convert.ToInt32(reader["OptionID"]),
                            QuestionID = Convert.ToInt32(reader["QuestionID"]),
                            OptionText = reader["OptionText"].ToString() ?? string.Empty,
                            IsCorrect = Convert.ToBoolean(reader["IsCorrect"])
                        });
                    }
                }
            }
            return options;
        }

        private Question MapQuestion(SqlDataReader reader)
        {
            return new Question
            {
                QuestionID = Convert.ToInt32(reader["QuestionID"]),
                LessonID = Convert.ToInt32(reader["LessonID"]),
                QuestionText = reader["QuestionText"].ToString() ?? string.Empty,
                Explanation = reader["Explanation"] != DBNull.Value ? reader["Explanation"].ToString() : null,
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                LessonTitle = reader["LessonTitle"].ToString() ?? string.Empty
            };
        }

        private QuizAttempt MapQuizAttempt(SqlDataReader reader)
        {
            return new QuizAttempt
            {
                AttemptID = Convert.ToInt32(reader["AttemptID"]),
                UserID = Convert.ToInt32(reader["UserID"]),
                LessonID = Convert.ToInt32(reader["LessonID"]),
                ScorePercentage = Convert.ToDecimal(reader["ScorePercentage"]),
                Passed = Convert.ToBoolean(reader["Passed"]),
                TotalQuestions = Convert.ToInt32(reader["TotalQuestions"]),
                CorrectAnswers = Convert.ToInt32(reader["CorrectAnswers"]),
                AttemptedAt = Convert.ToDateTime(reader["AttemptedAt"]),
                LessonTitle = reader["LessonTitle"].ToString() ?? string.Empty,
                UserFullName = reader["UserFullName"].ToString() ?? string.Empty,
                RoadmapTitle = reader["RoadmapTitle"].ToString() ?? string.Empty
            };
        }
    }
}
