// Purpose   : Gere as acções de CRUD para StudentProfile com controlo de acesso por role.
//             Admin acede a todos os perfis. Student acede apenas ao próprio.
// Consumed by: Views/Student/.
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
/// Controller de StudentProfile com controlo de acesso por role.
/// Admin acede a todos os perfis. Student acede apenas ao próprio.
/// </summary>
[Authorize]
public class StudentController : Controller
{
    private readonly StudentService _studentService;
    private readonly EnrolmentService _enrolmentService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<StudentController> _logger;

    /// <summary>
    /// Inicializa o controller com os serviços necessários.
    /// </summary>
    /// <param name="studentService">Service de perfis de alunos.</param>
    /// <param name="enrolmentService">Service de matrículas.</param>
    /// <param name="userManager">Serviço do Identity para obter o utilizador autenticado.</param>
    /// <param name="logger">Serviço de logging.</param>
    public StudentController(
        StudentService studentService,
        EnrolmentService enrolmentService,
        UserManager<ApplicationUser> userManager,
        ILogger<StudentController> logger)
    {
        _studentService = studentService;
        _enrolmentService = enrolmentService;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Lista todos os alunos (Admin) ou redireciona para o perfil próprio (Student).
    /// </summary>
    public async Task<IActionResult> Index()
    {
        if (User.IsInRole(ApplicationRoles.Admin))
        {
            var students = await _studentService.GetAllAsync();
            return View(students);
        }

        var userId = _userManager.GetUserId(User)!;
        var profile = await _studentService.GetByIdentityUserIdAsync(userId);

        if (profile == null)
        {
            return RedirectToAction(nameof(Create));
        }

        return RedirectToAction(nameof(Edit), new { id = profile.Id });
    }

    /// <summary>Apresenta o formulário de criação de perfil de aluno. Acesso exclusivo ao Admin.</summary>
    [Authorize(Roles = ApplicationRoles.Admin)]
    public IActionResult Create()
    {
        return View(new StudentViewModel());
    }

    /// <summary>Processa o formulário de criação de perfil de aluno. Acesso exclusivo ao Admin.</summary>
    /// <param name="model">Dados do formulário de criação.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = ApplicationRoles.Admin)]
    public async Task<IActionResult> Create(StudentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var student = new StudentProfile
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Phone = model.Phone,
            StreetName = model.StreetName,
            City = model.City,
            StudentNumber = model.StudentNumber,
            IdentityUserId = string.Empty
        };

        await _studentService.CreateAsync(student);
        _logger.LogInformation("Student profile created for {StudentNumber} by {User}.", model.StudentNumber, User.Identity!.Name);

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Apresenta o formulário de edição de perfil.
    /// Admin pode editar qualquer perfil. Student pode editar apenas o próprio.
    /// </summary>
    /// <param name="id">Identificador do perfil a editar.</param>
    public async Task<IActionResult> Edit(int id)
    {
        var userId = _userManager.GetUserId(User)!;
        var isAdmin = User.IsInRole(ApplicationRoles.Admin);

        try
        {
            var student = await _studentService.GetByIdAsync(id, userId, isAdmin);

            if (student == null)
            {
                return NotFound();
            }

            var model = new StudentViewModel
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                Phone = student.Phone,
                StreetName = student.StreetName,
                City = student.City,
                StudentNumber = student.StudentNumber
            };

            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            _logger.LogWarning("Unauthorized access attempt to student profile {ProfileId} by user {UserId}.", id, userId);
            return Forbid();
        }
    }

    /// <summary>
    /// Processa o formulário de edição de perfil.
    /// Admin pode editar qualquer perfil. Student pode editar apenas o próprio.
    /// </summary>
    /// <param name="id">Identificador do perfil a editar.</param>
    /// <param name="model">Dados do formulário de edição.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, StudentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = _userManager.GetUserId(User)!;
        var isAdmin = User.IsInRole(ApplicationRoles.Admin);

        try
        {
            // Obtém o IdentityUserId original do aluno para não o sobrescrever
            // com o ID do utilizador autenticado (que pode ser o Admin).
            var existing = await _studentService.GetByIdAsync(id, userId, isAdmin);

            if (existing == null)
            {
                return NotFound();
            }

            var student = new StudentProfile
            {
                Id = id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.Phone,
                StreetName = model.StreetName,
                City = model.City,
                StudentNumber = model.StudentNumber,
                IdentityUserId = existing.IdentityUserId
            };

            await _studentService.UpdateAsync(student, userId, isAdmin);
            _logger.LogInformation("Student profile {ProfileId} updated by {User}.", id, User.Identity!.Name);

            return RedirectToAction(nameof(Index));
        }
        catch (UnauthorizedAccessException)
        {
            _logger.LogWarning("Unauthorized update attempt on student profile {ProfileId} by user {UserId}.", id, userId);
            return Forbid();
        }
    }

    /// <summary>
    /// Lista todos os cursos em que um aluno está matriculado.
    /// Acesso exclusivo ao Admin.
    /// </summary>
    /// <param name="id">Identificador do perfil do aluno.</param>
    [Authorize(Roles = ApplicationRoles.Admin)]
    public async Task<IActionResult> Enrolments(int id)
    {
        var student = await _studentService.GetByIdAsync(id, _userManager.GetUserId(User)!, isAdmin: true);

        if (student == null)
        {
            return NotFound();
        }

        var enrolments = await _enrolmentService.GetByStudentAsync(id);
        ViewBag.StudentName = $"{student.FirstName} {student.LastName}";

        return View(enrolments);
    }

    /// <summary>Remove um perfil de aluno. Acesso exclusivo ao Admin.</summary>
    /// <param name="id">Identificador do perfil a remover.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = ApplicationRoles.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        await _studentService.DeleteAsync(id);
        _logger.LogInformation("Student profile {ProfileId} deleted by {User}.", id, User.Identity!.Name);

        return RedirectToAction(nameof(Index));
    }
}