using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using web.ViewModels;

namespace Rentoo.Web.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
               IdentityRole role = new IdentityRole() { Name=model.RoleName };
                IdentityResult result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
    }
}
