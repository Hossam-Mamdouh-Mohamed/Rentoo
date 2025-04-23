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
            var currentUser = await _userService.GetByIdAsync(userId);
            return View(currentUser);
        }
        [HttpPost]
        public async Task<IActionResult> UserProfile(ProfileViewModel profileViewModel, IFormFile? ProfileImage)
        {
            try
            {
                var id = user.Id;
                // Get the existing user from the database
                var existingUser = await _userService.GetByIdAsync(id);
                if (existingUser == null)
                {
                    ModelState.AddModelError("", "User not found");
                    return View("Profile", user);
                }

                // Handle profile image upload
                if (ProfileImageFile != null && ProfileImageFile.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(ProfileImageFile.FileName)}";
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    Directory.CreateDirectory(uploadPath);
                    var filePath = Path.Combine(uploadPath, fileName);

                    // Save image
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfileImageFile.CopyToAsync(stream);
                    }

                    // Update the user image path
                    existingUser.UserImage = $"uploads/{fileName}";
                }

                // Update user properties
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;
                existingUser.BirthDate = user.BirthDate;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.Address = user.Address;

                // Update the user in the database
                await _userService.UpdateAsync(existingUser);

                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating your profile. Please try again.");
                return View("Profile", user);
            }

        }
    }
}
