namespace AIITSM.Domain._07_M7_Automation
{
    public class Escalation
    {
        public int EscalationId { get; set; }

        public int IncidentId { get; set; }

        public int? EscalatedBy { get; set; }

        public int? EscalatedTo { get; set; }

        public string Reason { get; set; } = string.Empty;

        public DateTime EscalatedAt { get; set; }

        public DateTime? ResolvedAt { get; set; }
    }
}