using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Data.Configurations;

/// <summary>
/// Purpose: Define o mapeamento da entidade LecturerProfile para a tabela no banco de dados.
/// Consumed by: AppDbContext (via ApplyConfigurationsFromAssembly).
/// Layer: Data Configurations
/// </summary>
public class LecturerProfileConfiguration : IEntityTypeConfiguration<LecturerProfile>
{
    public void Configure(EntityTypeBuilder<LecturerProfile> builder)
    {
        builder.HasKey(lecturer => lecturer.Id);

        builder.Property(lecturer => lecturer.IdentityUserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.HasIndex(lecturer => lecturer.IdentityUserId)
            .IsUnique();

        builder.Property(lecturer => lecturer.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(lecturer => lecturer.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(lecturer => lecturer.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(lecturer => lecturer.Phone)
            .HasMaxLength(20);
    }
}