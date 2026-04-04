using VgcCollege.Domain.Enums;

namespace VgcCollege.Domain.Entities;


/// <summary>
/// Purpose: Representa a matrícula de um aluno num curso, com histórico de presenças.
/// Consumed by: VgcCollege.Application (services), VgcCollege.Data (EF Core configurations).
/// Layer: Domain, Entities 
/// </summary>
public class CourseEnrolment
{
    public int Id { get; set; }
    public int StudentProfileId { get; set; }
    public int CourseId { get; set; }
    public DateOnly EnrolDate { get; set; }
    public EnrolmentStatus Status { get; set; } = EnrolmentStatus.Active;
    public StudentProfile Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();

}