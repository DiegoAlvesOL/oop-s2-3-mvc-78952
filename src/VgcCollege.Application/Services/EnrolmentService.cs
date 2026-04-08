using VgcCollege.Application.Interfaces;
using VgcCollege.Domain.Entities;
using VgcCollege.Domain.Enums;

namespace VgcCollege.Application.Services;

/// <summary>
/// Service de CourseEnrolment. Contém as regras de negócio para
/// criação e gestão de matrículas de alunos em cursos.
/// </summary>
public class EnrolmentService
{
    private readonly IEnrolmentRepository _enrolmentRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ICourseRepository _courseRepository;

    /// <summary>
    /// Inicializa o service com os repositórios necessários.
    /// </summary>
    /// <param name="enrolmentRepository">Repositório de matrículas injectado via DI.</param>
    /// <param name="studentRepository">Repositório de alunos injectado via DI.</param>
    /// <param name="courseRepository">Repositório de cursos injectado via DI.</param>
    public EnrolmentService(
        IEnrolmentRepository enrolmentRepository,
        IStudentRepository studentRepository,
        ICourseRepository courseRepository)
    {
        _enrolmentRepository = enrolmentRepository;
        _studentRepository = studentRepository;
        _courseRepository = courseRepository;
    }

    /// <summary>
    /// Retorna todas as matrículas de um aluno específico.
    /// </summary>
    /// <param name="studentProfileId">Identificador do perfil do aluno.</param>
    public async Task<IEnumerable<CourseEnrolment>> GetByStudentAsync(int studentProfileId)
    {
        return await _enrolmentRepository.GetByStudentAsync(studentProfileId);
    }

    /// <summary>
    /// Retorna todas as matrículas de um curso específico.
    /// </summary>
    /// <param name="courseId">Identificador do curso.</param>
    public async Task<IEnumerable<CourseEnrolment>> GetByCourseAsync(int courseId)
    {
        return await _enrolmentRepository.GetByCourseAsync(courseId);
    }

    /// <summary>
    /// Retorna uma matrícula pelo seu identificador único.
    /// </summary>
    /// <param name="id">Identificador da matrícula.</param>
    public async Task<CourseEnrolment?> GetByIdAsync(int id)
    {
        return await _enrolmentRepository.GetByIdAsync(id);
    }

    /// <summary>
    /// Matricula um aluno num curso após validar que o aluno e o curso existem
    /// e que a matrícula não é duplicada.
    /// </summary>
    /// <param name="studentProfileId">Identificador do perfil do aluno.</param>
    /// <param name="courseId">Identificador do curso.</param>
    /// <exception cref="InvalidOperationException">
    /// Lançada quando o aluno ou curso não existem, ou quando a matrícula já existe.
    /// </exception>
    public async Task EnrolStudentAsync(int studentProfileId, int courseId)
    {
        var studentExists = await _studentRepository.GetByIdAsync(studentProfileId);

        if (studentExists == null)
        {
            throw new InvalidOperationException("Student not found.");
        }

        var courseExists = await _courseRepository.GetByIdAsync(courseId);

        if (courseExists == null)
        {
            throw new InvalidOperationException("Course not found.");
        }

        var alreadyEnrolled = await _enrolmentRepository.ExistsAsync(studentProfileId, courseId);

        if (alreadyEnrolled)
        {
            throw new InvalidOperationException("This student is already enrolled in this course.");
        }

        var enrolment = new CourseEnrolment
        {
            StudentProfileId = studentProfileId,
            CourseId = courseId,
            EnrolDate = DateOnly.FromDateTime(DateTime.Today),
            Status = EnrolmentStatus.Active
        };

        await _enrolmentRepository.AddAsync(enrolment);
    }

    /// <summary>
    /// Actualiza o estado de uma matrícula existente.
    /// </summary>
    /// <param name="enrolment">Entidade CourseEnrolment com o estado actualizado.</param>
    public async Task UpdateStatusAsync(CourseEnrolment enrolment)
    {
        await _enrolmentRepository.UpdateAsync(enrolment);
    }
}