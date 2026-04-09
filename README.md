# VGC College — Academic Management System

ASP.NET Core MVC web application for academic management, built with Clean Architecture across four source projects and three test projects.

## Stack

- **Framework:** ASP.NET Core MVC (.NET 9.0)
- **ORM:** Entity Framework Core 9 with Pomelo MySQL provider
- **Database:** MySQL 8
- **Authentication:** ASP.NET Core Identity
- **Logging:** Serilog (console + rolling file)
- **Testing:** xUnit + SQLite in-memory + Coverlet
- **CI:** GitHub Actions

## Architecture

```
src/
  VgcCollege.Domain/        ← Entities, enums, constants (no external dependencies)
  VgcCollege.Application/   ← Interfaces, services, business rules
  VgcCollege.Data/          ← EF Core, repositories, migrations, seed data
  VgcCollege.Web/           ← Controllers, views, viewmodels

tests/
  VgcCollege.Domain.Tests/
  VgcCollege.Application.Tests/   ← 43 tests passing
  VgcCollege.Data.Tests/
```

## Setup

### Prerequisites

- .NET 9.0 SDK
- MySQL 8 running locally

### 1. Clone the repository

```bash
git clone https://github.com/DiegoAlvesOL/oop-s2-3-mvc-78952.git
cd oop-s2-3-mvc-78952
```

### 2. Configure the database connection

Create `src/VgcCollege.Web/appsettings.Development.json` (not tracked by git):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=VgcCollegeDb;User=root;Password=YOUR_PASSWORD;"
  }
}
```

### 3. Apply migrations and run

```bash
cd src/VgcCollege.Web
dotnet run
```

The application applies all migrations and seeds initial data automatically on first run.

### 4. Run tests

```bash
dotnet test --verbosity normal
```

## Demo credentials

| Role     | Email              | Password     |
|----------|--------------------|--------------|
| Admin    | admin@vgc.ie       | Admin123!    |
| Lecturer | lecturer@vgc.ie    | Lecturer123! |
| Student  | student1@vgc.ie    | Student123!  |
| Student  | student2@vgc.ie    | Student123!  |

## Seed data

The application seeds the following data on first run:

- 3 branches (Dublin, Cork, Galway)
- 6 courses distributed across branches
- 1 lecturer profile
- 2 student profiles (Eva Alves, Arquimedes Oliveira)
- Enrolments, 4 weeks of attendance records
- 1 assignment with results
- 2 exams — one with results released, one provisional

## RBAC

| Action                        | Admin | Lecturer | Student |
|-------------------------------|-------|----------|---------|
| Manage branches and courses   | ✓     |          |         |
| Manage student profiles       | ✓     |          |         |
| Manage lecturer profiles      | ✓     |          |         |
| Enrol students                | ✓     |          |         |
| Record attendance             |       | ✓        |         |
| Create assignments and exams  |       | ✓        |         |
| Launch results                |       | ✓        |         |
| Release exam results          | ✓     |          |         |
| View own profile              |       |          | ✓       |
| View own grades               |       |          | ✓       |
| View own exam results         |       |          | ✓ *     |

*Only after Admin releases results (`ResultsReleased = true`).

## Key design decisions

**Single migration:** all 19 tables created in one migration (`InitialSchema`) to simplify setup and avoid migration chain issues in a demonstration environment.

**ApplicationUser in Data layer:** avoids circular references between Web and Data projects.

**ResultsReleased enforced in service layer:** students cannot access exam results regardless of UI state — the check happens in `ExamService.GetResultForStudentAsync()` before any data is returned.

**SQLite in-memory for tests:** isolates tests completely from the MySQL database. Each test creates its own clean context.