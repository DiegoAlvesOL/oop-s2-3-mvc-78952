using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Data.Models;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Data;


/// <summary>
/// Purpose: Contexto principal do Entity Framework Core. Define as tabelas e carrega as configurações de cada entidade.
/// Consumed by: VgcCollege.Web (Program.cs para registo no DI), VgcCollege.Data (repositórios).
/// Layer: Data Context
/// </summary>
public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Branch> Branches { get; set; } = null!;
    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<StudentProfile> StudentProfiles { get; set; } = null!;
    public DbSet<LecturerProfile> LecturerProfiles { get; set; } = null!;
    public DbSet<LecturerCourseAssignment> LecturerCourseAssignments { get; set; } = null!;
    public DbSet<CourseEnrolment> CourseEnrolments { get; set; } = null!;
    public DbSet<AttendanceRecord> AttendanceRecords { get; set; } = null!;
    public DbSet<Assignment> Assignments { get; set; } = null!;
    public DbSet<AssignmentResult> AssignmentResults { get; set; } = null!;
    public DbSet<Exam> Exams { get; set; } = null!;
    public DbSet<ExamResult> ExamResults { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
    
}