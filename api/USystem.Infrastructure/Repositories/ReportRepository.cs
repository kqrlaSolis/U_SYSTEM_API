// Indicadores agregados para el dashboard.
using Microsoft.EntityFrameworkCore;
using USystem.Domain.Entities;
using USystem.Domain.Interfaces;
using USystem.Infrastructure.Persistence;

namespace USystem.Infrastructure.Repositories;


public class ReportRepository : IReportRepository
{
    private readonly USystemDbContext _Context;

    public ReportRepository(USystemDbContext Context) => _Context = Context;

    public Task<int> CountStudentsAsync(CancellationToken Cancellation = default)
        => _Context.Students.CountAsync(Student => Student.Status == "Active", Cancellation);

    public Task<int> CountTeachersAsync(CancellationToken Cancellation = default)
        => _Context.Teachers.CountAsync(Teacher => Teacher.IsActive, Cancellation);

    public Task<int> CountOpenClassGroupsAsync(CancellationToken Cancellation = default)
        => _Context.ClassGroups.CountAsync(Group => Group.Status == "Open", Cancellation);

    public async Task<(int Count, decimal Amount)> GetPendingChargesAsync(
        CancellationToken Cancellation = default)
    {
        var Charges = await _Context.Charges
            .AsNoTracking()
            .Where(Charge => Charge.Status == "Pending")
            .Select(Charge => new { Charge.Amount, Charge.Discount, Charge.Surcharge })
            .ToListAsync(Cancellation);

        var Amount = Charges.Sum(Charge => Charge.Amount - Charge.Discount + Charge.Surcharge);
        return (Charges.Count, Amount);
    }

    public async Task<(int Requested, int Issued)> GetProceduresAsync(
        CancellationToken Cancellation = default)
    {
        var Requested = await _Context.Procedures.CountAsync(
            Procedure => Procedure.Status == "Requested",
            Cancellation);

        var Issued = await _Context.Procedures.CountAsync(
            Procedure => Procedure.Status == "Issued",
            Cancellation);

        return (Requested, Issued);
    }

    public Task<int> CountEnrollmentsAsync(CancellationToken Cancellation = default)
        => _Context.Enrollments.CountAsync(Enrollment => Enrollment.Status != "Cancelled", Cancellation);

    public Task<int> CountEnrollmentsInTermAsync(
        int AcademicTermId,
        CancellationToken Cancellation = default)
        => _Context.Enrollments.CountAsync(
            Enrollment => Enrollment.AcademicTermId == AcademicTermId && Enrollment.Status != "Cancelled",
            Cancellation);
}
