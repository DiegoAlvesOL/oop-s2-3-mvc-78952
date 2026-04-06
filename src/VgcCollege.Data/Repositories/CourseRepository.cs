using Microsoft.EntityFrameworkCore;
using VgcCollege.Application.Interfaces;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Data.Repositories;

/// <summary>
/// Purpose: Implementação concreta do ICourseRepository usando EF Core e MySQL.
/// Consumed by: CourseService (via ICourseRepository), Program.cs (registo no DI).
/// Layer: Data Repositories
/// </summary>
public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa o repositório com o contexto do banco de dados.
    /// </summary>
    /// <param name="context">Contexto do banco de dados injectado via DI.</param>
    public CourseRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>Retorna todos os cursos do sistema com a branch associada.</summary>
    public async Task<IEnumerable<Course>> GetAllAsync()
    {
        return await _context.Courses
            .Include(course => course.Branch)
            .OrderBy(course => course.CourseName)
            .ToListAsync();
    }

    /// <summary>Retorna todos os cursos de uma Branch específica.</summary>
    /// <param name="branchId">Identificador da branch.</param>
    public async Task<IEnumerable<Course>> GetByBranchAsync(int branchId)
    {
        return await _context.Courses
            .Include(course => course.Branch)
            .Where(course => course.BranchId == branchId)
            .OrderBy(course => course.CourseName)
            .ToListAsync();
    }

    /// <summary>Retorna um curso pelo seu identificador único com a branch associada.</summary>
    /// <param name="id">Identificador do curso.</param>
    public async Task<Course?> GetByIdAsync(int id)
    {
        return await _context.Courses
            .Include(course => course.Branch)
            .FirstOrDefaultAsync(course => course.Id == id);
    }

    /// <summary>Adiciona um novo curso ao banco de dados.</summary>
    /// <param name="course">Entidade Course a ser adicionada.</param>
    public async Task AddAsync(Course course)
    {
        await _context.Courses.AddAsync(course);
        await _context.SaveChangesAsync();
    }

    /// <summary>Actualiza os dados de um curso existente.</summary>
    /// <param name="course">Entidade Course com os dados actualizados.</param>
    public async Task UpdateAsync(Course course)
    {
        _context.Courses.Update(course);
        await _context.SaveChangesAsync();
    }

    /// <summary>Remove um curso pelo seu identificador único.</summary>
    /// <param name="id">Identificador do curso a ser removido.</param>
    public async Task DeleteAsync(int id)
    {
        var course = await _context.Courses.FindAsync(id);

        if (course != null)
        {
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }
    }
}