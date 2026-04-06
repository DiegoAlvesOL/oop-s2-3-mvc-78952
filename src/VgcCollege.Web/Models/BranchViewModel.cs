

using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models;

/// <summary>
/// Purpose: ViewModel para os formulários de criação e edição de Branch.
/// Contém as validações da camada de apresentação.
/// Consumed by: BranchController, Views/Branch/.
/// Layer: Web Models
/// </summary>
public class BranchViewModel
{
    /// <summary>Identificador da branch (usado apenas na edição).</summary>
    public int Id { get; set; }

    
    [Required(ErrorMessage = "Branch name is required.")]
    [MaxLength(100, ErrorMessage = "Branch name cannot exceed 100 characters.")]
    [Display(Name = "Branch name")]
    public string BranchName { get; set; } = string.Empty;

    
    [Required(ErrorMessage = "Street name is required.")]
    [MaxLength(200, ErrorMessage = "Street name cannot exceed 200 characters.")]
    [Display(Name = "Street name")]
    public string StreetName { get; set; } = string.Empty;

    
    [Required(ErrorMessage = "City is required.")]
    [MaxLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
    public string City { get; set; } = string.Empty;
}