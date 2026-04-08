
using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models;

/// <summary>
/// ViewModel para registo de presença de um aluno numa sessão.
/// </summary>
public class AttendanceViewModel
{
    public int EnrolmentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Session date is required.")]
    [Display(Name = "Session date")]
    public DateOnly SessionDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    [Display(Name = "Present")]
    public bool Present { get; set; }
}