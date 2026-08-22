using AIITSM.Domain._02_M2_IncidentManagement;

namespace AIITSM.Application._02_M2_IncidentManagement
{
    // Full shape for the Incident Details page.
    public class IncidentDetailsDto
    {
        public int IncidentId { get; set; }

        public string IncidentNumber { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public IncidentPriority Priority { get; set; }

        public IncidentStatus Status { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ResolvedAt { get; set; }
    }
}
