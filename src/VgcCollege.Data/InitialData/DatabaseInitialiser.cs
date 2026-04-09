using Microsoft.AspNetCore.Identity;
using VgcCollege.Data.InitialData;
using VgcCollege.Data.Models;

namespace VgcCollege.Data;

/// <summary>
///Purpose: Orquestra a execução dos dados iniciais em sequência no arranque da aplicação.
/// Cada método de seed é chamado por ordem de dependência.
/// Consumed by: Program.cs (VgcCollege.Web)
/// Layer: Data Initialiser
/// </summary>
public static class DatabaseInitialiser
{
    /// <summary>
    /// Executa todos os dados iniciais em sequência.
    /// Seguro para executar múltiplas vezes — cada classe verifica se os dados já existem.
    /// </summary>
    /// <param name="context">Contexto do banco de dados.</param>
    /// <param name="userManager">Serviço do Identity para gestão de utilizadores.</param>
    /// <param name="roleManager">Serviço do Identity para gestão de roles.</param>
    public static async Task SeedAsync(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        await UsersAndRolesData.SeedAsync(context, userManager, roleManager);
        await BranchesAndCoursesData.SeedAsync(context);
        await EnrolmentsAndAttendanceData.SeedAsync(context);
        await GradebookAndExamsData.SeedAsync(context);
    }
}