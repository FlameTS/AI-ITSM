
namespace AIITSM.Domain._02_M2_IncidentManagement_2
{
    public class IncidentComment
    {
        public int CommentId { get; set; }

        public int IncidentId { get; set; }

        public int UserId { get; set; }

        public string CommentText { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}