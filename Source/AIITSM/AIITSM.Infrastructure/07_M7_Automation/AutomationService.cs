using AIITSM.Application._07_M7_Automation;
using AIITSM.Domain._02_M2_IncidentManagement_2;
using AIITSM.Domain._07_M7_Automation;
using AIITSM.Infrastructure._06_M6_AI;

namespace AIITSM.Infrastructure._07_M7_Automation
{
    public class AutomationService : IAutomationService
    {
        private readonly AIITSMDbContext _context;

        public AutomationService(AIITSMDbContext context)
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
                CreatedAt = DateTime.UtcNow
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
                CreatedAt = DateTime.UtcNow
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
                CreatedAt = DateTime.UtcNow
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
                EscalatedAt = DateTime.UtcNow
            };

            _context.Escalations.Add(escalation);

            await _context.SaveChangesAsync();
        }
    }
}