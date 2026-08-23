namespace AIITSM.Application.Reporting;

public interface IReportingService
{
    Task<IncidentStatisticsDto> GetIncidentStatisticsAsync(
       CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UnresolvedIncidentDto>> GetUnresolvedIncidentsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SupportTeamPerformanceDto>> GetTeamPerformanceAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RecurringIncidentPatternDto>> GetRecurringIncidentPatternsAsync(
    CancellationToken cancellationToken = default);
}