using VgcCollege.Application.Interfaces;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Application.Services;

/// <summary>
/// Service de AttendanceRecord. Contém as regras de negócio para
/// registo e visualização de presenças por sessão.
/// </summary>
public class AttendanceService
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IEnrolmentRepository _enrolmentRepository;
    private readonly ILecturerCourseAssignmentRepository _assignmentRepository;

    /// <summary>
    /// Inicializa o service com os repositórios necessários.
    /// </summary>
    /// <param name="attendanceRepository">Repositório de presenças injectado via DI.</param>
    /// <param name="enrolmentRepository">Repositório de matrículas injectado via DI.</param>
    /// <param name="assignmentRepository">Repositório de atribuições de lecturers injectado via DI.</param>
    public AttendanceService(
        IAttendanceRepository attendanceRepository,
        IEnrolmentRepository enrolmentRepository,
        ILecturerCourseAssignmentRepository assignmentRepository)
    {
        _attendanceRepository = attendanceRepository;
        _enrolmentRepository = enrolmentRepository;
        _assignmentRepository = assignmentRepository;
    }

    /// <summary>
    /// Retorna todos os registos de presença de uma matrícula específica.
    /// </summary>
    /// <param name="enrolmentId">Identificador da matrícula.</param>
    public async Task<IEnumerable<AttendanceRecord>> GetByEnrolmentAsync(int enrolmentId)
    {
        return await _attendanceRepository.GetByEnrolmentAsync(enrolmentId);
    }

    /// <summary>
    /// Regista a presença de um aluno numa sessão.
    /// Valida que a matrícula existe, que o Lecturer está atribuído ao curso,
    /// e que não existe registo duplicado para a mesma sessão.
    /// </summary>
    /// <param name="enrolmentId">Identificador da matrícula do aluno.</param>
    /// <param name="sessionDate">Data da sessão.</param>
    /// <param name="present">Indica se o aluno esteve presente.</param>
    /// <param name="lecturerProfileId">Identificador do perfil do lecturer que regista.</param>
    /// <exception cref="InvalidOperationException">
    /// Lançada quando a matrícula não existe, o lecturer não está atribuído ao curso,
    /// ou já existe um registo para esta sessão.
    /// </exception>
    public async Task RecordAttendanceAsync(
        int enrolmentId,
        DateOnly sessionDate,
        bool present,
        int lecturerProfileId)
    {
        var enrolment = await _enrolmentRepository.GetByIdAsync(enrolmentId);

        if (enrolment == null)
        {
            throw new InvalidOperationException("Enrolment not found.");
        }

        // Verifica que o lecturer está atribuído ao curso desta matrícula.
        var isAssigned = await _assignmentRepository.ExistsAsync(lecturerProfileId, enrolment.CourseId);

        if (!isAssigned)
        {
            throw new InvalidOperationException("You are not assigned to this course.");
        }

        var alreadyRecorded = await _attendanceRepository.ExistsAsync(enrolmentId, sessionDate);

        if (alreadyRecorded)
        {
            throw new InvalidOperationException("Attendance for this session has already been recorded.");
        }

        var attendanceRecord = new AttendanceRecord
        {
            CourseEnrolmentId = enrolmentId,
            SessionDate = sessionDate,
            Present = present
        };

        await _attendanceRepository.AddAsync(attendanceRecord);
    }
}