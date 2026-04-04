namespace VgcCollege.Domain.Entities;

/// <summary>
/// Purpose: Representa um trabalho avaliativo de um Course.
/// Consumed by: VgcCollege.Application (services), VgcCollege.Data (EF Core configurations).
/// Layer: Domain, Entities 
/// </summary>
public class Assignment
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Title { get; set; } =  String.Empty;
    public int MaxScore {  get; set; }
    public DateOnly DueDate { get; set; }
    public Course Course { get; set; } = null!;
    public ICollection<AssignmentResult> Results { get; set; } = new List<AssignmentResult>();
}