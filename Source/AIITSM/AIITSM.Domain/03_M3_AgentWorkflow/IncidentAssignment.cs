namespace AIITSM.Domain._03_M3_AgentWorkflow
{
    public class IncidentAssignment
    {
        public int AssignmentId { get; set; }

        public int IncidentId { get; set; }

        public int AssignedTo { get; set; }

        public DateTime AssignedAt { get; set; }
    }
}