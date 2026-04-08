using Microsoft.EntityFrameworkCore;
using VgcCollege.Application.Services;
using VgcCollege.Data;
using VgcCollege.Data.Repositories;
using VgcCollege.Domain.Entities;
using VgcCollege.Domain.Enums;

namespace VgcCollege.Application.Tests.Services;

/// <summary>
/// Testes unitários do AttendanceService.
/// Foco nas regras de negócio — lecturer só regista para os seus cursos
/// e prevenção de registos duplicados.
/// </summary>
public class AttendanceServiceTests
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
    public async Task RecordAttendanceAsync_WithValidData_ShouldCreateAttendanceRecord()
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

        var course = new Course { CourseName = "Software Development", BranchId = branch.Id, StartDate = new DateOnly(2025, 9, 1), EndDate = new DateOnly(2026, 6, 30) };
        await context.Courses.AddAsync(course);
        await context.SaveChangesAsync();

        var student = new StudentProfile { IdentityUserId = "user-001", FirstName = "Alice", LastName = "Murphy", Email = "alice@vgc.ie", StudentNumber = "VGC001" };
        await context.StudentProfiles.AddAsync(student);
        await context.SaveChangesAsync();

        var lecturer = new LecturerProfile { IdentityUserId = "lecturer-001", FirstName = "John", LastName = "Smith", Email = "john@vgc.ie" };
        await context.LecturerProfiles.AddAsync(lecturer);
        await context.SaveChangesAsync();

        // Atribuir o lecturer ao curso
        await context.LecturerCourseAssignments.AddAsync(new LecturerCourseAssignment
        {
            LecturerProfileId = lecturer.Id,
            CourseId = course.Id,
            IsTutor = true
        });

        // Criar a matrícula do aluno no curso
        var enrolment = new CourseEnrolment
        {
            StudentProfileId = student.Id,
            CourseId = course.Id,
            EnrolDate = DateOnly.FromDateTime(DateTime.Today),
            Status = EnrolmentStatus.Active
        };
        await context.CourseEnrolments.AddAsync(enrolment);
        await context.SaveChangesAsync();

        // Act
        await service.RecordAttendanceAsync(
            enrolment.Id,
            sessionDate: new DateOnly(2025, 10, 1),
            present: true,
            lecturerProfileId: lecturer.Id);

        // Assert
        var record = await context.AttendanceRecords
            .FirstOrDefaultAsync(r => r.CourseEnrolmentId == enrolment.Id);
        Assert.NotNull(record);
        Assert.True(record.Present);
        Assert.Equal(new DateOnly(2025, 10, 1), record.SessionDate);
    }

    [Fact]
    public async Task RecordAttendanceAsync_WhenLecturerNotAssignedToCourse_ShouldThrowInvalidOperationException()
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

        var course = new Course { CourseName = "Software Development", BranchId = branch.Id, StartDate = new DateOnly(2025, 9, 1), EndDate = new DateOnly(2026, 6, 30) };
        await context.Courses.AddAsync(course);
        await context.SaveChangesAsync();

        var student = new StudentProfile { IdentityUserId = "user-001", FirstName = "Alice", LastName = "Murphy", Email = "alice@vgc.ie", StudentNumber = "VGC001" };
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

        // Act & Assert — lecturer com Id 999 não está atribuído ao curso
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.RecordAttendanceAsync(
                enrolment.Id,
                sessionDate: new DateOnly(2025, 10, 1),
                present: true,
                lecturerProfileId: 999));
    }

    [Fact]
    public async Task RecordAttendanceAsync_WhenDuplicateSession_ShouldThrowInvalidOperationException()
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

        var course = new Course { CourseName = "Software Development", BranchId = branch.Id, StartDate = new DateOnly(2025, 9, 1), EndDate = new DateOnly(2026, 6, 30) };
        await context.Courses.AddAsync(course);
        await context.SaveChangesAsync();

        var student = new StudentProfile { IdentityUserId = "user-001", FirstName = "Alice", LastName = "Murphy", Email = "alice@vgc.ie", StudentNumber = "VGC001" };
        await context.StudentProfiles.AddAsync(student);
        await context.SaveChangesAsync();

        var lecturer = new LecturerProfile { IdentityUserId = "lecturer-001", FirstName = "John", LastName = "Smith", Email = "john@vgc.ie" };
        await context.LecturerProfiles.AddAsync(lecturer);
        await context.SaveChangesAsync();

        await context.LecturerCourseAssignments.AddAsync(new LecturerCourseAssignment
        {
            LecturerProfileId = lecturer.Id,
            CourseId = course.Id,
            IsTutor = false
        });

        var enrolment = new CourseEnrolment
        {
            StudentProfileId = student.Id,
            CourseId = course.Id,
            EnrolDate = DateOnly.FromDateTime(DateTime.Today),
            Status = EnrolmentStatus.Active
        };
        await context.CourseEnrolments.AddAsync(enrolment);
        await context.SaveChangesAsync();

        var sessionDate = new DateOnly(2025, 10, 1);
        await service.RecordAttendanceAsync(enrolment.Id, sessionDate, present: true, lecturer.Id);

        // Act & Assert — segundo registo para a mesma sessão deve falhar
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.RecordAttendanceAsync(enrolment.Id, sessionDate, present: false, lecturer.Id));
    }

    [Fact]
    public async Task RecordAttendanceAsync_WithNonExistingEnrolment_ShouldThrowInvalidOperationException()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var attendanceRepository = new AttendanceRepository(context);
        var enrolmentRepository = new EnrolmentRepository(context);
        var assignmentRepository = new LecturerCourseAssignmentRepository(context);
        var service = new AttendanceService(attendanceRepository, enrolmentRepository, assignmentRepository);

        // Act & Assert — matrícula com Id 999 não existe
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.RecordAttendanceAsync(
                enrolmentId: 999,
                sessionDate: new DateOnly(2025, 10, 1),
                present: true,
                lecturerProfileId: 1));
    }
}