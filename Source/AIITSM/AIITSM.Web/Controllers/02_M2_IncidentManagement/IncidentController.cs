using AIITSM.Application._02_M2_IncidentManagement;
using AIITSM.Application.Common;
using AIITSM.Web.Models;
using Microsoft.AspNetCore.Mvc;
using AIITSM.Application._06_M6_AI.Contracts;
using AIITSM.Application._06_M6_AI.Services;
using Microsoft.AspNetCore.Authorization;

namespace AIITSM.Web.Controllers._02_M2_IncidentManagement
{
    // Any authenticated user can hit this controller (Details is used by
    // Employees viewing their own incident, and by Agents/Managers/Admins
    // arriving from Notifications, the Agent Queue, or Reports).
    // Index/Create are further locked to Employees below, since they use
    // GetMyIncidentsAsync — the "my incidents" view only makes sense for
    // the employee who filed them.
    [Authorize]
    public class IncidentController : Controller
    {
        private readonly IIncidentService _incidentService;
        private readonly ICurrentUserService _currentUser;
        private readonly IAIAnalysisService _aiAnalysisService;

        public IncidentController(
            IIncidentService incidentService, 
            ICurrentUserService currentUser,
            IAIAnalysisService aiAnalysisService)
        {
            _incidentService = incidentService;
            _currentUser = currentUser;
            _aiAnalysisService = aiAnalysisService;
        }

        // GET: /Incident  (My Incidents — only the logged-in employee's own incidents)
        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Index()
        {
            var incidents = await _incidentService.GetMyIncidentsAsync(_currentUser.UserId);
            return View(incidents);
        }

        // GET: /Incident/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var incident = await _incidentService.GetIncidentDetailsAsync(id);

            if (incident is null)
            {
                return NotFound();
            }

            return View(incident);
        }

        // GET: /Incident/Create
        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateIncidentViewModel
            {
                Categories = await _incidentService.GetCategoriesAsync()
            };

            return View(viewModel);
        }

        // POST: /Incident/Create
        [HttpPost]
        [Authorize(Roles = "Employee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateIncidentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _incidentService.GetCategoriesAsync();
                return View(model);
            }

            var request = new CreateIncidentRequest
            {
                Title = model.Title,
                Description = model.Description,
                CategoryId = model.CategoryId,
                Priority = model.Priority
            };

            // Status is forced to Open and creator to the logged-in
            // employee inside IncidentService — never trust the form for those.
            var incidentId = await _incidentService.CreateIncidentAsync(
                request,
                _currentUser.UserId);

            try
            {
                await _aiAnalysisService.RequestAnalysis(
                    new AnalyzeIncidentRequest
                    {
                        IncidentId = incidentId,
                        Title = request.Title,
                        Description = request.Description
                    });
            }
            catch
            {
                // The incident has already been successfully saved.
                // AI failure must not invalidate the incident.
                TempData["Message"] =
                    $"Incident #{incidentId} created successfully, " +
                    "but AI analysis could not be completed.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = incidentId });
            }

            TempData["Message"] =
                $"Incident #{incidentId} created successfully " +
                "and AI analysis completed.";

            return RedirectToAction(
                nameof(Details),
                new { id = incidentId });
        }
    }
}
