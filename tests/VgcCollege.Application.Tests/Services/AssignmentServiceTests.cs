using Microsoft.EntityFrameworkCore;
using VgcCollege.Application.Services;
using VgcCollege.Data;
using VgcCollege.Data.Repositories;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Application.Tests.Services;

/// <summary>
/// Testes unitários do AssignmentService.
/// Foco nas regras de negócio — validação de score e acesso por lecturer.
/// </summary>
public class AssignmentServiceTests
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
    public async Task CreateAssignmentAsync_WithValidData_ShouldPersistToDatabase()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var assignmentRepository = new AssignmentRepository(context);
        var courseRepository = new CourseRepository(context);
        var assignmentLecturerRepository = new LecturerCourseAssignmentRepository(context);
        var service = new AssignmentService(assignmentRepository, courseRepository, assignmentLecturerRepository);

        var branch = new Branch { BranchName = "VGC Dublin", StreetName = "Street A", City = "Dublin" };
        await context.Branches.AddAsync(branch);
        await context.SaveChangesAsync();

        var course = new Course { CourseName = "Software Development", BranchId = branch.Id, StartDate = new DateOnly(2025, 9, 1), EndDate = new DateOnly(2026, 6, 30) };
        await context.Courses.AddAsync(course);
        await context.SaveChangesAsync();

        var lecturer = new LecturerProfile { IdentityUserId = "lecturer-001", FirstName = "John", LastName = "Smith", Email = "john@vgc.ie" };
        await context.LecturerProfiles.AddAsync(lecturer);
        await context.SaveChangesAsync();

        await context.LecturerCourseAssignments.AddAsync(new LecturerCourseAssignment { LecturerProfileId = lecturer.Id, CourseId = course.Id, IsTutor = true });
        await context.SaveChangesAsync();

        var assignment = new Assignment { CourseId = course.Id, Title = "Project CA1", MaxScore = 100, DueDate = new DateOnly(2025, 11, 1) };

        // Act
        await service.CreateAssignmentAsync(assignment, lecturer.Id);

        // Assert
        var saved = await context.Assignments.FindAsync(assignment.Id);
        Assert.NotNull(saved);
        Assert.Equal("Project CA1", saved.Title);
        Assert.Equal(100, saved.MaxScore);
    }

    [Fact]
    public async Task CreateAssignmentAsync_WithEmptyTitle_ShouldThrowArgumentException()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var assignmentRepository = new AssignmentRepository(context);
        var courseRepository = new CourseRepository(context);
        var assignmentLecturerRepository = new LecturerCourseAssignmentRepository(context);
        var service = new AssignmentService(assignmentRepository, courseRepository, assignmentLecturerRepository);

        var assignment = new Assignment { CourseId = 1, Title = "", MaxScore = 100, DueDate = new DateOnly(2025, 11, 1) };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAssignmentAsync(assignment, lecturerProfileId: 1));
    }

    [Fact]
    public async Task SetResultAsync_WithScoreAboveMaxScore_ShouldThrowArgumentException()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var assignmentRepository = new AssignmentRepository(context);
        var courseRepository = new CourseRepository(context);
        var assignmentLecturerRepository = new LecturerCourseAssignmentRepository(context);
        var service = new AssignmentService(assignmentRepository, courseRepository, assignmentLecturerRepository);

        var branch = new Branch { BranchName = "VGC Dublin", StreetName = "Street A", City = "Dublin" };
        await context.Branches.AddAsync(branch);
        await context.SaveChangesAsync();

        var course = new Course { CourseName = "Software Development", BranchId = branch.Id, StartDate = new DateOnly(2025, 9, 1), EndDate = new DateOnly(2026, 6, 30) };
        await context.Courses.AddAsync(course);
        await context.SaveChangesAsync();

        var lecturer = new LecturerProfile { IdentityUserId = "lecturer-001", FirstName = "John", LastName = "Smith", Email = "john@vgc.ie" };
        await context.LecturerProfiles.AddAsync(lecturer);
        await context.SaveChangesAsync();

        await context.LecturerCourseAssignments.AddAsync(new LecturerCourseAssignment { LecturerProfileId = lecturer.Id, CourseId = course.Id, IsTutor = true });
        await context.SaveChangesAsync();

        var assignment = new Assignment { CourseId = course.Id, Title = "Project CA1", MaxScore = 100, DueDate = new DateOnly(2025, 11, 1) };
        await context.Assignments.AddAsync(assignment);
        await context.SaveChangesAsync();

        // Score de 150 excede MaxScore de 100
        var result = new AssignmentResult { AssignmentId = assignment.Id, StudentProfileId = 1, Score = 150 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.SetResultAsync(result, lecturer.Id));
    }

    [Fact]
    public async Task SetResultAsync_WithNegativeScore_ShouldThrowArgumentException()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var assignmentRepository = new AssignmentRepository(context);
        var courseRepository = new CourseRepository(context);
        var assignmentLecturerRepository = new LecturerCourseAssignmentRepository(context);
        var service = new AssignmentService(assignmentRepository, courseRepository, assignmentLecturerRepository);

        var branch = new Branch { BranchName = "VGC Dublin", StreetName = "Street A", City = "Dublin" };
        await context.Branches.AddAsync(branch);
        await context.SaveChangesAsync();

        var course = new Course { CourseName = "Software Development", BranchId = branch.Id, StartDate = new DateOnly(2025, 9, 1), EndDate = new DateOnly(2026, 6, 30) };
        await context.Courses.AddAsync(course);
        await context.SaveChangesAsync();

        var lecturer = new LecturerProfile { IdentityUserId = "lecturer-001", FirstName = "John", LastName = "Smith", Email = "john@vgc.ie" };
        await context.LecturerProfiles.AddAsync(lecturer);
        await context.SaveChangesAsync();

        await context.LecturerCourseAssignments.AddAsync(new LecturerCourseAssignment { LecturerProfileId = lecturer.Id, CourseId = course.Id, IsTutor = true });
        await context.SaveChangesAsync();

        var assignment = new Assignment { CourseId = course.Id, Title = "Project CA1", MaxScore = 100, DueDate = new DateOnly(2025, 11, 1) };
        await context.Assignments.AddAsync(assignment);
        await context.SaveChangesAsync();

        var result = new AssignmentResult { AssignmentId = assignment.Id, StudentProfileId = 1, Score = -5 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.SetResultAsync(result, lecturer.Id));
    }

    [Fact]
    public async Task SetResultAsync_WhenLecturerNotAssignedToCourse_ShouldThrowInvalidOperationException()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var assignmentRepository = new AssignmentRepository(context);
        var courseRepository = new CourseRepository(context);
        var assignmentLecturerRepository = new LecturerCourseAssignmentRepository(context);
        var service = new AssignmentService(assignmentRepository, courseRepository, assignmentLecturerRepository);

        var branch = new Branch { BranchName = "VGC Dublin", StreetName = "Street A", City = "Dublin" };
        await context.Branches.AddAsync(branch);
        await context.SaveChangesAsync();

        var course = new Course { CourseName = "Software Development", BranchId = branch.Id, StartDate = new DateOnly(2025, 9, 1), EndDate = new DateOnly(2026, 6, 30) };
        await context.Courses.AddAsync(course);
        await context.SaveChangesAsync();

        var assignment = new Assignment { CourseId = course.Id, Title = "Project CA1", MaxScore = 100, DueDate = new DateOnly(2025, 11, 1) };
        await context.Assignments.AddAsync(assignment);
        await context.SaveChangesAsync();

        var result = new AssignmentResult { AssignmentId = assignment.Id, StudentProfileId = 1, Score = 85 };

        // Act & Assert — lecturer com Id 999 não está atribuído ao curso
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.SetResultAsync(result, lecturerProfileId: 999));
    }
    
    [Fact] 
    public async Task GetResultsByStudentAsync_ShouldReturnOnlyResultsOfThatStudent() 
    { 
        // Arrange
        await using var context = CreateInMemoryContext();
        var assignmentRepository = new AssignmentRepository(context);
        var courseRepository = new CourseRepository(context);
        var assignmentLecturerRepository = new LecturerCourseAssignmentRepository(context);
        var service = new AssignmentService(assignmentRepository, courseRepository, assignmentLecturerRepository);

        var branch = new Branch { BranchName = "VGC Dublin", StreetName = "Street A", City = "Dublin" };
        await context.Branches.AddAsync(branch);
        await context.SaveChangesAsync();

        var course = new Course { CourseName = "Software Development", BranchId = branch.Id, StartDate = new DateOnly(2025, 9, 1), EndDate = new DateOnly(2026, 6, 30) };
        await context.Courses.AddAsync(course);
        await context.SaveChangesAsync();

        var assignment = new Assignment { CourseId = course.Id, Title = "Project CA1", MaxScore = 100, DueDate = new DateOnly(2025, 11, 1) };
        await context.Assignments.AddAsync(assignment);
        await context.SaveChangesAsync();

        // Criar os dois alunos antes de inserir os resultados
        var alice = new StudentProfile { IdentityUserId = "user-001", FirstName = "Alice", LastName = "Murphy", Email = "alice@vgc.ie", StudentNumber = "VGC001" };
        var brian = new StudentProfile { IdentityUserId = "user-002", FirstName = "Brian", LastName = "Kelly", Email = "brian@vgc.ie", StudentNumber = "VGC002" };
        await context.StudentProfiles.AddRangeAsync(alice, brian);
        await context.SaveChangesAsync();

        // Resultados de dois alunos diferentes no mesmo assignment
        await context.AssignmentResults.AddRangeAsync(
            new AssignmentResult { AssignmentId = assignment.Id, StudentProfileId = alice.Id, Score = 85 },
            new AssignmentResult { AssignmentId = assignment.Id, StudentProfileId = brian.Id, Score = 70 }
        );
        await context.SaveChangesAsync();

        // Act — buscar apenas os resultados de Alice
        var results = await service.GetResultsByStudentAsync(alice.Id);

        // Assert — apenas o resultado de Alice deve ser retornado
        Assert.Single(results);
        Assert.All(results, result => Assert.Equal(alice.Id, result.StudentProfileId)); 
    }
}