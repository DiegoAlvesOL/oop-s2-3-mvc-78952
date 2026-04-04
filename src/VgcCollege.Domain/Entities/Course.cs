namespace VgcCollege.Domain.Entities;

/// <summary>
/// Purpose: Representa um curso académico oferecido por uma Branch.
///Consumed by: VgcCollege.Application (services), VgcCollege.Data (EF Core configurations).
/// Layer: Domain, Entities
/// </summary>
public class Course
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public string CourseName { get; set; } =  string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public Branch Branch { get; set; } = null!;
    
    /// <summary>Matrículas de alunos neste curso.</summary>
    public ICollection<CourseEnrolment> Enrolments { get; set; } = new List<CourseEnrolment>();

    /// <summary>Lecturers atribuídos a este curso.</summary>
    public ICollection<LecturerCourseAssignment> LecturerCourseAssignments { get; set; } =
        new List<LecturerCourseAssignment>();

    /// <summary>Assignments (trabalhos avaliativos) deste curso.</summary>
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    /// <summary>Exames deste curso.</summary>
    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
    
}