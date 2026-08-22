namespace AIITSM.Application._02_M2_IncidentManagement_2.Notifications
{
    public class NotificationDto
    {
        public int NotificationId { get; set; }

        public int UserId { get; set; }

        public int? IncidentId { get; set; }

        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}