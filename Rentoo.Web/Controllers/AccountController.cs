using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
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
                            return RedirectToAction("Index", "Admin");
                        }
                        else if (_signInManager.IsSignedIn(User) && User.IsInRole("Teacher"))
                        {
                            return RedirectToAction("Index", "Owner");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Client");
                        }
                    }
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
//public async Task<IActionResult> Register(RegisterViewModel model)
// { if (ModelState.IsValid) { User user = new User() { FirstName = model.FirstName, LastName = model.LastName, Age = model.Age, Address = model.Address, PhoneNumber = model.PhoneNumber, UserName = model.UserName, Email = model.Email };
// IdentityResult result = await _userManager.CreateAsync(user, model.Password); 
//if (result.Succeeded) { await _userManager.AddToRoleAsync(user, model.Role);
// await _signInManager.SignInAsync(user, isPersistent: false);
//if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin")) 
//{ // return RedirectToAction("Index", "Admin"); //} 
//else if (_signInManager.IsSignedIn(User) && User.IsInRole("Owner")) 
//{ // return RedirectToAction("Index", "Owner"); //} //else 
//{ // return RedirectToAction("Index", "Client"); 
//} return View("Login"); } else { foreach (var error in result.Errors) { ModelState.AddModelError("", error.Description); } }