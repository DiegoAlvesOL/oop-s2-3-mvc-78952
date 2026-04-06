

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VgcCollege.Domain.Entities;
using VgcCollege.Domain.Enums;

namespace VgcCollege.Data.Configurations;

/// <summary>
///Purpose: Define o mapeamento da entidade CourseEnrolment para a tabela no banco de dados.
/// Consumed by: AppDbContext (via ApplyConfigurationsFromAssembly).
/// Layer: Data Configurations
/// </summary>
public class CourseEnrolmentConfiguration : IEntityTypeConfiguration<CourseEnrolment>
{
    public void Configure(EntityTypeBuilder<CourseEnrolment> builder)
    {
        builder.HasKey(enrolment => enrolment.Id);

        builder.Property(enrolment => enrolment.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasOne(enrolment => enrolment.Student)
            .WithMany(student => student.Enrolments)
            .HasForeignKey(enrolment => enrolment.StudentProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(enrolment => enrolment.Course)
            .WithMany(course => course.Enrolments)
            .HasForeignKey(enrolment => enrolment.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}