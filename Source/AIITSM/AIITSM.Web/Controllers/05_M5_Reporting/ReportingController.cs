using AIITSM.Application.Reporting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIITSM.Web.Controllers._05_M5_Reporting;

[Route("api/reporting")]
[ApiController]
[Authorize(Roles = "ITManager,ITAdministrator")]
public class ReportingController : ControllerBase
{
    private readonly IReportingService _reportingService;

    public ReportingController(IReportingService reportingService)
    {
        _reportingService = reportingService;
    }

    [HttpGet("statistics")]
    public async Task<ActionResult<IncidentStatisticsDto>> GetIncidentStatistics(
        CancellationToken cancellationToken)
    {
        var statistics =
            await _reportingService.GetIncidentStatisticsAsync(
                cancellationToken);

        return Ok(statistics);
    }

    [HttpGet("unresolved")]
    public async Task<ActionResult<IReadOnlyList<UnresolvedIncidentDto>>> GetUnresolvedIncidents(
    CancellationToken cancellationToken)
    {
        var incidents =
            await _reportingService.GetUnresolvedIncidentsAsync(
                cancellationToken);

        return Ok(incidents);
    }

    [HttpGet("escalated")]
    public ActionResult<IEnumerable<EscalatedIncidentDto>> GetEscalatedIncidents()
    {
        return Ok(Array.Empty<EscalatedIncidentDto>());
    }

    [HttpGet("team-performance")]
    public async Task<ActionResult<IReadOnlyList<SupportTeamPerformanceDto>>> GetTeamPerformance(
    CancellationToken cancellationToken)
    {
        var performance =
            await _reportingService.GetTeamPerformanceAsync(
                cancellationToken);

        return Ok(performance);
    }

    [HttpGet("recurring-patterns")]
    public async Task<ActionResult<IReadOnlyList<RecurringIncidentPatternDto>>> GetRecurringIncidentPatterns(
    CancellationToken cancellationToken)
    {
        var patterns =
            await _reportingService.GetRecurringIncidentPatternsAsync(
                cancellationToken);

        return Ok(patterns);
    }
}
