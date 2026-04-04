namespace VgcCollege.Domain.Entities;
/// <summary>
/// Purpose : Representa uma unidade física (campus) da faculdade.
/// Consumed by: VgcCollege.Application (services), VgcCollege.Data (EF Core configurations).
/// Layer: Domain, Entities
/// </summary>
public class Branch
{
    public int Id { get; set; }
    public string BranchName  { get; set; } = string.Empty;
    public string StreetName { get; set; } = string.Empty;
    public string City { get; set; }  = string.Empty;

    public ICollection<Course> Courses { get; set; } = new List<Course>();

}