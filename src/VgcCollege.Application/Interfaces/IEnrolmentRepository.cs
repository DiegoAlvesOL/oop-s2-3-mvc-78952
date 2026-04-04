using VgcCollege.Domain.Entities;

namespace VgcCollege.Application.Interfaces;




/// <summary>
/// Purpose: Define o contrato de acesso a dados para a entidade CourseEnrolment.
/// Consumed by: VgcCollege.Application (services), implementado por VgcCollege.Data.
/// Layer: Application Interfaces 
/// </summary>
public interface IEnrolmentRepository
{
    Task<IEnumerable<CourseEnrolment>> GetByStudentAsync(int studentProfileId);
    Task<IEnumerable<CourseEnrolment>> GetByCourseAsync(int courseId);
    Task<CourseEnrolment> GetByIdAsync(int id);
    
    /// <summary>
    /// Verifica se um aluno já está matriculado num curso específico.
    /// Utilizado para prevenir matrículas duplicadas.
    /// </summary>
    /// <param name="studentProfileId">Identificador do perfil do aluno</param>
    /// <param name="courseId">Identificador do curso</param>
    /// <returns></returns>
    Task<bool> ExistsAsync(int studentProfileId, int courseId);
    Task AddAsync(CourseEnrolment enrolment);
    Task UpdateAsync(CourseEnrolment enrolment);
}