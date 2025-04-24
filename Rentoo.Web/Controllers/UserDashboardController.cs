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
        private readonly IService<CarDocument> _carDocumentService;
        private readonly IService<CarImage> _carImageService;
        private readonly IService<Request> _requestService;
        private readonly IService<RequestReview> _reviewService;
        private readonly IService<RateCode> _rateCodeService;
        private readonly IService<RateCodeDay> _rateCodeDayService;

        public UserDashboardController(
            IService<User> userService, 
            IService<Car> carService,
            IService<CarDocument> carDocumentService,
            IService<CarImage> carImageService,
            IService<Request> requestService,
            IService<RequestReview> reviewService,
            IService<RateCode> rateCodeService,
            IService<RateCodeDay> rateCodeDayService)
        {
            _userService = userService;
            _carService = carService;
            _carDocumentService = carDocumentService;
            _carImageService = carImageService;
            _requestService = requestService;
            _reviewService = reviewService;
            _rateCodeService = rateCodeService;
            _rateCodeDayService = rateCodeDayService;
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userService.GetByIdAsync(userId);
            return View(currentUser);
        }
        [HttpPost]
        public async Task<IActionResult> UserProfile(User user, IFormFile? ProfileImage)
        {
            try
            {
                var id = user.Id;
                var existingUser = await _userService.GetByIdAsync(id);
                if (existingUser == null)
                {
                    ModelState.AddModelError("", "User not found");
                    return View("Profile", user);
                }
                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    var uploadPath = Path.Combine("wwwroot", "uploads");
                    var fileName = ProfileImage.FileName;
                    var filePath = Path.Combine(uploadPath, fileName);
                    await ProfileImage.CopyToAsync(new FileStream(filePath, FileMode.Create));
                    existingUser.UserImage = "uploads/" + fileName;
                }
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;
                existingUser.BirthDate = user.BirthDate;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.Address = user.Address;
                // Update the user in the database
                await _userService.UpdateAsync(existingUser);
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("UserProfile");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating your profile. Please try again.");
                return View("Profile", user);
            }
        }
        [HttpGet]
        public async Task<IActionResult> MyCar()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cars = await _carService.GetAllAsync(c => c.UserId == userId, "CarDocument", "Images");
            return View(cars);
        }

        [HttpPost]
        public async Task<IActionResult> AddCar([FromForm] Car car, IFormFile licenseUrl, List<IFormFile> carImages, int licenseNumber)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                car.UserId = userId;
                car.IsAvailable = true;
                await _carService.AddAsync(car); 
                CarDocument carDocument = null;
                if (licenseUrl != null && licenseUrl.Length > 0)
                {
                    var documentFileName = $"{Guid.NewGuid()}_{Path.GetFileName(licenseUrl.FileName)}";
                    var documentUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "documents");
                    Directory.CreateDirectory(documentUploadPath);
                    var documentFilePath = Path.Combine(documentUploadPath, documentFileName);

                    using (var stream = new FileStream(documentFilePath, FileMode.Create))
                    {
                        await licenseUrl.CopyToAsync(stream);
                    }
                    carDocument = new CarDocument
                    {
                        LicenseUrl = $"uploads/documents/{documentFileName}",
                        LicenseNumber = licenseNumber,
                        CarId = car.ID,
                        UserId = userId
                    };

                    await _carDocumentService.AddAsync(carDocument);
                    car.CarDocumentId = carDocument.ID;
                }
                if (carDocument != null)
                {
                    carDocument.CarId = car.ID;
                    await _carDocumentService.UpdateAsync(carDocument);
                }
                if (carImages != null && carImages.Any())
                {
                    var imageUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cars");
                    Directory.CreateDirectory(imageUploadPath);

                    foreach (var image in carImages)
                    {
                        var imageFileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
                        var imageFilePath = Path.Combine(imageUploadPath, imageFileName);

                        using (var stream = new FileStream(imageFilePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        var carImage = new CarImage
                        {
                            ImageUrl = $"uploads/cars/{imageFileName}",
                            CarId = car.ID
                        };

                        await _carImageService.AddAsync(carImage);
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCar([FromForm] Car car, IFormFile licenseUrl, List<IFormFile> carImages)
        {
            try
            {
                var existingCar = await _carService.GetByIdAsync(car.ID);
                if (existingCar == null)
                {
                    return Json(new { success = false, message = "Car not found" });
                }
                existingCar.Model = car.Model;
                existingCar.Transmission = car.Transmission;
                existingCar.Seats = car.Seats;
                existingCar.Color = car.Color;
                existingCar.AirCondition = car.AirCondition;
                existingCar.Description = car.Description;
                existingCar.FactoryYear = car.FactoryYear;
                existingCar.WithDriver = car.WithDriver;
                existingCar.Fuel = car.Fuel;
                existingCar.Mileage = car.Mileage;
                existingCar.Address = car.Address;
                existingCar.IsAvailable = car.IsAvailable;

                // Handle car document update
                if (licenseUrl != null && licenseUrl.Length > 0)
                {
                    var documentFileName = $"{Guid.NewGuid()}_{Path.GetFileName(licenseUrl.FileName)}";
                    var documentUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "documents");
                    Directory.CreateDirectory(documentUploadPath);
                    var documentFilePath = Path.Combine(documentUploadPath, documentFileName);

                    using (var stream = new FileStream(documentFilePath, FileMode.Create))
                    {
                        await licenseUrl.CopyToAsync(stream);
                    }

                    var existingDocument = await _carDocumentService.GetByIdAsync(existingCar.CarDocumentId);
                    if (existingDocument != null)
                    {
                        existingDocument.LicenseUrl = $"uploads/documents/{documentFileName}";
                        existingDocument.LicenseNumber = car.CarDocument.LicenseNumber;
                        await _carDocumentService.UpdateAsync(existingDocument);
                    }
                }

                // Handle car images update
                if (carImages != null && carImages.Any())
                {
                    var imageUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cars");
                    Directory.CreateDirectory(imageUploadPath);
                    var existingImages = await _carImageService.GetAllAsync(i => i.CarId == car.ID);
                    foreach (var image in existingImages)
                    {
                        await _carImageService.DeleteAsync(image.ID);
                    }
                    foreach (var image in carImages)
                    {
                        var imageFileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
                        var imageFilePath = Path.Combine(imageUploadPath, imageFileName);

                        using (var stream = new FileStream(imageFilePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        var carImage = new CarImage
                        {
                            ImageUrl = $"uploads/cars/{imageFileName}",
                            CarId = car.ID
                        };

                        await _carImageService.AddAsync(carImage);
                    }
                }

                await _carService.UpdateAsync(existingCar);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCar(int id)
        {
            try
            {
                var car = await _carService.GetByIdAsync(id);
                if (car == null)
                {
                    return Json(new { success = false, message = "Car not found" });
                }

                // Delete car document
                if (car.CarDocumentId > 0)
                {
                    await _carDocumentService.DeleteAsync(car.CarDocumentId);
                }

                // Delete car images
                var carImages = await _carImageService.GetAllAsync(i => i.CarId == id);
                foreach (var image in carImages)
                {
                    await _carImageService.DeleteAsync(image.ID);
                }

                // Delete the car
                await _carService.DeleteAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cars = await _carService.GetAllAsync(c => c.UserId == userId);
            var carIds = cars.Select(c => c.ID).ToList();
            var requests = await _requestService.GetAllAsync(r => carIds.Contains(r.CarId), "User", "Car");
            return View(requests);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRequestStatus(int requestId, RequestStatus status)
        {
            try
            {
                var request = await _requestService.GetByIdAsync(requestId);
                if (request == null)
                {
                    return Json(new { success = false, message = "Request not found" });
                }

                request.Status = status;
                await _requestService.UpdateAsync(request);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Reviews()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                // Get all cars owned by the user
                var userCars = await _carService.GetAllAsync(c => c.UserId == userId);

                // Get all reviews for the user's cars
                var reviews = await _reviewService.GetAllAsync(r => userCars.Select(c => c.ID).Contains(r.Request.CarId), "Request", "Request.Car", "Request.User");

                return View(reviews);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading reviews. Please try again later.";
                return RedirectToAction("MyCar");
            }
        }
    }

}
