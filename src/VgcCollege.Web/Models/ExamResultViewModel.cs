using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models;

public class ExamResultViewModel
{

    public int ExamId { get; set; }
    public int StudentProfileId { get; set; }
    public string ExamTitle { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public int MaxScore { get; set; }
    [Required(ErrorMessage = "Score is required.")]
    [Range(0, 1000, ErrorMessage = "Score must be between 0 and the maximum score.")]
    public int Score { get; set; }
    [MaxLength(10, ErrorMessage = "Grade cannot exceed 10 characters.")]
    public string? Grade { get; set; }
}