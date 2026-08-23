using AIITSM.Application._02_M2_IncidentManagement_2.Communication;
using AIITSM.Infrastructure._06_M6_AI;
using Microsoft.EntityFrameworkCore;

namespace AIITSM.Infrastructure._02_M2_IncidentManagement_2.Communication
{
    public class IncidentCommentService : IIncidentCommentService
    {
        private readonly AIITSMDbContext _dbContext;

        public IncidentCommentService(AIITSMDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<IncidentCommentDto>> GetCommentsAsync(
            int incidentId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.IncidentComments
                .AsNoTracking()
                .Where(x => x.IncidentId == incidentId)
                .OrderBy(x => x.CreatedAt)
                .Select(x => new IncidentCommentDto
                {
                    CommentId = x.CommentId,
                    IncidentId = x.IncidentId,
                    UserId = x.UserId,
                    CommentText = x.CommentText,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }

        public async Task AddCommentAsync(
            int incidentId,
            int userId,
            string commentText,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(commentText))
            {
                throw new ArgumentException(
                    "Comment cannot be empty.",
                    nameof(commentText));
            }

            var incidentExists = await _dbContext.Incidents
                .AsNoTracking()
                .AnyAsync(
                    x => x.IncidentId == incidentId,
                    cancellationToken);

            if (!incidentExists)
            {
                throw new InvalidOperationException(
                    "The specified incident does not exist.");
            }

            var comment = new AIITSM.Domain._02_M2_IncidentManagement_2.IncidentComment
            {
                IncidentId = incidentId,
                UserId = userId,
                CommentText = commentText.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.IncidentComments.Add(comment);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}