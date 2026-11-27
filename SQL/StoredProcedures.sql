-- SQL Script to Create Stored Procedures for GyanPath Database
-- CT050-3-2-WAPP Web Applications Assignment Project: GYANPATH

USE GyanPathDB;
GO

-- 1. Get User By Email Stored Procedure
IF OBJECT_ID('dbo.sp_GetUserByEmail', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetUserByEmail;
GO
CREATE PROCEDURE sp_GetUserByEmail
    @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserID, FullName, Email, PasswordHash, PasswordSalt, Phone, PreferredDomain, Role, CreatedAt
    FROM Users
    WHERE Email = @Email;
END;
GO

-- 2. Get Roadmap Completion Percentage for a User
IF OBJECT_ID('dbo.sp_GetRoadmapProgressByUser', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetRoadmapProgressByUser;
GO
CREATE PROCEDURE sp_GetRoadmapProgressByUser
    @UserID INT,
    @RoadmapID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TotalLessons INT = 0;
    DECLARE @CompletedLessons INT = 0;
    DECLARE @Percentage DECIMAL(5,2) = 0.00;
    
    SELECT @TotalLessons = COUNT(*) FROM Lessons WHERE RoadmapID = @RoadmapID;
    
    IF @TotalLessons > 0
    BEGIN
        SELECT @CompletedLessons = COUNT(DISTINCT lp.LessonID)
        FROM LessonProgress lp
        INNER JOIN Lessons l ON lp.LessonID = l.LessonID
        WHERE lp.UserID = @UserID AND l.RoadmapID = @RoadmapID AND lp.IsCompleted = 1;
        
        SET @Percentage = CAST((@CompletedLessons * 100.0) / @TotalLessons AS DECIMAL(5,2));
    END
    
    SELECT 
        @RoadmapID AS RoadmapID,
        @TotalLessons AS TotalLessons,
        @CompletedLessons AS CompletedLessons,
        @Percentage AS ProgressPercentage;
END;
GO

-- 3. Get Admin Dashboard Overall System Statistics
IF OBJECT_ID('dbo.sp_GetAdminDashboardStats', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetAdminDashboardStats;
GO
CREATE PROCEDURE sp_GetAdminDashboardStats
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        (SELECT COUNT(*) FROM Users WHERE Role = 'Member') AS TotalMembers,
        (SELECT COUNT(*) FROM Roadmaps) AS TotalRoadmaps,
        (SELECT COUNT(*) FROM Lessons) AS TotalLessons,
        (SELECT COUNT(*) FROM Enrollments) AS TotalEnrollments,
        (SELECT COUNT(*) FROM QuizAttempts) AS TotalQuizAttempts,
        (SELECT COUNT(*) FROM JobOpportunities) AS TotalJobs;
END;
GO
