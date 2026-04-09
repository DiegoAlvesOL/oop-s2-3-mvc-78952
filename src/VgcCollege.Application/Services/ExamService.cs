using VgcCollege.Application.Interfaces;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Application.Services;

/// <summary>
/// Service de Exam e ExamResult.
/// Contém as regras de negócio para criação de exames, lançamento de resultados
/// e controlo de visibilidade via ResultsReleased.
/// </summary>
public class ExamService
{
    private readonly IExamRepository _examRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ILecturerCourseAssignmentRepository _lecturerAssignmentRepository;

    /// <summary>
    /// Inicializa o service com os repositórios necessários.
    /// </summary>
    /// <param name="examRepository">Repositório de exames injectado via DI.</param>
    /// <param name="courseRepository">Repositório de cursos injectado via DI.</param>
    /// <param name="lecturerAssignmentRepository">Repositório de atribuições de lecturers injectado via DI.</param>
    public ExamService(
        IExamRepository examRepository,
        ICourseRepository courseRepository,
        ILecturerCourseAssignmentRepository lecturerAssignmentRepository)
    {
        _examRepository = examRepository;
        _courseRepository = courseRepository;
        _lecturerAssignmentRepository = lecturerAssignmentRepository;
    }

    /// <summary>
    /// Retorna todos os exames de um curso específico.
    /// </summary>
    /// <param name="courseId">Identificador do curso.</param>
    public async Task<IEnumerable<Exam>> GetByCourseAsync(int courseId)
    {
        return await _examRepository.GetByCourseAsync(courseId);
    }

    /// <summary>
    /// Retorna todos os resultados de um exame específico.
    /// Acesso exclusivo ao Lecturer e Admin.
    /// </summary>
    /// <param name="examId">Identificador do exame.</param>
    public async Task<IEnumerable<ExamResult>> GetResultsByExamAsync(int examId)
    {
        return await _examRepository.GetResultsByExamAsync(examId);
    }

    /// <summary>
    /// Retorna o resultado de um exame para um Student específico.
    /// REGRA CRÍTICA: lança UnauthorizedAccessException se ResultsReleased for false.
    /// Esta verificação acontece no service, nunca na UI.
    /// </summary>
    /// <param name="examId">Identificador do exame.</param>
    /// <param name="studentProfileId">Identificador do perfil do aluno.</param>
    /// <returns>O resultado do exame se ResultsReleased for true.</returns>
    /// <exception cref="InvalidOperationException">Lançada quando o exame não existe.</exception>
    /// <exception cref="UnauthorizedAccessException">
    /// Lançada quando ResultsReleased é false — resultado ainda não publicado.
    /// </exception>
    public async Task<ExamResult?> GetResultForStudentAsync(int examId, int studentProfileId)
    {
        var exam = await _examRepository.GetByIdAsync(examId);

        if (exam == null)
        {
            throw new InvalidOperationException("Exam not found.");
        }

        if (!exam.ResultsReleased)
        {
            throw new UnauthorizedAccessException("Results have not been released yet.");
        }

        return await _examRepository.GetResultByStudentAsync(examId, studentProfileId);
    }

    /// <summary>
    /// Cria um novo exame num curso após validar que o curso existe
    /// e que o Lecturer está atribuído a esse curso.
    /// </summary>
    /// <param name="exam">Entidade Exam a ser criada.</param>
    /// <param name="lecturerProfileId">Identificador do perfil do lecturer que cria o exame.</param>
    /// <exception cref="ArgumentException">Lançada quando o título está em branco.</exception>
    /// <exception cref="InvalidOperationException">Lançada quando o curso não existe ou o lecturer não está atribuído.</exception>
    public async Task CreateExamAsync(Exam exam, int lecturerProfileId)
    {
        if (string.IsNullOrWhiteSpace(exam.Title))
        {
            throw new ArgumentException("Exam title cannot be empty.");
        }

        if (exam.MaxScore <= 0)
        {
            throw new ArgumentException("MaxScore must be greater than zero.");
        }

        var courseExists = await _courseRepository.GetByIdAsync(exam.CourseId);

        if (courseExists == null)
        {
            throw new InvalidOperationException("Course not found.");
        }

        var isAssigned = await _lecturerAssignmentRepository.ExistsAsync(lecturerProfileId, exam.CourseId);

        if (!isAssigned)
        {
            throw new InvalidOperationException("You are not assigned to this course.");
        }

        await _examRepository.AddAsync(exam);
    }

    /// <summary>
    /// Lança o resultado de um aluno num exame.
    /// Valida que o score não excede o MaxScore e que o Lecturer está atribuído ao curso.
    /// </summary>
    /// <param name="result">Entidade ExamResult com o score a lançar.</param>
    /// <param name="lecturerProfileId">Identificador do perfil do lecturer que lança o resultado.</param>
    /// <exception cref="ArgumentException">Lançada quando o score é inválido.</exception>
    /// <exception cref="InvalidOperationException">Lançada quando o exame não existe ou o lecturer não está atribuído.</exception>
    public async Task SetResultAsync(ExamResult result, int lecturerProfileId)
    {
        var exam = await _examRepository.GetByIdAsync(result.ExamId);

        if (exam == null)
        {
            throw new InvalidOperationException("Exam not found.");
        }

        if (result.Score < 0)
        {
            throw new ArgumentException("Score cannot be negative.");
        }

        if (result.Score > exam.MaxScore)
        {
            throw new ArgumentException($"Score cannot exceed MaxScore of {exam.MaxScore}.");
        }

        var isAssigned = await _lecturerAssignmentRepository.ExistsAsync(lecturerProfileId, exam.CourseId);

        if (!isAssigned)
        {
            throw new InvalidOperationException("You are not assigned to this course.");
        }

        await _examRepository.AddResultAsync(result);
    }

    /// <summary>
    /// Liberta os resultados de um exame, tornando-os visíveis para os alunos.
    /// Muda ResultsReleased de false para true.
    /// </summary>
    /// <param name="examId">Identificador do exame a libertar.</param>
    /// <exception cref="InvalidOperationException">Lançada quando o exame não existe.</exception>
    public async Task ReleaseResultsAsync(int examId)
    {
        var exam = await _examRepository.GetByIdAsync(examId);

        if (exam == null)
        {
            throw new InvalidOperationException("Exam not found.");
        }

        exam.ResultsReleased = true;
        await _examRepository.UpdateAsync(exam);
    }
}