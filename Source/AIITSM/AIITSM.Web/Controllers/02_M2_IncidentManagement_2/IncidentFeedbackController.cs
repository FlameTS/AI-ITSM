using AIITSM.Application._02_M2_IncidentManagement_2.Feedback;
using AIITSM.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace AIITSM.Web.Controllers._02_M2_IncidentManagement_2
{
    public class IncidentFeedbackController : Controller
    {
        private readonly IIncidentFeedbackService _feedbackService;
        private readonly ICurrentUserService _currentUser;

        public IncidentFeedbackController(
            IIncidentFeedbackService feedbackService,
            ICurrentUserService currentUser)
        {
            _feedbackService = feedbackService;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            int incidentId,
            CancellationToken cancellationToken = default)
        {
            var feedback = await _feedbackService.GetFeedbackAsync(
                incidentId,
                _currentUser.UserId,
                cancellationToken);

            return PartialView("_Feedback", feedback);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(
            int incidentId,
            string? feedbackText,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _feedbackService.AddFeedbackAsync(
                    incidentId,
                    _currentUser.UserId,
                    feedbackText,
                    cancellationToken);

                TempData["FeedbackSuccess"] =
                    "Feedback submitted successfully.";
            }
            catch (UnauthorizedAccessException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                TempData["FeedbackError"] = ex.Message;
            }

            return RedirectToAction(
                "Details",
                "Incident",
                new { id = incidentId });
        }
    }
}