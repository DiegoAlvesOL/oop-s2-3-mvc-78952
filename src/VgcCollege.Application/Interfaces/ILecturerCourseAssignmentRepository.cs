using VgcCollege.Domain.Entities;

namespace VgcCollege.Application.Interfaces;

/// <summary>
/// Contrato de repositório para operações de LecturerCourseAssignment.
/// A implementação concreta reside na camada Data.
/// </summary>
public interface ILecturerCourseAssignmentRepository
{
    /// <summary>Retorna todos os cursos atribuídos a um lecturer específico.</summary>
    /// <param name="lecturerProfileId">Identificador do perfil do lecturer.</param>
    Task<IEnumerable<LecturerCourseAssignment>> GetByLecturerAsync(int lecturerProfileId);
    Task<bool> ExistsAsync(int lecturerProfileId, int courseId);
    Task AddAsync(LecturerCourseAssignment assignment);
    Task RemoveAsync(int lecturerProfileId, int courseId);
}