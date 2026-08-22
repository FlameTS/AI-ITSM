using AIITSM.Domain._02_M2_IncidentManagement;

namespace AIITSM.Application._02_M2_IncidentManagement
{
    // Input for creating an incident. Deliberately does NOT include
    // Status or CreatedBy — those are not user-supplied:
    //   - Status is always forced to Open on creation.
    //   - CreatedBy always comes from ICurrentUserService, never from
    //     client input (so an employee can't create an incident on
    //     someone else's behalf by tampering with a form field).
    public class CreateIncidentRequest
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public IncidentPriority Priority { get; set; }
    }
}
