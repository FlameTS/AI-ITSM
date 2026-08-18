namespace AIITSM.Application.Reporting;

public class ReportingService
{
    public IncidentStatisticsDto GetIncidentStatistics()
    {
        return new IncidentStatisticsDto
        {
            TotalIncidents = 0,
            OpenIncidents = 0,
            ResolvedIncidents = 0,
            EscalatedIncidents = 0
        };
    }
}