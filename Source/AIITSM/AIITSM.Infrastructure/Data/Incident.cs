using System;
using System.Collections.Generic;

namespace AIITSM.Infrastructure.Data;

public partial class Incident
{
    public int IncidentId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int CategoryId { get; set; }

    public string Priority { get; set; } = null!;

    public string Status { get; set; } = null!;

    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public virtual ICollection<Aianalysis> Aianalyses { get; set; } = new List<Aianalysis>();

    public virtual ICollection<AianalysisRelatedIncident> AianalysisRelatedIncidents { get; set; } = new List<AianalysisRelatedIncident>();

    public virtual Category Category { get; set; } = null!;

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Escalation> Escalations { get; set; } = new List<Escalation>();

    public virtual ICollection<IncidentAssignment> IncidentAssignments { get; set; } = new List<IncidentAssignment>();

    public virtual ICollection<IncidentComment> IncidentComments { get; set; } = new List<IncidentComment>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
