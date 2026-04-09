using Microsoft.EntityFrameworkCore;
using VgcCollege.Application.Interfaces;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Data.Repositories;

/// <summary>
/// Repositório de Exam e ExamResult.
/// Implementa IExamRepository usando o AppDbContext.
/// </summary>
public class ExamRepository : IExamRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa o repositório com o contexto do banco de dados.
    /// </summary>
    /// <param name="context">Contexto do banco de dados injectado via DI.</param>
    public ExamRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retorna todos os exames de um curso específico, ordenados por data.
    /// </summary>
    /// <param name="courseId">Identificador do curso.</param>
    public async Task<IEnumerable<Exam>> GetByCourseAsync(int courseId)
    {
        return await _context.Exams
            .Where(exam => exam.CourseId == courseId)
            .OrderBy(exam => exam.ExamDate)
            .ToListAsync();
    }

    /// <summary>Retorna um exame pelo seu identificador único.</summary>
    /// <param name="id">Identificador do exame.</param>
    public async Task<Exam?> GetByIdAsync(int id)
    {
        return await _context.Exams.FindAsync(id);
    }

    /// <summary>Adiciona um novo exame ao banco de dados.</summary>
    /// <param name="exam">Entidade Exam a ser adicionada.</param>
    public async Task AddAsync(Exam exam)
    {
        await _context.Exams.AddAsync(exam);
        await _context.SaveChangesAsync();
    }

    /// <summary>Actualiza os dados de um exame existente.</summary>
    /// <param name="exam">Entidade Exam com os dados actualizados.</param>
    public async Task UpdateAsync(Exam exam)
    {
        _context.Exams.Update(exam);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Retorna o resultado de um aluno num exame específico.
    /// Chamado pelo ExamService apenas após verificar ResultsReleased.
    /// </summary>
    /// <param name="examId">Identificador do exame.</param>
    /// <param name="studentProfileId">Identificador do perfil do aluno.</param>
    public async Task<ExamResult?> GetResultByStudentAsync(int examId, int studentProfileId)
    {
        return await _context.ExamResults
            .Include(result => result.Exam)
            .FirstOrDefaultAsync(result =>
                result.ExamId == examId &&
                result.StudentProfileId == studentProfileId);
    }

    /// <summary>
    /// Retorna todos os resultados de um exame específico,
    /// incluindo os dados do aluno para apresentação na vista do Lecturer.
    /// </summary>
    /// <param name="examId">Identificador do exame.</param>
    public async Task<IEnumerable<ExamResult>> GetResultsByExamAsync(int examId)
    {
        return await _context.ExamResults
            .Include(result => result.Student)
            .Where(result => result.ExamId == examId)
            .OrderBy(result => result.Student.LastName)
            .ToListAsync();
    }

    /// <summary>Adiciona um novo resultado de exame ao banco de dados.</summary>
    /// <param name="result">Entidade ExamResult a ser adicionada.</param>
    public async Task AddResultAsync(ExamResult result)
    {
        await _context.ExamResults.AddAsync(result);
        await _context.SaveChangesAsync();
    }

    /// <summary>Actualiza um resultado de exame existente.</summary>
    /// <param name="result">Entidade ExamResult com os dados actualizados.</param>
    public async Task UpdateResultAsync(ExamResult result)
    {
        _context.ExamResults.Update(result);
        await _context.SaveChangesAsync();
    }
}