using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Data.Configurations;

/// <summary>
/// Purpose: Define o mapeamento da entidade AttendanceRecord para a tabela no banco de dados.
/// Consumed by: AppDbContext (via ApplyConfigurationsFromAssembly).
/// Layer: Data — Configurations
/// </summary>
public class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
{
    public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
    {
        builder.HasKey(record => record.Id);

        builder.HasOne(record => record.Enrolment)
            .WithMany(enrolment => enrolment.AttendanceRecords)
            .HasForeignKey(record => record.CourseEnrolmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}