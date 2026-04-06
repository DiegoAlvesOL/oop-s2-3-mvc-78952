

using VgcCollege.Application.Interfaces;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Application.Services;

/// <summary>
///Purpose: Contém a lógica de negócio para gestão de cursos.
/// Usa ICourseRepository e IBranchRepository para validar dependências.
/// Consumed by: CourseController (VgcCollege.Web).
/// Layer: Application Services
/// </summary>
public class CourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IBranchRepository _branchRepository;

    /// <summary>
    /// Inicializa o service com os repositórios necessários.
    /// </summary>
    /// <param name="courseRepository">Repositório de cursos injectado via DI.</param>
    /// <param name="branchRepository">Repositório de branches injectado via DI.</param>
    public CourseService(ICourseRepository courseRepository, IBranchRepository branchRepository)
    {
        _courseRepository = courseRepository;
        _branchRepository = branchRepository;
    }

    /// <summary>Retorna todos os cursos do sistema.</summary>
    public async Task<IEnumerable<Course>> GetAllAsync()
    {
        return await _courseRepository.GetAllAsync();
    }

    /// <summary>Retorna todos os cursos de uma branch específica.</summary>
    /// <param name="branchId">Identificador da branch.</param>
    public async Task<IEnumerable<Course>> GetByBranchAsync(int branchId)
    {
        return await _courseRepository.GetByBranchAsync(branchId);
    }

    /// <summary>Retorna um curso pelo seu identificador único.</summary>
    /// <param name="id">Identificador do curso.</param>
    /// <returns>O curso encontrado ou null se não existir.</returns>
    public async Task<Course?> GetByIdAsync(int id)
    {
        return await _courseRepository.GetByIdAsync(id);
    }

    /// <summary>
    /// Cria um novo curso após validar que o nome não está em branco
    /// e que a branch associada existe no sistema.
    /// </summary>
    /// <param name="course">Entidade Course a ser criada.</param>
    /// <exception cref="ArgumentException">Lançada quando o nome do curso está em branco.</exception>
    /// <exception cref="InvalidOperationException">Lançada quando a branch não existe.</exception>
    public async Task CreateAsync(Course course)
    {
        if (string.IsNullOrWhiteSpace(course.CourseName))
        {
            throw new ArgumentException("Course name cannot be empty.");
        }

        var branchExists = await _branchRepository.GetByIdAsync(course.BranchId);

        if (branchExists == null)
        {
            throw new InvalidOperationException("The specified branch does not exist.");
        }

        await _courseRepository.AddAsync(course);
    }

    /// <summary>
    /// Actualiza um curso existente após validar que o nome não está em branco.
    /// </summary>
    /// <param name="course">Entidade Course com os dados actualizados.</param>
    /// <exception cref="ArgumentException">Lançada quando o nome do curso está em branco.</exception>
    public async Task UpdateAsync(Course course)
    {
        if (string.IsNullOrWhiteSpace(course.CourseName))
        {
            throw new ArgumentException("Course name cannot be empty.");
        }

        await _courseRepository.UpdateAsync(course);
    }

    /// <summary>Remove um curso pelo seu identificador único.</summary>
    /// <param name="id">Identificador do curso a ser removido.</param>
    public async Task DeleteAsync(int id)
    {
        await _courseRepository.DeleteAsync(id);
    }
}