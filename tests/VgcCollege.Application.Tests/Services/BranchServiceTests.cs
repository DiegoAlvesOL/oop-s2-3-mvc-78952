// Purpose   : Testes unitários do BranchService.
//             Verifica as regras de negócio de criação, validação e listagem de branches.
//             Usa SQLite in-memory para isolar os testes do banco MySQL real.
// Layer     : Tests — Application

using Microsoft.EntityFrameworkCore;
using VgcCollege.Application.Services;
using VgcCollege.Data;
using VgcCollege.Data.Repositories;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Application.Tests.Services;

/// <summary>
/// Testes unitários do BranchService.
/// Cada teste cria o seu próprio banco SQLite in-memory para garantir isolamento completo.
/// </summary>
public class BranchServiceTests
{
    /// <summary>
    /// Cria um AppDbContext limpo usando SQLite in-memory.
    /// Cada chamada gera um banco com nome único para evitar conflitos entre testes paralelos.
    /// </summary>
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
    public async Task CreateAsync_WithValidBranch_ShouldPersistToDatabase()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new BranchRepository(context);
        var service = new BranchService(repository);

        var branch = new Branch
        {
            BranchName = "VGC Dublin",
            StreetName = "123 Grafton Street",
            City = "Dublin"
        };

        // Act
        await service.CreateAsync(branch);

        // Assert
        var savedBranch = await context.Branches.FindAsync(branch.Id);
        Assert.NotNull(savedBranch);
        Assert.Equal("VGC Dublin", savedBranch.BranchName);
        Assert.Equal("Dublin", savedBranch.City);
    }

    [Fact]
    public async Task CreateAsync_WithEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new BranchRepository(context);
        var service = new BranchService(repository);

        var branch = new Branch
        {
            BranchName = "",
            StreetName = "123 Grafton Street",
            City = "Dublin"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(branch));
    }

    [Fact]
    public async Task CreateAsync_WithWhitespaceName_ShouldThrowArgumentException()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new BranchRepository(context);
        var service = new BranchService(repository);

        var branch = new Branch
        {
            BranchName = "   ",
            StreetName = "123 Grafton Street",
            City = "Dublin"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(branch));
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleBranches_ShouldReturnAllBranches()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new BranchRepository(context);
        var service = new BranchService(repository);

        await service.CreateAsync(new Branch { BranchName = "VGC Dublin", StreetName = "Street A", City = "Dublin" });
        await service.CreateAsync(new Branch { BranchName = "VGC Cork", StreetName = "Street B", City = "Cork" });
        await service.CreateAsync(new Branch { BranchName = "VGC Galway", StreetName = "Street C", City = "Galway" });

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnCorrectBranch()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new BranchRepository(context);
        var service = new BranchService(repository);

        var branch = new Branch { BranchName = "VGC Dublin", StreetName = "Street A", City = "Dublin" };
        await service.CreateAsync(branch);

        // Act
        var result = await service.GetByIdAsync(branch.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("VGC Dublin", result.BranchName);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new BranchRepository(context);
        var service = new BranchService(repository);

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }
}