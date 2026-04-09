using Microsoft.EntityFrameworkCore;
using VgcCollege.Application.Services;
using VgcCollege.Data;
using VgcCollege.Data.Repositories;
using VgcCollege.Domain.Entities;
using VgcCollege.Domain.Enums;

namespace VgcCollege.Application.Tests.Services;

/// <summary>
/// Testes de RBAC transversais. Cada teste simula um cenário de acesso indevido
/// e verifica que o service lança a excepção correcta antes de retornar qualquer dado.
/// </summary>
public class RbacTests
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
    public async Task StudentService_WhenStudentAccessesAnotherStudentProfile_ShouldThrow()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new StudentRepository(context);
        var service = new StudentService(repository);

        var alice = new StudentProfile
        {
            IdentityUserId = "alice-id",
            FirstName = "Alice",
            LastName = "Murphy",
            Email = "alice@vgc.ie",
            StudentNumber = "VGC001"
        };
        await context.StudentProfiles.AddAsync(alice);
        await context.SaveChangesAsync();

        // Act & Assert Brian tenta aceder ao perfil de Alice
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.GetByIdAsync(alice.Id, requestingUserId: "brian-id", isAdmin: false));
    }

    [Fact]
    public async Task StudentService_WhenAdminAccessesAnyProfile_ShouldNotThrow()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new StudentRepository(context);
        var service = new StudentService(repository);

        var alice = new StudentProfile
        {
            IdentityUserId = "alice-id",
            FirstName = "Alice",
            LastName = "Murphy",
            Email = "alice@vgc.ie",
            StudentNumber = "VGC001"
        };
        await context.StudentProfiles.AddAsync(alice);
        await context.SaveChangesAsync();

        // Act Admin acede ao perfil de Alice com um userId diferente
        var result = await service.GetByIdAsync(alice.Id, requestingUserId: "admin-id", isAdmin: true);

        // Assert Admin consegue aceder sem excepção
        Assert.NotNull(result);
    }
    

    [Fact]
    public async Task AttendanceService_WhenLecturerNotAssignedToCourse_ShouldThrow()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var attendanceRepository = new AttendanceRepository(context);
        var enrolmentRepository = new EnrolmentRepository(context);
        var assignmentRepository = new LecturerCourseAssignmentRepository(context);
        var service = new AttendanceService(attendanceRepository, enrolmentRepository, assignmentRepository);

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

        var enrolment = new CourseEnrolment
        {
            StudentProfileId = student.Id,
            CourseId = course.Id,
            EnrolDate = DateOnly.FromDateTime(DateTime.Today),
            Status = EnrolmentStatus.Active
        };
        await context.CourseEnrolments.AddAsync(enrolment);
        await context.SaveChangesAsync();

        // Act & Assert Lecturer com Id 999 não está atribuído ao curso
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.RecordAttendanceAsync(
                enrolment.Id,
                sessionDate: new DateOnly(2025, 10, 1),
                present: true,
                lecturerProfileId: 999));
    }
    

    [Fact]
    public async Task AssignmentService_WhenLecturerSetsResultForCourseNotAssigned_ShouldThrow()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var assignmentRepository = new AssignmentRepository(context);
        var courseRepository = new CourseRepository(context);
        var lecturerAssignmentRepository = new LecturerCourseAssignmentRepository(context);
        var service = new AssignmentService(assignmentRepository, courseRepository, lecturerAssignmentRepository);

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

        // Assignment no curso, mas o lecturer com Id 999 não está atribuído
        var assignment = new Assignment
        {
            CourseId = course.Id,
            Title = "Project CA1",
            MaxScore = 100,
            DueDate = new DateOnly(2025, 11, 1)
        };
        await context.Assignments.AddAsync(assignment);
        await context.SaveChangesAsync();

        var result = new AssignmentResult
        {
            AssignmentId = assignment.Id,
            StudentProfileId = 1,
            Score = 85
        };

        // Act & Assert, Lecturer não atribuído tenta lançar resultado
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SetResultAsync(result, lecturerProfileId: 999));
    }



    [Fact]
    public async Task ExamService_WhenStudentAccessesResultBeforeRelease_ShouldThrow()
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

        var course = new Course
        {
            CourseName = "Software Development",
            BranchId = branch.Id,
            StartDate = new DateOnly(2025, 9, 1),
            EndDate = new DateOnly(2026, 6, 30)
        };
        await context.Courses.AddAsync(course);
        await context.SaveChangesAsync();

        // Exame com ResultsReleased = false
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

        // Act & Assert Student tenta ver resultado antes da libertação
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.GetResultForStudentAsync(exam.Id, studentProfileId: 1));
    }

    [Fact]
    public async Task ExamService_WhenLecturerSetsResultForCourseNotAssigned_ShouldThrow()
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

        var course = new Course
        {
            CourseName = "Software Development",
            BranchId = branch.Id,
            StartDate = new DateOnly(2025, 9, 1),
            EndDate = new DateOnly(2026, 6, 30)
        };
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

        var result = new ExamResult
        {
            ExamId = exam.Id,
            StudentProfileId = 1,
            Score = 85
        };

        // Act & Assert Lecturer com Id 999 não está atribuído ao curso
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SetResultAsync(result, lecturerProfileId: 999));
    }
}