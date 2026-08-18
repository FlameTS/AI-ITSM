namespace AIITSM.Application.Reporting;

public class IncidentStatisticsDto
{
    public int TotalIncidents { get; set; }

    public int OpenIncidents { get; set; }

    public int ResolvedIncidents { get; set; }

    public int EscalatedIncidents { get; set; }
}