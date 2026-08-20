using System;
using System.Collections.Generic;

namespace AIITSM.Infrastructure.Data;

public partial class AianalysisRelatedIncident
{
    public int AianalysisRelatedIncidentId { get; set; }

    public int AianalysisId { get; set; }

    public int RelatedIncidentId { get; set; }

    public string RelationshipType { get; set; } = null!;

    public decimal? SimilarityScore { get; set; }

    public virtual Aianalysis Aianalysis { get; set; } = null!;

    public virtual Incident RelatedIncident { get; set; } = null!;
}
