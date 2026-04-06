

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Data.Configurations;

/// <summary>
///Purpose: Define o mapeamento da entidade ExamResult para a tabela no banco de dados.
/// Consumed by: AppDbContext (via ApplyConfigurationsFromAssembly).
/// Layer: Data Configurations
/// </summary>
public class ExamResultConfiguration : IEntityTypeConfiguration<ExamResult>
{
    public void Configure(EntityTypeBuilder<ExamResult> builder)
    {
        builder.HasKey(result => result.Id);

        builder.Property(result => result.Grade)
            .HasMaxLength(10);

        builder.HasOne(result => result.Exam)
            .WithMany(exam => exam.Results)
            .HasForeignKey(result => result.ExamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(result => result.Student)
            .WithMany(student => student.ExamResults)
            .HasForeignKey(result => result.StudentProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}