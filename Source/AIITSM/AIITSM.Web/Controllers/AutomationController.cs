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

        private bool HasValidWebhookSecret()
        {
            var expected = Environment.GetEnvironmentVariable("N8N_WEBHOOK_SECRET");
            var provided = Request.Headers["X-AIITSM-Webhook-Secret"].ToString();
            return !string.IsNullOrEmpty(expected) && provided == expected;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SendAssignmentNotification(
            int incidentId,
            int assignedTo)
        {
            if (!HasValidWebhookSecret()) return Unauthorized();
            await _automationService.SendAssignmentNotificationAsync(
                incidentId,
                assignedTo);

            return Ok("Assignment notification sent successfully.");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SendStatusChangeNotification(
            int incidentId,
            int userId,
            string newStatus)
        {
            if (!HasValidWebhookSecret()) return Unauthorized();
            await _automationService.SendStatusChangeNotificationAsync(
                incidentId,
                userId,
                newStatus);

            return Ok("Status change notification sent successfully.");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SendCriticalIncidentNotification(
            int incidentId,
            int userId)
        {
            if (!HasValidWebhookSecret()) return Unauthorized();
            await _automationService.SendCriticalIncidentNotificationAsync(
                incidentId,
                userId);

            return Ok("Critical incident notification sent successfully.");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> EscalateIncident(
            int incidentId,
            int escalatedBy,
            int escalatedTo,
            string reason)
        {
            if (!HasValidWebhookSecret()) return Unauthorized();
            await _automationService.EscalateIncidentAsync(
                incidentId,
                escalatedBy,
                escalatedTo,
                reason);

            return Ok("Incident escalated successfully.");
        }
    }
}