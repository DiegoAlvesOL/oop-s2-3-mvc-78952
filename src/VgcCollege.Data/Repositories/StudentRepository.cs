
using Microsoft.EntityFrameworkCore;
using VgcCollege.Application.Interfaces;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Data.Repositories;

/// <summary>
///Purpose: Implementação concreta do IStudentRepository usando EF Core e MySQL.
/// Consumed by: StudentService (via IStudentRepository), Program.cs (registo no DI).
/// Layer: Data Repositories
/// </summary>
public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa o repositório com o contexto do banco de dados.
    /// </summary>
    /// <param name="context">Contexto do banco de dados injectado via DI.</param>
    public StudentRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>Retorna todos os perfis de alunos do sistema.</summary>
    public async Task<IEnumerable<StudentProfile>> GetAllAsync()
    {
        return await _context.StudentProfiles
            .OrderBy(student => student.LastName)
            .ThenBy(student => student.FirstName)
            .ToListAsync();
    }

    /// <summary>Retorna um perfil de aluno pelo seu identificador único.</summary>
    /// <param name="id">Identificador do perfil.</param>
    public async Task<StudentProfile?> GetByIdAsync(int id)
    {
        return await _context.StudentProfiles.FindAsync(id);
    }

    /// <summary>
    /// Retorna o perfil de aluno associado a um utilizador de Identity.
    /// Usado para encontrar o perfil do aluno autenticado a partir do seu UserId.
    /// </summary>
    /// <param name="identityUserId">Identificador do utilizador no ASP.NET Core Identity.</param>
    public async Task<StudentProfile?> GetByIdentityUserIdAsync(string identityUserId)
    {
        return await _context.StudentProfiles
            .FirstOrDefaultAsync(student => student.IdentityUserId == identityUserId);
    }

    /// <summary>Adiciona um novo perfil de aluno ao banco de dados.</summary>
    /// <param name="student">Entidade StudentProfile a ser adicionada.</param>
    public async Task AddAsync(StudentProfile student)
    {
        await _context.StudentProfiles.AddAsync(student);
        await _context.SaveChangesAsync();
    }

    /// <summary>Actualiza os dados de um perfil de aluno existente.</summary>
    /// <param name="student">Entidade StudentProfile com os dados actualizados.</param>
    public async Task UpdateAsync(StudentProfile student)
    {
        var tracked = await _context.StudentProfiles.FindAsync(student.Id);

        if (tracked != null)
        {
            _context.Entry(tracked).CurrentValues.SetValues(student);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>Remove um perfil de aluno pelo seu identificador único.</summary>
    /// <param name="id">Identificador do perfil a ser removido.</param>
    public async Task DeleteAsync(int id)
    {
        var student = await _context.StudentProfiles.FindAsync(id);

        if (student != null)
        {
            _context.StudentProfiles.Remove(student);
            await _context.SaveChangesAsync();
        }
    }
}