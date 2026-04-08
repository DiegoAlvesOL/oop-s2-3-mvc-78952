using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models;

/// <summary>
///Purpose: ViewModel para os formulários de criação e edição de StudentProfile.
/// Contém as validações da camada de apresentação.
/// Consumed by: StudentController, Views/Student/.
/// Layer: WebModels
/// </summary>
public class StudentViewModel
{
    /// <summary>Identificador do perfil (usado apenas na edição).</summary>
    public int Id { get; set; }

    /// <summary>Primeiro nome do aluno.</summary>
    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Apelido do aluno.</summary>
    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>Email de contacto do aluno.</summary>
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [MaxLength(200, ErrorMessage = "Email cannot exceed 200 characters.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Número de telefone do aluno (opcional).</summary>
    [Phone(ErrorMessage = "Invalid phone number format.")]
    [MaxLength(20, ErrorMessage = "Phone cannot exceed 20 characters.")]
    public string? Phone { get; set; }

    /// <summary>Nome da rua (opcional).</summary>
    [MaxLength(200, ErrorMessage = "Street name cannot exceed 200 characters.")]
    [Display(Name = "Street name")]
    public string? StreetName { get; set; }

    /// <summary>Cidade (opcional).</summary>
    [MaxLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
    public string? City { get; set; }

    /// <summary>Número de aluno único na faculdade.</summary>
    [Required(ErrorMessage = "Student number is required.")]
    [MaxLength(20, ErrorMessage = "Student number cannot exceed 20 characters.")]
    [Display(Name = "Student number")]
    public string StudentNumber { get; set; } = string.Empty;
}