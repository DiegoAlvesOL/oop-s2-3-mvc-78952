using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VgcCollege.Application.Services;
using VgcCollege.Data.Models;
using VgcCollege.Domain.Constants;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers;

/// <summary>
/// Controller de AttendanceRecord com controlo de acesso por role.
/// </summary>
[Authorize]
public class AttendanceController : Controller
{
    private readonly AttendanceService _attendanceService;
    private readonly EnrolmentService _enrolmentService;
    private readonly LecturerService _lecturerService;
    private readonly StudentService _studentService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AttendanceController> _logger;

    /// <summary>
    /// Inicializa o controller com os services necessários.
    /// </summary>
    public AttendanceController(
        AttendanceService attendanceService,
        EnrolmentService enrolmentService,
        LecturerService lecturerService,
        StudentService studentService,
        UserManager<ApplicationUser> userManager,
        ILogger<AttendanceController> logger)
    {
        _attendanceService = attendanceService;
        _enrolmentService = enrolmentService;
        _lecturerService = lecturerService;
        _studentService = studentService;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Lista os registos de presença de uma matrícula específica.
    /// Student só pode ver a sua própria matrícula.
    /// </summary>
    /// <param name="enrolmentId">Identificador da matrícula.</param>
    public async Task<IActionResult> Index(int enrolmentId)
    {
        var userId = _userManager.GetUserId(User)!;

        if (User.IsInRole(ApplicationRoles.Student))
        {
            var studentProfile = await _studentService.GetByIdentityUserIdAsync(userId);

            if (studentProfile == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var enrolment = await _enrolmentService.GetByIdAsync(enrolmentId);

            if (enrolment == null || enrolment.StudentProfileId != studentProfile.Id)
            {
                _logger.LogWarning(
                    "Unauthorized attendance access attempt by student {UserId} for enrolment {EnrolmentId}.",
                    userId, enrolmentId);
                return Forbid();
            }
        }

        var records = await _attendanceService.GetByEnrolmentAsync(enrolmentId);
        return View(records);
    }

    /// <summary>
    /// Apresenta o formulário de registo de presença. Acesso exclusivo ao Lecturer.
    /// </summary>
    /// <param name="enrolmentId">Identificador da matrícula do aluno.</param>
    [Authorize(Roles = ApplicationRoles.Lecturer)]
    public async Task<IActionResult> Create(int enrolmentId)
    {
        var enrolment = await _enrolmentService.GetByIdAsync(enrolmentId);

        if (enrolment == null)
        {
            return NotFound();
        }

        var model = new AttendanceViewModel
        {
            EnrolmentId = enrolmentId,
            StudentName = $"{enrolment.Student?.FirstName} {enrolment.Student?.LastName}",
            CourseName = enrolment.Course?.CourseName ?? string.Empty,
            SessionDate = DateOnly.FromDateTime(DateTime.Today)
        };

        return View(model);
    }

    /// <summary>
    /// Processa o formulário de registo de presença. Acesso exclusivo ao Lecturer.
    /// </summary>
    /// <param name="model">Dados do formulário de registo de presença.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = ApplicationRoles.Lecturer)]
    public async Task<IActionResult> Create(AttendanceViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = _userManager.GetUserId(User)!;
        var lecturerProfile = await _lecturerService.GetByIdentityUserIdAsync(userId);

        if (lecturerProfile == null)
        {
            return Forbid();
        }

        try
        {
            await _attendanceService.RecordAttendanceAsync(
                model.EnrolmentId,
                model.SessionDate,
                model.Present,
                lecturerProfile.Id);

            _logger.LogInformation(
                "Attendance recorded for enrolment {EnrolmentId} on {Date} by lecturer {LecturerId}.",
                model.EnrolmentId, model.SessionDate, lecturerProfile.Id);

            return RedirectToAction(nameof(Index), new { enrolmentId = model.EnrolmentId });
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(model);
        }
    }
}