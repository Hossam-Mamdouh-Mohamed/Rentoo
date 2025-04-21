using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Rentoo.Application.Interfaces;
using Rentoo.Application.Services;
using Rentoo.Domain.Entities;
using Rentoo.Infrastructure.Data;

namespace Rentoo.Web.Controllers
{
    public class UserDashboardController : Controller
    {
        //private readonly string  stringuserId= "ff237ee5-a469-4677-86ab-0d3c06c311a6";
        IService<User> _userService;
        public UserDashboardController(IService<User> _userService)
        {
            this._userService = _userService;


        }
        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            var currentUser = await _userService.GetByIdAsync(userId);
            return View(currentUser);
        }
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> UserProfile(User user, IFormFile? ProfileImageFile)
        {
            if (ProfileImageFile != null && ProfileImageFile.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(ProfileImageFile.FileName)}";

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                Directory.CreateDirectory(uploadPath); 

                var filePath = Path.Combine(uploadPath, fileName);

                // Save image
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfileImageFile.CopyToAsync(stream);
                }
                user.userimage = $"uploads/{fileName}";
            }

            if (ModelState.IsValid)
            {
                await _userService.UpdateAsync(user);
                return RedirectToAction("UserProfile");
            }

            return View(user);
        }

    }
}
