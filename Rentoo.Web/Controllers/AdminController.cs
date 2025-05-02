using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Rentoo.Application.Interfaces;
using Rentoo.Domain.Entities;
using Rentoo.Infrastructure.Data;
using Rentoo.Web.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;
using web.ViewModels;
using X.PagedList;
using X.PagedList.Extensions;
namespace Rentoo.Web.Controllers
{
    public class AdminController : Controller
    {

        private readonly IService<User> service;
        private readonly IService<Car> carService;
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> userManager;
        private readonly IService<Request> RentalService;
        private readonly IService<Car> _carService;
        private readonly IService<CarDocument> DocumentService;
        private IService<UserDocument> UserDocumentService;
        private readonly RentooDbContext rentooDbContext;
        public AdminController(RentooDbContext rentooDbContext,IService<UserDocument> userdoc,IService<CarDocument> _document, IService<User> _service, IService<Request> Rental, IService<Car> carService, Microsoft.AspNetCore.Identity.UserManager<User> _userManager, IService<Car> carservice)
        {
            service = _service;
            this.carService = carService;
            userManager = _userManager;
            _carService = carservice;
            RentalService = Rental;
            DocumentService = _document;
            UserDocumentService = userdoc;
            this.rentooDbContext=rentooDbContext;
        }

        public IActionResult Index()
        {
            AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel();
            adminDashboardViewModel.OwnersCount = userManager.GetUsersInRoleAsync("Owner").Result?.Count() ?? 0;
            adminDashboardViewModel.RentersCount = userManager.GetUsersInRoleAsync("Client").Result?.Count() ?? 0;
            adminDashboardViewModel.CarsCount = carService.GetAllAsync().Result?.Count() ?? 0;
            adminDashboardViewModel.RentalsCount = RentalService.GetAllAsync().Result?.Count() ?? 0;
            adminDashboardViewModel.RentalsInProgressCount = RentalService.GetAllAsync(x => x.Status == RequestStatus.Accepted).Result?.Count() ?? 0;
            adminDashboardViewModel.RentalsCompletedCount = RentalService.GetAllAsync(x => x.Status == RequestStatus.Completed).Result?.Count() ?? 0;
            adminDashboardViewModel.RentalsCancelledCount = RentalService.GetAllAsync(x => x.Status == RequestStatus.Rejected).Result?.Count() ?? 0;
            adminDashboardViewModel.RentalsPendingCount = RentalService.GetAllAsync(x => x.Status == RequestStatus.Pending).Result?.Count() ?? 0;
            return View(adminDashboardViewModel);
        }

        public async Task<IActionResult> Cars(int page = 1)
        {
            try
            {
                var allCars = await carService.GetAllAsync(x => x.CarDocument.status == DocumentStatus.Accepted, ["User"]);
                var pagedCars = allCars.ToPagedList(page, 8);

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
                var owners = await userManager.GetUsersInRoleAsync("Owner");

                var userIds = owners.Select(u => u.Id).ToList();

                // Assuming you have access to the DbContext
                var usersWithCars = await rentooDbContext.Users
                    .Where(u => userIds.Contains(u.Id))
                    .Include(u => u.Cars)
                    .ToListAsync();

                var pagedUsers = usersWithCars.ToPagedList(page, 10);

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
            var isAdmin = roles.Contains("Admin");
            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                if (isClient)
                {
                    TempData["SuccessMessage"] = "Client deleted successfully.";
                    return RedirectToAction("Clients");
                }
                else if (isOwner)
                {
                    TempData["SuccessMessage"] = "Owner deleted successfully.";
                    return RedirectToAction("Owners");
                }
                else if (isAdmin)
                {
                    TempData["SuccessMessage"] = "Admin deleted successfully.";
                    return RedirectToAction("SystemAdmins");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting user.";
            }

            return RedirectToAction("Index"); // fallback
        }

        [HttpPost]
        public async Task<IActionResult> UserProfile(User user, IFormFile? ProfileImageFile, string? NewPassword, string? ConfirmPassword)
        {
            try
            {
                var existingUser = await service.GetByIdAsync(user.Id);
                if (existingUser == null)
                {
                    ModelState.AddModelError("", "User not found.");
                    return View("Profile", user);
                }

                // Handle profile image upload
                if (ProfileImageFile != null && ProfileImageFile.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(ProfileImageFile.FileName)}";
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    Directory.CreateDirectory(uploadPath);
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfileImageFile.CopyToAsync(stream);
                    }

                    existingUser.UserImage = $"uploads/{fileName}";
                }


                if (!string.IsNullOrWhiteSpace(NewPassword))
                {
                    // 1. Validate match
                    if (NewPassword != ConfirmPassword)
                    {
                        TempData["ErrorMessage"] = "Passwords do not match.";
                        return View("Profile", user);
                    }
                    var hasUpper = NewPassword.Any(char.IsUpper);
                    var hasLower = NewPassword.Any(char.IsLower);
                    var hasDigit = NewPassword.Any(char.IsDigit);
                    var hasSymbol = NewPassword.Any(ch => !char.IsLetterOrDigit(ch));
                    var hasMinLength = NewPassword.Length >= 8;

                    if (!(hasUpper && hasLower && hasDigit && hasSymbol && hasMinLength))
                    {
                        TempData["ErrorMessage"] = "Password must be at least 8 characters and include uppercase, lowercase, number, and special character.";
                        return View("Profile", user);
                    }
                    var removePasswordResult = await userManager.RemovePasswordAsync(existingUser);
                    if (!removePasswordResult.Succeeded)
                    {
                        TempData["ErrorMessage"] = "Failed to remove the old password.";
                        return View("Profile", user);
                    }

                    var addPasswordResult = await userManager.AddPasswordAsync(existingUser, NewPassword);
                    if (!addPasswordResult.Succeeded)
                    {
                        TempData["ErrorMessage"] = "Failed to set the new password: " + string.Join(", ", addPasswordResult.Errors.Select(e => e.Description));
                        return View("Profile", user);
                    }
                }


                // Update user details
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;
                existingUser.BirthDate = user.BirthDate;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.Address = user.Address;

                await service.UpdateAsync(existingUser);

                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating your profile.";
                return View("Profile", user);
            }
        }

        public async Task<IActionResult> Rentals(int page = 1)
        {
            const int PageSize = 8;

            try
            {
                var acceptedRequests = await RentalService.GetAllAsync(x => x.Status == RequestStatus.Completed, ["User","Car"]);
                var paginatedRequests = acceptedRequests.ToPagedList(page, PageSize);

                return View(paginatedRequests);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while fetching accepted rental requests.";
                return RedirectToAction("Index");
            }
        }
        public async Task<IActionResult> PendingApprovements(int page = 1, string documentType = "Car")
        {
            try
            {
                var documentStatus = DocumentStatus.Pending;
                // Fetch the documents based on the type (Car or User) and status (Pending)
                if (documentType == "User")
                {
                    var userDocuments = await UserDocumentService.GetAllAsync(x =>x.Status== UserDocumentStatus.Pending, ["User"]);
                    var pagedUserDocs = userDocuments.ToPagedList(page, 8);

                    ViewBag.DocumentType = "User";
                    return View(pagedUserDocs);
                }
                else
                {
                    // Get the pending car documents
                    var carDocuments = await DocumentService.GetAllAsync(x => x.status == DocumentStatus.Pending, ["Car", "User"]);
                    var pagedCarDocs = carDocuments.ToPagedList(page, 8);       
                    ViewBag.DocumentType = "Car";
                    return View(pagedCarDocs);
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the fetch
                TempData["ErrorMessage"] = "An error occurred while fetching pending requests.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PendingCarDetails(int id)
        {
            try
            {
                var CarDoc = await DocumentService.GetAllAsync(x => x.ID == id, ["Car", "User"]);
                var car = CarDoc.FirstOrDefault()?.Car;  

                if (car != null)
                {
                    var carImage = car.Images?.FirstOrDefault();
                    ViewBag.CarImage = carImage?.ImageUrl;  
                }

                return View(CarDoc.FirstOrDefault());
            }
            catch
            {
                TempData["ErrorMessage"] = "An error occurred while fetching the pending request.";
                return RedirectToAction("PendingApprovements");
            }
        }
        public async Task<IActionResult> PendingUserDetails(int id)
        {
            try
            {
                var UserDoc = await UserDocumentService.GetAllAsync(x => x.ID == id, ["User"]);
                return View(UserDoc.FirstOrDefault());
            }
            catch
            {
                TempData["ErrorMessage"] = "An error occurred while fetching the pending request.";
                return RedirectToAction("PendingApprovements");
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            try
            {
                var document = await DocumentService.GetByIdAsync(id);
                if (document == null)
                {
                    TempData["ErrorMessage"] = "Document not found.";
                    return RedirectToAction("PendingApprovements");
                }

                if (Enum.TryParse<DocumentStatus>(status, out var newStatus))
                {
                    document.status = newStatus;
                    document.ReviewdAt = DateTime.UtcNow;

                    await DocumentService.UpdateAsync(document);

                    TempData["SuccessMessage"] = $"Document has been {status.ToLower()} successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid status value.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the document status.";
            }

            return RedirectToAction("PendingApprovements");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatus2(int id, string status)
        {
            try
            {
                var document = await UserDocumentService.GetByIdAsync(id);
                if (document == null)
                {
                    TempData["ErrorMessage"] = "Document not found.";
                    return RedirectToAction("PendingApprovements");
                }

                if (Enum.TryParse<UserDocumentStatus>(status, out var newStatus))
                {
                    document.Status= newStatus;
                    document.ReviewdAt = DateTime.UtcNow;

                    await UserDocumentService.UpdateAsync(document);

                    TempData["SuccessMessage"] = $"Document has been {status.ToLower()} successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid status value.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the document status.";
            }

            return RedirectToAction("PendingApprovements");
        }

        public async Task<IActionResult> SystemAdmins(int page = 1)
        {
            try
            {
                var users = await userManager.GetUsersInRoleAsync("Admin");
                var pagedUsers = users.ToPagedList(page, 8);
                return View("SystemAdmins", pagedUsers);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while fetching Admins List.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RegisterViewModel model)
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
            var hasUpper = model.Password.Any(char.IsUpper);
            var hasLower = model.Password.Any(char.IsLower);
            var hasDigit = model.Password.Any(char.IsDigit);
            var hasSymbol = model.Password.Any(ch => !char.IsLetterOrDigit(ch));
            var hasMinLength = model.Password.Length >= 8;

            if (!(hasUpper && hasLower && hasDigit && hasSymbol && hasMinLength))
            {
                TempData["ErrorMessage"] = "Password must be at least 8 characters and include uppercase, lowercase, number, and special character.";
                return View("Create", model);
            }
            try
            {
                Microsoft.AspNetCore.Identity.IdentityResult result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, model.Role);
                    TempData["SuccessMessage"] = "User Registered Successfully";
                    return RedirectToAction("SystemAdmins", "admin");
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
                TempData["ErrorMessage"] = "This Phone Number Already Exist";
                return View(model);
            }

        }

        public async Task<IActionResult> OwnerCars(string id)
        {
            var cars = await _carService.GetAllAsync(c => c.UserId == id, "CarDocument", "Images");
            return View(cars);
        }
    }

}

