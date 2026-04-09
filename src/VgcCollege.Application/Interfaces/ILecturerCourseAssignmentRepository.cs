using VgcCollege.Domain.Entities;

namespace VgcCollege.Application.Interfaces;

/// <summary>
/// Contrato de repositório para operações de LecturerCourseAssignment.
/// A implementação concreta reside na camada Data.
/// </summary>
public interface ILecturerCourseAssignmentRepository
{
    Task<IEnumerable<LecturerCourseAssignment>> GetByLecturerAsync(int lecturerProfileId);
    Task<IEnumerable<LecturerCourseAssignment>> GetByCourseAsync(int courseId);
    Task<bool> ExistsAsync(int lecturerProfileId, int courseId);
    Task AddAsync(LecturerCourseAssignment assignment);
    Task RemoveAsync(int lecturerProfileId, int courseId);
}