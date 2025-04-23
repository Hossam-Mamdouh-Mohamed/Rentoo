using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rentoo.Application.Interfaces;
using Rentoo.Domain.Entities;
using System.Security.Claims;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;
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
        public async Task<IActionResult> Cars(int page = 1)
        {
            // Fix for the problematic code
            var allCars = await carService.GetAllAsync();
           // var carsWithOwners = allCars.AsQueryable().Include(c => c.User); // Convert to IQueryable before using Include
            var pagedCars = allCars.ToPagedList(page, 10); // 10 cars per page

            return View(pagedCars);
        }

        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userData = await service.GetByIdAsync(userId);
            return View(userData);
        }
        public async Task<IActionResult> Clients(int page = 1)
        {
            var users = await userManager.GetUsersInRoleAsync("Client");
            var pagedUsers = users.ToPagedList(page, 10);
            return View("Clients", pagedUsers);
        }
        public async Task<IActionResult> Owners(int page = 1)
        {
            var users = await userManager.GetUsersInRoleAsync("Owner");
            var pagedUsers = users.ToPagedList(page, 10);
            return View("Owners", pagedUsers);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting user.";
            }

            return RedirectToAction("Clients");
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

