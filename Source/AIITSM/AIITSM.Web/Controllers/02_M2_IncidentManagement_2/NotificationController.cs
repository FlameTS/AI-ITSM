using AIITSM.Application._02_M2_IncidentManagement_2.Notifications;
using AIITSM.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace AIITSM.Web.Controllers._02_M2_IncidentManagement_2
{
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly ICurrentUserService _currentUser;

        public NotificationController(
            INotificationService notificationService,
            ICurrentUserService currentUser)
        {
            _notificationService = notificationService;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            CancellationToken cancellationToken = default)
        {
            var notifications =
                await _notificationService.GetMyNotificationsAsync(
                    _currentUser.UserId,
                    cancellationToken);

            return View(notifications);
        }

        [HttpGet]
        public async Task<IActionResult> UnreadCount(
            CancellationToken cancellationToken = default)
        {
            var count =
                await _notificationService.GetUnreadCountAsync(
                    _currentUser.UserId,
                    cancellationToken);

            return Ok(new { count });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(
            int id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _notificationService.MarkAsReadAsync(
                    id,
                    _currentUser.UserId,
                    cancellationToken);

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }
    }
}