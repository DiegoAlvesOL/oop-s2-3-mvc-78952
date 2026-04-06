using Microsoft.AspNetCore.Identity;
using VgcCollege.Data.Models;
using VgcCollege.Domain.Constants;

namespace VgcCollege.Data.InitialData;

/// <summary>
/// Purpose   : Cria os roles e utilizadores iniciais da aplicação no arranque. Idempotente verifica se os dados já existem antes de inserir.
/// Consumed by: DatabaseInitialiser.cs
/// Layer: Data InitialData
/// </summary>
public static class UsersAndRolesData
{
    /// <summary>
    /// Cria os três roles do sistema e os utilizadores de demonstração.
    /// Não insere dados se estes já existirem no banco.
    /// </summary>
    /// <param name="userManager">Serviço do Identity para gestão de utilizadores.</param>
    /// <param name="roleManager">Serviço do Identity para gestão de roles.</param>
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        await CreateRoleIfNotExistsAsync(roleManager, ApplicationRoles.Admin);
        await CreateRoleIfNotExistsAsync(roleManager, ApplicationRoles.Lecturer);
        await CreateRoleIfNotExistsAsync(roleManager, ApplicationRoles.Student);

        await CreateUserIfNotExistsAsync(
            userManager,
            email: "admin@vgc.ie",
            password: "Admin123!",
            role: ApplicationRoles.Admin);

        await CreateUserIfNotExistsAsync(
            userManager,
            email: "lecturer@vgc.ie",
            password: "Lecturer123!",
            role: ApplicationRoles.Lecturer);

        await CreateUserIfNotExistsAsync(
            userManager,
            email: "student1@vgc.ie",
            password: "Student123!",
            role: ApplicationRoles.Student);

        await CreateUserIfNotExistsAsync(
            userManager,
            email: "student2@vgc.ie",
            password: "Student123!",
            role: ApplicationRoles.Student);
    }

    /// <summary>
    /// Cria um role se ainda não existir no banco.
    /// </summary>
    /// <param name="roleManager">Serviço do Identity para gestão de roles.</param>
    /// <param name="roleName">Nome do role a criar.</param>
    private static async Task CreateRoleIfNotExistsAsync(
        RoleManager<IdentityRole> roleManager,
        string roleName)
    {
        var roleExists = await roleManager.RoleExistsAsync(roleName);

        if (!roleExists)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    /// <summary>
    /// Cria um utilizador com o role especificado se ainda não existir no banco.
    /// </summary>
    /// <param name="userManager">Serviço do Identity para gestão de utilizadores.</param>
    /// <param name="email">Email do utilizador, usado também como username.</param>
    /// <param name="password">Password inicial do utilizador.</param>
    /// <param name="role">Role a atribuir ao utilizador.</param>
    private static async Task CreateUserIfNotExistsAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string role)
    {
        var existingUser = await userManager.FindByEmailAsync(email);

        if (existingUser != null)
        {
            return;
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }
}