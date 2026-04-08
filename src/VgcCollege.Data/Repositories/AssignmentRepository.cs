using Microsoft.EntityFrameworkCore;
using VgcCollege.Application.Interfaces;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Data.Repositories;

/// <summary>
/// Repositório de Assignment e AssignmentResult.
/// Implementa IAssignmentRepository usando o AppDbContext.
/// </summary>
public class AssignmentRepository : IAssignmentRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa o repositório com o contexto do banco de dados.
    /// </summary>
    /// <param name="context">Contexto do banco de dados injectado via DI.</param>
    public AssignmentRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retorna todos os assignments de um curso específico, ordenados por data de entrega.
    /// </summary>
    /// <param name="courseId">Identificador do curso.</param>
    public async Task<IEnumerable<Assignment>> GetByCourseAsync(int courseId)
    {
        return await _context.Assignments
            .Where(assignment => assignment.CourseId == courseId)
            .OrderBy(assignment => assignment.DueDate)
            .ToListAsync();
    }

    /// <summary>Retorna um assignment pelo seu identificador único.</summary>
    /// <param name="id">Identificador do assignment.</param>
    public async Task<Assignment?> GetByIdAsync(int id)
    {
        return await _context.Assignments.FindAsync(id);
    }

    /// <summary>Adiciona um novo assignment ao banco de dados.</summary>
    /// <param name="assignment">Entidade Assignment a ser adicionada.</param>
    public async Task AddAsync(Assignment assignment)
    {
        await _context.Assignments.AddAsync(assignment);
        await _context.SaveChangesAsync();
    }

    /// <summary>Actualiza os dados de um assignment existente.</summary>
    /// <param name="assignment">Entidade Assignment com os dados actualizados.</param>
    public async Task UpdateAsync(Assignment assignment)
    {
        _context.Assignments.Update(assignment);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Retorna todos os resultados de um aluno específico,
    /// incluindo os dados do assignment para apresentação na vista do Student.
    /// </summary>
    /// <param name="studentProfileId">Identificador do perfil do aluno.</param>
    public async Task<IEnumerable<AssignmentResult>> GetResultsByStudentAsync(int studentProfileId)
    {
        return await _context.AssignmentResults
            .Include(result => result.Assignment)
            .Where(result => result.StudentProfileId == studentProfileId)
            .OrderBy(result => result.Assignment.DueDate)
            .ToListAsync();
    }

    /// <summary>
    /// Retorna todos os resultados de um assignment específico,
    /// incluindo os dados do aluno para apresentação na vista do Lecturer.
    /// </summary>
    /// <param name="assignmentId">Identificador do assignment.</param>
    public async Task<IEnumerable<AssignmentResult>> GetResultsByAssignmentAsync(int assignmentId)
    {
        return await _context.AssignmentResults
            .Include(result => result.Student)
            .Where(result => result.AssignmentId == assignmentId)
            .OrderBy(result => result.Student.LastName)
            .ToListAsync();
    }

    /// <summary>Adiciona um novo resultado de assignment ao banco de dados.</summary>
    /// <param name="result">Entidade AssignmentResult a ser adicionada.</param>
    public async Task AddResultAsync(AssignmentResult result)
    {
        await _context.AssignmentResults.AddAsync(result);
        await _context.SaveChangesAsync();
    }

    /// <summary>Actualiza um resultado de assignment existente.</summary>
    /// <param name="result">Entidade AssignmentResult com os dados actualizados.</param>
    public async Task UpdateResultAsync(AssignmentResult result)
    {
        _context.AssignmentResults.Update(result);
        await _context.SaveChangesAsync();
    }
}