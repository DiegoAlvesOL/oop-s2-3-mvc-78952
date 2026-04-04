namespace VgcCollege.Domain.Entities;


/// <summary>
/// Purpose: Representa um exame de um Course, com controlo de visibilidade dos resultados.
/// Consumed by: VgcCollege.Application (services), VgcCollege.Data (EF Core configurations).
/// Layer: Domain, Entities 
/// </summary>
public class Exam
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateOnly ExamDate { get; set; }
    public int MaxScore { get; set; }
    
    /// <summary>
    /// Indica se os resultados foram publicados para os alunos.
    /// Enquanto false, nenhum Student pode visualizar os seus resultados.
    /// </summary>
    public bool ResultsReleased { get; set; }

    public Course Course { get; set; } = null!;
    public ICollection<ExamResult> Results { get; set; } = new List<ExamResult>();
}