namespace VgcCollege.Domain.Entities;

/// <summary>
/// Purpose: Representa o perfil de um lecturer da faculdade, ligado ao utilizador de Identity.
/// Consumed by: VgcCollege.Application (services), VgcCollege.Data (EF Core configurations).
/// Layer: Domain, Entities
/// </summary>
public class LecturerProfile
{
    public int Id { get; set; }
    public string IdentityUserId { get; set; } = string.Empty;
    public string FirstName { get; set; } =  string.Empty;
    public string LastName { get; set; } =  string.Empty;
    public string Email { get; set; } =   string.Empty;
    public string Phone { get; set; } =   string.Empty;

    public ICollection<LecturerCourseAssignment> CourseAssignments { get; set; } = new List<LecturerCourseAssignment>();
}