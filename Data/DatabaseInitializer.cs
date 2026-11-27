using System;
using System.IO;
using Microsoft.Data.SqlClient;

namespace GyanPath.Data
{
    public static class DatabaseInitializer
    {
        public static void Initialize(string connectionString, string contentRootPath)
        {
            try
            {
                // Parse master connection string to ensure DB exists
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
                string dbName = builder.InitialCatalog;
                builder.InitialCatalog = "master";

                using (SqlConnection conn = new SqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    string checkDbQuery = $"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{dbName}') CREATE DATABASE [{dbName}];";
                    using (SqlCommand cmd = new SqlCommand(checkDbQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                // Execute Table scripts if Users table doesn't exist or Roadmaps is empty
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string checkUsersQuery = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users'";
                    using (SqlCommand cmd = new SqlCommand(checkUsersQuery, conn))
                    {
                        int tableCount = (int)cmd.ExecuteScalar();
                        if (tableCount == 0)
                        {
                            string tablesSqlPath = Path.Combine(contentRootPath, "SQL", "Tables.sql");
                            if (File.Exists(tablesSqlPath))
                            {
                                string sql = File.ReadAllText(tablesSqlPath);
                                ExecuteScriptBlocks(conn, sql);
                            }
                        }
                    }

                    string checkRoadmapsQuery = "SELECT COUNT(*) FROM Roadmaps";
                    bool isRoadmapsEmpty = true;
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(checkRoadmapsQuery, conn))
                        {
                            int count = (int)cmd.ExecuteScalar();
                            isRoadmapsEmpty = (count == 0);
                        }
                    }
                    catch { isRoadmapsEmpty = true; }

                    if (isRoadmapsEmpty)
                    {
                        string tablesSqlPath = Path.Combine(contentRootPath, "SQL", "Tables.sql");
                        if (File.Exists(tablesSqlPath))
                        {
                            string sql = File.ReadAllText(tablesSqlPath);
                            ExecuteScriptBlocks(conn, sql);
                        }

                        string seedSqlPath = Path.Combine(contentRootPath, "SQL", "SeedData.sql");
                        if (File.Exists(seedSqlPath))
                        {
                            string seedSql = File.ReadAllText(seedSqlPath);
                            ExecuteScriptBlocks(conn, seedSql);
                        }

                        string spSqlPath = Path.Combine(contentRootPath, "SQL", "StoredProcedures.sql");
                        if (File.Exists(spSqlPath))
                        {
                            string spSql = File.ReadAllText(spSqlPath);
                            ExecuteScriptBlocks(conn, spSql);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DatabaseInitializer Warning] Could not auto-initialize DB: {ex.Message}");
            }
        }

        private static string tablesSqlSqlScript(string path)
        {
            return path;
        }

        private static void ExecuteScriptBlocks(SqlConnection conn, string script)
        {
            string normalizedScript = script.Replace("\r\n", "\n").Replace("\r", "\n");
            string[] blocks = System.Text.RegularExpressions.Regex.Split(
                normalizedScript, 
                @"^\s*GO\s*$", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);

            foreach (string block in blocks)
            {
                string trimmed = block.Trim();
                if (!string.IsNullOrWhiteSpace(trimmed) && !string.Equals(trimmed, "GO", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(trimmed, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ExecuteScriptBlocks Error] {ex.Message}");
                    }
                }
            }
        }
    }
}
