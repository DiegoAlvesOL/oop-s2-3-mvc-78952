using VgcCollege.Domain.Entities;

namespace VgcCollege.Web.Models;

/// <summary>
/// ViewModel que representa um curso com os seus lecturers e alunos
/// para apresentação na página de detalhes de uma Branch.
/// </summary>
public class BranchCourseDetailViewModel
{
    
    public Course Course { get; set; } = null!;
    public List<CourseEnrolment> Enrolments { get; set; } = new();
    public List<LecturerCourseAssignment> LecturerAssignments { get; set; } = new();
}