-- SQL Script to Seed Demo Data for GyanPath Database
-- CT050-3-2-WAPP Web Applications Assignment Project: GYANPATH

USE GyanPathDB;
GO

-- Clear existing data (optional / safe)
DELETE FROM QuizAttempts;
DELETE FROM LessonProgress;
DELETE FROM Enrollments;
DELETE FROM QuizOptions;
DELETE FROM Questions;
DELETE FROM Lessons;
DELETE FROM Roadmaps;
DELETE FROM JobOpportunities;
DELETE FROM Users;
GO

-- 1. SEED USERS
-- Admin Password: Admin@123
-- Student Password: Student@123
INSERT INTO Users (FullName, Email, PasswordHash, PasswordSalt, Phone, PreferredDomain, Role, CreatedAt)
VALUES 
(N'GyanPath Administrator', N'admin@gyanpath.com', N'IJxXJWHLFk8x9KeC9+UJcJxbN3ZbPl2LVN7nnL716tU=', N'dXNlcjFzYWx0MTIzNDU2Nw==', N'+977-9801234567', N'Technology', N'Admin', GETDATE()),
(N'Demo Student', N'student@gyanpath.com', N'axzZE9+nxAcWGMpgdE9wFvsbtyyXMzrpu8k68sCkDlA=', N'dXNlcjFzYWx0MTIzNDU2Nw==', N'+977-9841987654', N'Technology', N'Member', GETDATE()),
(N'Aarav Sharma', N'aarav@gyanpath.com', N'axzZE9+nxAcWGMpgdE9wFvsbtyyXMzrpu8k68sCkDlA=', N'dXNlcjFzYWx0MTIzNDU2Nw==', N'+977-9851122334', N'Design', N'Member', GETDATE()),
(N'Sita Adhikari', N'sita@gyanpath.com', N'axzZE9+nxAcWGMpgdE9wFvsbtyyXMzrpu8k68sCkDlA=', N'dXNlcjFzYWx0MTIzNDU2Nw==', N'+977-9812233445', N'Business', N'Member', GETDATE());
GO

-- 2. SEED ROADMAPS
INSERT INTO Roadmaps (Title, Domain, Description, Difficulty, EstimatedDuration, ThumbnailUrl, LearningObjectives, CreatedAt)
VALUES
(N'Full-Stack Web Development', N'Technology', N'Master modern frontend and backend web development using HTML5, CSS3, JavaScript, Bootstrap, C#, and ASP.NET Core MVC with SQL Server.', N'Beginner', N'8 Weeks', N'/images/roadmaps/web-dev.jpg', N'Build responsive web interfaces; Implement server-side logic in ASP.NET Core; Connect to SQL Server using ADO.NET; Deploy web applications.', GETDATE()),
(N'C# & Software Engineering', N'Technology', N'Learn core C# programming, object-oriented principles, data structures, and database programming with SQL Server.', N'Intermediate', N'6 Weeks', N'/images/roadmaps/csharp-dev.jpg', N'Understand C# syntax and OOP concepts; Build console and GUI apps; Work with ADO.NET database connectivity; Apply clean software architecture.', GETDATE()),
(N'UI/UX Design Fundamentals', N'Design', N'Learn user research, wireframing, interactive prototyping in Figma, and design systems for web and mobile interfaces.', N'Beginner', N'6 Weeks', N'/images/roadmaps/ui-ux.jpg', N'Conduct user research & personas; Create low and high-fidelity wireframes; Master Figma auto-layout & components; Test usability.', GETDATE()),
(N'Digital Marketing & Analytics', N'Business', N'Master Search Engine Optimization (SEO), social media marketing strategy, Google Analytics, and online brand growth.', N'Beginner', N'4 Weeks', N'/images/roadmaps/digital-marketing.jpg', N'Perform keyword research & SEO audit; Run social media ad campaigns; Analyze web traffic with Analytics; Create content calendars.', GETDATE()),
(N'Digital Content Creation', N'Creative Arts', N'Learn video editing, storytelling, scriptwriting, and channel growth for YouTube, TikTok, and professional brand media.', N'Beginner', N'5 Weeks', N'/images/roadmaps/content-creation.jpg', N'Write compelling video scripts; Edit videos with Adobe Premiere / CapCut; Optimize thumbnails and titles; Build a personal brand.', GETDATE());
GO

-- 3. SEED LESSONS FOR ROADMAP 1 (Full-Stack Web Development - RoadmapID = 1)
INSERT INTO Lessons (RoadmapID, Title, Description, Content, VideoUrl, LessonOrder, Resources, CreatedAt)
VALUES
(1, N'Lesson 1: Introduction to Web Development & HTML5', N'Understand how the web works, client-server architecture, and basic HTML5 structure.', 
N'### Overview of Web Applications
Web applications operate on a **client-server architecture**. The browser sends HTTP requests, and the server responds with HTML, CSS, JavaScript, or JSON.

### Key HTML5 Elements
HTML5 introduced semantic tags that describe their content clearly to browsers and search engines:
- `<header>` - Page or section header
- `<nav>` - Navigation links
- `<main>` - Main body content
- `<section>` - Theme-grouped content
- `<article>` - Independent self-contained content
- `<footer>` - Page footer information

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>My First GyanPath Web Page</title>
</head>
<body>
    <header><h1>Welcome to GyanPath</h1></header>
    <main><p>Building skills for Nepal fresh graduates.</p></main>
</body>
</html>
```', 
N'https://www.youtube.com/embed/mU6anWqZJcc', 1, N'MDN Web Docs HTML5 Guide, W3Schools HTML Tutorial', GETDATE()),

(1, N'Lesson 2: CSS3 Layouts & Responsive Design', N'Master external CSS stylesheets, flexbox, CSS grid, and media queries.', 
N'### Styling Web Pages with CSS3
CSS (Cascading Style Sheets) controls layout, typography, colors, and visual effects.

### Flexbox vs Grid
- **Flexbox** is best for one-dimensional layouts (rows OR columns).
- **CSS Grid** is best for two-dimensional layouts (rows AND columns).

```css
.card-container {
    display: flex;
    gap: 1.5rem;
    flex-wrap: wrap;
}
.card {
    flex: 1 1 300px;
    background: #ffffff;
    border-radius: 8px;
    box-shadow: 0 4px 6px rgba(0,0,0,0.1);
}
```', 
N'https://www.youtube.com/embed/1Rs2ND1ryYc', 2, N'CSS-Tricks Complete Guide to Flexbox, MDN CSS Grid', GETDATE()),

(1, N'Lesson 3: JavaScript & DOM Manipulation', N'Learn client-side programming, event listeners, form validation, and DOM updates.', 
N'### Interactive Client-Side JavaScript
JavaScript enables dynamic user interactions without full page reloads.

### Form Validation Example
```javascript
document.getElementById("regForm").addEventListener("submit", function(e) {
    const password = document.getElementById("password").value;
    const confirm = document.getElementById("confirmPassword").value;
    
    if (password !== confirm) {
        e.preventDefault();
        alert("Passwords do not match!");
    }
});
```', 
N'https://www.youtube.com/embed/hdI2bqOjy3c', 3, N'JavaScript.info Tutorial, MDN JS Reference', GETDATE()),

(1, N'Lesson 4: ASP.NET Core MVC Architecture & ADO.NET', N'Learn Controller-View-Model workflow and database integration with SQL Server using ADO.NET.', 
N'### ASP.NET Core MVC Overview
ASP.NET Core MVC separates concerns into three distinct layers:
- **Models**: Business data and domain logic.
- **Views**: UI templates rendered with Razor syntax (`.cshtml`).
- **Controllers**: Handle HTTP requests, call repositories, and return views.

### ADO.NET Database Query Example
```csharp
using (SqlConnection conn = new SqlConnection(connectionString))
{
    string query = "SELECT * FROM Users WHERE Email = @Email";
    SqlCommand cmd = new SqlCommand(query, conn);
    cmd.Parameters.AddWithValue("@Email", email);
    conn.Open();
    SqlDataReader reader = cmd.ExecuteReader();
}
```', 
N'https://www.youtube.com/embed/hZ1DASYd95U', 4, N'Microsoft Docs ASP.NET Core MVC, ADO.NET Developer Guide', GETDATE());
GO

-- 4. SEED LESSONS FOR ROADMAP 2 (C# & Software Engineering - RoadmapID = 2)
INSERT INTO Lessons (RoadmapID, Title, Description, Content, VideoUrl, LessonOrder, Resources, CreatedAt)
VALUES
(2, N'Lesson 1: C# Object-Oriented Fundamentals', N'Understand classes, objects, inheritance, encapsulation, and polymorphism in C#.', 
N'### Object-Oriented Programming in C#
C# is a strictly typed object-oriented language.

```csharp
public class User
{
    public int UserID { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    
    public void DisplayInfo()
    {
        Console.WriteLine($"User: {FullName} ({Email})");
    }
}
```', 
N'https://www.youtube.com/embed/gfkTfcpWqAY', 1, N'Microsoft C# Documentation, C# Yellow Book', GETDATE()),

(2, N'Lesson 2: SQL Server Database Integration with ADO.NET', N'Master SqlCommand, SqlDataReader, SqlDataAdapter, and stored procedures.', 
N'### Database Connectivity with ADO.NET
ADO.NET provides direct, lightweight control over Microsoft SQL Server connections.

Always use parameterized queries (`SqlParameter`) to prevent SQL Injection vulnerability!', 
N'https://www.youtube.com/embed/3e20_Gz_-7g', 2, N'ADO.NET Overview Microsoft Docs', GETDATE());
GO

-- 5. SEED LESSONS FOR ROADMAP 3 (UI/UX Design - RoadmapID = 3)
INSERT INTO Lessons (RoadmapID, Title, Description, Content, VideoUrl, LessonOrder, Resources, CreatedAt)
VALUES
(3, N'Lesson 1: User Research & Persona Creation', N'Learn how to conduct user interviews, identify pain points, and build user personas.', 
N'### User Research Foundations
UI/UX design starts with empathy. Discover what users truly need through survey research and behavioral mapping.', 
N'https://www.youtube.com/embed/c9Wg6Cb_YlU', 1, N'Nielsen Norman Group UX Basics', GETDATE()),

(3, N'Lesson 2: Figma Prototyping & Design Systems', N'Create interactive click-through prototypes and reusable design components in Figma.', 
N'### Figma Auto-Layout & Design Tokens
Design systems keep web UI consistent across screens and devices.', 
N'https://www.youtube.com/embed/jk1T0CeHwV0', 2, N'Figma Official Beginner Guide', GETDATE());
GO

-- 6. SEED QUESTIONS & QUIZ OPTIONS
-- For Lesson 1 (Web Dev)
INSERT INTO Questions (LessonID, QuestionText, Explanation, CreatedAt)
VALUES (1, N'Which HTML5 element is used to specify the main content area of a document?', N'<main> represents the dominant content of the <body> of a document.', GETDATE());

INSERT INTO QuizOptions (QuestionID, OptionText, IsCorrect)
VALUES 
(1, N'<section>', 0),
(1, N'<main>', 1),
(1, N'<article>', 0),
(1, N'<div>', 0);

INSERT INTO Questions (LessonID, QuestionText, Explanation, CreatedAt)
VALUES (1, N'What is the purpose of client-server architecture in web applications?', N'Clients send requests and servers process logic and respond with data.', GETDATE());

INSERT INTO QuizOptions (QuestionID, OptionText, IsCorrect)
VALUES 
(2, N'To store client browser files locally', 0),
(2, N'To allow client browsers to send requests and server to respond with resources', 1),
(2, N'To replace database servers entirely', 0),
(2, N'To run JavaScript code on hardware routers', 0);

-- For Lesson 2 (Web Dev)
INSERT INTO Questions (LessonID, QuestionText, Explanation, CreatedAt)
VALUES (3, N'Which CSS display property value is best for two-dimensional grid layouts?', N'CSS Grid (display: grid) handles both rows and columns simultaneously.', GETDATE());

INSERT INTO QuizOptions (QuestionID, OptionText, IsCorrect)
VALUES 
(3, N'display: inline-block', 0),
(3, N'display: flex', 0),
(3, N'display: grid', 1),
(3, N'display: table', 0);

-- For Lesson 3 (Web Dev)
INSERT INTO Questions (LessonID, QuestionText, Explanation, CreatedAt)
VALUES (4, N'How do you prevent a form from submitting when validation fails in JavaScript?', N'Calling event.preventDefault() stops the browser default submit action.', GETDATE());

INSERT INTO QuizOptions (QuestionID, OptionText, IsCorrect)
VALUES 
(4, N'event.stopPropagation()', 0),
(4, N'event.preventDefault()', 1),
(4, N'return false inside HTML', 0),
(4, N'form.stopSubmit()', 0);

-- For Lesson 4 (Web Dev)
INSERT INTO Questions (LessonID, QuestionText, Explanation, CreatedAt)
VALUES (5, N'Why should developers use parameterized queries (SqlParameter) in ADO.NET?', N'Parameterized queries prevent SQL Injection attacks by ensuring inputs are treated as data, not code.', GETDATE());

INSERT INTO QuizOptions (QuestionID, OptionText, IsCorrect)
VALUES 
(5, N'To make SQL queries execute 100x faster', 0),
(5, N'To prevent SQL Injection security vulnerabilities', 1),
(5, N'To automatically create database backup files', 0),
(5, N'To bypass database table permissions', 0);

-- For Lesson 1 of Roadmap 2 (C#)
INSERT INTO Questions (LessonID, QuestionText, Explanation, CreatedAt)
VALUES (6, N'Which C# OOP concept allows a derived class to provide a specific implementation of a method defined in a base class?', N'Polymorphism allows overriding base class virtual methods.', GETDATE());

INSERT INTO QuizOptions (QuestionID, OptionText, IsCorrect)
VALUES 
(6, N'Encapsulation', 0),
(6, N'Polymorphism', 1),
(6, N'Abstraction', 0),
(6, N'Multiple Inheritance', 0);

-- For Lesson 1 of Roadmap 3 (UI/UX)
INSERT INTO Questions (LessonID, QuestionText, Explanation, CreatedAt)
VALUES (7, N'What is a User Persona in UX Design?', N'A user persona is a fictional representation of your target user based on research data.', GETDATE());

INSERT INTO QuizOptions (QuestionID, OptionText, IsCorrect)
VALUES 
(7, N'A real photograph of the product owner', 0),
(7, N'A fictional representation of the ideal target user based on research', 1),
(7, N'The database schema diagram for user accounts', 0),
(7, N'A marketing slogan', 0);

GO

-- 7. SEED JOB / INTERNSHIP OPPORTUNITIES
INSERT INTO JobOpportunities (Title, Company, Domain, Location, JobType, Description, RequiredSkills, ApplicationUrl, PostedDate)
VALUES
(N'Junior ASP.NET Core Developer', N'F1Soft International', N'Technology', N'Lalitpur, Nepal (Hybrid)', N'Full-time', N'Looking for a motivated junior developer skilled in C#, ASP.NET Core MVC, SQL Server, and REST APIs. Great growth environment.', N'C#, ASP.NET Core, SQL Server, ADO.NET, HTML5, Bootstrap 5', N'https://f1soft.com/careers', GETDATE()),
(N'Frontend Development Intern', N'Leapfrog Technology Nepal', N'Technology', N'Kathmandu, Nepal', N'Internship', N'Join our 3-month paid internship program. Work alongside senior software engineers on real web application client projects.', N'HTML5, CSS3, JavaScript, Responsive Web Design', N'https://www.leapfrog.technology/careers', GETDATE()),
(N'Associate UI/UX Designer', N'Cotiviti Nepal', N'Design', N'Kathmandu, Nepal (Remote option)', N'Full-time', N'Design intuitive mobile and desktop UI components. Create wireframes, user journeys, and prototypes using Figma.', N'Figma, User Research, Wireframing, Prototyping, CSS', N'https://www.cotiviti.com/careers', GETDATE()),
(N'Digital Marketing Executive', N'Daraz Nepal (Alibaba Group)', N'Business', N'Kathmandu, Nepal', N'Full-time', N'Manage online advertising campaigns, social media channels, and Google Search Engine Optimization (SEO) strategies.', N'SEO, Google Analytics, Social Media Ads, Content Writing', N'https://careers.daraz.com', GETDATE()),
(N'Content & Video Creator Intern', N'TechPana Nepal', N'Creative Arts', N'Lalitpur, Nepal', N'Internship', N'Create tech review scripts, edit short-form videos for TikTok/Reels, and assist in video production.', N'Video Editing, CapCut / Premiere Pro, Scriptwriting, Camera Handling', N'https://techpana.com/join-us', GETDATE());
GO

-- 8. SEED SAMPLE ENROLLMENTS, PROGRESS & QUIZ ATTEMPTS FOR DEMO MEMBER (UserID = 2)
INSERT INTO Enrollments (UserID, RoadmapID, EnrolledAt, IsCompleted, CompletedAt)
VALUES 
(2, 1, DATEADD(DAY, -10, GETDATE()), 0, NULL),
(2, 2, DATEADD(DAY, -5, GETDATE()), 0, NULL),
(3, 3, DATEADD(DAY, -7, GETDATE()), 0, NULL);

INSERT INTO LessonProgress (UserID, LessonID, IsCompleted, CompletedAt)
VALUES 
(2, 1, 1, DATEADD(DAY, -9, GETDATE())),
(2, 2, 1, DATEADD(DAY, -8, GETDATE())),
(2, 3, 0, NULL);

INSERT INTO QuizAttempts (UserID, LessonID, ScorePercentage, Passed, TotalQuestions, CorrectAnswers, AttemptedAt)
VALUES 
(2, 1, 100.00, 1, 2, 2, DATEADD(DAY, -9, GETDATE())),
(2, 2, 100.00, 1, 1, 1, DATEADD(DAY, -8, GETDATE()));
GO
