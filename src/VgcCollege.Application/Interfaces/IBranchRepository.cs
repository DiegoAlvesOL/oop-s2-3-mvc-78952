using VgcCollege.Domain.Entities;

namespace VgcCollege.Application.Interfaces;

/// <summary>
/// Purpose: Define o contrato de acesso a dados para a entidade Branch.
/// Consumed by: VgcCollege.Application (services), implementado por VgcCollege.Data.
/// Layer: Application, Interfaces
/// </summary>
public interface IBranchRepository
{
    Task<IEnumerable<Branch>> GetAllAsync();
    Task<Branch?> GetByIdAsync(int id);
    Task AddAsync(Branch branch);
    Task UpdateAsync(Branch branch);
    Task DeleteAsync(int id);
    
}