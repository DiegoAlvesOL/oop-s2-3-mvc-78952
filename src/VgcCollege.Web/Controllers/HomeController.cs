using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers;

/// <summary>
/// Purpose: Controller da página inicial da aplicação.
/// Consumed by: Views/Home/Index.cshtml, Views/Home/Privacy.cshtml.
/// Layer: Web Controllers
/// </summary>
[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// Inicializa o controller com o serviço de logging.
    /// </summary>
    /// <param name="logger">Serviço de logging.</param>
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
    
    public IActionResult Index()
    {
        return View();
    }

    
    public IActionResult Privacy()
    {
        return View();
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}