using AIITSM.Application._02_M2_IncidentManagement;
using AIITSM.Application.Common;
using AIITSM.Web.Models;
using Microsoft.AspNetCore.Mvc;
using AIITSM.Application._06_M6_AI.Contracts;
using AIITSM.Application._06_M6_AI.Services;

namespace AIITSM.Web.Controllers._02_M2_IncidentManagement
{
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
