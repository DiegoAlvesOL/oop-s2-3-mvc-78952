using VgcCollege.Domain.Entities;

namespace VgcCollege.Application.Interfaces;

/// <summary>
/// Purpose: Define o contrato de acesso a dados para a entidade Course.
/// Consumed by: VgcCollege.Application (services), implementado por VgcCollege.Data.
/// Layer: Application Interfaces
/// </summary>
public interface ICourseRepository
{
    /// <summary>Retorna todos os cursos do sistema.</summary>
    Task<IEnumerable<Course>> GetAllAsync();
    
    Task<IEnumerable<Course>> GetByBranchAsync(int branchId);
    Task<Course?> GetByIdAsync(int id);
    Task AddAsync(Course course);
    Task UpdateAsync(Course course);
    Task DeleteAsync(int id);
}