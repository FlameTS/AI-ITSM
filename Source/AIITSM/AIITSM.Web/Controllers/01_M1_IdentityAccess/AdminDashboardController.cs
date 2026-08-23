using AITSM.Infrastructure._01_M1_IdentityAccess.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AIITSM.Web.Controllers._01_M1_IdentityAccess
{
    [Authorize(Roles = "ITAdministrator")]
    public class AdminDashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminDashboardController(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();

            ViewBag.TotalUsers = users.Count;
            ViewBag.ActiveUsers = users.Count(u => u.IsActive);
            ViewBag.InactiveUsers = users.Count(u => !u.IsActive);

            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            var agents = await _userManager.GetUsersInRoleAsync("HelpDeskAgent");
            var managers = await _userManager.GetUsersInRoleAsync("ITManager");

            ViewBag.EmployeeCount = employees.Count;
            ViewBag.AgentCount = agents.Count;
            ViewBag.ManagerCount = managers.Count;

            return View();
        }
    }
}