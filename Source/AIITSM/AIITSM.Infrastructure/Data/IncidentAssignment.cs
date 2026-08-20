using System;
using System.Collections.Generic;

namespace AIITSM.Infrastructure.Data;

public partial class IncidentAssignment
{
    public int AssignmentId { get; set; }

    public int IncidentId { get; set; }

    public int AssignedTo { get; set; }

    public DateTime AssignedAt { get; set; }

    public virtual User AssignedToNavigation { get; set; } = null!;

    public virtual Incident Incident { get; set; } = null!;
}
