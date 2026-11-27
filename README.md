# GYANPATH — Web-Based Career Learning Platform

> **University Web Applications (CT050-3-2-WAPP) Assignment Project**

---

## 1. Project Overview

**GyanPath** is a full-stack, database-driven ASP.NET Core MVC web application designed to empower university students and fresh graduates in Nepal with practical, industry-aligned career learning roadmaps. 

It addresses the gap between academic education and real-world employment by offering structured learning pathways across 4 major career domains:
1. **Technology** (Full-Stack Web Development, C# & Software Engineering)
2. **Design** (UI/UX Design, Figma Prototyping)
3. **Business** (Digital Marketing & Web Analytics)
4. **Creative Arts** (Digital Content Creation & Video Production)

Learners follow lesson materials, view embedded educational video guides, complete interactive MCQ assessments, track their progress in real-time, and receive rule-based job & internship recommendations.

---

## 2. Technology Stack

- **Backend Framework:** ASP.NET Core 9.0 (MVC Architecture using C#)
- **Database System:** Microsoft SQL Server (`GyanPathDB`)
- **Database Connectivity:** **Pure ADO.NET** (`SqlConnection`, `SqlCommand`, `SqlDataReader`, `SqlParameter`, `SqlTransaction`)
- **Frontend Presentation:** HTML5 Semantic Elements & Razor Views (`.cshtml`)
- **Styling:** Custom External CSS (`wwwroot/css/site.css` & `admin.css`) + Bootstrap 5
- **Client Processing:** Native Vanilla JavaScript (`wwwroot/js/site.js`, `validation.js`, `quiz.js`)
- **Authentication & Security:** Cookie-based Authentication (`CookieAuthenticationDefaults`), Role-Based Access Control (`Admin` & `Member`), and PBKDF2 Password Hashing with SHA256 & Salt (`PasswordHasher.cs`).

---

## 3. Demo Login Credentials

The application comes pre-populated with realistic demo data.

| User Role | Email Address | Password | Privileges |
| :--- | :--- | :--- | :--- |
| **Administrator** | `admin@gyanpath.com` | `Admin@123` | Full System CRUD (Users, Roadmaps, Lessons, Quizzes, Jobs) & Stats Dashboard |
| **Member (Learner)** | `student@gyanpath.com` | `Student@123` | Enrolling in roadmaps, tracking progress, taking quizzes, profile management, job recommendations |
| **Member (Learner 2)** | `aarav@gyanpath.com` | `Student@123` | Alternate learner account |
| **Guest User** | *N/A (No Login)* | *N/A* | Browse public home, about, roadmaps, and job listings |

---

## 4. SQL Server Database Setup

The project includes complete SQL scripts in the `SQL/` directory:

- `SQL/Database.sql`: Creates `GyanPathDB` database if it does not exist.
- `SQL/Tables.sql`: Creates all 9 relational tables with PK/FK constraints:
  - `Users`
  - `Roadmaps`
  - `Lessons`
  - `Questions`
  - `QuizOptions`
  - `Enrollments`
  - `LessonProgress`
  - `QuizAttempts`
  - `JobOpportunities`
- `SQL/SeedData.sql`: Inserts demo users, roadmaps, lessons, MCQ questions, options, and job opportunities.
- `SQL/StoredProcedures.sql`: Defines stored procedures (`sp_GetUserByEmail`, `sp_GetRoadmapProgressByUser`, `sp_GetAdminDashboardStats`).

### Automatic Startup Initialization
**Note:** The application contains an automatic database initializer (`Data/DatabaseInitializer.cs`). Upon launching `dotnet run`, if `GyanPathDB` or its tables are missing, they are **automatically created and seeded**.

### Manual SSMS Execution (Optional)
To set up manually in SQL Server Management Studio (SSMS):
1. Connect to your SQL Server instance (`(localdb)\mssqllocaldb` or `.\SQLEXPRESS`).
2. Open and execute `SQL/Database.sql`.
3. Open and execute `SQL/Tables.sql`.
4. Open and execute `SQL/SeedData.sql`.
5. Open and execute `SQL/StoredProcedures.sql`.

---

## 5. Connection String Configuration

The database connection string is located in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=GyanPathDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;"
  }
}
```

*If using SQL Express instead of LocalDB, update the `Server` value to `.\\SQLEXPRESS`.*

---

## 6. How to Run the Application

1. Open a command prompt or terminal in the project directory (`c:\SEM IV\WAPP\GyanPath`).
2. Run the build command:
   ```bash
   dotnet build
   ```
3. Launch the ASP.NET Core dev server:
   ```bash
   dotnet run
   ```
4. Open your browser and navigate to:
   ```text
   http://localhost:5000
   ```

---

## 7. Project Architecture & Directory Structure

```text
GyanPath/
│
├── Controllers/
│   ├── HomeController.cs           # Public home, about, and error handling
│   ├── AccountController.cs        # Registration, login, logout, password validation
│   ├── RoadmapsController.cs       # Search, domain filter, details, enrollment
│   ├── LessonsController.cs        # Lesson viewer, embedded YouTube player, completion
│   ├── QuizController.cs           # MCQ assessment engine, score calculation, attempt logging
│   ├── DashboardController.cs      # Learner dashboard, progress bars, recommendations
│   ├── ProgressController.cs       # Full progress history overview
│   ├── JobsController.cs           # Job search, domain filter, details
│   ├── ProfileController.cs        # Learner profile management & password update
│   ├── AdminDashboardController.cs # Admin control panel summary & stats
│   ├── AdminUsersController.cs     # Admin User CRUD operations
│   ├── AdminRoadmapsController.cs  # Admin Roadmap CRUD operations
│   ├── AdminLessonsController.cs   # Admin Lesson CRUD operations
│   ├── AdminQuizzesController.cs   # Admin Quiz Question CRUD operations
│   └── AdminJobsController.cs      # Admin Job Opportunity CRUD operations
│
├── Data/
│   ├── DatabaseConnection.cs       # Central ADO.NET connection helper
│   ├── DatabaseInitializer.cs      # Auto DB creation & script executor
│   ├── PasswordHasher.cs           # PBKDF2 with SHA256 & salt hasher
│   ├── UserRepository.cs           # ADO.NET repository for Users table
│   ├── RoadmapRepository.cs        # ADO.NET repository for Roadmaps table
│   ├── LessonRepository.cs         # ADO.NET repository for Lessons table
│   ├── QuizRepository.cs           # ADO.NET repository for Questions & Attempts
│   ├── ProgressRepository.cs       # ADO.NET repository for Enrollments & Progress
│   └── JobRepository.cs            # ADO.NET repository for Jobs & Recommendations
│
├── Models/
│   ├── User.cs                     # User entity model
│   ├── Roadmap.cs                  # Roadmap entity model
│   ├── Lesson.cs                   # Lesson entity model
│   ├── Question.cs                 # Quiz question & options models
│   ├── Enrollment.cs               # Enrollment model
│   ├── LessonProgress.cs           # Lesson completion progress model
│   ├── QuizAttempt.cs              # Quiz attempt result model
│   ├── JobOpportunity.cs           # Job opportunity model
│   └── ViewModels/                 # Strongly typed view models for pages
│
├── Views/                          # Razor views (.cshtml)
│   ├── Shared/                     # Layout, validation scripts, admin sidebar
│   ├── Home/                       # Index, About
│   ├── Account/                    # Login, Register, AccessDenied
│   ├── Roadmaps/                   # Index, Details
│   ├── Lessons/                    # Details
│   ├── Quiz/                       # Start, Result
│   ├── Dashboard/                  # Index
│   ├── Progress/                   # Index
│   ├── Jobs/                       # Index, Details
│   ├── Profile/                    # Index, Edit
│   ├── AdminDashboard/             # Index
│   ├── AdminUsers/                 # Index, Create, Edit
│   ├── AdminRoadmaps/              # Index, Create, Edit
│   ├── AdminLessons/               # Index, Create, Edit
│   ├── AdminQuizzes/               # Index, Create, Edit
│   └── AdminJobs/                  # Index, Create, Edit
│
├── wwwroot/                        # Static web assets
│   ├── css/
│   │   ├── site.css                # Custom brand stylesheet
│   │   └── admin.css               # Custom admin sidebar stylesheet
│   ├── js/
│   │   ├── site.js                 # UI interactions & delete confirmations
│   │   ├── validation.js           # Password match validation
│   │   └── quiz.js                 # MCQ radio highlight & submit prompt
│
├── SQL/
│   ├── Database.sql                # DB creation script
│   ├── Tables.sql                  # Table schema script
│   ├── SeedData.sql                # Demo data seed script
│   └── StoredProcedures.sql        # Stored procedures script
│
├── appsettings.json                # Database connection string
├── Program.cs                      # Dependency injection, authentication, and pipeline
└── README.md                       # Comprehensive project documentation
```

---

## 8. Summary of Completed Assignment Requirements

- [x] Interlinked Webpages & Dynamic Navigation
- [x] HTML5 Semantic Elements (`<header>`, `<nav>`, `<main>`, `<section>`, `<article>`, `<footer>`)
- [x] External Custom CSS (`site.css`, `admin.css`) & Responsive Bootstrap 5 Grid
- [x] Client-side Processing & Form Validation (Vanilla JS, jQuery Validation)
- [x] Server-side Processing & Form Validation (ASP.NET Core ModelState, DataAnnotations, Controller checks)
- [x] ASP.NET Core 9.0 MVC Architecture (Strict Model-View-Controller separation)
- [x] Microsoft SQL Server Database (`GyanPathDB`) & **Pure ADO.NET Connectivity** (`SqlConnection`, `SqlCommand`, `SqlDataReader`)
- [x] Parameterized SQL Queries protecting against SQL Injection
- [x] **Full ADO.NET CRUD Demonstration** (INSERT, SELECT, UPDATE, DELETE) across Users, Roadmaps, Lessons, Quizzes, and Jobs
- [x] Secure Registration & Cookie Authentication with PBKDF2 Password Hashing
- [x] Role-Based Access Control (`Admin` vs `Member` vs `Guest`)
- [x] MCQ Assessment Engine with threshold calculation (>= 60% Pass) & attempt logging in SQL Server
- [x] Progress Tracking per user/roadmap calculated from database tables
- [x] Rule-Based Job & Internship Recommendation System
- [x] Multimedia Usage (Embedded YouTube tutorial videos & icons)
- [x] Search & Filtering for Roadmaps and Jobs
