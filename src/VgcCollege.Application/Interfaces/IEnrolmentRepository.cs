using VgcCollege.Domain.Entities;

namespace VgcCollege.Application.Interfaces;


/// <summary>
/// Purpose: Define o contrato de acesso a dados para a entidade CourseEnrolment.
/// Consumed by: VgcCollege.Application (services), implementado por VgcCollege.Data.
/// Layer: Application Interfaces 
/// </summary>
public interface IEnrolmentRepository
{
    /// <summary>Retorna todas as matrículas de um aluno específico.</summary>
    /// <param name="studentProfileId">Identificador do perfil do aluno.</param>
    Task<IEnumerable<CourseEnrolment>> GetByStudentAsync(int studentProfileId);
    Task<IEnumerable<CourseEnrolment>> GetByCourseAsync(int courseId);
    Task<CourseEnrolment?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(int studentProfileId, int courseId);
    Task AddAsync(CourseEnrolment enrolment);
    Task UpdateAsync(CourseEnrolment enrolment);
}