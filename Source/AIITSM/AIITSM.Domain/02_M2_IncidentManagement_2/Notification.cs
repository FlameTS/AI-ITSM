namespace AIITSM.Domain._02_M2_IncidentManagement_2
{
    public class Notification
    {
        public int NotificationId { get; set; }

        public int UserId { get; set; }

        public int? IncidentId { get; set; }

        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}