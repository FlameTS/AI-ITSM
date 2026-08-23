
namespace AIITSM.Domain._02_M2_IncidentManagement
{
    // NOTE: No Category entity existed anywhere in the codebase yet
    // (only the Categories table in Database.sql). Incident creation
    // needs a CategoryId, so it's added here alongside Incident.
    // Move this file if the team later gives Category its own module.
    public class Category
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;
    }
}
