using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models;

/// <summary>
/// Purpose: ViewModel para o formulário de login. Contém os campos e validações necessários.
/// Consumed by: AccountController (Login action), Views/Account/Login.cshtml.
/// Layer: Web Models
/// </summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } =  string.Empty;
    
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    
    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; } = false;
}