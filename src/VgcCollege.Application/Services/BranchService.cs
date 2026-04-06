// Purpose   : Contém a lógica de negócio para gestão de branches.
//             Usa IBranchRepository para acesso a dados sem depender do EF Core.
// Consumed by: BranchController (VgcCollege.Web).
// Layer     : Application — Services

using VgcCollege.Application.Interfaces;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Application.Services;

/// <summary>
/// Service de Branch. Contém as regras de negócio para criação,
/// edição e remoção de branches.
/// </summary>
public class BranchService
{
    private readonly IBranchRepository _branchRepository;

    /// <summary>
    /// Inicializa o service com o repositório de branches.
    /// </summary>
    /// <param name="branchRepository">Repositório de branches injectado via DI.</param>
    public BranchService(IBranchRepository branchRepository)
    {
        _branchRepository = branchRepository;
    }
    
    public async Task<IEnumerable<Branch>> GetAllAsync()
    {
        return await _branchRepository.GetAllAsync();
    }

    /// <summary>Retorna uma branch pelo seu identificador único.</summary>
    /// <param name="id">Identificador da branch.</param>
    /// <returns>A branch encontrada ou null se não existir.</returns>
    public async Task<Branch?> GetByIdAsync(int id)
    {
        return await _branchRepository.GetByIdAsync(id);
    }

    /// <summary>
    /// Cria uma nova branch após validar que o nome não está em branco.
    /// </summary>
    /// <param name="branch">Entidade Branch a ser criada.</param>
    /// <exception cref="ArgumentException">Lançada quando o nome da branch está em branco.</exception>
    public async Task CreateAsync(Branch branch)
    {
        if (string.IsNullOrWhiteSpace(branch.BranchName))
        {
            throw new ArgumentException("Branch name cannot be empty.");
        }

        await _branchRepository.AddAsync(branch);
    }

    /// <summary>
    /// Atualiza uma branch existente após validar que o nome não está em branco.
    /// </summary>
    /// <param name="branch">Entidade Branch com os dados actualizados.</param>
    /// <exception cref="ArgumentException">Lançada quando o nome da branch está em branco.</exception>
    public async Task UpdateAsync(Branch branch)
    {
        if (string.IsNullOrWhiteSpace(branch.BranchName))
        {
            throw new ArgumentException("Branch name cannot be empty.");
        }

        await _branchRepository.UpdateAsync(branch);
    }

    /// <summary>Remove uma branch pelo seu identificador único.</summary>
    /// <param name="id">Identificador da branch a ser removida.</param>
    public async Task DeleteAsync(int id)
    {
        await _branchRepository.DeleteAsync(id);
    }
}