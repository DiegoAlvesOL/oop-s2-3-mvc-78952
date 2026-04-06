using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Data.Configurations;

/// <summary>
/// Purpose: Define o mapeamento da entidade Branch para a tabela no banco de dados.
/// Consumed by: AppDbContext (via ApplyConfigurationsFromAssembly).
/// Layer: Data Configurations
/// </summary>
public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.HasKey(branch => branch.Id);
        builder.Property(branch => branch.BranchName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(branch => branch.StreetName)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(branch => branch.City)
            .IsRequired()
            .HasMaxLength(100);
    }
}