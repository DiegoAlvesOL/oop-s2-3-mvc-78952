namespace VgcCollege.Domain.Entities;

/// <summary>
/// Purpose: Representa a atribuição de um Lecturer a um Course, com indicação se é tutor.
/// Consumed by: VgcCollege.Application (services), VgcCollege.Data (EF Core configurations).
/// Layer: Domain, Entities
/// </summary>
public class LecturerCourseAssignment
{
    public int Id { get; set; }
    public int LecturerProfileId { get; set; }
    public int CourseId { get; set; }

    /// <summary>
    /// Indica se o lecturer é tutor deste curso.
    /// Apenas tutores têm acesso aos dados de contacto (email e telefone) dos alunos.
    /// </summary>
    public bool IsTutor { get; set; }
    
    public LecturerProfile Lecturer { get; set; } = null!;

    public Course Course { get; set; } = null!;
}