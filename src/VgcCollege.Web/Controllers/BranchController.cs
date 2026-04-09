// Purpose   : Gere as acções de CRUD para Branch. Acesso exclusivo ao role Admin.
// Consumed by: Views/Branch/.
// Layer     : Web — Controllers

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VgcCollege.Application.Services;
using VgcCollege.Domain.Constants;
using VgcCollege.Domain.Entities;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers;

/// <summary>
/// Controller de Branch. Todas as acções requerem o role Admin.
/// </summary>
[Authorize(Roles = ApplicationRoles.Admin)]
public class BranchController : Controller
{
    private readonly BranchService _branchService;
    private readonly CourseService _courseService;
    private readonly EnrolmentService _enrolmentService;
    private readonly LecturerService _lecturerService;
    private readonly ILogger<BranchController> _logger;

    /// <summary>
    /// Inicializa o controller com os services necessários e o logger.
    /// </summary>
    /// <param name="branchService">Service de branches injectado via DI.</param>
    /// <param name="courseService">Service de cursos injectado via DI.</param>
    /// <param name="enrolmentService">Service de matrículas injectado via DI.</param>
    /// <param name="lecturerService">Service de lecturers injectado via DI.</param>
    /// <param name="logger">Serviço de logging.</param>
    public BranchController(
        BranchService branchService,
        CourseService courseService,
        EnrolmentService enrolmentService,
        LecturerService lecturerService,
        ILogger<BranchController> logger)
    {
        _branchService = branchService;
        _courseService = courseService;
        _enrolmentService = enrolmentService;
        _lecturerService = lecturerService;
        _logger = logger;
    }

    /// <summary>Lista todas as branches do sistema.</summary>
    public async Task<IActionResult> Index()
    {
        var branches = await _branchService.GetAllAsync();
        return View(branches);
    }

    /// <summary>
    /// Apresenta os detalhes de uma branch — cursos, lecturers e alunos matriculados.
    /// </summary>
    /// <param name="id">Identificador da branch.</param>
    public async Task<IActionResult> Details(int id)
    {
        var branch = await _branchService.GetByIdAsync(id);

        if (branch == null)
        {
            return NotFound();
        }

        var courses = await _courseService.GetByBranchAsync(id);
        var courseDetails = new List<BranchCourseDetailViewModel>();

        foreach (var course in courses)
        {
            var enrolments = await _enrolmentService.GetByCourseAsync(course.Id);
            var lecturerAssignments = await _lecturerService.GetAssignmentsByCourseAsync(course.Id);

            courseDetails.Add(new BranchCourseDetailViewModel
            {
                Course = course,
                Enrolments = enrolments.ToList(),
                LecturerAssignments = lecturerAssignments.ToList()
            });
        }

        ViewBag.Branch = branch;
        return View(courseDetails);
    }

    /// <summary>Apresenta o formulário de criação de uma nova branch.</summary>
    public IActionResult Create()
    {
        return View(new BranchViewModel());
    }

    /// <summary>Processa o formulário de criação de uma nova branch.</summary>
    /// <param name="model">Dados do formulário de criação.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BranchViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var branch = new Branch
        {
            BranchName = model.BranchName,
            StreetName = model.StreetName,
            City = model.City
        };

        await _branchService.CreateAsync(branch);
        _logger.LogInformation("Branch {BranchName} created by {User}.", branch.BranchName, User.Identity!.Name);

        return RedirectToAction(nameof(Index));
    }

    /// <summary>Apresenta o formulário de edição de uma branch existente.</summary>
    /// <param name="id">Identificador da branch a editar.</param>
    public async Task<IActionResult> Edit(int id)
    {
        var branch = await _branchService.GetByIdAsync(id);

        if (branch == null)
        {
            return NotFound();
        }

        var model = new BranchViewModel
        {
            Id = branch.Id,
            BranchName = branch.BranchName,
            StreetName = branch.StreetName,
            City = branch.City
        };

        return View(model);
    }

    /// <summary>Processa o formulário de edição de uma branch existente.</summary>
    /// <param name="id">Identificador da branch a editar.</param>
    /// <param name="model">Dados do formulário de edição.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BranchViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var branch = new Branch
        {
            Id = id,
            BranchName = model.BranchName,
            StreetName = model.StreetName,
            City = model.City
        };

        await _branchService.UpdateAsync(branch);
        _logger.LogInformation("Branch {BranchId} updated by {User}.", id, User.Identity!.Name);

        return RedirectToAction(nameof(Index));
    }

    /// <summary>Remove uma branch pelo seu identificador único.</summary>
    /// <param name="id">Identificador da branch a remover.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _branchService.DeleteAsync(id);
        _logger.LogInformation("Branch {BranchId} deleted by {User}.", id, User.Identity!.Name);

        return RedirectToAction(nameof(Index));
    }
}