namespace AIITSM.Application._02_M2_IncidentManagement_2.Feedback
{
    public class IncidentFeedbackDto
    {
        public int FeedbackId { get; set; }

        public int IncidentId { get; set; }

        public int UserId { get; set; }

        public string? FeedbackText { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}