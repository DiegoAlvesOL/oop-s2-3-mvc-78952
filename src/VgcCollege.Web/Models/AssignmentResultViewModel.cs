using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models;

/// <summary>
/// ViewModel para lançamento de resultado de um Assignment.
/// </summary>
public class AssignmentResultViewModel
{
    public int AssignmentId { get; set; }
    public int StudentProfileId { get; set; }
    public string AssignmentTitle { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public int MaxScore { get; set; }
    [Required(ErrorMessage = "Score is required.")]
    [Range(0, 1000, ErrorMessage = "Score must be between 0 and the maximum score.")]
    public int Score { get; set; }
    [MaxLength(1000, ErrorMessage = "Feedback cannot exceed 1000 characters.")]
    public string? Feedback { get; set; }
}