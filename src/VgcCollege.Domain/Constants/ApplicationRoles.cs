namespace VgcCollege.Domain.Constants;

/// <summary>
/// Define os nomes dos roles de autorização usados em toda a aplicação.
/// Consumido por: Web (Authorize), Application (services), Data (seed).
/// </summary>
public class ApplicationRoles
{
    public const string Admin = "Admin";
    public const string Lecturer = "Lecturer";
    public const string Student = "Student";
}