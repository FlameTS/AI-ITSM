using AIITSM.Application._07_M7_Automation;
using AIITSM.Infrastructure.Data;

namespace AIITSM.Infrastructure._07_M7_Automation
{
    public class AutomationService : IAutomationService
    {
        private readonly ITServiceDeskContext _context;

        public AutomationService(ITServiceDeskContext context)
        {
            _context = context;
        }

        public async Task SendAssignmentNotificationAsync(
            int incidentId,
            int assignedTo)
        {
            var notification = new Notification
            {
                UserId = assignedTo,
                IncidentId = incidentId,
                Message = $"Incident {incidentId} has been assigned to you.",
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();
        }

        public async Task SendStatusChangeNotificationAsync(
            int incidentId,
            int userId,
            string newStatus)
        {
            var notification = new Notification
            {
                UserId = userId,
                IncidentId = incidentId,
                Message = $"Incident {incidentId} status changed to {newStatus}.",
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();
        }

        public async Task SendCriticalIncidentNotificationAsync(
            int incidentId,
            int userId)
        {
            var notification = new Notification
            {
                UserId = userId,
                IncidentId = incidentId,
                Message = $"Critical incident alert: Incident {incidentId} requires immediate attention.",
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();
        }

        public async Task EscalateIncidentAsync(
            int incidentId,
            int escalatedBy,
            int escalatedTo,
            string reason)
        {
            var escalation = new Escalation
            {
                IncidentId = incidentId,
                EscalatedBy = escalatedBy,
                EscalatedTo = escalatedTo,
                Reason = reason,
                EscalatedAt = DateTime.Now
            };

            _context.Escalations.Add(escalation);

            await _context.SaveChangesAsync();
        }
    }
}