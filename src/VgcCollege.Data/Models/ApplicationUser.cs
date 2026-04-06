using Microsoft.AspNetCore.Identity;

namespace VgcCollege.Data.Models;

/// <summary>
/// Purpose: Representa o utilizador de autenticação do sistema, baseado no ASP.NET Core Identity.
/// Consumed by: AppDbContext, VgcCollege.Web (Program.cs, controllers de autenticação).
/// Layer: Data Models
/// </summary>
public class ApplicationUser : IdentityUser
{
}