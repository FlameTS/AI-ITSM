using System;
using System.Collections.Generic;

namespace AIITSM.Infrastructure.Data;

public partial class Aianalysis
{
    public int AianalysisId { get; set; }

    public int IncidentId { get; set; }

    public string? SuggestedCategory { get; set; }

    public string? SuggestedPriority { get; set; }

    public string? SuggestedResolution { get; set; }

    public decimal? ConfidenceScore { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<AianalysisRelatedIncident> AianalysisRelatedIncidents { get; set; } = new List<AianalysisRelatedIncident>();

    public virtual Incident Incident { get; set; } = null!;
}
