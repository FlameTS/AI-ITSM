using AIITSM.Application._02_M2_IncidentManagement_2.Notifications;
using AIITSM.Infrastructure._06_M6_AI;
using Microsoft.EntityFrameworkCore;

namespace AIITSM.Infrastructure._02_M2_IncidentManagement_2.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly AIITSMDbContext _dbContext;

        public NotificationService(AIITSMDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<NotificationDto>> GetMyNotificationsAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Notifications
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new NotificationDto
                {
                    NotificationId = x.NotificationId,
                    UserId = x.UserId,
                    IncidentId = x.IncidentId,
                    Message = x.Message,
                    IsRead = x.IsRead,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetUnreadCountAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Notifications
                .AsNoTracking()
                .CountAsync(
                    x => x.UserId == userId && !x.IsRead,
                    cancellationToken);
        }

        public async Task CreateNotificationAsync(
            int userId,
            int? incidentId,
            string message,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(
                    "Notification message cannot be empty.",
                    nameof(message));
            }

            var notification = new AIITSM.Domain._02_M2_IncidentManagement_2.Notification
            {
                UserId = userId,
                IncidentId = incidentId,
                Message = message.Trim(),
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Notifications.Add(notification);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task MarkAsReadAsync(
            int notificationId,
            int userId,
            CancellationToken cancellationToken = default)
        {
            var notification = await _dbContext.Notifications
                .FirstOrDefaultAsync(
                    x => x.NotificationId == notificationId &&
                         x.UserId == userId,
                    cancellationToken);

            if (notification == null)
            {
                throw new InvalidOperationException(
                    "Notification was not found.");
            }

            notification.IsRead = true;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}