using Microsoft.EntityFrameworkCore;
using VgcCollege.Application.Services;
using VgcCollege.Data;
using VgcCollege.Data.Repositories;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Application.Tests.Services;

/// <summary>
/// Testes unitários do StudentService.
/// Foco principal nas regras de acesso verificar que um student
/// não consegue aceder ao perfil de outro student.
/// </summary>
public class StudentServiceTests
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
    public async Task CreateAsync_WithValidStudent_ShouldPersistToDatabase()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new StudentRepository(context);
        var service = new StudentService(repository);

        var student = new StudentProfile
        {
            IdentityUserId = "user-001",
            FirstName = "Alice",
            LastName = "Murphy",
            Email = "alice@vgc.ie",
            StudentNumber = "VGC001"
        };

        // Act
        await service.CreateAsync(student);

        // Assert
        var saved = await context.StudentProfiles.FindAsync(student.Id);
        Assert.NotNull(saved);
        Assert.Equal("Alice", saved.FirstName);
        Assert.Equal("VGC001", saved.StudentNumber);
    }

    [Fact]
    public async Task CreateAsync_WithEmptyFirstName_ShouldThrowArgumentException()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new StudentRepository(context);
        var service = new StudentService(repository);

        var student = new StudentProfile
        {
            IdentityUserId = "user-001",
            FirstName = "",
            LastName = "Murphy",
            Email = "alice@vgc.ie",
            StudentNumber = "VGC001"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(student));
    }

    [Fact]
    public async Task GetByIdAsync_WhenStudentAccessesOwnProfile_ShouldReturnProfile()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new StudentRepository(context);
        var service = new StudentService(repository);

        var student = new StudentProfile
        {
            IdentityUserId = "user-001",
            FirstName = "Alice",
            LastName = "Murphy",
            Email = "alice@vgc.ie",
            StudentNumber = "VGC001"
        };

        await service.CreateAsync(student);

        // Act — student acede ao próprio perfil
        var result = await service.GetByIdAsync(student.Id, requestingUserId: "user-001", isAdmin: false);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Alice", result.FirstName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenStudentAccessesAnotherStudentProfile_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new StudentRepository(context);
        var service = new StudentService(repository);

        var alice = new StudentProfile
        {
            IdentityUserId = "user-001",
            FirstName = "Alice",
            LastName = "Murphy",
            Email = "alice@vgc.ie",
            StudentNumber = "VGC001"
        };

        await service.CreateAsync(alice);

        // Act & Assert — Brian tenta aceder ao perfil de Alice
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.GetByIdAsync(alice.Id, requestingUserId: "user-002", isAdmin: false));
    }

    [Fact]
    public async Task GetByIdAsync_WhenAdminAccessesAnyProfile_ShouldReturnProfile()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new StudentRepository(context);
        var service = new StudentService(repository);

        var student = new StudentProfile
        {
            IdentityUserId = "user-001",
            FirstName = "Alice",
            LastName = "Murphy",
            Email = "alice@vgc.ie",
            StudentNumber = "VGC001"
        };

        await service.CreateAsync(student);

        // Act — Admin acede ao perfil de Alice com um userId diferente
        var result = await service.GetByIdAsync(student.Id, requestingUserId: "admin-user", isAdmin: true);

        // Assert — Admin consegue aceder mesmo sendo userId diferente
        Assert.NotNull(result);
        Assert.Equal("Alice", result.FirstName);
    }
}