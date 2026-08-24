using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIITSM.Web.Controllers._05_M5_Reporting
{
    // MVC front-end for the JSON-only ReportingController (/api/reporting/*).
    // ReportingController itself is an [ApiController] and can never return
    // a Razor view, which is why ITManager previously had no page to land
    // on at all. This controller just serves the shell page; the view
    // pulls the actual numbers client-side from the existing API routes.
    [Authorize(Roles = "ITManager,ITAdministrator")]
    public class ReportsController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
