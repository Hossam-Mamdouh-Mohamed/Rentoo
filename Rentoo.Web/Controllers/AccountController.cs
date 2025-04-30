using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Rentoo.Domain.Entities;
using web.ViewModels;

namespace Rentoo.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User user = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                BirthDate = model.BirthDate,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                UserName = model.UserName,
                Email = model.Email
            };
            try
            {
                var hasUpper = model.Password.Any(char.IsUpper);
                var hasLower = model.Password.Any(char.IsLower);
                var hasDigit = model.Password.Any(char.IsDigit);
                var hasSymbol = model.Password.Any(ch => !char.IsLetterOrDigit(ch));
                var hasMinLength = model.Password.Length >= 8;

                if (!(hasUpper && hasLower && hasDigit && hasSymbol && hasMinLength))
                {
                    TempData["ErrorMessage"] = "Password must be at least 8 characters and include uppercase, lowercase, number, and special character.";
                    return View("Register", model);
                }
                IdentityResult result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    TempData["SuccessMessage"] = "User Registered Successfully,Please Sign in to continue";

                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);

                }
                TempData["ErrorMessage"] = "Plese Try Again Later";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Plese Try Again Later";
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                return View(model);
            }

            // Get roles
            var roles = await _userManager.GetRolesAsync(user);

            // Create custom claims
            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
             new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.GivenName, user.FirstName ?? ""),
        new Claim(ClaimTypes.Surname, user.LastName ?? ""),
        new Claim("UserImage", user.UserImage ?? "/images/default-profile.png")
    };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Sign in with Identity's application scheme
            var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
            var principal = new ClaimsPrincipal(identity);

            await _signInManager.SignOutAsync(); // Clear previous sessions
            await _signInManager.Context.SignInAsync(IdentityConstants.ApplicationScheme, principal);

            // Redirect based on role
            if (roles.Contains("SuperAdmin") || roles.Contains("Admin"))
            {
                TempData["SuccessMessage"] = "Sign in Successfully";
                return RedirectToAction("Index", "Admin");
            }
            else if (roles.Contains("Owner"))
            {
                TempData["SuccessMessage"] = "Sign in Successfully";
                return RedirectToAction("UserProfile", "UserDashboard");
            }
            else
            {
                TempData["SuccessMessage"] = "Sign in Successfully";
                return RedirectToAction("Index", "Client");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
