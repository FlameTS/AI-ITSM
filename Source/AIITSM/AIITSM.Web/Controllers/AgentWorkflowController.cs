using AIITSM.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace AIITSM.Web.Controllers
{
    public class AgentWorkflowController : Controller
    {
        public IActionResult Index()
        {
            var incident = new AgentWorkflowModel
            {
                IncidentId = 1001,
                Title = "Network connectivity issue",
                Description = "User is unable to access the internal network.",
                Priority = "High",
                Status = "Open",
                AssignedTo = null,
                Comment = "Incident received and waiting for agent assignment."
            };

            return View(incident);
        }

        [HttpPost]
        public IActionResult UpdateStatus(
            int incidentId,
            string status)
        {
            TempData["Message"] =
                $"Incident #{incidentId} status updated to {status}.";

            return RedirectToAction("Index");
        }
    }
}