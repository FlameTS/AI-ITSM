using AIITSM.Application._02_M2_IncidentManagement;
using AIITSM.Application._02_M2_IncidentManagement_2.Attachments;
using AIITSM.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIITSM.Web.Controllers._02_M2_IncidentManagement_2
{
    // OwnsIncidentAsync() only checks the current employee's own incidents,
    // so this stays Employee-only until agents/managers get their own
    // attachment-viewing path.
    [Authorize(Roles = "Employee")]
    public class IncidentAttachmentController : Controller
    {
        private readonly IIncidentAttachmentService _attachmentService;
        private readonly IIncidentService _incidentService;
        private readonly ICurrentUserService _currentUser;
        private readonly IWebHostEnvironment _environment;

        private static readonly string[] AllowedExtensions =
        {
            ".pdf",
            ".png",
            ".jpg",
            ".jpeg",
            ".doc",
            ".docx",
            ".txt"
        };

        private const long MaxFileSize = 10 * 1024 * 1024;

        public IncidentAttachmentController(
            IIncidentAttachmentService attachmentService,
            IIncidentService incidentService,
            ICurrentUserService currentUser,
            IWebHostEnvironment environment)
        {
            _attachmentService = attachmentService;
            _incidentService = incidentService;
            _currentUser = currentUser;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> List(
            int incidentId,
            CancellationToken cancellationToken = default)
        {
            if (!await OwnsIncidentAsync(incidentId, cancellationToken))
            {
                return NotFound();
            }

            var attachments =
                await _attachmentService.GetAttachmentsAsync(
                    incidentId,
                    cancellationToken);

            return PartialView("_Attachments", attachments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(MaxFileSize)]
        public async Task<IActionResult> Upload(
            int incidentId,
            IFormFile? file,
            CancellationToken cancellationToken = default)
        {
            if (!await OwnsIncidentAsync(incidentId, cancellationToken))
            {
                return NotFound();
            }

            if (file == null || file.Length == 0)
            {
                TempData["AttachmentError"] = "Please select a file.";
                return RedirectToAction(
                    "Details",
                    "Incident",
                    new { id = incidentId });
            }

            if (file.Length > MaxFileSize)
            {
                TempData["AttachmentError"] =
                    "The selected file is larger than 10 MB.";

                return RedirectToAction(
                    "Details",
                    "Incident",
                    new { id = incidentId });
            }

            var extension =
                Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
            {
                TempData["AttachmentError"] =
                    "This file type is not supported.";

                return RedirectToAction(
                    "Details",
                    "Incident",
                    new { id = incidentId });
            }

            var originalFileName =
                Path.GetFileName(file.FileName);

            var storedFileName =
                $"{Guid.NewGuid():N}{extension}";

            var uploadDirectory =
                Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "incidents");

            Directory.CreateDirectory(uploadDirectory);

            var filePath =
                Path.Combine(uploadDirectory, storedFileName);

            try
            {
                await using (var stream = new FileStream(
                    filePath,
                    FileMode.CreateNew))
                {
                    await file.CopyToAsync(
                        stream,
                        cancellationToken);
                }

                await _attachmentService.AddAttachmentAsync(
                    incidentId,
                    originalFileName,
                    storedFileName,
                    file.ContentType ?? "application/octet-stream",
                    file.Length,
                    cancellationToken);

                TempData["AttachmentMessage"] =
                    "Attachment uploaded successfully.";
            }
            catch
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                TempData["AttachmentError"] =
                    "The attachment could not be uploaded.";
            }

            return RedirectToAction(
                "Details",
                "Incident",
                new { id = incidentId });
        }

        private async Task<bool> OwnsIncidentAsync(
            int incidentId,
            CancellationToken cancellationToken)
        {
            var incidents =
                await _incidentService.GetMyIncidentsAsync(
                    _currentUser.UserId,
                    cancellationToken);

            return incidents.Any(
                x => x.IncidentId == incidentId);
        }
    }
}