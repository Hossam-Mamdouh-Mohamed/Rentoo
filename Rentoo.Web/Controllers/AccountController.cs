using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Rentoo.Domain.Entities;
using web.ViewModels;

namespace Rentoo.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.RoleList = new SelectList(new[] { "Admin", "Client", "Owner" });
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ViewBag.RoleList = new SelectList(new[] { "Admin", "Client", "Owner" });

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingPhoneUser = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);
            if (existingPhoneUser != null)
            {
                TempData["ErrorMessage"] = "Phone number is already in use."; 
                return View(model);
            }


            // Check if username already exists
            var existingUsername = await _userManager.FindByNameAsync(model.UserName);
            if (existingUsername != null)
            {
                TempData["ErrorMessage"] = "Username is already taken.";
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

                // Check if the role exists
                if (!await _roleManager.RoleExistsAsync(model.Role))
                {
                    IdentityRole newRole = new IdentityRole(model.Role);
                    var roleResult = await _roleManager.CreateAsync(newRole);
                    if (!roleResult.Succeeded)
                    {
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        TempData["ErrorMessage"] = "Role creation failed. Please try again.";
                        return View(model);
                    }
                }

                IdentityResult result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    TempData["SuccessMessage"] = "User registered successfully. Please sign in to continue.";
                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                TempData["ErrorMessage"] = "Please try again later.";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred. Please try again later.";
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
                        if (_signInManager.IsSignedIn(User) && (User.IsInRole("Admin")|| User.IsInRole("SuperAdmin")))
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
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    TempData["ErrorMessage"] = "Error Singning in User.";
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
