using VgcCollege.Domain.Entities;

namespace VgcCollege.Application.Interfaces;



/// <summary>
/// Purpose: Define o contrato de acesso a dados para as entidades Exam e ExamResult.
/// Consumed by: VgcCollege.Application (services), implementado por VgcCollege.Data.
/// Layer: Application Interfaces
/// </summary>
public interface IExamRepository
{
    /// <summary>Retorna todos os exames de um curso específico.</summary>
    /// <param name="courseId">Identificador do curso.</param>
    Task<IEnumerable<Exam>> GetByCourseAsync(int courseId);
    Task<Exam?> GetByIdAsync(int id);
    Task AddAsync(Exam exam);
    Task UpdateAsync(Exam exam);
    Task<ExamResult?> GetResultByStudentAsync(int examId, int studentProfileId);
    Task<IEnumerable<ExamResult>> GetResultsByExamAsync(int examId);
    Task AddResultAsync(ExamResult result);
    Task UpdateResultAsync(ExamResult result);
}