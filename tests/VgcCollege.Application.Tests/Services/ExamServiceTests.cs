using Microsoft.EntityFrameworkCore;
using VgcCollege.Application.Services;
using VgcCollege.Data;
using VgcCollege.Data.Repositories;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Application.Tests.Services;

/// <summary>
/// Testes unitários do ExamService.
/// A regra ResultsReleased é o foco central destes testes.
/// </summary>
public class ExamServiceTests
{
    private static AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        var context = new AppDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();

        return context;
    }

    [Fact]
    public async Task GetResultForStudentAsync_WhenResultsNotReleased_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var examRepository = new ExamRepository(context);
        var courseRepository = new CourseRepository(context);
        var assignmentRepository = new LecturerCourseAssignmentRepository(context);
        var service = new ExamService(examRepository, courseRepository, assignmentRepository);

        var branch = new Branch { BranchName = "VGC Dublin", StreetName = "Street A", City = "Dublin" };
        await context.Branches.AddAsync(branch);
        await context.SaveChangesAsync();

        var course = new Course { CourseName = "Software Development", BranchId = branch.Id, StartDate = new DateOnly(2025, 9, 1), EndDate = new DateOnly(2026, 6, 30) };
        await context.Courses.AddAsync(course);
        await context.SaveChangesAsync();

        // Exame com ResultsReleased = false — resultado não publicado
        var exam = new Exam
        {
            CourseId = course.Id,
            Title = "Final Exam",
            ExamDate = new DateOnly(2025, 12, 1),
            MaxScore = 100,
            ResultsReleased = false
        };
        await context.Exams.AddAsync(exam);
        await context.SaveChangesAsync();

        // Act & Assert — Student tenta ver resultado antes da libertação
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.GetResultForStudentAsync(exam.Id, studentProfileId: 1));
    }

    [Fact]
    public async Task GetResultForStudentAsync_WhenResultsReleased_ShouldReturnResult()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var examRepository = new ExamRepository(context);
        var courseRepository = new CourseRepository(context);
        var assignmentRepository = new LecturerCourseAssignmentRepository(context);
        var service = new ExamService(examRepository, courseRepository, assignmentRepository);

        var branch = new Branch { BranchName = "VGC Dublin", StreetName = "Street A", City = "Dublin" };
        await context.Branches.AddAsync(branch);
        await context.SaveChangesAsync();

        var course = new Course { CourseName = "Software Development", BranchId = branch.Id, StartDate = new DateOnly(2025, 9, 1), EndDate = new DateOnly(2026, 6, 30) };
        await context.Courses.AddAsync(course);
        await context.SaveChangesAsync();

        var student = new StudentProfile { IdentityUserId = "user-001", FirstName = "Alice", LastName = "Murphy", Email = "alice@vgc.ie", StudentNumber = "VGC001" };
        await context.StudentProfiles.AddAsync(student);
        await context.SaveChangesAsync();

        // Exame com ResultsReleased = true — resultado publicado
        var exam = new Exam
        {
            CourseId = course.Id,
            Title = "Final Exam",
            ExamDate = new DateOnly(2025, 12, 1),
            MaxScore = 100,
            ResultsReleased = true
        };
        await context.Exams.AddAsync(exam);
        await context.SaveChangesAsync();

        await context.ExamResults.AddAsync(new ExamResult
        {
            ExamId = exam.Id,
            StudentProfileId = student.Id,
            Score = 85,
            Grade = "A"
        });
        await context.SaveChangesAsync();

        // Act — Student acede ao resultado após libertação
        var result = await service.GetResultForStudentAsync(exam.Id, student.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(85, result.Score);
        Assert.Equal("A", result.Grade);
    }

    [Fact]
    public async Task ReleaseResultsAsync_ShouldSetResultsReleasedToTrue()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var examRepository = new ExamRepository(context);
        var courseRepository = new CourseRepository(context);
        var assignmentRepository = new LecturerCourseAssignmentRepository(context);
        var service = new ExamService(examRepository, courseRepository, assignmentRepository);

        var branch = new Branch { BranchName = "VGC Dublin", StreetName = "Street A", City = "Dublin" };
        await context.Branches.AddAsync(branch);
        await context.SaveChangesAsync();

        var course = new Course { CourseName = "Software Development", BranchId = branch.Id, StartDate = new DateOnly(2025, 9, 1), EndDate = new DateOnly(2026, 6, 30) };
        await context.Courses.AddAsync(course);
        await context.SaveChangesAsync();

        var exam = new Exam
        {
            CourseId = course.Id,
            Title = "Final Exam",
            ExamDate = new DateOnly(2025, 12, 1),
            MaxScore = 100,
            ResultsReleased = false
        };
        await context.Exams.AddAsync(exam);
        await context.SaveChangesAsync();

        // Act — Admin liberta os resultados
        await service.ReleaseResultsAsync(exam.Id);

        // Assert — ResultsReleased deve ser true após a libertação
        var updated = await context.Exams.FindAsync(exam.Id);
        Assert.NotNull(updated);
        Assert.True(updated.ResultsReleased);
    }

    [Fact]
    public async Task SetResultAsync_WithScoreAboveMaxScore_ShouldThrowArgumentException()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var examRepository = new ExamRepository(context);
        var courseRepository = new CourseRepository(context);
        var assignmentRepository = new LecturerCourseAssignmentRepository(context);
        var service = new ExamService(examRepository, courseRepository, assignmentRepository);

        var branch = new Branch { BranchName = "VGC Dublin", StreetName = "Street A", City = "Dublin" };
        await context.Branches.AddAsync(branch);
        await context.SaveChangesAsync();

        var course = new Course { CourseName = "Software Development", BranchId = branch.Id, StartDate = new DateOnly(2025, 9, 1), EndDate = new DateOnly(2026, 6, 30) };
        await context.Courses.AddAsync(course);
        await context.SaveChangesAsync();

        var lecturer = new LecturerProfile { IdentityUserId = "lecturer-001", FirstName = "John", LastName = "Smith", Email = "john@vgc.ie" };
        await context.LecturerProfiles.AddAsync(lecturer);
        await context.SaveChangesAsync();

        await context.LecturerCourseAssignments.AddAsync(new LecturerCourseAssignment
        {
            LecturerProfileId = lecturer.Id,
            CourseId = course.Id,
            IsTutor = true
        });
        await context.SaveChangesAsync();

        var exam = new Exam { CourseId = course.Id, Title = "Final Exam", ExamDate = new DateOnly(2025, 12, 1), MaxScore = 100, ResultsReleased = false };
        await context.Exams.AddAsync(exam);
        await context.SaveChangesAsync();

        var result = new ExamResult { ExamId = exam.Id, StudentProfileId = 1, Score = 150 };

        // Act & Assert — score de 150 excede MaxScore de 100
        await Assert.ThrowsAsync<ArgumentException>(() => service.SetResultAsync(result, lecturer.Id));
    }

    [Fact]
    public async Task GetResultForStudentAsync_AfterReleaseResultsAsync_ShouldBeAccessible()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var examRepository = new ExamRepository(context);
        var courseRepository = new CourseRepository(context);
        var assignmentRepository = new LecturerCourseAssignmentRepository(context);
        var service = new ExamService(examRepository, courseRepository, assignmentRepository);

        var branch = new Branch { BranchName = "VGC Dublin", StreetName = "Street A", City = "Dublin" };
        await context.Branches.AddAsync(branch);
        await context.SaveChangesAsync();

        var course = new Course { CourseName = "Software Development", BranchId = branch.Id, StartDate = new DateOnly(2025, 9, 1), EndDate = new DateOnly(2026, 6, 30) };
        await context.Courses.AddAsync(course);
        await context.SaveChangesAsync();

        var student = new StudentProfile { IdentityUserId = "user-001", FirstName = "Alice", LastName = "Murphy", Email = "alice@vgc.ie", StudentNumber = "VGC001" };
        await context.StudentProfiles.AddAsync(student);
        await context.SaveChangesAsync();

        var exam = new Exam { CourseId = course.Id, Title = "Final Exam", ExamDate = new DateOnly(2025, 12, 1), MaxScore = 100, ResultsReleased = false };
        await context.Exams.AddAsync(exam);
        await context.SaveChangesAsync();

        await context.ExamResults.AddAsync(new ExamResult { ExamId = exam.Id, StudentProfileId = student.Id, Score = 78 });
        await context.SaveChangesAsync();

        // Verificar que antes da libertação o Student não consegue ver
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.GetResultForStudentAsync(exam.Id, student.Id));

        // Admin liberta os resultados
        await service.ReleaseResultsAsync(exam.Id);

        // Act — Student tenta novamente após libertação
        var result = await service.GetResultForStudentAsync(exam.Id, student.Id);

        // Assert — agora consegue ver
        Assert.NotNull(result);
        Assert.Equal(78, result.Score);
    }
}