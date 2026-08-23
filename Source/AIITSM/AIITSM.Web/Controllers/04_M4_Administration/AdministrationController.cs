using AIITSM.Application._04_M4_Administration.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIITSM.Web.Controllers._04_M4_Administration;

[Authorize(Roles = "ITAdministrator")]
public class AdministrationController : Controller
{
    private readonly IUserAdministrationService _userAdministrationService;
    private readonly ICategoryAdministrationService _categoryAdministrationService;

    public AdministrationController(
        IUserAdministrationService userAdministrationService,
        ICategoryAdministrationService categoryAdministrationService)
    {
        _userAdministrationService = userAdministrationService;
        _categoryAdministrationService = categoryAdministrationService;
    }

    public async Task<IActionResult> Users()
    {
        var users = await _userAdministrationService.GetUsersAsync();

        return View(users);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetUserStatus(
        string userId,
        bool isActive)
    {
        var result = await _userAdministrationService
            .SetUserActiveStatusAsync(userId, isActive);

        if (!result)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignRole(
        string userId,
        string roleName)
    {
        var result = await _userAdministrationService
            .AssignRoleAsync(userId, roleName);

        if (!result)
        {
            return BadRequest();
        }

        return RedirectToAction(nameof(Users));
    }

    public async Task<IActionResult> Categories()
    {
        var categories = await _categoryAdministrationService
            .GetCategoriesAsync();

        return View(categories);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(string categoryName)
    {
        var result = await _categoryAdministrationService
            .CreateCategoryAsync(categoryName);

        if (!result)
        {
            return BadRequest();
        }

        return RedirectToAction(nameof(Categories));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCategory(
        int categoryId,
        string categoryName)
    {
        var result = await _categoryAdministrationService
            .UpdateCategoryAsync(categoryId, categoryName);

        if (!result)
        {
            return BadRequest();
        }

        return RedirectToAction(nameof(Categories));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int categoryId)
    {
        var result = await _categoryAdministrationService
            .DeleteCategoryAsync(categoryId);

        if (!result)
        {
            return BadRequest();
        }

        return RedirectToAction(nameof(Categories));
    }
}
