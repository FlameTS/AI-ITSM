using AIITSM.Application.Reporting;
using Microsoft.AspNetCore.Mvc;

namespace AIITSM.Web.Controllers._05_M5_Reporting;

[Route("api/reporting")]
[ApiController]
public class ReportingController : ControllerBase
{
    [HttpGet("statistics")]
    public ActionResult<IncidentStatisticsDto> GetIncidentStatistics()
    {
        return Ok(new IncidentStatisticsDto
        {
            TotalIncidents = 0,
            OpenIncidents = 0,
            ResolvedIncidents = 0,
            EscalatedIncidents = 0
        });
    }

    [HttpGet("unresolved")]
    public ActionResult<IEnumerable<UnresolvedIncidentDto>> GetUnresolvedIncidents()
    {
        return Ok(Array.Empty<UnresolvedIncidentDto>());
    }

    [HttpGet("escalated")]
    public ActionResult<IEnumerable<EscalatedIncidentDto>> GetEscalatedIncidents()
    {
        return Ok(Array.Empty<EscalatedIncidentDto>());
    }

    [HttpGet("team-performance")]
    public ActionResult<IEnumerable<SupportTeamPerformanceDto>> GetTeamPerformance()
    {
        return Ok(Array.Empty<SupportTeamPerformanceDto>());
    }
}
