using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VgcCollege.Web.Models;

/// <summary>
/// Purpose: ViewModel para os formulários de criação e edição de Course.
/// Contém as validações da camada de apresentação.
/// Consumed by: CourseController, Views/Course/.
/// Layer: Web Models
/// </summary>
public class CourseViewModel
{
    /// <summary>Identificador do curso (usado apenas na edição).</summary>
    public int Id { get; set; }

    /// <summary>Nome do curso.</summary>
    [Required(ErrorMessage = "Course name is required.")]
    [MaxLength(150, ErrorMessage = "Course name cannot exceed 150 characters.")]
    [Display(Name = "Course name")]
    public string CourseName { get; set; } = string.Empty;

    /// <summary>Identificador da branch à qual o curso pertence.</summary>
    [Required(ErrorMessage = "Branch is required.")]
    [Display(Name = "Branch")]
    public int BranchId { get; set; }

    /// <summary>Data de início do curso.</summary>
    [Required(ErrorMessage = "Start date is required.")]
    [Display(Name = "Start date")]
    public DateOnly StartDate { get; set; }

    /// <summary>Data de fim do curso.</summary>
    [Required(ErrorMessage = "End date is required.")]
    [Display(Name = "End date")]
    public DateOnly EndDate { get; set; }

    /// <summary>
    /// Lista de branches disponíveis para o dropdown de selecção.
    /// Populada pelo controller antes de apresentar o formulário.
    /// </summary>
    public List<SelectListItem> AvailableBranches { get; set; } = new();
}