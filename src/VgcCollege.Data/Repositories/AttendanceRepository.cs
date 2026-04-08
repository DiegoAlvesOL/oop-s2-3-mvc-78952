using Microsoft.EntityFrameworkCore;
using VgcCollege.Application.Interfaces;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Data.Repositories;

/// <summary>
/// Repositório de AttendanceRecord. Implementa IAttendanceRepository usando o AppDbContext.
/// </summary>
public class AttendanceRepository : IAttendanceRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa o repositório com o contexto do banco de dados.
    /// </summary>
    /// <param name="context">Contexto do banco de dados injectado via DI.</param>
    public AttendanceRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retorna todos os registos de presença de uma matrícula específica,
    /// ordenados por data de sessão.
    /// </summary>
    /// <param name="enrolmentId">Identificador da matrícula.</param>
    public async Task<IEnumerable<AttendanceRecord>> GetByEnrolmentAsync(int enrolmentId)
    {
        return await _context.AttendanceRecords
            .Where(record => record.CourseEnrolmentId == enrolmentId)
            .OrderBy(record => record.SessionDate)
            .ToListAsync();
    }

    /// <summary>
    /// Verifica se já existe um registo de presença para uma matrícula numa data específica.
    /// Usado para prevenir registos duplicados na mesma sessão.
    /// </summary>
    /// <param name="enrolmentId">Identificador da matrícula.</param>
    /// <param name="sessionDate">Data da sessão.</param>
    public async Task<bool> ExistsAsync(int enrolmentId, DateOnly sessionDate)
    {
        return await _context.AttendanceRecords
            .AnyAsync(record =>
                record.CourseEnrolmentId == enrolmentId &&
                record.SessionDate == sessionDate);
    }

    /// <summary>Adiciona um novo registo de presença ao banco de dados.</summary>
    /// <param name="attendanceRecord">Entidade AttendanceRecord a ser adicionada.</param>
    public async Task AddAsync(AttendanceRecord attendanceRecord)
    {
        await _context.AttendanceRecords.AddAsync(attendanceRecord);
        await _context.SaveChangesAsync();
    }

    /// <summary>Actualiza um registo de presença existente.</summary>
    /// <param name="attendanceRecord">Entidade AttendanceRecord com os dados actualizados.</param>
    public async Task UpdateAsync(AttendanceRecord attendanceRecord)
    {
        _context.AttendanceRecords.Update(attendanceRecord);
        await _context.SaveChangesAsync();
    }
}