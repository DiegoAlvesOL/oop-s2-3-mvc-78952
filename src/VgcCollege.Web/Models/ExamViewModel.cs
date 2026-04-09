using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models;

/// <summary>
/// ViewModel para criação de um novo Exam.
/// </summary>
public class ExamViewModel
{
    
    public int Id { get; set; }
    
    public int CourseId { get; set; }
    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;
    [Required(ErrorMessage = "Exam date is required.")]
    [Display(Name = "Exam date")]
    public DateOnly ExamDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(30));
    [Required(ErrorMessage = "Max score is required.")]
    [Range(1, 1000, ErrorMessage = "Max score must be between 1 and 1000.")]
    [Display(Name = "Max score")]
    public int MaxScore { get; set; }
    [Display(Name = "Results released")]
    public bool ResultsReleased { get; set; }
}