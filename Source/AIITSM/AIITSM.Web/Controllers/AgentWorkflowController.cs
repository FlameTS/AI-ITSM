using AIITSM.Application._02_M2_IncidentManagement;
using AIITSM.Application._02_M2_IncidentManagement_2.Communication;
using AIITSM.Application._03_M3_AgentWorkflow;
using AIITSM.Application.Common;
using AIITSM.Domain._02_M2_IncidentManagement;
using AIITSM.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace AIITSM.Web.Controllers
{
    public class AgentWorkflowController : Controller
    {
        private readonly IIncidentService _incidentService;
        private readonly IIncidentCommentService _commentService;
        private readonly IIncidentAssignmentService _assignmentService;
        private readonly ICurrentUserService _currentUser;

        public AgentWorkflowController(
            IIncidentService incidentService,
            IIncidentCommentService commentService,
            IIncidentAssignmentService assignmentService,
            ICurrentUserService currentUser)
        {
            _incidentService = incidentService;
            _commentService = commentService;
            _assignmentService = assignmentService;
            _currentUser = currentUser;
        }

        public async Task<IActionResult> Index(int id = 1)
        {
            var incident = await _incidentService.GetIncidentDetailsAsync(id);

            if (incident is null)
            {
                return NotFound();
            }

            var comments = await _commentService.GetCommentsAsync(id);
            var assignedTo = await _assignmentService.GetAssignedAgentAsync(id);

            var model = new AgentWorkflowModel
            {
                IncidentId = incident.IncidentId,
                Title = incident.Title,
                Description = incident.Description,
                Priority = incident.Priority.ToString(),
                Status = ToDisplayStatus(incident.Status),
                AssignedTo = assignedTo,
                Comment = comments.LastOrDefault()?.CommentText
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
            int incidentId,
            string status,
            CancellationToken cancellationToken = default)
        {
            if (!TryParseStatus(status, out var parsedStatus))
            {
                TempData["Message"] = "Invalid incident status.";
                return RedirectToAction(nameof(Index), new { id = incidentId });
            }

            try
            {
                await _incidentService.UpdateStatusAsync(
                    incidentId,
                    parsedStatus,
                    cancellationToken);

                TempData["Message"] =
                    $"Incident #{incidentId} status updated to {ToDisplayStatus(parsedStatus)}.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Message"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { id = incidentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignAgent(
            int incidentId,
            int? assignedTo,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _assignmentService.AssignAgentAsync(
                    incidentId,
                    assignedTo,
                    cancellationToken);

                TempData["Message"] = assignedTo.HasValue
                    ? $"Incident #{incidentId} assigned to Agent #{assignedTo}."
                    : $"Agent assignment removed from Incident #{incidentId}.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Message"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { id = incidentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(
            int incidentId,
            string comment,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                TempData["Message"] = "Comment cannot be empty.";
                return RedirectToAction(nameof(Index), new { id = incidentId });
            }

            try
            {
                await _commentService.AddCommentAsync(
                    incidentId,
                    _currentUser.UserId,
                    comment,
                    cancellationToken);

                TempData["Message"] =
                    $"Comment added to Incident #{incidentId}.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Message"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { id = incidentId });
        }

        private static bool TryParseStatus(string value, out IncidentStatus status)
        {
            status = value?.Trim() switch
            {
                "Open" => IncidentStatus.Open,
                "In Progress" => IncidentStatus.InProgress,
                "Resolved" => IncidentStatus.Resolved,
                "Closed" => IncidentStatus.Closed,
                _ => default
            };

            return value?.Trim() is "Open" or "In Progress" or "Resolved" or "Closed";
        }

        private static string ToDisplayStatus(IncidentStatus status) => status switch
        {
            IncidentStatus.InProgress => "In Progress",
            _ => status.ToString()
        };
    }
}