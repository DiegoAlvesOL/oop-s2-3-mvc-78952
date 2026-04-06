using Microsoft.EntityFrameworkCore;
using VgcCollege.Application.Interfaces;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Data.Repositories;

/// <summary>
/// Purpose: Implementação concreta do IBranchRepository usando EF Core e MySQL.
/// Consumed by: BranchService (via IBranchRepository), Program.cs (registo no DI).
/// Layer: Data Repositories
/// </summary>
public class BranchRepository : IBranchRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa o repositório com o contexto do banco de dados.
    /// </summary>
    /// <param name="context">Contexto do banco de dados injectado via DI.</param>
    public BranchRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Branch>> GetAllAsync()
    {
        return await _context.Branches
            .OrderBy(branch => branch.BranchName)
            .ToListAsync();
    }

    
    public async Task<Branch?> GetByIdAsync(int id)
    {
        return await _context.Branches.FindAsync(id);
    }

    /// <summary>Adiciona uma nova branch ao banco de dados.</summary>
    /// <param name="branch">Entidade Branch a ser adicionada.</param>
    public async Task AddAsync(Branch branch)
    {
        await _context.Branches.AddAsync(branch);
        await _context.SaveChangesAsync();
    }

    /// <summary>Actualiza os dados de uma branch existente.</summary>
    /// <param name="branch">Entidade Branch com os dados actualizados.</param>
    public async Task UpdateAsync(Branch branch)
    {
        _context.Branches.Update(branch);
        await _context.SaveChangesAsync();
    }

    /// <summary>Remove uma branch pelo seu identificador único.</summary>
    /// <param name="id">Identificador da branch a ser removida.</param>
    public async Task DeleteAsync(int id)
    {
        var branch = await _context.Branches.FindAsync(id);

        if (branch != null)
        {
            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();
        }
    }
}