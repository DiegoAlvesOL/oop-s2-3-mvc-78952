using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models;

/// <summary>
/// ViewModel para criação e edição de LecturerProfile.
/// Consumed pelo LecturerController, Views/Lecturer/.
/// </summary>
public class LecturerViewModel
{
    /// <summary>Identificador do perfil (usado apenas na edição).</summary>
    public int Id { get; set; }

    /// <summary>Primeiro nome do lecturer.</summary>
    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Apelido do lecturer.</summary>
    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>Email de contacto do lecturer.</summary>
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [MaxLength(200, ErrorMessage = "Email cannot exceed 200 characters.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Número de telefone do lecturer (opcional).</summary>
    [Phone(ErrorMessage = "Invalid phone number format.")]
    [MaxLength(20, ErrorMessage = "Phone cannot exceed 20 characters.")]
    public string? Phone { get; set; }
}