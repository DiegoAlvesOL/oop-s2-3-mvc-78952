using Microsoft.EntityFrameworkCore;
using VgcCollege.Application.Interfaces;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Data.Repositories;

/// <summary>
/// Repositório de CourseEnrolment. Implementa IEnrolmentRepository usando o AppDbContext.
/// </summary>
public class EnrolmentRepository : IEnrolmentRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa o repositório com o contexto do banco de dados.
    /// </summary>
    /// <param name="context">Contexto do banco de dados injectado via DI.</param>
    public EnrolmentRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retorna todas as matrículas de um aluno específico,
    /// incluindo os dados do curso e da branch associada.
    /// </summary>
    /// <param name="studentProfileId">Identificador do perfil do aluno.</param>
    public async Task<IEnumerable<CourseEnrolment>> GetByStudentAsync(int studentProfileId)
    {
        return await _context.CourseEnrolments
            .Include(enrolment => enrolment.Course)
                .ThenInclude(course => course.Branch)
            .Where(enrolment => enrolment.StudentProfileId == studentProfileId)
            .OrderBy(enrolment => enrolment.EnrolDate)
            .ToListAsync();
    }

    /// <summary>
    /// Retorna todas as matrículas de um curso específico,
    /// incluindo os dados do aluno para visualização pelo Lecturer.
    /// </summary>
    /// <param name="courseId">Identificador do curso.</param>
    public async Task<IEnumerable<CourseEnrolment>> GetByCourseAsync(int courseId)
    {
        return await _context.CourseEnrolments
            .Include(enrolment => enrolment.Student)
            .Include(enrolment => enrolment.Course)
            .Where(enrolment => enrolment.CourseId == courseId)
            .OrderBy(enrolment => enrolment.Student.LastName)
            .ToListAsync();
    }

    /// <summary>Retorna uma matrícula pelo seu identificador único.</summary>
    /// <param name="id">Identificador da matrícula.</param>
    public async Task<CourseEnrolment?> GetByIdAsync(int id)
    {
        return await _context.CourseEnrolments
            .Include(enrolment => enrolment.Student)
            .Include(enrolment => enrolment.Course)
            .FirstOrDefaultAsync(enrolment => enrolment.Id == id);
    }

    /// <summary>
    /// Verifica se um aluno já está matriculado num curso específico.
    /// Usado para prevenir matrículas duplicadas.
    /// </summary>
    /// <param name="studentProfileId">Identificador do perfil do aluno.</param>
    /// <param name="courseId">Identificador do curso.</param>
    public async Task<bool> ExistsAsync(int studentProfileId, int courseId)
    {
        return await _context.CourseEnrolments
            .AnyAsync(enrolment =>
                enrolment.StudentProfileId == studentProfileId &&
                enrolment.CourseId == courseId);
    }

    /// <summary>Adiciona uma nova matrícula ao banco de dados.</summary>
    /// <param name="enrolment">Entidade CourseEnrolment a ser adicionada.</param>
    public async Task AddAsync(CourseEnrolment enrolment)
    {
        await _context.CourseEnrolments.AddAsync(enrolment);
        await _context.SaveChangesAsync();
    }

    /// <summary>Actualiza os dados de uma matrícula existente.</summary>
    /// <param name="enrolment">Entidade CourseEnrolment com os dados actualizados.</param>
    public async Task UpdateAsync(CourseEnrolment enrolment)
    {
        _context.CourseEnrolments.Update(enrolment);
        await _context.SaveChangesAsync();
    }
}