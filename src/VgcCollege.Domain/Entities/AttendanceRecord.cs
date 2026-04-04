namespace VgcCollege.Domain.Entities;



/// <summary>
///Purpose: Representa o registo de presença de um aluno numa sessão do curso.
/// Consumed by: VgcCollege.Application (services), VgcCollege.Data (EF Core configurations).
/// Layer: Domain, Entities 
/// </summary>
public class AttendanceRecord
{
    public int Id { get; set; }
    public int CourseEnrolmentId { get; set; }
    public DateOnly SessionDate { get; set; }
    public bool Present { get; set; }
    public CourseEnrolment Enrolment { get; set; } = null!;

}