using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Data.Configurations;

/// <summary>
/// Purpose: Define o mapeamento da entidade Exam para a tabela no banco de dados.
/// Consumed by: AppDbContext (via ApplyConfigurationsFromAssembly).
/// Layer: Data Configurations
/// </summary>
public class ExamConfiguration : IEntityTypeConfiguration<Exam>
{
    public void Configure(EntityTypeBuilder<Exam> builder)
    {
        builder.HasKey(exam => exam.Id);

        builder.Property(exam => exam.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(exam => exam.Course)
            .WithMany(course => course.Exams)
            .HasForeignKey(exam => exam.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}