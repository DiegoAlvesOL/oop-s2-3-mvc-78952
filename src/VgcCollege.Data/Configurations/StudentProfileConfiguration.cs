using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Data.Configurations;

/// <summary>
/// Purpose: Define o mapeamento da entidade StudentProfile para a tabela no banco de dados.
/// Consumed by: AppDbContext (via ApplyConfigurationsFromAssembly).
/// Layer: Data Configurations
/// </summary>
public class StudentProfileConfiguration : IEntityTypeConfiguration<StudentProfile>
{
    public void Configure(EntityTypeBuilder<StudentProfile> builder)
    {
        builder.HasKey(student => student.Id);

        builder.Property(student => student.IdentityUserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.HasIndex(student => student.IdentityUserId)
            .IsUnique();

        builder.Property(student => student.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(student => student.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(student => student.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(student => student.Phone)
            .HasMaxLength(20);

        builder.Property(student => student.StreetName)
            .HasMaxLength(200);

        builder.Property(student => student.City)
            .HasMaxLength(100);

        builder.Property(student => student.StudentNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(student => student.StudentNumber)
            .IsUnique();
    }
}