
namespace AIITSM.Domain._02_M2_IncidentManagement
{
    public class Incident
    {
        public int IncidentId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        public IncidentPriority Priority { get; set; }

        public IncidentStatus Status { get; set; }

        // Matches Incidents.CreatedBy in Database.sql (FK -> Users.UserId).
        // We deliberately do NOT hold a User navigation property here —
        // same pattern M6 used for IncidentId: reference by id only, don't
        // take ownership of an entity that belongs to another module (M1).
        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ResolvedAt { get; set; }
    }
}
