namespace VgcCollege.Domain.Entities;

/// <summary>
/// Purpose: Representa o perfil académico de um aluno, ligado ao utilizador de Identity.
/// Consumed by: VgcCollege.Application (services), VgcCollege.Data (EF Core configurations).
/// Layer: Domain, Entities
/// </summary>
public class StudentProfile
{
    public int Id { get; set; }
    public string IdentityUserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } =  string.Empty;
    public string? Phone { get; set; } =  string.Empty;
    public string? StreetName { get; set; } =  string.Empty;
    public string? City { get; set; } =  string.Empty;
    public string StudentNumber { get; set; } = string.Empty;

    /// <summary>Matrículas deste aluno em cursos.</summary>
    public ICollection<CourseEnrolment> Enrolments { get; set; } = new List<CourseEnrolment>();

    /// <summary>Resultados de assignments deste aluno.</summary>
    public ICollection<AssignmentResult> AssignmentResults { get; set; } = new List<AssignmentResult>();

    /// <summary>Resultados de exames deste aluno.</summary>
    public ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
}