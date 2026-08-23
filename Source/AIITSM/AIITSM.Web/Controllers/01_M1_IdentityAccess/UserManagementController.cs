using AITSM.Infrastructure._01_M1_IdentityAccess.Identity;
using AIITSM.Web.Models._01_M1_IdentityAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AIITSM.Web.Controllers._01_M1_IdentityAccess
{
    [Authorize(Roles = "ITAdministrator")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public UserManagementController(
         UserManager<ApplicationUser> userManager,
         RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();

            var model = new List<UserListViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                model.Add(new UserListViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? string.Empty,
                    Role = roles.FirstOrDefault() ?? "No Role",
                    IsActive = user.IsActive
                });
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Roles = _roleManager.Roles
                .Select(r => r.Name)
                .ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = _roleManager.Roles
                    .Select(r => r.Name)
                    .ToList();

                return View(model);
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "A user with this email already exists.");

                ViewBag.Roles = _roleManager.Roles
                    .Select(r => r.Name)
                    .ToList();

                return View(model);
            }

            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                ModelState.AddModelError("Role", "Invalid role selected.");

                ViewBag.Roles = _roleManager.Roles
                    .Select(r => r.Name)
                    .ToList();

                return View(model);
            }

            var user = new ApplicationUser
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email,
                IsActive = true,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);

                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            ViewBag.Roles = _roleManager.Roles
                .Select(r => r.Name)
                .ToList();

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? string.Empty,
                IsActive = user.IsActive
            };

            ViewBag.Roles = _roleManager.Roles
                .Select(r => r.Name)
                .ToList();

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = _roleManager.Roles
                    .Select(r => r.Name)
                    .ToList();

                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return NotFound();
            }

            // Check whether another user already has this email
            var userWithSameEmail = await _userManager.FindByEmailAsync(model.Email);

            if (userWithSameEmail != null && userWithSameEmail.Id != user.Id)
            {
                ModelState.AddModelError(
                    "Email",
                    "A user with this email already exists.");

                ViewBag.Roles = _roleManager.Roles
                    .Select(r => r.Name)
                    .ToList();

                return View(model);
            }

            // Make sure selected role is valid
            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                ModelState.AddModelError(
                    "Role",
                    "Invalid role selected.");

                ViewBag.Roles = _roleManager.Roles
                    .Select(r => r.Name)
                    .ToList();

                return View(model);
            }

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.IsActive = model.IsActive;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                ViewBag.Roles = _roleManager.Roles
                    .Select(r => r.Name)
                    .ToList();

                return View(model);
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (!currentRoles.Contains(model.Role))
            {
                if (currentRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(
                        user,
                        currentRoles);
                }

                await _userManager.AddToRoleAsync(
                    user,
                    model.Role);
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var model = new ResetPasswordViewModel
            {
                UserId = user.Id,
                Email = user.Email ?? string.Empty
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(
            ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                return NotFound();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(
                user,
                token,
                model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(model);
            }

            TempData["SuccessMessage"] =
                "Password reset successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}