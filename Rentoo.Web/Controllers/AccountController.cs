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
            if (ModelState.IsValid)
            {
                
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                    return View(model);
                }
                if (user != null && !await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                    return View(model);
                }
                else
                {
                    var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                        {
                            TempData["SuccessMessage"] = "Sign in Successfully";
                            return RedirectToAction("Index", "Admin");
                        }
                        else if (_signInManager.IsSignedIn(User) && User.IsInRole("Owner"))
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
                    TempData["ErrorMessage"] = "Error Singning in User.";
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
