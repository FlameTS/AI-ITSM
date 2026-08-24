using AIITSM.Application._07_M7_Automation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIITSM.Web.Controllers
{
    // Manual automation triggers (notifications, escalation) are for staff
    // who work incidents, not employees.
    [Authorize(Roles = "HelpDeskAgent,ITManager,ITAdministrator")]
    public class AutomationController : Controller
    {
        private readonly IAutomationService _automationService;

        public AutomationController(IAutomationService automationService)
        {
            _automationService = automationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendAssignmentNotification(
            int incidentId,
            int assignedTo)
        {
            await _automationService.SendAssignmentNotificationAsync(
                incidentId,
                assignedTo);

            return Ok("Assignment notification sent successfully.");
        }

        [HttpPost]
        public async Task<IActionResult> SendStatusChangeNotification(
            int incidentId,
            int userId,
            string newStatus)
        {
            await _automationService.SendStatusChangeNotificationAsync(
                incidentId,
                userId,
                newStatus);

            return Ok("Status change notification sent successfully.");
        }

        [HttpPost]
        public async Task<IActionResult> SendCriticalIncidentNotification(
            int incidentId,
            int userId)
        {
            await _automationService.SendCriticalIncidentNotificationAsync(
                incidentId,
                userId);

            return Ok("Critical incident notification sent successfully.");
        }

        [HttpPost]
        public async Task<IActionResult> EscalateIncident(
            int incidentId,
            int escalatedBy,
            int escalatedTo,
            string reason)
        {
            await _automationService.EscalateIncidentAsync(
                incidentId,
                escalatedBy,
                escalatedTo,
                reason);

            return Ok("Incident escalated successfully.");
        }
    }
}