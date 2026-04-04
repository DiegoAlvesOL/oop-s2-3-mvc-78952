namespace VgcCollege.Domain.Entities;


/// <summary>
/// Purpose: Representa o resultado de um aluno num Assignment específico.
/// Consumed by: VgcCollege.Application (services), VgcCollege.Data (EF Core configurations).
/// Layer: Domain, Entities 
/// </summary>
public class AssignmentResult
{
    public int Id { get; set; }
    public int AssignmentId { get; set; }
    public int StudentProfileId { get; set; }
    public int Score { get; set; }
    public string? Feedback {  get; set; }
    public Assignment Assignment { get; set; } = null!;
    public StudentProfile Student { get; set; } = null!;
}