using VgcCollege.Domain.Entities;

namespace VgcCollege.Application.Interfaces;

/// <summary>
/// Purpose: Define o contrato de acesso a dados para as entidades Assignment e AssignmentResult.
/// Consumed by: VgcCollege.Application (services), implementado por VgcCollege.Data.
/// Layer: Application Interfaces
/// </summary>
public interface IAssignmentRepository
{
    /// <summary>Retorna todos os assignments de um curso específico.</summary>
    /// <param name="courseId">Identificador do curso.</param>
    Task<IEnumerable<Assignment>> GetByCourseAsync(int courseId);
    Task<Assignment?> GetByIdAsync(int id);
    Task AddAsync(Assignment assignment);
    Task UpdateAsync(Assignment assignment);
    Task<IEnumerable<AssignmentResult>> GetResultsByStudentAsync(int studentProfileId);
    Task<IEnumerable<AssignmentResult>> GetResultsByAssignmentAsync(int assignmentId);
    Task AddResultAsync(AssignmentResult result);
    Task UpdateResultAsync(AssignmentResult result);
}