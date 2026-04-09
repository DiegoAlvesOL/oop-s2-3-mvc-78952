using VgcCollege.Application.Interfaces;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Application.Services;

/// <summary>
/// Service de Assignment e AssignmentResult.
/// Contém as regras de negócio para criação de assignments e lançamento de resultados.
/// </summary>
public class AssignmentService
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ILecturerCourseAssignmentRepository _lecturerAssignmentRepository;

    /// <summary>
    /// Inicializa o service com os repositórios necessários.
    /// </summary>
    /// <param name="assignmentRepository">Repositório de assignments injectado via DI.</param>
    /// <param name="courseRepository">Repositório de cursos injectado via DI.</param>
    /// <param name="lecturerAssignmentRepository">Repositório de atribuições de lecturers injectado via DI.</param>
    public AssignmentService(
        IAssignmentRepository assignmentRepository,
        ICourseRepository courseRepository,
        ILecturerCourseAssignmentRepository lecturerAssignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
        _courseRepository = courseRepository;
        _lecturerAssignmentRepository = lecturerAssignmentRepository;
    }

    /// <summary>Retorna todos os assignments de um curso específico.</summary>
    /// <param name="courseId">Identificador do curso.</param>
    public async Task<IEnumerable<Assignment>> GetByCourseAsync(int courseId)
    {
        return await _assignmentRepository.GetByCourseAsync(courseId);
    }

    /// <summary>Retorna um assignment pelo seu identificador único.</summary>
    /// <param name="id">Identificador do assignment.</param>
    public async Task<Assignment?> GetByIdAsync(int id)
    {
        return await _assignmentRepository.GetByIdAsync(id);
    }

    /// <summary>Retorna todos os resultados de um aluno específico.</summary>
    /// <param name="studentProfileId">Identificador do perfil do aluno.</param>
    public async Task<IEnumerable<AssignmentResult>> GetResultsByStudentAsync(int studentProfileId)
    {
        return await _assignmentRepository.GetResultsByStudentAsync(studentProfileId);
    }

    /// <summary>Retorna todos os resultados de um assignment específico.</summary>
    /// <param name="assignmentId">Identificador do assignment.</param>
    public async Task<IEnumerable<AssignmentResult>> GetResultsByAssignmentAsync(int assignmentId)
    {
        return await _assignmentRepository.GetResultsByAssignmentAsync(assignmentId);
    }

    /// <summary>
    /// Cria um novo assignment num curso após validar que o curso existe
    /// e que o Lecturer está atribuído a esse curso.
    /// </summary>
    /// <param name="assignment">Entidade Assignment a ser criada.</param>
    /// <param name="lecturerProfileId">Identificador do perfil do lecturer que cria o assignment.</param>
    /// <exception cref="ArgumentException">Lançada quando o título está em branco ou MaxScore é inválido.</exception>
    /// <exception cref="InvalidOperationException">Lançada quando o curso não existe ou o lecturer não está atribuído.</exception>
    public async Task CreateAssignmentAsync(Assignment assignment, int lecturerProfileId)
    {
        if (string.IsNullOrWhiteSpace(assignment.Title))
        {
            throw new ArgumentException("Assignment title cannot be empty.");
        }

        if (assignment.MaxScore <= 0)
        {
            throw new ArgumentException("MaxScore must be greater than zero.");
        }

        var courseExists = await _courseRepository.GetByIdAsync(assignment.CourseId);

        if (courseExists == null)
        {
            throw new InvalidOperationException("Course not found.");
        }

        var isAssigned = await _lecturerAssignmentRepository.ExistsAsync(lecturerProfileId, assignment.CourseId);

        if (!isAssigned)
        {
            throw new InvalidOperationException("You are not assigned to this course.");
        }

        await _assignmentRepository.AddAsync(assignment);
    }

    /// <summary>
    /// Lança o resultado de um aluno num assignment.
    /// Valida que o score não excede o MaxScore e que não é negativo.
    /// Valida que o Lecturer está atribuído ao curso do assignment.
    /// </summary>
    /// <param name="result">Entidade AssignmentResult com o score a lançar.</param>
    /// <param name="lecturerProfileId">Identificador do perfil do lecturer que lança o resultado.</param>
    /// <exception cref="ArgumentException">Lançada quando o score é inválido.</exception>
    /// <exception cref="InvalidOperationException">Lançada quando o assignment não existe ou o lecturer não está atribuído.</exception>
    public async Task SetResultAsync(AssignmentResult result, int lecturerProfileId)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(result.AssignmentId);

        if (assignment == null)
        {
            throw new InvalidOperationException("Assignment not found.");
        }

        if (result.Score < 0)
        {
            throw new ArgumentException("Score cannot be negative.");
        }

        if (result.Score > assignment.MaxScore)
        {
            throw new ArgumentException($"Score cannot exceed MaxScore of {assignment.MaxScore}.");
        }

        var isAssigned = await _lecturerAssignmentRepository.ExistsAsync(lecturerProfileId, assignment.CourseId);

        if (!isAssigned)
        {
            throw new InvalidOperationException("You are not assigned to this course.");
        }

        await _assignmentRepository.AddResultAsync(result);
    }
}