using AITSM.Infrastructure._01_M1_IdentityAccess.Identity;
using AIITSM.Web.Models._01_M1_IdentityAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AIITSM.Web.Controllers._01_M1_IdentityAccess
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Find user using email
            var user = await _userManager.FindByEmailAsync(model.Email);

            // User does not exist
            if (user == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email or password.");

                return View(model);
            }

            // Check whether administrator has deactivated the account
            if (!user.IsActive)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Your account is inactive. Please contact the administrator.");

                return View(model);
            }

            // Check password and sign in
            var result = await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(
                string.Empty,
                "Invalid email or password.");

            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}