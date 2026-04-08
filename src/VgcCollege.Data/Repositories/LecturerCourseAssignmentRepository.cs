using Microsoft.EntityFrameworkCore;
using VgcCollege.Application.Interfaces;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Data.Repositories;

/// <summary>
/// Repositório de LecturerCourseAssignment.
/// Implementa ILecturerCourseAssignmentRepository usando o AppDbContext.
/// </summary>
public class LecturerCourseAssignmentRepository : ILecturerCourseAssignmentRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa o repositório com o contexto do banco de dados.
    /// </summary>
    /// <param name="context">Contexto do banco de dados injectado via DI.</param>
    public LecturerCourseAssignmentRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retorna todos os cursos atribuídos a um lecturer específico,
    /// incluindo os dados do curso e da branch associada.
    /// </summary>
    /// <param name="lecturerProfileId">Identificador do perfil do lecturer.</param>
    public async Task<IEnumerable<LecturerCourseAssignment>> GetByLecturerAsync(int lecturerProfileId)
    {
        return await _context.LecturerCourseAssignments
            .Include(assignment => assignment.Course)
                .ThenInclude(course => course.Branch)
            .Where(assignment => assignment.LecturerProfileId == lecturerProfileId)
            .ToListAsync();
    }

    /// <summary>
    /// Verifica se um lecturer já está atribuído a um curso específico.
    /// </summary>
    /// <param name="lecturerProfileId">Identificador do perfil do lecturer.</param>
    /// <param name="courseId">Identificador do curso.</param>
    public async Task<bool> ExistsAsync(int lecturerProfileId, int courseId)
    {
        return await _context.LecturerCourseAssignments
            .AnyAsync(assignment =>
                assignment.LecturerProfileId == lecturerProfileId &&
                assignment.CourseId == courseId);
    }

    /// <summary>Atribui um lecturer a um curso.</summary>
    /// <param name="assignment">Entidade LecturerCourseAssignment a ser criada.</param>
    public async Task AddAsync(LecturerCourseAssignment assignment)
    {
        await _context.LecturerCourseAssignments.AddAsync(assignment);
        await _context.SaveChangesAsync();
    }

    /// <summary>Remove a atribuição de um lecturer a um curso.</summary>
    /// <param name="lecturerProfileId">Identificador do perfil do lecturer.</param>
    /// <param name="courseId">Identificador do curso.</param>
    public async Task RemoveAsync(int lecturerProfileId, int courseId)
    {
        var assignment = await _context.LecturerCourseAssignments
            .FirstOrDefaultAsync(a =>
                a.LecturerProfileId == lecturerProfileId &&
                a.CourseId == courseId);

        if (assignment != null)
        {
            _context.LecturerCourseAssignments.Remove(assignment);
            await _context.SaveChangesAsync();
        }
    }
}