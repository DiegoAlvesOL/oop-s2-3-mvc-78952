using VgcCollege.Application.Interfaces;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Application.Services;

/// <summary>
/// Service de LecturerProfile. Contém as regras de negócio para criação,
/// edição e atribuição de lecturers a cursos.
/// </summary>
public class LecturerService
{
    private readonly ILecturerRepository _lecturerRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ILecturerCourseAssignmentRepository _assignmentRepository;

    /// <summary>
    /// Inicializa o service com os repositórios necessários.
    /// </summary>
    /// <param name="lecturerRepository">Repositório de lecturers injectado via DI.</param>
    /// <param name="courseRepository">Repositório de cursos injectado via DI.</param>
    /// <param name="assignmentRepository">Repositório de atribuições injectado via DI.</param>
    public LecturerService(
        ILecturerRepository lecturerRepository,
        ICourseRepository courseRepository,
        ILecturerCourseAssignmentRepository assignmentRepository)
    {
        _lecturerRepository = lecturerRepository;
        _courseRepository = courseRepository;
        _assignmentRepository = assignmentRepository;
    }

    /// <summary>Retorna todos os perfis de lecturers do sistema.</summary>
    public async Task<IEnumerable<LecturerProfile>> GetAllAsync()
    {
        return await _lecturerRepository.GetAllAsync();
    }

    /// <summary>Retorna um perfil de lecturer pelo seu identificador único.</summary>
    /// <param name="id">Identificador do perfil.</param>
    public async Task<LecturerProfile?> GetByIdAsync(int id)
    {
        return await _lecturerRepository.GetByIdAsync(id);
    }

    /// <summary>Retorna todos os cursos atribuídos a um lecturer.</summary>
    /// <param name="lecturerProfileId">Identificador do perfil do lecturer.</param>
    public async Task<IEnumerable<LecturerCourseAssignment>> GetCourseAssignmentsAsync(int lecturerProfileId)
    {
        return await _assignmentRepository.GetByLecturerAsync(lecturerProfileId);
    }

    /// <summary>
    /// Cria um novo perfil de lecturer após validar os campos obrigatórios.
    /// </summary>
    /// <param name="lecturer">Entidade LecturerProfile a ser criada.</param>
    /// <exception cref="ArgumentException">Lançada quando o nome está em branco.</exception>
    public async Task CreateAsync(LecturerProfile lecturer)
    {
        if (string.IsNullOrWhiteSpace(lecturer.FirstName))
        {
            throw new ArgumentException("First name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(lecturer.LastName))
        {
            throw new ArgumentException("Last name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(lecturer.Email))
        {
            throw new ArgumentException("Email cannot be empty.");
        }

        await _lecturerRepository.AddAsync(lecturer);
    }

    /// <summary>
    /// Actualiza um perfil de lecturer existente após validar os campos obrigatórios.
    /// </summary>
    /// <param name="lecturer">Entidade LecturerProfile com os dados actualizados.</param>
    /// <exception cref="ArgumentException">Lançada quando o nome está em branco.</exception>
    public async Task UpdateAsync(LecturerProfile lecturer)
    {
        if (string.IsNullOrWhiteSpace(lecturer.FirstName))
        {
            throw new ArgumentException("First name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(lecturer.LastName))
        {
            throw new ArgumentException("Last name cannot be empty.");
        }

        await _lecturerRepository.UpdateAsync(lecturer);
    }

    /// <summary>Remove um perfil de lecturer pelo seu identificador único.</summary>
    /// <param name="id">Identificador do perfil a remover.</param>
    public async Task DeleteAsync(int id)
    {
        await _lecturerRepository.DeleteAsync(id);
    }

    /// <summary>
    /// Atribui um lecturer a um curso com a flag IsTutor especificada.
    /// Valida que o lecturer e o curso existem, e que a atribuição não é duplicada.
    /// </summary>
    /// <param name="lecturerProfileId">Identificador do perfil do lecturer.</param>
    /// <param name="courseId">Identificador do curso.</param>
    /// <param name="isTutor">Indica se o lecturer é tutor deste curso.</param>
    /// <exception cref="InvalidOperationException">
    /// Lançada quando o lecturer ou curso não existem, ou a atribuição já existe.
    /// </exception>
    public async Task AssignToCourseAsync(int lecturerProfileId, int courseId, bool isTutor)
    {
        var lecturerExists = await _lecturerRepository.GetByIdAsync(lecturerProfileId);

        if (lecturerExists == null)
        {
            throw new InvalidOperationException("Lecturer not found.");
        }

        var courseExists = await _courseRepository.GetByIdAsync(courseId);

        if (courseExists == null)
        {
            throw new InvalidOperationException("Course not found.");
        }

        var alreadyAssigned = await _assignmentRepository.ExistsAsync(lecturerProfileId, courseId);

        if (alreadyAssigned)
        {
            throw new InvalidOperationException("This lecturer is already assigned to this course.");
        }

        var assignment = new LecturerCourseAssignment
        {
            LecturerProfileId = lecturerProfileId,
            CourseId = courseId,
            IsTutor = isTutor
        };

        await _assignmentRepository.AddAsync(assignment);
    }

    /// <summary>Remove a atribuição de um lecturer a um curso.</summary>
    /// <param name="lecturerProfileId">Identificador do perfil do lecturer.</param>
    /// <param name="courseId">Identificador do curso.</param>
    public async Task RemoveFromCourseAsync(int lecturerProfileId, int courseId)
    {
        await _assignmentRepository.RemoveAsync(lecturerProfileId, courseId);
    }
    
    /// <summary>
    /// Retorna o perfil de lecturer associado a um utilizador de Identity.
    /// </summary>
    /// <param name="identityUserId">Identificador do utilizador no ASP.NET Core Identity.</param>
    public async Task<LecturerProfile?> GetByIdentityUserIdAsync(string identityUserId)
    {
        return await _lecturerRepository.GetByIdentityUserIdAsync(identityUserId);
    }
    
    /// <summary>
    /// Retorna todos os lecturers atribuídos a um curso específico.
    /// </summary>
    /// <param name="courseId">Identificador do curso.</param>
    public async Task<IEnumerable<LecturerCourseAssignment>> GetAssignmentsByCourseAsync(int courseId)
    {
        return await _assignmentRepository.GetByCourseAsync(courseId);
    }
    
}