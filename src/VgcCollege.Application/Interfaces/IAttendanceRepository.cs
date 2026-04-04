using VgcCollege.Domain.Entities;

namespace VgcCollege.Application.Interfaces;


/// <summary>
/// Purpose: Define o contrato de acesso a dados para a entidade AttendanceRecord.
/// Consumed by: VgcCollege.Application (services), implementado por VgcCollege.Data.
/// Layer: Application Interfaces
/// </summary>
public interface IAttendanceRepository
{
    Task<IEnumerable<AttendanceRecord>> GetByEnrolmentAsync (int enrolmentId);
    
    /// <summary>
    /// Verifica se já existe um registo de presença para uma matrícula numa data específica.
    /// Utilizado para prevenir registos duplicados na mesma sessão.
    /// </summary>
    /// <param name="enrolmentId">Identificador da matrícula.</param>
    /// <param name="sessionDate">Data da sessão.</param>
    Task<bool> ExistsAsync(int enrolmentId, DateOnly sessionDate);
    Task AddAsync(AttendanceRecord attendanceRecord);
    Task UpdateAsync(AttendanceRecord attendanceRecord);
}