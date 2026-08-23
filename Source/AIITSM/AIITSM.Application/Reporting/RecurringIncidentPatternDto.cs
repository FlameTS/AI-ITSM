namespace AIITSM.Application.Reporting;

public class RecurringIncidentPatternDto
{
    public int CategoryId { get; set; }

    public string Pattern { get; set; } = string.Empty;

    public int IncidentCount { get; set; }
}