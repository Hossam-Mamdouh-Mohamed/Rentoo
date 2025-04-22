using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rentoo.Application.Interfaces;
using Rentoo.Domain.Entities;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rentoo.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IService<User> service;
        private readonly IService<Car> carService;
        private readonly UserManager<User> userManager;
        public AdminController(IService<User> _service, IService<Car> carService,UserManager<User> _userManager)
        {
            service = _service;
            this.carService = carService;
            userManager=_userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Users()
        {
           var users = service.GetAllAsync().Result;
            return View();
        }
        public async Task<IActionResult> Cars()
        {
            var cars = carService.GetAllAsync().Result;
            return View();
        }
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userData = await service.GetByIdAsync(userId);
            return View(userData);
        }
        public async Task<IActionResult> Clients()
        {
            var Users = await userManager.GetUsersInRoleAsync("Client");
                return View("Clients",Users);
        }
        public async Task<IActionResult> Owners()
        {
            var Users = await userManager.GetUsersInRoleAsync("Owner");
            return View("Owners", Users);
          
        }
        public async Task<IActionResult> UserProfile(User user, IFormFile? ProfileImageFile)
        {
            try
            {
                var id=user.Id;
                // Get the existing user from the database
                var existingUser = await service.GetByIdAsync(id);
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
                await service.UpdateAsync(existingUser);
                
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

