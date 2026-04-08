using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VgcCollege.Web.Models;

/// <summary>
/// ViewModel para a atribuição de um LecturerProfile a um Course.
/// Consumed by: LecturerController (Assign action), Views/Lecturer/Assign.cshtml.
/// </summary>
public class AssignLecturerViewModel
{
    /// <summary>Identificador do perfil do lecturer a atribuir.</summary>
    public int LecturerProfileId { get; set; }

    /// <summary>Nome do lecturer para apresentação na view.</summary>
    public string LecturerName { get; set; } = string.Empty;

    /// <summary>Identificador do curso seleccionado.</summary>
    [Required(ErrorMessage = "Please select a course.")]
    [Display(Name = "Course")]
    public int CourseId { get; set; }

    /// <summary>
    /// Indica se o lecturer é tutor deste curso.
    /// Tutores têm acesso aos dados de contacto dos alunos.
    /// </summary>
    [Display(Name = "Is tutor")]
    public bool IsTutor { get; set; }

    /// <summary>
    /// Lista de cursos disponíveis para o dropdown de selecção.
    /// Populada pelo controller antes de apresentar o formulário.
    /// </summary>
    public List<SelectListItem> AvailableCourses { get; set; } = new();
}