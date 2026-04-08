
using Microsoft.EntityFrameworkCore;
using VgcCollege.Application.Interfaces;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Data.Repositories;

/// <summary>
/// Repositório de LecturerProfile. Implementa ILecturerRepository usando o AppDbContext.
/// Consumed pelo LecturerService (via ILecturerRepository), Program.cs (registo no DI).
/// </summary>
public class LecturerRepository : ILecturerRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa o repositório com o contexto do banco de dados.
    /// </summary>
    /// <param name="context">Contexto do banco de dados injectado via DI.</param>
    public LecturerRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<LecturerProfile>> GetAllAsync()
    {
        return await _context.LecturerProfiles
            .OrderBy(lecturer => lecturer.LastName)
            .ThenBy(lecturer => lecturer.FirstName)
            .ToListAsync();
    }

    /// <summary>Retorna um perfil de lecturer pelo seu identificador único.</summary>
    /// <param name="id">Identificador do perfil.</param>
    public async Task<LecturerProfile?> GetByIdAsync(int id)
    {
        return await _context.LecturerProfiles.FindAsync(id);
    }

    /// <summary>
    /// Retorna o perfil de lecturer associado a um utilizador de Identity.
    /// Usado para encontrar o perfil do lecturer autenticado a partir do seu UserId.
    /// </summary>
    /// <param name="identityUserId">Identificador do utilizador no ASP.NET Core Identity.</param>
    public async Task<LecturerProfile?> GetByIdentityUserIdAsync(string identityUserId)
    {
        return await _context.LecturerProfiles
            .FirstOrDefaultAsync(lecturer => lecturer.IdentityUserId == identityUserId);
    }

    /// <summary>
    /// Retorna todos os cursos atribuídos a um lecturer, incluindo a flag IsTutor
    /// e os dados do curso associado.
    /// </summary>
    /// <param name="lecturerProfileId">Identificador do perfil do lecturer.</param>
    public async Task<IEnumerable<LecturerCourseAssignment>> GetCourseAssignmentsAsync(int lecturerProfileId)
    {
        return await _context.LecturerCourseAssignments
            .Include(assignment => assignment.Course)
            .Where(assignment => assignment.LecturerProfileId == lecturerProfileId)
            .ToListAsync();
    }

    /// <summary>Adiciona um novo perfil de lecturer ao banco de dados.</summary>
    /// <param name="lecturer">Entidade LecturerProfile a ser adicionada.</param>
    public async Task AddAsync(LecturerProfile lecturer)
    {
        await _context.LecturerProfiles.AddAsync(lecturer);
        await _context.SaveChangesAsync();
    }

    /// <summary>Actualiza os dados de um perfil de lecturer existente.</summary>
    /// <param name="lecturer">Entidade LecturerProfile com os dados actualizados.</param>
    public async Task UpdateAsync(LecturerProfile lecturer)
    {
        _context.LecturerProfiles.Update(lecturer);
        await _context.SaveChangesAsync();
    }

    /// <summary>Remove um perfil de lecturer pelo seu identificador único.</summary>
    /// <param name="id">Identificador do perfil a ser removido.</param>
    public async Task DeleteAsync(int id)
    {
        var lecturer = await _context.LecturerProfiles.FindAsync(id);

        if (lecturer != null)
        {
            _context.LecturerProfiles.Remove(lecturer);
            await _context.SaveChangesAsync();
        }
    }
}