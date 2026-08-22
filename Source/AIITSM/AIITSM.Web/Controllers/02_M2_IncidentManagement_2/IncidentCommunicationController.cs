using AIITSM.Application._02_M2_IncidentManagement_2.Communication;
using AIITSM.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace AIITSM.Web.Controllers._02_M2_IncidentManagement_2
{
    public class IncidentCommunicationController : Controller
    {
        private readonly IIncidentCommentService _commentService;
        private readonly ICurrentUserService _currentUser;

        public IncidentCommunicationController(
            IIncidentCommentService commentService,
            ICurrentUserService currentUser)
        {
            _commentService = commentService;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<IActionResult> Comments(
            int incidentId,
            CancellationToken cancellationToken = default)
        {
            var comments = await _commentService.GetCommentsAsync(
                incidentId,
                cancellationToken);

            return Ok(comments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(
            int incidentId,
            string commentText,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(commentText))
            {
                TempData["CommentError"] = "Comment cannot be empty.";
                return RedirectToAction(
                    "Details",
                    "Incident",
                    new { id = incidentId });
            }

            try
            {
                await _commentService.AddCommentAsync(
                    incidentId,
                    _currentUser.UserId,
                    commentText,
                    cancellationToken);

                TempData["CommentSuccess"] = "Comment added successfully.";
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }

            return RedirectToAction(
                "Details",
                "Incident",
                new { id = incidentId });
        }
    }
}