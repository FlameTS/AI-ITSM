using AIITSM.Application._07_M7_Automation;

namespace AIITSM.Infrastructure._07_M7_Automation
{
    public class AutomationService : IAutomationService
    {
        public Task SendAssignmentNotificationAsync(
            int incidentId,
            int assignedTo)
        {
            Console.WriteLine(
                $"Notification: Incident {incidentId} assigned to user {assignedTo}.");

            return Task.CompletedTask;
        }

        public Task SendStatusChangeNotificationAsync(
            int incidentId,
            int userId,
            string newStatus)
        {
            Console.WriteLine(
                $"Notification: Incident {incidentId} status changed to {newStatus}.");

            return Task.CompletedTask;
        }

        public Task SendCriticalIncidentNotificationAsync(
            int incidentId,
            int userId)
        {
            Console.WriteLine(
                $"Critical incident notification for Incident {incidentId}.");

            return Task.CompletedTask;
        }

        public Task EscalateIncidentAsync(
            int incidentId,
            int escalatedBy,
            int escalatedTo,
            string reason)
        {
            Console.WriteLine(
                $"Incident {incidentId} escalated from {escalatedBy} to {escalatedTo}. Reason: {reason}");

            return Task.CompletedTask;
        }
    }
}