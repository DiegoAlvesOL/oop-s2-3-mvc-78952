using Microsoft.EntityFrameworkCore;
using VgcCollege.Domain.Entities;
using VgcCollege.Domain.Enums;

namespace VgcCollege.Data.InitialData;

/// <summary>
/// Responsável por criar as matrículas e presenças de demonstração no arranque da aplicação.
/// Depende de BranchesAndCoursesData e UsersAndRolesData terem sido executados primeiro.
/// </summary>
public static class EnrolmentsAndAttendanceData
{
    /// <summary>
    /// Cria matrículas para os alunos de demonstração e registos de presença das últimas 4 semanas.
    /// Não insere dados se já existirem matrículas no banco.
    /// </summary>
    /// <param name="context">Contexto do banco de dados.</param>
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.CourseEnrolments.AnyAsync())
        {
            return;
        }

        // Obtém os alunos e cursos criados pelos dados iniciais anteriores.
        var student1 = await context.StudentProfiles
            .FirstOrDefaultAsync(student => student.StudentNumber == "VGC001");

        var student2 = await context.StudentProfiles
            .FirstOrDefaultAsync(student => student.StudentNumber == "VGC002");

        var softwareDevelopmentDublin = await context.Courses
            .FirstOrDefaultAsync(course =>
                course.CourseName == "Software Development" &&
                course.Branch.City == "Dublin");

        var dataAnalyticsDublin = await context.Courses
            .FirstOrDefaultAsync(course =>
                course.CourseName == "Data Analytics" &&
                course.Branch.City == "Dublin");

        if (student1 == null || student2 == null ||
            softwareDevelopmentDublin == null || dataAnalyticsDublin == null)
        {
            return;
        }

        // Matricular Student1 em Software Development Dublin
        var enrolment1 = new CourseEnrolment
        {
            StudentProfileId = student1.Id,
            CourseId = softwareDevelopmentDublin.Id,
            EnrolDate = new DateOnly(2025, 9, 1),
            Status = EnrolmentStatus.Active
        };

        // Matricular Student2 em Software Development Dublin
        var enrolment2 = new CourseEnrolment
        {
            StudentProfileId = student2.Id,
            CourseId = softwareDevelopmentDublin.Id,
            EnrolDate = new DateOnly(2025, 9, 1),
            Status = EnrolmentStatus.Active
        };

        // Matricular Student2 também em Data Analytics Dublin
        var enrolment3 = new CourseEnrolment
        {
            StudentProfileId = student2.Id,
            CourseId = dataAnalyticsDublin.Id,
            EnrolDate = new DateOnly(2025, 9, 1),
            Status = EnrolmentStatus.Active
        };

        await context.CourseEnrolments.AddRangeAsync(enrolment1, enrolment2, enrolment3);
        await context.SaveChangesAsync();

        // Criar registos de presença das últimas 4 semanas para cada matrícula.
        // Datas fixas para garantir consistência dos dados de demonstração.
        var sessionDates = new[]
        {
            new DateOnly(2025, 9, 8),
            new DateOnly(2025, 9, 15),
            new DateOnly(2025, 9, 22),
            new DateOnly(2025, 9, 29)
        };

        var attendanceRecords = new List<AttendanceRecord>();

        // Student1 — Software Development: presente em todas as sessões
        foreach (var sessionDate in sessionDates)
        {
            attendanceRecords.Add(new AttendanceRecord
            {
                CourseEnrolmentId = enrolment1.Id,
                SessionDate = sessionDate,
                Present = true
            });
        }

        // Student2 — Software Development: ausente na segunda sessão
        attendanceRecords.Add(new AttendanceRecord { CourseEnrolmentId = enrolment2.Id, SessionDate = sessionDates[0], Present = true });
        attendanceRecords.Add(new AttendanceRecord { CourseEnrolmentId = enrolment2.Id, SessionDate = sessionDates[1], Present = false });
        attendanceRecords.Add(new AttendanceRecord { CourseEnrolmentId = enrolment2.Id, SessionDate = sessionDates[2], Present = true });
        attendanceRecords.Add(new AttendanceRecord { CourseEnrolmentId = enrolment2.Id, SessionDate = sessionDates[3], Present = true });

        // Student2 — Data Analytics: presente em todas as sessões
        foreach (var sessionDate in sessionDates)
        {
            attendanceRecords.Add(new AttendanceRecord
            {
                CourseEnrolmentId = enrolment3.Id,
                SessionDate = sessionDate,
                Present = true
            });
        }

        await context.AttendanceRecords.AddRangeAsync(attendanceRecords);
        await context.SaveChangesAsync();
    }
}