
using AIITSM.Application._02_M2_IncidentManagement_2.Feedback;
using AIITSM.Domain._02_M2_IncidentManagement_2;
using AIITSM.Domain._02_M2_IncidentManagement;
using AIITSM.Infrastructure._06_M6_AI;
using Microsoft.EntityFrameworkCore;

namespace AIITSM.Infrastructure._02_M2_IncidentManagement_2.Feedback
{
    public class IncidentFeedbackService : IIncidentFeedbackService
    {
        private readonly AIITSMDbContext _dbContext;

        public IncidentFeedbackService(AIITSMDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IncidentFeedbackDto?> GetFeedbackAsync(
            int incidentId,
            int userId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.IncidentFeedback
                .AsNoTracking()
                .Where(x =>
                    x.IncidentId == incidentId &&
                    x.UserId == userId)
                .Select(x => new IncidentFeedbackDto
                {
                    FeedbackId = x.FeedbackId,
                    IncidentId = x.IncidentId,
                    UserId = x.UserId,
                    FeedbackText = x.FeedbackText,
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddFeedbackAsync(
            int incidentId,
            int userId,
            string? feedbackText,
            CancellationToken cancellationToken = default)
        {
            var incident = await _dbContext.Incidents
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.IncidentId == incidentId,
                    cancellationToken);

            if (incident == null)
            {
                throw new InvalidOperationException(
                    "The specified incident does not exist.");
            }

            if (incident.CreatedBy != userId)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to provide feedback for this incident.");
            }

            if (incident.Status != IncidentStatus.Resolved)
            {
                throw new InvalidOperationException(
                    "Feedback can only be provided after the incident is resolved.");
            }

            var existingFeedback = await _dbContext.IncidentFeedback
                .AsNoTracking()
                .AnyAsync(
                    x =>
                        x.IncidentId == incidentId &&
                        x.UserId == userId,
                    cancellationToken);

            if (existingFeedback)
            {
                throw new InvalidOperationException(
                    "Feedback has already been provided for this incident.");
            }

            var feedback = new IncidentFeedback
            {
                IncidentId = incidentId,
                UserId = userId,
                FeedbackText = string.IsNullOrWhiteSpace(feedbackText)
                    ? null
                    : feedbackText.Trim(),
                CreatedAt = DateTime.Now
            };

            _dbContext.IncidentFeedback.Add(feedback);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}