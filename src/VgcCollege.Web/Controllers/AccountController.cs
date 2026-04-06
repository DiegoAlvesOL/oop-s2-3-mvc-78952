using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VgcCollege.Data.Models;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers;

/// <summary>
/// Purpose: Gere as acções de autenticação, login e logout.
/// Consumed by: Views/Account/Login.cshtml, _Layout.cshtml (links de login/logout).
/// Layer: Web Controllers
/// </summary>
public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    /// <summary>
    /// Inicializa o controller com os serviços de autenticação e logging.
    /// </summary>
    /// <param name="signInManager">Serviço de autenticação do Identity.</param>
    /// <param name="logger">Serviço de logging.</param>
    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }

    /// <summary>
    /// Apresenta o formulário de login.
    /// </summary>
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    /// <summary>
    /// Processa o formulário de login submetido pelo utilizador.
    /// </summary>
    /// <param name="model">Dados do formulário de login.</param>
    /// <param name="returnUrl">URL para redirecionar após login bem-sucedido.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            _logger.LogInformation("User {Email} successfully authenticated.", model.Email);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        _logger.LogWarning("Failed login attempt for email {Email}.", model.Email);
        ModelState.AddModelError(string.Empty, "Invalid email or password.");
        return View(model);
    }

    /// <summary>
    /// Termina a sessão do utilizador autenticado.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User signed out.");
        return RedirectToAction("Index", "Home");
    }
}