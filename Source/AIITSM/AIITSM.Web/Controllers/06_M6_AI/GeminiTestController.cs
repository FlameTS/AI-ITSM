using AIITSM.Application._06_M6_AI.Providers;
using Microsoft.AspNetCore.Mvc;

namespace AIITSM.Web.Controllers._06_M6_AI
{
    public class GeminiTestController : Controller
    {
        private readonly IAIProvider _aiProvider;

        public GeminiTestController(IAIProvider aiProvider)
        {
            _aiProvider = aiProvider;
        }

        public async Task<IActionResult> Analyze()
        {
            var request = new AIProviderRequest
            {
                IncidentId = 999,
                Title = "VPN connection failure",
                Description =
                    "An employee is unable to connect to the company VPN from their laptop. " +
                    "The VPN client shows a connection timeout."
            };

            var result = await _aiProvider.AnalyzeIncidentAsync(request);

            return Json(result);
        }
    }
}