

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Data.Configurations;

/// <summary>
///Purpose: Define o mapeamento da entidade AssignmentResult para a tabela no banco de dados.
/// Consumed by: AppDbContext (via ApplyConfigurationsFromAssembly).
/// Layer: Data — Configurations
/// </summary>
public class AssignmentResultConfiguration : IEntityTypeConfiguration<AssignmentResult>
{
    public void Configure(EntityTypeBuilder<AssignmentResult> builder)
    {
        builder.HasKey(result => result.Id);

        builder.Property(result => result.Feedback)
            .HasMaxLength(1000);

        builder.HasOne(result => result.Assignment)
            .WithMany(assignment => assignment.Results)
            .HasForeignKey(result => result.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(result => result.Student)
            .WithMany(student => student.AssignmentResults)
            .HasForeignKey(result => result.StudentProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}