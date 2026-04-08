using Microsoft.EntityFrameworkCore;
using VgcCollege.Application.Services;
using VgcCollege.Data;
using VgcCollege.Data.Repositories;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Application.Tests.Services;

/// <summary>
/// Testes unitários do LecturerService.
/// Foco principal nas regras de atribuição a cursos e na flag IsTutor.
/// </summary>
public class LecturerServiceTests
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
    public async Task CreateAsync_WithValidLecturer_ShouldPersistToDatabase()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var lecturerRepository = new LecturerRepository(context);
        var courseRepository = new CourseRepository(context);
        var assignmentRepository = new LecturerCourseAssignmentRepository(context);
        var service = new LecturerService(lecturerRepository, courseRepository, assignmentRepository);

        var lecturer = new LecturerProfile
        {
            IdentityUserId = "user-001",
            FirstName = "John",
            LastName = "Smith",
            Email = "john@vgc.ie"
        };

        // Act
        await service.CreateAsync(lecturer);

        // Assert
        var saved = await context.LecturerProfiles.FindAsync(lecturer.Id);
        Assert.NotNull(saved);
        Assert.Equal("John", saved.FirstName);
    }

    [Fact]
    public async Task CreateAsync_WithEmptyFirstName_ShouldThrowArgumentException()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var lecturerRepository = new LecturerRepository(context);
        var courseRepository = new CourseRepository(context);
        var assignmentRepository = new LecturerCourseAssignmentRepository(context);
        var service = new LecturerService(lecturerRepository, courseRepository, assignmentRepository);

        var lecturer = new LecturerProfile
        {
            IdentityUserId = "user-001",
            FirstName = "",
            LastName = "Smith",
            Email = "john@vgc.ie"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(lecturer));
    }

    [Fact]
    public async Task AssignToCourseAsync_WithValidData_ShouldCreateAssignment()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var lecturerRepository = new LecturerRepository(context);
        var courseRepository = new CourseRepository(context);
        var assignmentRepository = new LecturerCourseAssignmentRepository(context);
        var service = new LecturerService(lecturerRepository, courseRepository, assignmentRepository);

        var branch = new Branch { BranchName = "VGC Dublin", StreetName = "Street A", City = "Dublin" };
        await context.Branches.AddAsync(branch);
        await context.SaveChangesAsync();

        var course = new Course
        {
            CourseName = "Software Development",
            BranchId = branch.Id,
            StartDate = new DateOnly(2025, 9, 1),
            EndDate = new DateOnly(2026, 6, 30)
        };
        await context.Courses.AddAsync(course);
        await context.SaveChangesAsync();

        var lecturer = new LecturerProfile
        {
            IdentityUserId = "user-001",
            FirstName = "John",
            LastName = "Smith",
            Email = "john@vgc.ie"
        };
        await service.CreateAsync(lecturer);

        // Act
        await service.AssignToCourseAsync(lecturer.Id, course.Id, isTutor: true);

        // Assert
        var assignment = await context.LecturerCourseAssignments
            .FirstOrDefaultAsync(a => a.LecturerProfileId == lecturer.Id && a.CourseId == course.Id);
        Assert.NotNull(assignment);
        Assert.True(assignment.IsTutor);
    }

    [Fact]
    public async Task AssignToCourseAsync_WithIsTutorFalse_ShouldCreateAssignmentWithoutTutorAccess()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var lecturerRepository = new LecturerRepository(context);
        var courseRepository = new CourseRepository(context);
        var assignmentRepository = new LecturerCourseAssignmentRepository(context);
        var service = new LecturerService(lecturerRepository, courseRepository, assignmentRepository);

        var branch = new Branch { BranchName = "VGC Dublin", StreetName = "Street A", City = "Dublin" };
        await context.Branches.AddAsync(branch);
        await context.SaveChangesAsync();

        var course = new Course
        {
            CourseName = "Software Development",
            BranchId = branch.Id,
            StartDate = new DateOnly(2025, 9, 1),
            EndDate = new DateOnly(2026, 6, 30)
        };
        await context.Courses.AddAsync(course);
        await context.SaveChangesAsync();

        var lecturer = new LecturerProfile
        {
            IdentityUserId = "user-001",
            FirstName = "John",
            LastName = "Smith",
            Email = "john@vgc.ie"
        };
        await service.CreateAsync(lecturer);

        // Act
        await service.AssignToCourseAsync(lecturer.Id, course.Id, isTutor: false);

        // Assert
        var assignment = await context.LecturerCourseAssignments
            .FirstOrDefaultAsync(a => a.LecturerProfileId == lecturer.Id && a.CourseId == course.Id);
        Assert.NotNull(assignment);
        Assert.False(assignment.IsTutor);
    }

    [Fact]
    public async Task AssignToCourseAsync_WhenAlreadyAssigned_ShouldThrowInvalidOperationException()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var lecturerRepository = new LecturerRepository(context);
        var courseRepository = new CourseRepository(context);
        var assignmentRepository = new LecturerCourseAssignmentRepository(context);
        var service = new LecturerService(lecturerRepository, courseRepository, assignmentRepository);

        var branch = new Branch { BranchName = "VGC Dublin", StreetName = "Street A", City = "Dublin" };
        await context.Branches.AddAsync(branch);
        await context.SaveChangesAsync();

        var course = new Course
        {
            CourseName = "Software Development",
            BranchId = branch.Id,
            StartDate = new DateOnly(2025, 9, 1),
            EndDate = new DateOnly(2026, 6, 30)
        };
        await context.Courses.AddAsync(course);
        await context.SaveChangesAsync();

        var lecturer = new LecturerProfile
        {
            IdentityUserId = "user-001",
            FirstName = "John",
            LastName = "Smith",
            Email = "john@vgc.ie"
        };
        await service.CreateAsync(lecturer);
        await service.AssignToCourseAsync(lecturer.Id, course.Id, isTutor: false);

        // Act & Assert — segunda atribuição ao mesmo curso deve falhar
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AssignToCourseAsync(lecturer.Id, course.Id, isTutor: true));
    }

    [Fact]
    public async Task AssignToCourseAsync_WithNonExistingCourse_ShouldThrowInvalidOperationException()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var lecturerRepository = new LecturerRepository(context);
        var courseRepository = new CourseRepository(context);
        var assignmentRepository = new LecturerCourseAssignmentRepository(context);
        var service = new LecturerService(lecturerRepository, courseRepository, assignmentRepository);

        var lecturer = new LecturerProfile
        {
            IdentityUserId = "user-001",
            FirstName = "John",
            LastName = "Smith",
            Email = "john@vgc.ie"
        };
        await service.CreateAsync(lecturer);

        // Act & Assert — curso com Id 999 não existe no banco
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AssignToCourseAsync(lecturer.Id, courseId: 999, isTutor: false));
    }
}