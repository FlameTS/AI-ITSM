using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIITSM.Web.Controllers._01_M1_IdentityAccess
{
    [Authorize(Roles = "ITAdministrator")]
    public class AdminTestController : Controller
    {
        public IActionResult Index()
        {
            return Content("You are authorized as IT Administrator.");
        }
    }
}