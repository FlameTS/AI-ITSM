using AIITSM.Application._04_M4_Administration.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIITSM.Web.Controllers._04_M4_Administration;

[Authorize(Roles = "ITAdministrator")]
public class AdministrationController : Controller
{
    private readonly IUserAdministrationService _userAdministrationService;

    public AdministrationController(
        IUserAdministrationService userAdministrationService)
    {
        _userAdministrationService = userAdministrationService;
    }

    public async Task<IActionResult> Users()
    {
        var users = await _userAdministrationService.GetUsersAsync();

        return View(users);
    }
}
