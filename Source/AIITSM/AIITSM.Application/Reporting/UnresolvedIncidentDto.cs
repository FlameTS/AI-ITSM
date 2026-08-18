namespace AIITSM.Application.Reporting;

public class UnresolvedIncidentDto
{
    public int IncidentId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Priority { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
