namespace AIITSM.Web.Models
{
    public class AgentWorkflowModel
    {
        public int IncidentId { get; set; }

        public string Title { get; set; } = "";

        public string Description { get; set; } = "";

        public string Priority { get; set; } = "";

        public string Status { get; set; } = "Open";

        public int? AssignedTo { get; set; }

        public string? Comment { get; set; }
    }
}