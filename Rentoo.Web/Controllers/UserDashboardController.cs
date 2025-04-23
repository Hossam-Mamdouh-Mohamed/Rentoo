using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Rentoo.Application.Interfaces;
using Rentoo.Domain.Entities;
using Rentoo.Web.ViewModels;

namespace Rentoo.Web.Controllers
{
    public class UserDashboardController : Controller
    {
        private readonly IService<User> _userService;
        private readonly IService<Car> _carService;

        public UserDashboardController(IService<User> userService, IService<Car> carService)
        {
            _userService = userService;
            _carService = carService;
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.GetByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UserProfile(ProfileViewModel profileViewModel, IFormFile? ProfileImage)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
                return NotFound();

            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                var uploadPath = Path.Combine("wwwroot", "uploads");
                var fileName = ProfileImage.FileName;
                var filePath = Path.Combine(uploadPath, fileName);
                await ProfileImage.CopyToAsync(new FileStream(filePath, FileMode.Create));
                user.UserImage = "uploads/" + fileName;
            }

            if (ModelState.IsValid)
            {
                user.FirstName = profileViewModel.FirstName;
                user.LastName = profileViewModel.LastName;
                user.Email = profileViewModel.Email;
                user.PhoneNumber = profileViewModel.PhoneNumber;
                user.Address = profileViewModel.Address;
                user.BirthDate = profileViewModel.BirthDate;
                await _userService.UpdateAsync(user);
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("UserProfile");
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult MyCars()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cars = _carService.GetByIdAsync(userId);
            return View(cars);
        }
    }
}
