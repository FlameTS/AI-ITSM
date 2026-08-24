using AIITSM.Application._06_M6_AI.Contracts;
using AIITSM.Application._06_M6_AI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIITSM.Web.Controllers._06_M6_AI
{
    // Dev/diagnostic endpoint — not part of any actor's workflow, not
    // linked from any menu. Locked to admins so it isn't wide open.
    [Authorize(Roles = "ITAdministrator")]
    public class AIAnalysisTestController : Controller
    {
        private readonly IAIAnalysisService _aiAnalysisService;

        public AIAnalysisTestController(IAIAnalysisService aiAnalysisService)
        {
            _aiAnalysisService = aiAnalysisService;
        }

        public async Task<IActionResult> CreateTestAnalysis()
        {
            var request = new AnalyzeIncidentRequest
            {
                IncidentId = 1,
                Title = "M6 Persistence Test",
                Description = "Testing AIAnalysis persistence from ASP.NET Core to SQL Server."
            };

            var analysisId = await _aiAnalysisService.RequestAnalysis(request);

            return Content($"AIAnalysis created successfully. ID = {analysisId}");
        }
    }
}