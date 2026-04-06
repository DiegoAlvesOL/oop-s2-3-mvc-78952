using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Data.Configurations;

/// <summary>
/// Purpose: Define o mapeamento da entidade LecturerCourseAssignment para a tabela no banco de dados.
/// Consumed by: AppDbContext (via ApplyConfigurationsFromAssembly).
/// Layer: Data Configurations
/// </summary>
public class LecturerCourseAssignmentConfiguration : IEntityTypeConfiguration<LecturerCourseAssignment>
{
    public void Configure(EntityTypeBuilder<LecturerCourseAssignment> builder)
    {
        builder.HasKey(assignment => assignment.Id);

        builder.HasOne(assignment => assignment.Lecturer)
            .WithMany(lecturer => lecturer.CourseAssignments)
            .HasForeignKey(assignment => assignment.LecturerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(assignment => assignment.Course)
            .WithMany(course => course.LecturerCourseAssignments)
            .HasForeignKey(assignment => assignment.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}