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
        private readonly IService<Request> RentalService;
        public AdminController(IService<User> _service, IService<Car> carService, UserManager<User> _userManager)
        {
            service = _service;
            this.carService = carService;
            userManager = _userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Cars(int page = 1)
        {
            try
            {
                var allCars = await carService.GetAllAsync();
                var pagedCars = allCars.ToPagedList(page, 10);

                return View(pagedCars);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while fetching cars.";
                return RedirectToAction("Index");
            }

        }
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userData = await service.GetByIdAsync(userId);
            return View(userData);
        }
        public async Task<IActionResult> Clients(int page = 1)
        {
            try
            {
                var users = await userManager.GetUsersInRoleAsync("Client");
                var pagedUsers = users.ToPagedList(page, 10);
                return View("Clients", pagedUsers);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while fetching clients.";
                return RedirectToAction("Index");
            }
        }
        public async Task<IActionResult> Owners(int page = 1)
        {
            try
            {
                var users = await userManager.GetUsersInRoleAsync("Owner");
                var pagedUsers = users.ToPagedList(page, 10);
                return View("Owners", pagedUsers);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while fetching owners.";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await userManager.GetRolesAsync(user);
            var isClient = roles.Contains("Client");
            var isOwner = roles.Contains("Owner");

            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting user.";
            }

            // Redirect based on role
            if (isClient)
            {
                return RedirectToAction("Clients");
            }
            else if (isOwner)
            {
                return RedirectToAction("Owners");
            }

            return RedirectToAction("Index"); // fallback
        }
        public async Task<IActionResult> UserProfile(User user, IFormFile? ProfileImageFile)
        {
            try
            {
                var id = user.Id;
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
        public async Task<IActionResult> Rentals(int page = 1)
        {
            const int PageSize = 10;
            try
            {
                var acceptedRequests = await RentalService.GetAllAsync(x => x.Status == RequestStatus.Accepted);
                var paginatedRequests = acceptedRequests
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
                return View(paginatedRequests);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while fetching accepted rental requests.";
                return RedirectToAction("Index");
            }
        }
        public async Task<IActionResult> PendingApprovements(int page = 1)
        {
            const int PageSize = 10;
            try
            {
                var acceptedRequests = await RentalService.GetAllAsync(x => x.Status == RequestStatus.Accepted);
                var paginatedRequests = acceptedRequests
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
                return View(paginatedRequests);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while fetching accepted rental requests.";
                return RedirectToAction("Index");
            }
        }
    }
}

