using VgcCollege.Application.Interfaces;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Application.Services;

/// <summary>
///Purpose: Contém a lógica de negócio para gestão de perfis de alunos.
/// Implementa filtragem por userId, student só acede ao próprio perfil.
/// Consumed by: StudentController (VgcCollege.Web).
/// Layer: Application Services
/// </summary>
public class StudentService
{
    private readonly IStudentRepository _studentRepository;

    /// <summary>
    /// Inicializa o service com o repositório de alunos.
    /// </summary>
    /// <param name="studentRepository">Repositório de alunos injectado via DI.</param>
    public StudentService(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }


    public async Task<IEnumerable<StudentProfile>> GetAllAsync()
    {
        return await _studentRepository.GetAllAsync();
    }

    /// <summary>
    /// Retorna um perfil de aluno pelo identificador.
    /// Um Student só pode aceder ao seu próprio perfil.
    /// Um Admin pode aceder a qualquer perfil.
    /// </summary>
    /// <param name="id">Identificador do perfil a aceder.</param>
    /// <param name="requestingUserId">IdentityUserId do utilizador que faz o pedido.</param>
    /// <param name="isAdmin">Indica se o utilizador que faz o pedido é Admin.</param>
    /// <returns>O perfil encontrado.</returns>
    /// <exception cref="UnauthorizedAccessException">
    /// Lançada quando um Student tenta aceder ao perfil de outro Student.
    /// </exception>
    public async Task<StudentProfile?> GetByIdAsync(int id, string requestingUserId, bool isAdmin)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student == null)
        {
            return null;
        }

        if (!isAdmin && student.IdentityUserId != requestingUserId)
        {
            throw new UnauthorizedAccessException("Access denied. You can only view your own profile.");
        }

        return student;
    }

    /// <summary>
    /// Retorna o perfil do aluno autenticado a partir do seu IdentityUserId.
    /// </summary>
    /// <param name="identityUserId">Identificador do utilizador no ASP.NET Core Identity.</param>
    public async Task<StudentProfile?> GetByIdentityUserIdAsync(string identityUserId)
    {
        return await _studentRepository.GetByIdentityUserIdAsync(identityUserId);
    }

    /// <summary>
    /// Cria um novo perfil de aluno após validar os campos obrigatórios.
    /// </summary>
    /// <param name="student">Entidade StudentProfile a ser criada.</param>
    /// <exception cref="ArgumentException">Lançada quando o nome ou email estão em branco.</exception>
    public async Task CreateAsync(StudentProfile student)
    {
        if (string.IsNullOrWhiteSpace(student.FirstName))
        {
            throw new ArgumentException("First name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(student.LastName))
        {
            throw new ArgumentException("Last name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(student.Email))
        {
            throw new ArgumentException("Email cannot be empty.");
        }

        await _studentRepository.AddAsync(student);
    }

    /// <summary>
    /// Actualiza um perfil de aluno após validar os campos obrigatórios.
    /// Um Student só pode actualizar o seu próprio perfil.
    /// </summary>
    /// <param name="student">Entidade StudentProfile com os dados actualizados.</param>
    /// <param name="requestingUserId">IdentityUserId do utilizador que faz o pedido.</param>
    /// <param name="isAdmin">Indica se o utilizador que faz o pedido é Admin.</param>
    /// <exception cref="UnauthorizedAccessException">
    /// Lançada quando um Student tenta actualizar o perfil de outro Student.
    /// </exception>
    public async Task UpdateAsync(StudentProfile student, string requestingUserId, bool isAdmin)
    {
        var existing = await _studentRepository.GetByIdAsync(student.Id);

        if (existing == null)
        {
            return;
        }

        if (!isAdmin && existing.IdentityUserId != requestingUserId)
        {
            throw new UnauthorizedAccessException("Access denied. You can only edit your own profile.");
        }

        if (string.IsNullOrWhiteSpace(student.FirstName))
        {
            throw new ArgumentException("First name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(student.LastName))
        {
            throw new ArgumentException("Last name cannot be empty.");
        }

        await _studentRepository.UpdateAsync(student);
    }

    /// <summary>Remove um perfil de aluno. Acesso exclusivo ao Admin.</summary>
    /// <param name="id">Identificador do perfil a remover.</param>
    public async Task DeleteAsync(int id)
    {
        await _studentRepository.DeleteAsync(id);
    }
}