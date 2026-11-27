using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using GyanPath.Models;

namespace GyanPath.Data
{
    public class JobRepository
    {
        private readonly DatabaseConnection _db;

        public JobRepository(DatabaseConnection db)
        {
            _db = db;
        }

        public List<JobOpportunity> GetAll(string? search = null, string? domain = null, string? jobType = null)
        {
            List<JobOpportunity> jobs = new List<JobOpportunity>();
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"SELECT JobID, Title, Company, Domain, Location, JobType, Description, RequiredSkills, ApplicationUrl, PostedDate
                                 FROM JobOpportunities
                                 WHERE 1=1";

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query += " AND (Title LIKE @Search OR Company LIKE @Search OR RequiredSkills LIKE @Search)";
                }
                if (!string.IsNullOrWhiteSpace(domain) && domain != "All")
                {
                    query += " AND Domain = @Domain";
                }
                if (!string.IsNullOrWhiteSpace(jobType) && jobType != "All")
                {
                    query += " AND JobType = @JobType";
                }

                query += " ORDER BY PostedDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        cmd.Parameters.AddWithValue("@Search", $"%{search.Trim()}%");
                    }
                    if (!string.IsNullOrWhiteSpace(domain) && domain != "All")
                    {
                        cmd.Parameters.AddWithValue("@Domain", domain);
                    }
                    if (!string.IsNullOrWhiteSpace(jobType) && jobType != "All")
                    {
                        cmd.Parameters.AddWithValue("@JobType", jobType);
                    }

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            jobs.Add(MapJob(reader));
                        }
                    }
                }
            }
            return jobs;
        }

        public JobOpportunity? GetById(int jobId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "SELECT JobID, Title, Company, Domain, Location, JobType, Description, RequiredSkills, ApplicationUrl, PostedDate FROM JobOpportunities WHERE JobID = @JobID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@JobID", jobId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapJob(reader);
                        }
                    }
                }
            }
            return null;
        }

        public List<JobOpportunity> GetRecommendationsForUser(int userId, string userPreferredDomain)
        {
            // Rule-Based Recommendation Algorithm:
            // 1. Get completed roadmaps domains for the user
            // 2. Prioritize jobs matching completed roadmap domains
            // 3. Next match jobs matching user's preferred domain
            // 4. Return top recommended opportunities

            HashSet<string> completedDomains = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"SELECT DISTINCT r.Domain
                                 FROM Enrollments e
                                 INNER JOIN Roadmaps r ON e.RoadmapID = r.RoadmapID
                                 WHERE e.UserID = @UserID AND e.IsCompleted = 1";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            completedDomains.Add(reader["Domain"].ToString() ?? string.Empty);
                        }
                    }
                }
            }

            List<JobOpportunity> allJobs = GetAll();
            foreach (var job in allJobs)
            {
                if (completedDomains.Contains(job.Domain))
                {
                    job.IsRecommended = true;
                    job.RecommendationReason = $"Matched from your completed '{job.Domain}' career roadmap!";
                }
                else if (string.Equals(job.Domain, userPreferredDomain, StringComparison.OrdinalIgnoreCase))
                {
                    job.IsRecommended = true;
                    job.RecommendationReason = $"Matches your preferred learning domain ({userPreferredDomain})";
                }
                else
                {
                    job.IsRecommended = false;
                }
            }

            return allJobs.OrderByDescending(j => j.IsRecommended)
                          .ThenByDescending(j => j.PostedDate)
                          .ToList();
        }

        public int Create(JobOpportunity job)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"INSERT INTO JobOpportunities (Title, Company, Domain, Location, JobType, Description, RequiredSkills, ApplicationUrl, PostedDate)
                                VALUES (@Title, @Company, @Domain, @Location, @JobType, @Description, @RequiredSkills, @ApplicationUrl, GETDATE());
                                SELECT SCOPE_IDENTITY();";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", job.Title);
                    cmd.Parameters.AddWithValue("@Company", job.Company);
                    cmd.Parameters.AddWithValue("@Domain", job.Domain);
                    cmd.Parameters.AddWithValue("@Location", job.Location);
                    cmd.Parameters.AddWithValue("@JobType", job.JobType);
                    cmd.Parameters.AddWithValue("@Description", job.Description);
                    cmd.Parameters.AddWithValue("@RequiredSkills", job.RequiredSkills);
                    cmd.Parameters.AddWithValue("@ApplicationUrl", job.ApplicationUrl);

                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public bool Update(JobOpportunity job)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"UPDATE JobOpportunities
                                SET Title = @Title,
                                    Company = @Company,
                                    Domain = @Domain,
                                    Location = @Location,
                                    JobType = @JobType,
                                    Description = @Description,
                                    RequiredSkills = @RequiredSkills,
                                    ApplicationUrl = @ApplicationUrl
                                WHERE JobID = @JobID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", job.Title);
                    cmd.Parameters.AddWithValue("@Company", job.Company);
                    cmd.Parameters.AddWithValue("@Domain", job.Domain);
                    cmd.Parameters.AddWithValue("@Location", job.Location);
                    cmd.Parameters.AddWithValue("@JobType", job.JobType);
                    cmd.Parameters.AddWithValue("@Description", job.Description);
                    cmd.Parameters.AddWithValue("@RequiredSkills", job.RequiredSkills);
                    cmd.Parameters.AddWithValue("@ApplicationUrl", job.ApplicationUrl);
                    cmd.Parameters.AddWithValue("@JobID", job.JobID);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Delete(int jobId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "DELETE FROM JobOpportunities WHERE JobID = @JobID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@JobID", jobId);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        private JobOpportunity MapJob(SqlDataReader reader)
        {
            return new JobOpportunity
            {
                JobID = Convert.ToInt32(reader["JobID"]),
                Title = reader["Title"].ToString() ?? string.Empty,
                Company = reader["Company"].ToString() ?? string.Empty,
                Domain = reader["Domain"].ToString() ?? "Technology",
                Location = reader["Location"].ToString() ?? string.Empty,
                JobType = reader["JobType"].ToString() ?? "Full-time",
                Description = reader["Description"].ToString() ?? string.Empty,
                RequiredSkills = reader["RequiredSkills"].ToString() ?? string.Empty,
                ApplicationUrl = reader["ApplicationUrl"].ToString() ?? string.Empty,
                PostedDate = Convert.ToDateTime(reader["PostedDate"])
            };
        }
    }
}
