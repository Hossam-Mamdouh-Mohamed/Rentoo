using System.Linq;
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
                var existingCar = await _carService.GetByIdAsync(car.ID, "CarDocument");
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
                        // Only update license number if it's provided in the form
                        if (car.CarDocument != null)
                        {
                            existingDocument.LicenseNumber = car.CarDocument.LicenseNumber;
                        }
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
            var requests = await _requestService.GetAllAsync(r => carIds.Contains(r.CarId.Value), "User", "Car");
            return View(requests);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRequestStatus(int requestId, string status)
        {
            try
            {
                // First, get all requests for the current user's cars
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userCars = await _carService.GetAllAsync(c => c.UserId == userId);
                var carIds = userCars.Select(c => c.ID).ToList();

                // Get the request with the specified ID and ensure it belongs to one of the user's cars
                var request = await _requestService.GetByIdAsync(requestId);

                if (request == null)
                {
                    TempData["ErrorMessage"] = "Request not found";
                    return RedirectToAction("MyRequests");
                }

                // Verify that the request belongs to one of the user's cars
                if (request.CarId == null || !carIds.Contains(request.CarId.Value))
                {
                    TempData["ErrorMessage"] = "You don't have permission to update this request";
                    return RedirectToAction("MyRequests");
                }

                // Convert the status string to the correct enum value
                RequestStatus newStatus;
                switch (status.ToLower())
                {
                    case "accepted":
                        newStatus = RequestStatus.Accepted;
                        break;
                    case "rejected":
                        newStatus = RequestStatus.Rejected;
                        break;
                    default:
                        TempData["ErrorMessage"] = "Invalid status value";
                        return RedirectToAction("MyRequests");
                }

                request.Status = newStatus;
                await _requestService.UpdateAsync(request);

                TempData["SuccessMessage"] = $"Request has been {status} successfully";
                return RedirectToAction("MyRequests");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("MyRequests");
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

                // احفظ الـ IDs في قائمة فعلية
                var carIds = userCars.Select(c => c.ID).ToList();

                // Get all reviews for the user's cars
                var reviews = await _reviewService.GetAllAsync(
                    r => carIds.Contains(r.Request.CarId.Value),
                    "Request", "Request.Car", "Request.User"
                );

                return View(reviews);

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading reviews. Please try again later.";
                return RedirectToAction("MyCar");
            }
        }
        [HttpGet]
        public async Task<IActionResult> PricePlans()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var rateCodes = await _rateCodeService.GetAllAsync(c => c.UserId == userId, "RateCodeDays");
            
            var viewModels = rateCodes.Select(rc => new PricePlanViewModel
            {
                ID = rc.ID,
                Name = rc.Name,
                Days = rc.RateCodeDays?.Select(rcd => new PricePlanDayViewModel
                {
                    ID = rcd.ID,
                    DayId = rcd.DayId,
                    Price = rcd.Price
                }).ToList() ?? new List<PricePlanDayViewModel>()
            }).ToList();

            return View(viewModels);
        }
        [HttpPost]
        public async Task<IActionResult> CreatePricePlan([FromBody] PricePlanViewModel model)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var rateCode = new RateCode
                {
                    Name = model.Name,
                    UserId = userId
                };

                await _rateCodeService.AddAsync(rateCode);
                
                if (model.Days != null && model.Days.Any())
                {
                    foreach (var day in model.Days)
                    {
                        var rateCodeDay = new RateCodeDay
                        {
                            RateCodeId = rateCode.ID,
                            DayId = day.DayId,
                            Price = day.Price
                        };
                        await _rateCodeDayService.AddAsync(rateCodeDay);
                    }
                }
                
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePricePlan(int id)
        {
            try
            {
                // First, get all rate code days associated with this rate code
                var rateCodeDays = await _rateCodeDayService.GetAllAsync(rcd => rcd.RateCodeId == id);
                
                // Check if the rate code is assigned to any car
                var cars = await _carService.GetAllAsync(c => c.RateCodeId == id);
                if (cars.Any())
                {
                    var carModels = cars.Select(c => c.Model).ToList();
                    var message = carModels.Count == 1 
                        ? $"This price plan is assigned to car: {carModels[0]}. Please unassign it before deleting."
                        : $"This price plan is assigned to {carModels.Count} cars: {string.Join(", ", carModels)}. Please unassign them before deleting.";
                    
                    return Json(new { success = false, message });
                }

                // Delete all rate code days
                foreach (var day in rateCodeDays)
                {
                    await _rateCodeDayService.DeleteAsync(day.ID);
                }

                // Then delete the rate code itself
                await _rateCodeService.DeleteAsync(id);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPricePlan(int id)
        {
            try
            {
                var rateCodes = await _rateCodeService.GetAllAsync(rc => rc.ID == id, "RateCodeDays");
                var rateCode = rateCodes.FirstOrDefault();
                
                if (rateCode == null)
                {
                    return Json(new { success = false, message = "Price plan not found" });
                }

                var viewModel = new PricePlanViewModel
                {
                    ID = rateCode.ID,
                    Name = rateCode.Name,
                    Days = rateCode.RateCodeDays?.Select(rcd => new PricePlanDayViewModel
                    {
                        ID = rcd.ID,
                        DayId = rcd.DayId,
                        Price = rcd.Price
                    }).ToList() ?? new List<PricePlanDayViewModel>()
                };

                return Json(new { success = true, data = viewModel });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePricePlan([FromBody] PricePlanViewModel model)
        {
            try
            {
                var rateCode = await _rateCodeService.GetByIdAsync(model.ID);
                if (rateCode == null)
                {
                    return Json(new { success = false, message = "Price plan not found" });
                }

                rateCode.Name = model.Name;
                await _rateCodeService.UpdateAsync(rateCode);

                // Delete existing days
                var existingDays = await _rateCodeDayService.GetAllAsync(rcd => rcd.RateCodeId == model.ID);
                foreach (var day in existingDays)
                {
                    await _rateCodeDayService.DeleteAsync(day.ID);
                }

                // Add new days
                if (model.Days != null && model.Days.Any())
                {
                    foreach (var day in model.Days)
                    {
                        var rateCodeDay = new RateCodeDay
                        {
                            RateCodeId = rateCode.ID,
                            DayId = day.DayId,
                            Price = day.Price
                        };
                        await _rateCodeDayService.AddAsync(rateCodeDay);
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCars()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var cars = await _carService.GetAllAsync(c => c.UserId == userId);
                return Json(cars.Select(c => new { id = c.ID, model = c.Model, color = c.Color, rateCodeId = c.RateCodeId }));
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignRateCodeToCar([FromBody] AssignRateCodeToCarViewModel model)
        {
            try
            {
                var car = await _carService.GetByIdAsync(model.CarId);
                if (car == null)
                {
                    return Json(new { success = false, message = "Car not found" });
                }

                car.RateCodeId = model.RateCodeId;
                await _carService.UpdateAsync(car);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCar(int id)
        {
            try
            {
                var car = await _carService.GetByIdAsync(id, "CarDocument");
                if (car == null)
                {
                    return Json(new { success = false, message = "Car not found" });
                }

                return Json(new
                {
                    success = true,
                    id = car.ID,
                    model = car.Model,
                    transmission = car.Transmission,
                    seats = car.Seats,
                    color = car.Color,
                    factoryYear = car.FactoryYear,
                    fuel = car.Fuel,
                    mileage = car.Mileage,
                    address = car.Address,
                    description = car.Description,
                    airCondition = car.AirCondition,
                    withDriver = car.WithDriver.ToString(),
                    isAvailable = car.IsAvailable,
                    carDocument = new
                    {
                        licenseNumber = car.CarDocument?.LicenseNumber
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRequest(int id)
        {
            try
            {
                var request = await _requestService.GetByIdAsync(id, "User", "Car");
                if (request == null)
                {
                    return Json(new { success = false, message = "Request not found" });
                }

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        id = request.ID,
                        car = new
                        {
                            model = request.Car?.Model
                        },
                        user = new
                        {
                            firstName = request.User?.FirstName,
                            lastName = request.User?.LastName
                        },
                        startDate = DateTime.Parse(request.StartDate).ToString("yyyy-MM-dd"),
                        endDate = DateTime.Parse(request.EndDate).ToString("yyyy-MM-dd"),
                        pickupAddress = request.pickupAddress,
                        totalPrice = request.TotalPrice,
                        withDriver = request.WithDriver,
                        status = request.Status.ToString()
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

}
