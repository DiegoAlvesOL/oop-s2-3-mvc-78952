
namespace VgcCollege.Domain.Entities;


/// <summary>
/// Purpose : Representa o resultado de um aluno num Exam específico.
/// Consumed by: VgcCollege.Application (services), VgcCollege.Data (EF Core configurations).
/// Layer: Domain — Entities 
/// </summary>
public class ExamResult
{
    public int Id { get; set; }
    public int ExamId { get; set; }
    public int StudentProfileId { get; set; }
    public int Score { get; set; }
    public string? Grade { get; set; }
    public Exam Exam { get; set; } = null!;
    public StudentProfile Student { get; set; } = null!;
}