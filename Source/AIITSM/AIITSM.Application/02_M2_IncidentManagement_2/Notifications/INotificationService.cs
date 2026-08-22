namespace AIITSM.Application._02_M2_IncidentManagement_2.Notifications
{
    public interface INotificationService
    {
        Task<IReadOnlyList<NotificationDto>> GetMyNotificationsAsync(
            int userId,
            CancellationToken cancellationToken = default);

        Task<int> GetUnreadCountAsync(
            int userId,
            CancellationToken cancellationToken = default);

        Task CreateNotificationAsync(
            int userId,
            int? incidentId,
            string message,
            CancellationToken cancellationToken = default);

        Task MarkAsReadAsync(
            int notificationId,
            int userId,
            CancellationToken cancellationToken = default);
    }
}