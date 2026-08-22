using AIITSM.Domain._02_M2_IncidentManagement;

namespace AIITSM.Application._02_M2_IncidentManagement
{
    // Row shape for "My Incidents".
    public class IncidentSummaryDto
    {
        public int IncidentId { get; set; }

        // Human-friendly incident number, e.g. "INC-000042".
        // Computed at read time (Infrastructure), not stored in the DB.
        public string IncidentNumber { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public IncidentPriority Priority { get; set; }

        public IncidentStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
