// Purpose   : Gere as acções de CRUD para Course. Acesso exclusivo ao role Admin.
// Consumed by: Views/Course/.
// Layer     : Web — Controllers

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VgcCollege.Application.Services;
using VgcCollege.Domain.Constants;
using VgcCollege.Domain.Entities;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers;

/// <summary>
/// Controller de Course. Todas as acções requerem o role Admin.
/// </summary>
[Authorize(Roles = ApplicationRoles.Admin)]
public class CourseController : Controller
{
    private readonly CourseService _courseService;
    private readonly BranchService _branchService;
    private readonly ILogger<CourseController> _logger;

    /// <summary>
    /// Inicializa o controller com os services necessários e o logger.
    /// </summary>
    /// <param name="courseService">Service de cursos injectado via DI.</param>
    /// <param name="branchService">Service de branches injectado via DI.</param>
    /// <param name="logger">Serviço de logging.</param>
    public CourseController(
        CourseService courseService,
        BranchService branchService,
        ILogger<CourseController> logger)
    {
        _courseService = courseService;
        _branchService = branchService;
        _logger = logger;
    }

    /// <summary>Lista todos os cursos do sistema.</summary>
    public async Task<IActionResult> Index()
    {
        var courses = await _courseService.GetAllAsync();
        return View(courses);
    }

    /// <summary>Apresenta o formulário de criação de um novo curso.</summary>
    public async Task<IActionResult> Create()
    {
        var model = new CourseViewModel
        {
            AvailableBranches = await GetBranchSelectListAsync()
        };

        return View(model);
    }

    /// <summary>Processa o formulário de criação de um novo curso.</summary>
    /// <param name="model">Dados do formulário de criação.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CourseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableBranches = await GetBranchSelectListAsync();
            return View(model);
        }

        var course = new Course
        {
            CourseName = model.CourseName,
            BranchId = model.BranchId,
            StartDate = model.StartDate,
            EndDate = model.EndDate
        };

        try
        {
            await _courseService.CreateAsync(course);
            _logger.LogInformation("Course {CourseName} created by {User}.", course.CourseName, User.Identity!.Name);
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            model.AvailableBranches = await GetBranchSelectListAsync();
            return View(model);
        }
    }

    /// <summary>Apresenta o formulário de edição de um curso existente.</summary>
    /// <param name="id">Identificador do curso a editar.</param>
    public async Task<IActionResult> Edit(int id)
    {
        var course = await _courseService.GetByIdAsync(id);

        if (course == null)
        {
            return NotFound();
        }

        var model = new CourseViewModel
        {
            Id = course.Id,
            CourseName = course.CourseName,
            BranchId = course.BranchId,
            StartDate = course.StartDate,
            EndDate = course.EndDate,
            AvailableBranches = await GetBranchSelectListAsync()
        };

        return View(model);
    }

    /// <summary>Processa o formulário de edição de um curso existente.</summary>
    /// <param name="id">Identificador do curso a editar.</param>
    /// <param name="model">Dados do formulário de edição.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CourseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableBranches = await GetBranchSelectListAsync();
            return View(model);
        }

        var course = new Course
        {
            Id = id,
            CourseName = model.CourseName,
            BranchId = model.BranchId,
            StartDate = model.StartDate,
            EndDate = model.EndDate
        };

        await _courseService.UpdateAsync(course);
        _logger.LogInformation("Course {CourseId} updated by {User}.", id, User.Identity!.Name);

        return RedirectToAction(nameof(Index));
    }

    /// <summary>Remove um curso pelo seu identificador único.</summary>
    /// <param name="id">Identificador do curso a remover.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _courseService.DeleteAsync(id);
        _logger.LogInformation("Course {CourseId} deleted by {User}.", id, User.Identity!.Name);

        return RedirectToAction(nameof(Index));
    }

    /// <summary>Método auxiliar que constrói a lista de branches para o dropdown.</summary>
    private async Task<List<SelectListItem>> GetBranchSelectListAsync()
    {
        var branches = await _branchService.GetAllAsync();

        return branches.Select(branch => new SelectListItem
        {
            Value = branch.Id.ToString(),
            Text = $"{branch.BranchName} — {branch.City}"
        }).ToList();
    }
}