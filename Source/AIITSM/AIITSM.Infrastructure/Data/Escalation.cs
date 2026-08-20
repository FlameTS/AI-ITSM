using System;
using System.Collections.Generic;

namespace AIITSM.Infrastructure.Data;

public partial class Escalation
{
    public int EscalationId { get; set; }

    public int IncidentId { get; set; }

    public int? EscalatedBy { get; set; }

    public int? EscalatedTo { get; set; }

    public string Reason { get; set; } = null!;

    public DateTime EscalatedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public virtual User? EscalatedByNavigation { get; set; }

    public virtual User? EscalatedToNavigation { get; set; }

    public virtual Incident Incident { get; set; } = null!;
}
