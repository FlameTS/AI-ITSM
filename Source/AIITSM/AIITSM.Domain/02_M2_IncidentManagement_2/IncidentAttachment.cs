namespace AIITSM.Domain._02_M2_IncidentManagement_2
{
    public class IncidentAttachment
    {
        public int AttachmentId { get; set; }

        public int IncidentId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string StoredFileName { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; }
    }
}