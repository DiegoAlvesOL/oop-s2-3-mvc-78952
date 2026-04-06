

using Microsoft.EntityFrameworkCore;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Data.InitialData;

/// <summary>
/// Purpose: Cria as branches e cursos iniciais da aplicação no arranque.
/// Idempotente — verifica se os dados já existem antes de inserir.
/// Consumed by: DatabaseInitialiser.cs
/// Layer: Data InitialData
/// </summary>
public static class BranchesAndCoursesData
{
    /// <summary>
    /// Cria 3 branches e os respectivos cursos.
    /// Não insere dados se estes já existirem no banco.
    /// </summary>
    /// <param name="context">Contexto do banco de dados.</param>
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Branches.AnyAsync())
        {
            return;
        }

        var dublin = new Branch
        {
            BranchName = "VGC Dublin",
            StreetName = "123 Grafton Street",
            City = "Dublin"
        };

        var cork = new Branch
        {
            BranchName = "VGC Cork",
            StreetName = "45 Patrick Street",
            City = "Cork"
        };

        var galway = new Branch
        {
            BranchName = "VGC Galway",
            StreetName = "78 Shop Street",
            City = "Galway"
        };

        await context.Branches.AddRangeAsync(dublin, cork, galway);
        await context.SaveChangesAsync();

        var courses = new List<Course>
        {
            new Course
            {
                BranchId = dublin.Id,
                CourseName = "Software Development",
                StartDate = new DateOnly(2025, 9, 1),
                EndDate = new DateOnly(2026, 6, 30)
            },
            new Course
            {
                BranchId = dublin.Id,
                CourseName = "Data Analytics",
                StartDate = new DateOnly(2025, 9, 1),
                EndDate = new DateOnly(2026, 6, 30)
            },
            new Course
            {
                BranchId = cork.Id,
                CourseName = "Business Management",
                StartDate = new DateOnly(2025, 9, 1),
                EndDate = new DateOnly(2026, 6, 30)
            },
            new Course
            {
                BranchId = cork.Id,
                CourseName = "Software Development",
                StartDate = new DateOnly(2025, 9, 1),
                EndDate = new DateOnly(2026, 6, 30)
            },
            new Course
            {
                BranchId = galway.Id,
                CourseName = "Digital Marketing",
                StartDate = new DateOnly(2025, 9, 1),
                EndDate = new DateOnly(2026, 6, 30)
            },
            new Course
            {
                BranchId = galway.Id,
                CourseName = "Data Analytics",
                StartDate = new DateOnly(2025, 9, 1),
                EndDate = new DateOnly(2026, 6, 30)
            }
        };

        await context.Courses.AddRangeAsync(courses);
        await context.SaveChangesAsync();
    }
}