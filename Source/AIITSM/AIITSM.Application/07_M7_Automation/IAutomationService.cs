namespace AIITSM.Application._07_M7_Automation
{
    public interface IAutomationService
    {
        Task SendAssignmentNotificationAsync(int incidentId, int assignedTo);

        Task SendStatusChangeNotificationAsync(
            int incidentId,
            int userId,
            string newStatus);

        Task SendCriticalIncidentNotificationAsync(
            int incidentId,
            int userId);

        Task EscalateIncidentAsync(
            int incidentId,
            int escalatedBy,
            int escalatedTo,
            string reason);
    }
}