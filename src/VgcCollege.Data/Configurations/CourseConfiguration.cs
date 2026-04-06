using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Data.Configurations;

/// <summary>
///Purpose: Define o mapeamento da entidade Course para a tabela no banco de dados.
/// Consumed by: AppDbContext (via ApplyConfigurationsFromAssembly).
/// Layer: Data Configurations
/// </summary>
public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(course => course.Id);

        builder.Property(course => course.CourseName)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasOne(course => course.Branch)
            .WithMany(branch => branch.Courses)
            .HasForeignKey(course => course.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}