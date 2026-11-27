-- SQL Script to Create Tables for GyanPath Database
-- CT050-3-2-WAPP Web Applications Assignment Project: GYANPATH

USE GyanPathDB;
GO

-- Drop tables in reverse dependency order if re-running
IF OBJECT_ID('dbo.JobOpportunities', 'U') IS NOT NULL DROP TABLE dbo.JobOpportunities;
IF OBJECT_ID('dbo.QuizAttempts', 'U') IS NOT NULL DROP TABLE dbo.QuizAttempts;
IF OBJECT_ID('dbo.LessonProgress', 'U') IS NOT NULL DROP TABLE dbo.LessonProgress;
IF OBJECT_ID('dbo.Enrollments', 'U') IS NOT NULL DROP TABLE dbo.Enrollments;
IF OBJECT_ID('dbo.QuizOptions', 'U') IS NOT NULL DROP TABLE dbo.QuizOptions;
IF OBJECT_ID('dbo.Questions', 'U') IS NOT NULL DROP TABLE dbo.Questions;
IF OBJECT_ID('dbo.Lessons', 'U') IS NOT NULL DROP TABLE dbo.Lessons;
IF OBJECT_ID('dbo.Roadmaps', 'U') IS NOT NULL DROP TABLE dbo.Roadmaps;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
GO

-- 1. Users Table
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,
    PasswordSalt NVARCHAR(256) NOT NULL,
    Phone NVARCHAR(20) NULL,
    PreferredDomain NVARCHAR(50) NOT NULL DEFAULT 'Technology',
    Role NVARCHAR(20) NOT NULL DEFAULT 'Member', -- 'Member' or 'Admin'
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

-- 2. Roadmaps Table
CREATE TABLE Roadmaps (
    RoadmapID INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(100) NOT NULL,
    Domain NVARCHAR(50) NOT NULL, -- Technology, Design, Business, Creative Arts
    Description NVARCHAR(MAX) NOT NULL,
    Difficulty NVARCHAR(20) NOT NULL, -- Beginner, Intermediate, Advanced
    EstimatedDuration NVARCHAR(50) NOT NULL,
    ThumbnailUrl NVARCHAR(255) NULL,
    LearningObjectives NVARCHAR(MAX) NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

-- 3. Lessons Table
CREATE TABLE Lessons (
    LessonID INT IDENTITY(1,1) PRIMARY KEY,
    RoadmapID INT NOT NULL,
    Title NVARCHAR(150) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    VideoUrl NVARCHAR(255) NULL,
    LessonOrder INT NOT NULL DEFAULT 1,
    Resources NVARCHAR(MAX) NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Lessons_Roadmaps FOREIGN KEY (RoadmapID) REFERENCES Roadmaps(RoadmapID) ON DELETE CASCADE
);
GO

-- 4. Questions Table (Quiz questions associated with lessons)
CREATE TABLE Questions (
    QuestionID INT IDENTITY(1,1) PRIMARY KEY,
    LessonID INT NOT NULL,
    QuestionText NVARCHAR(MAX) NOT NULL,
    Explanation NVARCHAR(MAX) NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Questions_Lessons FOREIGN KEY (LessonID) REFERENCES Lessons(LessonID) ON DELETE CASCADE
);
GO

-- 5. QuizOptions Table
CREATE TABLE QuizOptions (
    OptionID INT IDENTITY(1,1) PRIMARY KEY,
    QuestionID INT NOT NULL,
    OptionText NVARCHAR(MAX) NOT NULL,
    IsCorrect BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_QuizOptions_Questions FOREIGN KEY (QuestionID) REFERENCES Questions(QuestionID) ON DELETE CASCADE
);
GO

-- 6. Enrollments Table
CREATE TABLE Enrollments (
    EnrollmentID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    RoadmapID INT NOT NULL,
    EnrolledAt DATETIME DEFAULT GETDATE(),
    IsCompleted BIT NOT NULL DEFAULT 0,
    CompletedAt DATETIME NULL,
    CONSTRAINT FK_Enrollments_Users FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    CONSTRAINT FK_Enrollments_Roadmaps FOREIGN KEY (RoadmapID) REFERENCES Roadmaps(RoadmapID) ON DELETE CASCADE,
    CONSTRAINT UQ_User_Roadmap UNIQUE (UserID, RoadmapID)
);
GO

-- 7. LessonProgress Table
CREATE TABLE LessonProgress (
    ProgressID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    LessonID INT NOT NULL,
    IsCompleted BIT NOT NULL DEFAULT 0,
    CompletedAt DATETIME NULL,
    CONSTRAINT FK_LessonProgress_Users FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    CONSTRAINT FK_LessonProgress_Lessons FOREIGN KEY (LessonID) REFERENCES Lessons(LessonID) ON DELETE CASCADE,
    CONSTRAINT UQ_User_Lesson UNIQUE (UserID, LessonID)
);
GO

-- 8. QuizAttempts Table
CREATE TABLE QuizAttempts (
    AttemptID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    LessonID INT NOT NULL,
    ScorePercentage DECIMAL(5,2) NOT NULL,
    Passed BIT NOT NULL DEFAULT 0,
    TotalQuestions INT NOT NULL,
    CorrectAnswers INT NOT NULL,
    AttemptedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_QuizAttempts_Users FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    CONSTRAINT FK_QuizAttempts_Lessons FOREIGN KEY (LessonID) REFERENCES Lessons(LessonID) ON DELETE CASCADE
);
GO

-- 9. JobOpportunities Table
CREATE TABLE JobOpportunities (
    JobID INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(100) NOT NULL,
    Company NVARCHAR(100) NOT NULL,
    Domain NVARCHAR(50) NOT NULL, -- Technology, Design, Business, Creative Arts
    Location NVARCHAR(100) NOT NULL,
    JobType NVARCHAR(50) NOT NULL, -- Full-time, Internship, Part-time, Remote
    Description NVARCHAR(MAX) NOT NULL,
    RequiredSkills NVARCHAR(MAX) NOT NULL,
    ApplicationUrl NVARCHAR(255) NOT NULL,
    PostedDate DATETIME DEFAULT GETDATE()
);
GO
