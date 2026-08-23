using AIITSM.Application._02_M2_IncidentManagement_2.Attachments;
using AIITSM.Domain._02_M2_IncidentManagement_2;
using AIITSM.Infrastructure._06_M6_AI;
using Microsoft.EntityFrameworkCore;

namespace AIITSM.Infrastructure._02_M2_IncidentManagement_2.Attachments
{
    public class IncidentAttachmentService : IIncidentAttachmentService
    {
        private readonly AIITSMDbContext _context;

        public IncidentAttachmentService(AIITSMDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<IncidentAttachmentDto>> GetAttachmentsAsync(
            int incidentId,
            CancellationToken cancellationToken = default)
        {
            return await _context.IncidentAttachments
                .Where(x => x.IncidentId == incidentId)
                .OrderByDescending(x => x.UploadedAt)
                .Select(x => new IncidentAttachmentDto
                {
                    AttachmentId = x.AttachmentId,
                    IncidentId = x.IncidentId,
                    FileName = x.FileName,
                    ContentType = x.ContentType,
                    FileSize = x.FileSize,
                    UploadedAt = x.UploadedAt
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<int> AddAttachmentAsync(
            int incidentId,
            string fileName,
            string storedFileName,
            string contentType,
            long fileSize,
            CancellationToken cancellationToken = default)
        {
            var incidentExists = await _context.Incidents
                .AnyAsync(x => x.IncidentId == incidentId, cancellationToken);

            if (!incidentExists)
            {
                throw new InvalidOperationException("The referenced incident does not exist.");
            }

            var attachment = new IncidentAttachment
            {
                IncidentId = incidentId,
                FileName = fileName,
                StoredFileName = storedFileName,
                ContentType = contentType,
                FileSize = fileSize,
                UploadedAt = DateTime.UtcNow
            };

            _context.IncidentAttachments.Add(attachment);

            await _context.SaveChangesAsync(cancellationToken);

            return attachment.AttachmentId;
        }
    }
}