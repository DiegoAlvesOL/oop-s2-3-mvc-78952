using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Data.Configurations;

/// <summary>
/// Purpose: Define o mapeamento da entidade Assignment para a tabela no banco de dados.
/// Consumed by: AppDbContext (via ApplyConfigurationsFromAssembly).
/// Layer: Data — Configurations
/// </summary>
public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        builder.HasKey(assignment => assignment.Id);

        builder.Property(assignment => assignment.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(assignment => assignment.Course)
            .WithMany(course => course.Assignments)
            .HasForeignKey(assignment => assignment.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}