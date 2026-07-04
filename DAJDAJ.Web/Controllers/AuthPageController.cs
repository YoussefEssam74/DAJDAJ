using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DAJDAJ.Utilities;

namespace DAJDAJ.Web.Controllers
{
    [AllowAnonymous]
    [Route("[controller]/[action]")]
    public class AuthPageController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthPageController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Login(string returnUrl = null)
        {
            // If user is already authenticated, redirect to home or return URL
            if (User.Identity?.IsAuthenticated == true)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View("~/Views/Auth/Login.cshtml");
        }

        public IActionResult AdminLogin(string returnUrl = null)
        {
            // If user is already authenticated as admin, redirect to dashboard
            if (User.Identity?.IsAuthenticated == true && User.IsInRole(SD.AdminRole))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View("~/Views/Auth/AdminLogin.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminLogin(string email, string password, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return RedirectToAction("AdminLogin", new { error = "invalid" });
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return RedirectToAction("AdminLogin", new { error = "invalid" });
            }

            // Check if user is admin
            var isAdmin = await _userManager.IsInRoleAsync(user, SD.AdminRole);
            if (!isAdmin)
            {
                return RedirectToAction("AdminLogin", new { error = "invalid" });
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: true, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            return RedirectToAction("AdminLogin", new { error = "invalid" });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }
    }
}
