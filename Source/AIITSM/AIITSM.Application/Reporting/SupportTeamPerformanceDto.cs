namespace AIITSM.Application.Reporting;

public class SupportTeamPerformanceDto
{
    public int UserId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public int AssignedIncidents { get; set; }

    public int ResolvedIncidents { get; set; }

    public int OpenIncidents { get; set; }
}
