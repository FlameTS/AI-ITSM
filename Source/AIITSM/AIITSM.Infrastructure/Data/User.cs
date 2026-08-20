using System;
using System.Collections.Generic;

namespace AIITSM.Infrastructure.Data;

public partial class User
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual ICollection<Escalation> EscalationEscalatedByNavigations { get; set; } = new List<Escalation>();

    public virtual ICollection<Escalation> EscalationEscalatedToNavigations { get; set; } = new List<Escalation>();

    public virtual ICollection<IncidentAssignment> IncidentAssignments { get; set; } = new List<IncidentAssignment>();

    public virtual ICollection<IncidentComment> IncidentComments { get; set; } = new List<IncidentComment>();

    public virtual ICollection<Incident> Incidents { get; set; } = new List<Incident>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Role Role { get; set; } = null!;
}
