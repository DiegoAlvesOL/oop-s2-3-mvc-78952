// Purpose   : Gere as acções do gradebook com controlo de acesso por role.
//             Lecturer cria assignments e lança resultados. Student vê apenas os próprios.
// Consumed by: Views/Gradebook/.
// Layer     : Web — Controllers

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VgcCollege.Application.Services;
using VgcCollege.Data.Models;
using VgcCollege.Domain.Constants;
using VgcCollege.Domain.Entities;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers;

/// <summary>
/// Controller do gradebook com controlo de acesso por role.
/// </summary>
[Authorize]
public class GradebookController : Controller
{
    private readonly AssignmentService _assignmentService;
    private readonly LecturerService _lecturerService;
    private readonly StudentService _studentService;
    private readonly EnrolmentService _enrolmentService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<GradebookController> _logger;

    /// <summary>
    /// Inicializa o controller com os services necessários.
    /// </summary>
    public GradebookController(
        AssignmentService assignmentService,
        LecturerService lecturerService,
        StudentService studentService,
        EnrolmentService enrolmentService,
        UserManager<ApplicationUser> userManager,
        ILogger<GradebookController> logger)
    {
        _assignmentService = assignmentService;
        _lecturerService = lecturerService;
        _studentService = studentService;
        _enrolmentService = enrolmentService;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Apresenta o gradebook conforme o role do utilizador autenticado.
    /// Lecturer vê os assignments do seu curso. Student vê os seus resultados.
    /// </summary>
    /// <param name="courseId">Identificador do curso (para o Lecturer).</param>
    public async Task<IActionResult> Index(int? courseId)
    {
        var userId = _userManager.GetUserId(User)!;

        if (User.IsInRole(ApplicationRoles.Lecturer))
        {
            var lecturerProfile = await _lecturerService.GetByIdentityUserIdAsync(userId);

            if (lecturerProfile == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var assignments = await _lecturerService.GetCourseAssignmentsAsync(lecturerProfile.Id);
            var firstCourse = assignments.FirstOrDefault();

            if (firstCourse == null)
            {
                return View("NoAssignments");
            }

            var targetCourseId = courseId ?? firstCourse.CourseId;
            var courseAssignments = await _assignmentService.GetByCourseAsync(targetCourseId);
            ViewBag.LecturerCourses = assignments;
            ViewBag.CurrentCourseId = targetCourseId;
            ViewBag.LecturerProfileId = lecturerProfile.Id;
            return View(courseAssignments);
        }

        var studentProfile = await _studentService.GetByIdentityUserIdAsync(userId);

        if (studentProfile == null)
        {
            return View(Enumerable.Empty<AssignmentResult>());
        }

        var results = await _assignmentService.GetResultsByStudentAsync(studentProfile.Id);
        return View("StudentResults", results);
    }

    /// <summary>
    /// Lista todos os alunos matriculados no curso do assignment e os seus resultados.
    /// Acesso exclusivo ao Lecturer.
    /// </summary>
    /// <param name="assignmentId">Identificador do assignment.</param>
    [Authorize(Roles = ApplicationRoles.Lecturer)]
    public async Task<IActionResult> Results(int assignmentId)
    {
        var userId = _userManager.GetUserId(User)!;
        var lecturerProfile = await _lecturerService.GetByIdentityUserIdAsync(userId);

        if (lecturerProfile == null)
        {
            return Forbid();
        }

        var assignment = await _assignmentService.GetByIdAsync(assignmentId);

        if (assignment == null)
        {
            return NotFound();
        }

        var existingResults = await _assignmentService.GetResultsByAssignmentAsync(assignmentId);
        var enrolments = await _enrolmentService.GetByCourseAsync(assignment.CourseId);

        ViewBag.Assignment = assignment;
        ViewBag.ExistingResults = existingResults.ToDictionary(r => r.StudentProfileId, r => r);

        return View(enrolments);
    }

    /// <summary>
    /// Apresenta o formulário de criação de assignment. Acesso exclusivo ao Lecturer.
    /// </summary>
    /// <param name="courseId">Identificador do curso ao qual o assignment pertence.</param>
    [Authorize(Roles = ApplicationRoles.Lecturer)]
    public IActionResult Create(int courseId)
    {
        return View(new AssignmentViewModel { CourseId = courseId });
    }

    /// <summary>
    /// Processa o formulário de criação de assignment. Acesso exclusivo ao Lecturer.
    /// </summary>
    /// <param name="model">Dados do formulário de criação.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = ApplicationRoles.Lecturer)]
    public async Task<IActionResult> Create(AssignmentViewModel model)
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
            var assignment = new Assignment
            {
                CourseId = model.CourseId,
                Title = model.Title,
                MaxScore = model.MaxScore,
                DueDate = model.DueDate
            };

            await _assignmentService.CreateAssignmentAsync(assignment, lecturerProfile.Id);
            _logger.LogInformation(
                "Assignment {Title} created for course {CourseId} by lecturer {LecturerId}.",
                model.Title, model.CourseId, lecturerProfile.Id);

            return RedirectToAction(nameof(Index), new { courseId = model.CourseId });
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(model);
        }
    }

    /// <summary>
    /// Apresenta o formulário de lançamento de resultado. Acesso exclusivo ao Lecturer.
    /// </summary>
    /// <param name="assignmentId">Identificador do assignment.</param>
    /// <param name="studentProfileId">Identificador do perfil do aluno.</param>
    [Authorize(Roles = ApplicationRoles.Lecturer)]
    public async Task<IActionResult> SetResult(int assignmentId, int studentProfileId)
    {
        var assignment = await _assignmentService.GetByIdAsync(assignmentId);

        var student = await _studentService.GetByIdAsync(studentProfileId, _userManager.GetUserId(User)!, isAdmin: true);

        if (student == null)
        {
            return NotFound();
        }

        var model = new AssignmentResultViewModel
        {
            AssignmentId = assignmentId,
            StudentProfileId = studentProfileId,
            AssignmentTitle = assignment?.Title ?? string.Empty,
            StudentName = $"{student.FirstName} {student.LastName}",
            MaxScore = assignment?.MaxScore ?? 0
        };

        return View(model);
    }

    /// <summary>
    /// Processa o formulário de lançamento de resultado. Acesso exclusivo ao Lecturer.
    /// </summary>
    /// <param name="model">Dados do formulário de lançamento.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = ApplicationRoles.Lecturer)]
    public async Task<IActionResult> SetResult(AssignmentResultViewModel model)
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
            var result = new AssignmentResult
            {
                AssignmentId = model.AssignmentId,
                StudentProfileId = model.StudentProfileId,
                Score = model.Score,
                Feedback = model.Feedback
            };

            await _assignmentService.SetResultAsync(result, lecturerProfile.Id);
            _logger.LogInformation(
                "Result set for student {StudentId} on assignment {AssignmentId} by lecturer {LecturerId}.",
                model.StudentProfileId, model.AssignmentId, lecturerProfile.Id);

            return RedirectToAction(nameof(Results), new { assignmentId = model.AssignmentId });
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(model);
        }
    }
}