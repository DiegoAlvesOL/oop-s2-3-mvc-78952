using VgcCollege.Domain.Entities;

namespace VgcCollege.Application.Interfaces;


/// <summary>
/// Purpose: Define o contrato de acesso a dados para a entidade LecturerProfile.
/// Consumed by: VgcCollege.Application (services), implementado por VgcCollege.Data.
/// Layer: Application Interfaces 
/// </summary>
public interface ILecturerRepository
{
    Task<IEnumerable<LecturerProfile>> GetAllAsync();
    Task<LecturerProfile> GetByIdAsync(int id);
    Task<LecturerProfile> GetByIdentityUserIdAsync (string identityUserId);
    Task<IEnumerable<LecturerCourseAssignment>> GetCourseAssignmentsAsync(int lecturerProfileId);
    Task AddAsync(LecturerProfile lecturer);
    Task UpdateAsync(LecturerProfile lecturer);
    Task DeleteAsync(int id);

}