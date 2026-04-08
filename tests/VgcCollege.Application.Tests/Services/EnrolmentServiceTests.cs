using Microsoft.EntityFrameworkCore;
using VgcCollege.Application.Services;
using VgcCollege.Data;
using VgcCollege.Data.Repositories;
using VgcCollege.Domain.Entities;
using VgcCollege.Domain.Enums;

namespace VgcCollege.Application.Tests.Services;

/// <summary>
/// Testes unitários do EnrolmentService.
/// Foco nas regras de negócio — prevenção de matrículas duplicadas
/// e validação de entidades existentes.
/// </summary>
public class EnrolmentServiceTests
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
    public async Task EnrolStudentAsync_WithValidData_ShouldCreateEnrolment()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var enrolmentRepository = new EnrolmentRepository(context);
        var studentRepository = new StudentRepository(context);
        var courseRepository = new CourseRepository(context);
        var service = new EnrolmentService(enrolmentRepository, studentRepository, courseRepository);

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

        var student = new StudentProfile
        {
            IdentityUserId = "user-001",
            FirstName = "Alice",
            LastName = "Murphy",
            Email = "alice@vgc.ie",
            StudentNumber = "VGC001"
        };
        await context.StudentProfiles.AddAsync(student);
        await context.SaveChangesAsync();

        // Act
        await service.EnrolStudentAsync(student.Id, course.Id);

        // Assert
        var enrolment = await context.CourseEnrolments
            .FirstOrDefaultAsync(e => e.StudentProfileId == student.Id && e.CourseId == course.Id);
        Assert.NotNull(enrolment);
        Assert.Equal(EnrolmentStatus.Active, enrolment.Status);
    }

    [Fact]
    public async Task EnrolStudentAsync_WhenAlreadyEnrolled_ShouldThrowInvalidOperationException()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var enrolmentRepository = new EnrolmentRepository(context);
        var studentRepository = new StudentRepository(context);
        var courseRepository = new CourseRepository(context);
        var service = new EnrolmentService(enrolmentRepository, studentRepository, courseRepository);

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

        var student = new StudentProfile
        {
            IdentityUserId = "user-001",
            FirstName = "Alice",
            LastName = "Murphy",
            Email = "alice@vgc.ie",
            StudentNumber = "VGC001"
        };
        await context.StudentProfiles.AddAsync(student);
        await context.SaveChangesAsync();

        await service.EnrolStudentAsync(student.Id, course.Id);

        // Act & Assert — segunda matrícula no mesmo curso deve falhar
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.EnrolStudentAsync(student.Id, course.Id));
    }

    [Fact]
    public async Task EnrolStudentAsync_WithNonExistingStudent_ShouldThrowInvalidOperationException()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var enrolmentRepository = new EnrolmentRepository(context);
        var studentRepository = new StudentRepository(context);
        var courseRepository = new CourseRepository(context);
        var service = new EnrolmentService(enrolmentRepository, studentRepository, courseRepository);

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

        // Act & Assert — aluno com Id 999 não existe
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.EnrolStudentAsync(studentProfileId: 999, course.Id));
    }

    [Fact]
    public async Task EnrolStudentAsync_WithNonExistingCourse_ShouldThrowInvalidOperationException()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var enrolmentRepository = new EnrolmentRepository(context);
        var studentRepository = new StudentRepository(context);
        var courseRepository = new CourseRepository(context);
        var service = new EnrolmentService(enrolmentRepository, studentRepository, courseRepository);

        var student = new StudentProfile
        {
            IdentityUserId = "user-001",
            FirstName = "Alice",
            LastName = "Murphy",
            Email = "alice@vgc.ie",
            StudentNumber = "VGC001"
        };
        await context.StudentProfiles.AddAsync(student);
        await context.SaveChangesAsync();

        // Act & Assert — curso com Id 999 não existe
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.EnrolStudentAsync(student.Id, courseId: 999));
    }

    [Fact]
    public async Task GetByStudentAsync_ShouldReturnOnlyEnrolmentsOfThatStudent()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var enrolmentRepository = new EnrolmentRepository(context);
        var studentRepository = new StudentRepository(context);
        var courseRepository = new CourseRepository(context);
        var service = new EnrolmentService(enrolmentRepository, studentRepository, courseRepository);

        var branch = new Branch { BranchName = "VGC Dublin", StreetName = "Street A", City = "Dublin" };
        await context.Branches.AddAsync(branch);
        await context.SaveChangesAsync();

        var course1 = new Course { CourseName = "Software Development", BranchId = branch.Id, StartDate = new DateOnly(2025, 9, 1), EndDate = new DateOnly(2026, 6, 30) };
        var course2 = new Course { CourseName = "Data Analytics", BranchId = branch.Id, StartDate = new DateOnly(2025, 9, 1), EndDate = new DateOnly(2026, 6, 30) };
        await context.Courses.AddRangeAsync(course1, course2);
        await context.SaveChangesAsync();

        var alice = new StudentProfile { IdentityUserId = "user-001", FirstName = "Alice", LastName = "Murphy", Email = "alice@vgc.ie", StudentNumber = "VGC001" };
        var brian = new StudentProfile { IdentityUserId = "user-002", FirstName = "Brian", LastName = "Kelly", Email = "brian@vgc.ie", StudentNumber = "VGC002" };
        await context.StudentProfiles.AddRangeAsync(alice, brian);
        await context.SaveChangesAsync();

        await service.EnrolStudentAsync(alice.Id, course1.Id);
        await service.EnrolStudentAsync(alice.Id, course2.Id);
        await service.EnrolStudentAsync(brian.Id, course1.Id);

        // Act — buscar apenas as matrículas de Alice
        var aliceEnrolments = await service.GetByStudentAsync(alice.Id);

        // Assert — apenas as 2 matrículas de Alice devem ser retornadas
        Assert.Equal(2, aliceEnrolments.Count());
        Assert.All(aliceEnrolments, e => Assert.Equal(alice.Id, e.StudentProfileId));
    }
}