namespace AIITSM.Application.Reporting;

public class EscalatedIncidentDto
{
    public int IncidentId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Priority { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public int? EscalatedBy { get; set; }

    public int? EscalatedTo { get; set; }

    public string Reason { get; set; } = string.Empty;

    public DateTime EscalatedAt { get; set; }
}
