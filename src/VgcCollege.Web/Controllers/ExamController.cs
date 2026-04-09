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
/// Controller de Exam com controlo de acesso por role.
/// </summary>
[Authorize]
public class ExamController : Controller
{
    private readonly ExamService _examService;
    private readonly LecturerService _lecturerService;
    private readonly StudentService _studentService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ExamController> _logger;

    /// <summary>
    /// Inicializa o controller com os services necessários.
    /// </summary>
    public ExamController(
        ExamService examService,
        LecturerService lecturerService,
        StudentService studentService,
        UserManager<ApplicationUser> userManager,
        ILogger<ExamController> logger)
    {
        _examService = examService;
        _lecturerService = lecturerService;
        _studentService = studentService;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Lista os exames de um curso. Lecturer e Admin vêem todos.
    /// Student é redirecionado para os seus próprios resultados.
    /// </summary>
    /// <param name="courseId">Identificador do curso.</param>
    public async Task<IActionResult> Index(int courseId)
    {
        if (User.IsInRole(ApplicationRoles.Student))
        {
            return RedirectToAction(nameof(MyResults));
        }

        var exams = await _examService.GetByCourseAsync(courseId);
        ViewBag.CourseId = courseId;
        return View(exams);
    }

    /// <summary>
    /// Apresenta o formulário de criação de exame. Acesso exclusivo ao Lecturer.
    /// </summary>
    /// <param name="courseId">Identificador do curso ao qual o exame pertence.</param>
    [Authorize(Roles = ApplicationRoles.Lecturer)]
    public IActionResult Create(int courseId)
    {
        return View(new ExamViewModel { CourseId = courseId });
    }

    /// <summary>
    /// Processa o formulário de criação de exame. Acesso exclusivo ao Lecturer.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = ApplicationRoles.Lecturer)]
    public async Task<IActionResult> Create(ExamViewModel model)
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
            var exam = new Exam
            {
                CourseId = model.CourseId,
                Title = model.Title,
                ExamDate = model.ExamDate,
                MaxScore = model.MaxScore,
                ResultsReleased = false
            };

            await _examService.CreateExamAsync(exam, lecturerProfile.Id);
            _logger.LogInformation(
                "Exam {Title} created for course {CourseId} by lecturer {LecturerId}.",
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
    /// Liberta os resultados de um exame. Acesso exclusivo ao Admin.
    /// Muda ResultsReleased de false para true.
    /// </summary>
    /// <param name="examId">Identificador do exame a libertar.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = ApplicationRoles.Admin)]
    public async Task<IActionResult> ReleaseResults(int examId, int courseId)
    {
        await _examService.ReleaseResultsAsync(examId);
        _logger.LogInformation("Results released for exam {ExamId} by {User}.", examId, User.Identity!.Name);

        return RedirectToAction(nameof(Index), new { courseId });
    }

    /// <summary>
    /// Apresenta o formulário de lançamento de resultado. Acesso exclusivo ao Lecturer.
    /// </summary>
    [Authorize(Roles = ApplicationRoles.Lecturer)]
    public async Task<IActionResult> SetResult(int examId, int studentProfileId)
    {
        var exam = await _examService.GetByCourseAsync(0);
        var examEntity = (await _examService.GetByCourseAsync(
            (await _examService.GetResultsByExamAsync(examId))
            .FirstOrDefault()?.Exam?.CourseId ?? 0))
            .FirstOrDefault(e => e.Id == examId);

        var userId = _userManager.GetUserId(User)!;
        var student = await _studentService.GetByIdAsync(studentProfileId, userId, isAdmin: true);

        if (student == null)
        {
            return NotFound();
        }

        var model = new ExamResultViewModel
        {
            ExamId = examId,
            StudentProfileId = studentProfileId,
            ExamTitle = examEntity?.Title ?? string.Empty,
            StudentName = $"{student.FirstName} {student.LastName}",
            MaxScore = examEntity?.MaxScore ?? 0
        };

        return View(model);
    }

    /// <summary>
    /// Processa o formulário de lançamento de resultado. Acesso exclusivo ao Lecturer.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = ApplicationRoles.Lecturer)]
    public async Task<IActionResult> SetResult(ExamResultViewModel model)
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
            var result = new ExamResult
            {
                ExamId = model.ExamId,
                StudentProfileId = model.StudentProfileId,
                Score = model.Score,
                Grade = model.Grade
            };

            await _examService.SetResultAsync(result, lecturerProfile.Id);
            _logger.LogInformation(
                "Exam result set for student {StudentId} on exam {ExamId} by lecturer {LecturerId}.",
                model.StudentProfileId, model.ExamId, lecturerProfile.Id);

            return RedirectToAction(nameof(Index), new { courseId = 0 });
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(model);
        }
    }

    /// <summary>
    /// Apresenta os resultados dos exames do Student autenticado.
    /// Apenas exames com ResultsReleased = true são apresentados.
    /// </summary>
    [Authorize(Roles = ApplicationRoles.Student)]
    public async Task<IActionResult> MyResults()
    {
        var userId = _userManager.GetUserId(User)!;
        var studentProfile = await _studentService.GetByIdentityUserIdAsync(userId);

        if (studentProfile == null)
        {
            return View(Enumerable.Empty<ExamResult>());
        }

        return View(studentProfile);
    }
}