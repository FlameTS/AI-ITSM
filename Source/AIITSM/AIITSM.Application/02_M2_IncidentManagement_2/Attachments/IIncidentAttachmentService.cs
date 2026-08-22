namespace AIITSM.Application._02_M2_IncidentManagement_2.Attachments
{
    public interface IIncidentAttachmentService
    {
        Task<IReadOnlyList<IncidentAttachmentDto>> GetAttachmentsAsync(
            int incidentId,
            CancellationToken cancellationToken = default);

        Task<int> AddAttachmentAsync(
            int incidentId,
            string fileName,
            string storedFileName,
            string contentType,
            long fileSize,
            CancellationToken cancellationToken = default);
    }
}