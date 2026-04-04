using VgcCollege.Domain.Entities;

namespace VgcCollege.Application.Interfaces;

/// <summary>
/// Purpose: Define o contrato de acesso a dados para a entidade Course.
/// Consumed by: VgcCollege.Application (services), implementado por VgcCollege.Data.
/// Layer: Application Interfaces
/// </summary>
public interface ICourseRepository
{
    /// <summary>
    ///<summary>Retorna todos os cursos do sistema.</summary>
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<Course>> GetAllCourses();
    
    Task<IEnumerable<Course>> GetByBranchAssync(int branchId);
    Task<Course?> GetCourseById(int id);
    Task AddSync(Course course);
    Task UpdateSync(Course course);
    Task UpdateAssync(Course course);
    Task DeleteSync(int id);
}