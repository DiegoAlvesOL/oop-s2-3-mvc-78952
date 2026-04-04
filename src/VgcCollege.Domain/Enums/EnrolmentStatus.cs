namespace VgcCollege.Domain.Enums;


/// <summary>
/// Purpose: Define os estados possíveis de uma matrícula no sistema.
/// Consumed by: VgcCollege.Domain (CourseEnrolment), VgcCollege.Application (services).
/// Layer: Domain, Enums
/// </summary>
public enum EnrolmentStatus
{
    Active,
    Completed,
    Withdrawn
}