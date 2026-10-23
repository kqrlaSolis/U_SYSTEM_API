// Dashboard: agrega indicadores de las áreas del sistema y el periodo en curso.
using USystem.Application.Dtos;
using USystem.Domain.Interfaces;

namespace USystem.Application.Services;

public interface IReportService
{
    Task<DashboardDto> GetDashboardAsync(CancellationToken Cancellation = default);
}

public class ReportService : IReportService
{
    private readonly IReportRepository _ReportRepository;
    private readonly IAcademicTermRepository _TermRepository;

    public ReportService(
        IReportRepository ReportRepository,
        IAcademicTermRepository TermRepository)
    {
        _ReportRepository = ReportRepository;
        _TermRepository = TermRepository;
    }

    public async Task<DashboardDto> GetDashboardAsync(CancellationToken Cancellation = default)
    {
        var CurrentTerm = await _TermRepository.GetCurrentAsync(Cancellation);
        var PendingCharges = await _ReportRepository.GetPendingChargesAsync(Cancellation);
        var Procedures = await _ReportRepository.GetProceduresAsync(Cancellation);
        var OpenGroups = await _ReportRepository.CountOpenClassGroupsAsync(Cancellation);

        return new DashboardDto
        {
            CurrentTermId = CurrentTerm?.Id,
            CurrentTermCode = CurrentTerm?.Code,
            CurrentTermName = CurrentTerm?.Name,
            CurrentTermStatus = CurrentTerm?.Status,
            CurrentTermStartDate = CurrentTerm?.StartDate,
            CurrentTermEndDate = CurrentTerm?.EndDate,
            ActiveStudents = await _ReportRepository.CountStudentsAsync(Cancellation),
            ActiveTeachers = await _ReportRepository.CountTeachersAsync(Cancellation),
            OpenClassGroups = OpenGroups,
            TermEnrollments = CurrentTerm is null
                ? 0
                : await _ReportRepository.CountEnrollmentsInTermAsync(CurrentTerm.Id, Cancellation),
            PendingCharges = PendingCharges.Count,
            PendingAmount = PendingCharges.Amount,
            RequestedProcedures = Procedures.Requested,
            IssuedProcedures = Procedures.Issued,
            TotalEnrollments = await _ReportRepository.CountEnrollmentsAsync(Cancellation),
        };
    }
}
