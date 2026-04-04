using System.Collections;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Application.Interfaces;




/// <summary>
/// Purpose: Define o contrato de acesso a dados para a entidade StudentProfile.
/// Consumed by: VgcCollege.Application (services), implementado por VgcCollege.Data.
/// Layer: Application Interfaces
/// </summary>
public interface IStudentRepository
{
    Task<IEnumerable<StudentProfile>> GeteAllAsync();
    
    Task<StudentProfile> GetByIdAsync(int id);
    Task<StudentProfile> GetByIdentityUserIdAsync(string identityUserId);
    Task AddAsync (StudentProfile student);
    Task UpdateAsync(StudentProfile student);
    Task DeleteAsync(int id);
}