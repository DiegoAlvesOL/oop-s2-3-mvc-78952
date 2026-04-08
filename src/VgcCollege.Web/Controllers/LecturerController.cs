using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VgcCollege.Application.Services;
using VgcCollege.Domain.Constants;
using VgcCollege.Domain.Entities;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers;

/// <summary>
/// Controller de LecturerProfile. Todas as acções requerem o role Admin.
/// </summary>
[Authorize(Roles = ApplicationRoles.Admin)]
public class LecturerController : Controller
{
    private readonly LecturerService _lecturerService;
    private readonly CourseService _courseService;
    private readonly ILogger<LecturerController> _logger;

    /// <summary>
    /// Inicializa o controller com os services necessários e o logger.
    /// </summary>
    /// <param name="lecturerService">Service de perfis de lecturers.</param>
    /// <param name="courseService">Service de cursos, usado para popular o dropdown de atribuição.</param>
    /// <param name="logger">Serviço de logging.</param>
    public LecturerController(
        LecturerService lecturerService,
        CourseService courseService,
        ILogger<LecturerController> logger)
    {
        _lecturerService = lecturerService;
        _courseService = courseService;
        _logger = logger;
    }

    /// <summary>Lista todos os lecturers do sistema.</summary>
    public async Task<IActionResult> Index()
    {
        var lecturers = await _lecturerService.GetAllAsync();
        return View(lecturers);
    }

    /// <summary>Apresenta o formulário de criação de perfil de lecturer.</summary>
    public IActionResult Create()
    {
        return View(new LecturerViewModel());
    }

    /// <summary>Processa o formulário de criação de perfil de lecturer.</summary>
    /// <param name="model">Dados do formulário de criação.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LecturerViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var lecturer = new LecturerProfile
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Phone = model.Phone ?? string.Empty,
            IdentityUserId = string.Empty
        };

        await _lecturerService.CreateAsync(lecturer);
        _logger.LogInformation("Lecturer profile created for {Email} by {User}.", model.Email, User.Identity!.Name);

        return RedirectToAction(nameof(Index));
    }

    /// <summary>Apresenta o formulário de edição de perfil de lecturer.</summary>
    /// <param name="id">Identificador do perfil a editar.</param>
    public async Task<IActionResult> Edit(int id)
    {
        var lecturer = await _lecturerService.GetByIdAsync(id);

        if (lecturer == null)
        {
            return NotFound();
        }

        var model = new LecturerViewModel
        {
            Id = lecturer.Id,
            FirstName = lecturer.FirstName,
            LastName = lecturer.LastName,
            Email = lecturer.Email,
            Phone = lecturer.Phone
        };

        return View(model);
    }

    /// <summary>Processa o formulário de edição de perfil de lecturer.</summary>
    /// <param name="id">Identificador do perfil a editar.</param>
    /// <param name="model">Dados do formulário de edição.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LecturerViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var lecturer = new LecturerProfile
        {
            Id = id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Phone = model.Phone ?? string.Empty,
            IdentityUserId = string.Empty
        };

        await _lecturerService.UpdateAsync(lecturer);
        _logger.LogInformation("Lecturer profile {LecturerId} updated by {User}.", id, User.Identity!.Name);

        return RedirectToAction(nameof(Index));
    }

    /// <summary>Remove um perfil de lecturer pelo seu identificador único.</summary>
    /// <param name="id">Identificador do perfil a remover.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _lecturerService.DeleteAsync(id);
        _logger.LogInformation("Lecturer profile {LecturerId} deleted by {User}.", id, User.Identity!.Name);

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Apresenta o formulário de atribuição de um lecturer a um curso.
    /// </summary>
    /// <param name="id">Identificador do perfil do lecturer.</param>
    public async Task<IActionResult> Assign(int id)
    {
        var lecturer = await _lecturerService.GetByIdAsync(id);

        if (lecturer == null)
        {
            return NotFound();
        }

        var courses = await _courseService.GetAllAsync();

        var model = new AssignLecturerViewModel
        {
            LecturerProfileId = id,
            LecturerName = $"{lecturer.FirstName} {lecturer.LastName}",
            AvailableCourses = courses.Select(course => new SelectListItem
            {
                Value = course.Id.ToString(),
                Text = $"{course.CourseName} — {course.Branch?.BranchName}"
            }).ToList()
        };

        return View(model);
    }

    /// <summary>Processa o formulário de atribuição de um lecturer a um curso.</summary>
    /// <param name="model">Dados do formulário de atribuição.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(AssignLecturerViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var courses = await _courseService.GetAllAsync();
            model.AvailableCourses = courses.Select(course => new SelectListItem
            {
                Value = course.Id.ToString(),
                Text = $"{course.CourseName} — {course.Branch?.BranchName}"
            }).ToList();

            return View(model);
        }

        try
        {
            await _lecturerService.AssignToCourseAsync(
                model.LecturerProfileId,
                model.CourseId,
                model.IsTutor);

            _logger.LogInformation(
                "Lecturer {LecturerId} assigned to course {CourseId} (IsTutor: {IsTutor}) by {User}.",
                model.LecturerProfileId, model.CourseId, model.IsTutor, User.Identity!.Name);

            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);

            var courses = await _courseService.GetAllAsync();
            model.AvailableCourses = courses.Select(course => new SelectListItem
            {
                Value = course.Id.ToString(),
                Text = $"{course.CourseName} — {course.Branch?.BranchName}"
            }).ToList();

            return View(model);
        }
    }
}