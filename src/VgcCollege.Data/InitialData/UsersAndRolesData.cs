using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Data.Models;
using VgcCollege.Domain.Constants;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Data.InitialData;

/// <summary>
/// Purpose   : Cria os roles e utilizadores iniciais da aplicação no arranque. Idempotente verifica se os dados já existem antes de inserir.
/// Consumed by: DatabaseInitialiser.cs
/// Layer: Data InitialData
/// </summary>
public static class UsersAndRolesData
{
    /// <summary>
    /// Cria os três roles, os utilizadores de demonstração e os respectivos perfis.
    /// Não insere dados se estes já existirem no banco.
    /// </summary>
    /// <param name="context">Contexto do banco de dados.</param>
    /// <param name="userManager">Serviço do Identity para gestão de utilizadores.</param>
    /// <param name="roleManager">Serviço do Identity para gestão de roles.</param>
    public static async Task SeedAsync(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        await CreateRoleIfNotExistsAsync(roleManager, ApplicationRoles.Admin);
        await CreateRoleIfNotExistsAsync(roleManager, ApplicationRoles.Lecturer);
        await CreateRoleIfNotExistsAsync(roleManager, ApplicationRoles.Student);

        await CreateUserIfNotExistsAsync(userManager, "admin@vgc.ie", "Admin123!", ApplicationRoles.Admin);

        var lecturer = await CreateUserIfNotExistsAsync(userManager, "lecturer@vgc.ie", "Lecturer123!", ApplicationRoles.Lecturer);
        var student1 = await CreateUserIfNotExistsAsync(userManager, "student1@vgc.ie", "Student123!", ApplicationRoles.Student);
        var student2 = await CreateUserIfNotExistsAsync(userManager, "student2@vgc.ie", "Student123!", ApplicationRoles.Student);

        if (student1 != null && !await context.StudentProfiles.AnyAsync(s => s.IdentityUserId == student1.Id))
        {
            await context.StudentProfiles.AddAsync(new StudentProfile
            {
                IdentityUserId = student1.Id,
                FirstName = "Alice",
                LastName = "Murphy",
                Email = "student1@vgc.ie",
                StudentNumber = "VGC001",
                City = "Dublin"
            });
        }

        if (student2 != null && !await context.StudentProfiles.AnyAsync(s => s.IdentityUserId == student2.Id))
        {
            await context.StudentProfiles.AddAsync(new StudentProfile
            {
                IdentityUserId = student2.Id,
                FirstName = "Brian",
                LastName = "Kelly",
                Email = "student2@vgc.ie",
                StudentNumber = "VGC002",
                City = "Cork"
            });
        }

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Cria um role se ainda não existir no banco.
    /// </summary>
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
    /// Retorna o utilizador criado ou null se já existia.
    /// </summary>
    private static async Task<ApplicationUser?> CreateUserIfNotExistsAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string role)
    {
        var existingUser = await userManager.FindByEmailAsync(email);

        if (existingUser != null)
        {
            return null;
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
            return user;
        }

        return null;
    }
}