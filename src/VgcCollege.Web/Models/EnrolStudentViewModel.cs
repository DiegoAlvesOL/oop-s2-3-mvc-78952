// Purpose   : ViewModel para o formulário de matrícula de um aluno num curso.
//             Permite ao Admin seleccionar o aluno e o curso.
// Consumed by: EnrolmentController, Views/Enrolment/Create.cshtml.
// Layer     : Web — Models

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VgcCollege.Web.Models;

/// <summary>
/// ViewModel para criação de uma nova matrícula.
/// </summary>
public class EnrolStudentViewModel
{
    [Required(ErrorMessage = "Please select a student.")]
    [Display(Name = "Student")]
    public int StudentProfileId { get; set; }
    [Required(ErrorMessage = "Please select a course.")]
    [Display(Name = "Course")]
    public int CourseId { get; set; }
    public List<SelectListItem> AvailableStudents { get; set; } = new();
    public List<SelectListItem> AvailableCourses { get; set; } = new();
}