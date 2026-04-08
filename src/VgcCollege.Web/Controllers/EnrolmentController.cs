using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VgcCollege.Application.Services;
using VgcCollege.Data.Models;
using VgcCollege.Domain.Constants;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers;

/// <summary>
/// Controller de CourseEnrolment com controlo de acesso por role.
/// </summary>
[Authorize]
public class EnrolmentController : Controller
{
    private readonly EnrolmentService _enrolmentService;
    private readonly StudentService _studentService;
    private readonly CourseService _courseService;
    private readonly LecturerService _lecturerService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<EnrolmentController> _logger;

    /// <summary>
    /// Inicializa o controller com os services necessários.
    /// </summary>
    public EnrolmentController(
        EnrolmentService enrolmentService,
        StudentService studentService,
        CourseService courseService,
        LecturerService lecturerService,
        UserManager<ApplicationUser> userManager,
        ILogger<EnrolmentController> logger)
    {
        _enrolmentService = enrolmentService;
        _studentService = studentService;
        _courseService = courseService;
        _lecturerService = lecturerService;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Lista matrículas conforme o role do utilizador autenticado.
    /// Admin vê todas por curso. Lecturer vê as dos seus cursos. Student vê as próprias.
    /// </summary>
    public async Task<IActionResult> Index(int? courseId)
    {
        var userId = _userManager.GetUserId(User)!;

        if (User.IsInRole(ApplicationRoles.Admin))
        {
            if (courseId.HasValue)
            {
                var enrolments = await _enrolmentService.GetByCourseAsync(courseId.Value);
                return View(enrolments);
            }

            var courses = await _courseService.GetAllAsync();
            ViewBag.Courses = courses;
            return View("SelectCourse");
        }

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
                return View(Enumerable.Empty<VgcCollege.Domain.Entities.CourseEnrolment>());
            }

            var targetCourseId = courseId ?? firstCourse.CourseId;
            var enrolments = await _enrolmentService.GetByCourseAsync(targetCourseId);
            ViewBag.LecturerCourses = assignments;
            return View(enrolments);
        }

        var studentProfile = await _studentService.GetByIdentityUserIdAsync(userId);

        if (studentProfile == null)
        {
            return View(Enumerable.Empty<VgcCollege.Domain.Entities.CourseEnrolment>());
        }

        var studentEnrolments = await _enrolmentService.GetByStudentAsync(studentProfile.Id);
        return View(studentEnrolments);
    }

    /// <summary>
    /// Apresenta o formulário de matrícula. Acesso exclusivo ao Admin.
    /// </summary>
    [Authorize(Roles = ApplicationRoles.Admin)]
    public async Task<IActionResult> Create()
    {
        var model = new EnrolStudentViewModel
        {
            AvailableStudents = await GetStudentSelectListAsync(),
            AvailableCourses = await GetCourseSelectListAsync()
        };

        return View(model);
    }

    /// <summary>
    /// Processa o formulário de matrícula. Acesso exclusivo ao Admin.
    /// </summary>
    /// <param name="model">Dados do formulário de matrícula.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = ApplicationRoles.Admin)]
    public async Task<IActionResult> Create(EnrolStudentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableStudents = await GetStudentSelectListAsync();
            model.AvailableCourses = await GetCourseSelectListAsync();
            return View(model);
        }

        try
        {
            await _enrolmentService.EnrolStudentAsync(model.StudentProfileId, model.CourseId);
            _logger.LogInformation(
                "Student {StudentId} enrolled in course {CourseId} by {User}.",
                model.StudentProfileId, model.CourseId, User.Identity!.Name);

            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            model.AvailableStudents = await GetStudentSelectListAsync();
            model.AvailableCourses = await GetCourseSelectListAsync();
            return View(model);
        }
    }

    /// <summary>
    /// Método auxiliar que constrói a lista de alunos para o dropdown.
    /// </summary>
    private async Task<List<SelectListItem>> GetStudentSelectListAsync()
    {
        var students = await _studentService.GetAllAsync();

        return students.Select(student => new SelectListItem
        {
            Value = student.Id.ToString(),
            Text = $"{student.FirstName} {student.LastName} ({student.StudentNumber})"
        }).ToList();
    }

    /// <summary>
    /// Método auxiliar que constrói a lista de cursos para o dropdown.
    /// </summary>
    private async Task<List<SelectListItem>> GetCourseSelectListAsync()
    {
        var courses = await _courseService.GetAllAsync();

        return courses.Select(course => new SelectListItem
        {
            Value = course.Id.ToString(),
            Text = $"{course.CourseName} — {course.Branch?.BranchName}"
        }).ToList();
    }
}