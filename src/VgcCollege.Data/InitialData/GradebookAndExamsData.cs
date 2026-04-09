using Microsoft.EntityFrameworkCore;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Data.InitialData;

/// <summary>
/// Responsável por criar os dados iniciais de gradebook e exames no arranque da aplicação.
/// Depende de EnrolmentsAndAttendanceData ter sido executado primeiro.
/// </summary>
public static class GradebookAndExamsData
{
    /// <summary>
    /// Cria assignments com resultados e exames com e sem ResultsReleased.
    /// Não insere dados se já existirem assignments no banco.
    /// </summary>
    /// <param name="context">Contexto do banco de dados.</param>
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Assignments.AnyAsync())
        {
            return;
        }

        // Obtém os dados criados pelos seeds anteriores.
        var softwareDevelopmentDublin = await context.Courses
            .FirstOrDefaultAsync(course =>
                course.CourseName == "Software Development" &&
                course.Branch.City == "Dublin");

        var student1 = await context.StudentProfiles
            .FirstOrDefaultAsync(student => student.StudentNumber == "VGC001");

        var student2 = await context.StudentProfiles
            .FirstOrDefaultAsync(student => student.StudentNumber == "VGC002");

        if (softwareDevelopmentDublin == null || student1 == null || student2 == null)
        {
            return;
        }

        // Criar um assignment de demonstração.
        var assignment = new Assignment
        {
            CourseId = softwareDevelopmentDublin.Id,
            Title = "Project CA1 Web Application",
            MaxScore = 100,
            DueDate = new DateOnly(2025, 11, 14)
        };

        await context.Assignments.AddAsync(assignment);
        await context.SaveChangesAsync();

        // Lançar resultados para os dois alunos.
        await context.AssignmentResults.AddRangeAsync(
            new AssignmentResult
            {
                AssignmentId = assignment.Id,
                StudentProfileId = student1.Id,
                Score = 88,
                Feedback = "Excellent work. Well structured and clearly documented."
            },
            new AssignmentResult
            {
                AssignmentId = assignment.Id,
                StudentProfileId = student2.Id,
                Score = 74,
                Feedback = "Good effort. Consider improving error handling."
            }
        );
        await context.SaveChangesAsync();

        // Exame intermédio ResultsReleased = true (alunos podem ver).
        var midtermExam = new Exam
        {
            CourseId = softwareDevelopmentDublin.Id,
            Title = "Midterm Exam Semester 2",
            ExamDate = new DateOnly(2025, 11, 20),
            MaxScore = 100,
            ResultsReleased = true
        };

        // Exame final ResultsReleased = false (resultados provisórios, bloqueados).
        var finalExam = new Exam
        {
            CourseId = softwareDevelopmentDublin.Id,
            Title = "Final Exam  Semester 2",
            ExamDate = new DateOnly(2025, 12, 15),
            MaxScore = 100,
            ResultsReleased = false
        };

        await context.Exams.AddRangeAsync(midtermExam, finalExam);
        await context.SaveChangesAsync();

        // Resultados do exame intermédio (visíveis para os alunos).
        await context.ExamResults.AddRangeAsync(
            new ExamResult
            {
                ExamId = midtermExam.Id,
                StudentProfileId = student1.Id,
                Score = 82,
                Grade = "A"
            },
            new ExamResult
            {
                ExamId = midtermExam.Id,
                StudentProfileId = student2.Id,
                Score = 68,
                Grade = "B"
            }
        );

        // Resultados do exame final (provisórios — bloqueados até libertação).
        await context.ExamResults.AddRangeAsync(
            new ExamResult
            {
                ExamId = finalExam.Id,
                StudentProfileId = student1.Id,
                Score = 91,
                Grade = "A"
            },
            new ExamResult
            {
                ExamId = finalExam.Id,
                StudentProfileId = student2.Id,
                Score = 77,
                Grade = "B"
            }
        );

        await context.SaveChangesAsync();
    }
}