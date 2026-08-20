using System;
using System.Collections.Generic;

namespace AIITSM.Infrastructure.Data;

public partial class IncidentComment
{
    public int CommentId { get; set; }

    public int IncidentId { get; set; }

    public int UserId { get; set; }

    public string CommentText { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Incident Incident { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
