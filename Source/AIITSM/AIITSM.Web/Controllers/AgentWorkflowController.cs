using AIITSM.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace AIITSM.Web.Controllers
{
    public class AgentWorkflowController : Controller
    {
        private static AgentWorkflowModel incident = new AgentWorkflowModel
        {
            IncidentId = 1001,
            Title = "Network connectivity issue",
            Description = "User is unable to access the internal network.",
            Priority = "High",
            Status = "Open",
            AssignedTo = null,
            Comment = "Incident received and waiting for agent assignment."
        };

        public IActionResult Index()
        {
            return View(incident);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int incidentId, string status)
        {
            if (incident.IncidentId == incidentId)
            {
                incident.Status = status;

                TempData["Message"] =
                    $"Incident #{incidentId} status updated to {status}.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignAgent(int incidentId, int? assignedTo)
        {
            if (incident.IncidentId == incidentId)
            {
                incident.AssignedTo = assignedTo;

                TempData["Message"] = assignedTo.HasValue
                    ? $"Incident #{incidentId} assigned to Agent #{assignedTo}."
                    : $"Agent assignment removed from Incident #{incidentId}.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(int incidentId, string comment)
        {
            if (incident.IncidentId == incidentId)
            {
                if (!string.IsNullOrWhiteSpace(comment))
                {
                    incident.Comment = comment.Trim();

                    TempData["Message"] =
                        $"Comment added to Incident #{incidentId}.";
                }
                else
                {
                    TempData["Message"] =
                        "Comment cannot be empty.";
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}