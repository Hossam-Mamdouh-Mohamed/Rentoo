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
                    TempData["ErrorMessage"] = "User not found";
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
                TempData["ErrorMessage"] = "An error occurred while updating your profile. Please try again.";
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
        [HttpGet]
        public IActionResult AddCar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCar(AddCarViewModel addCarViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(addCarViewModel);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
   
                var car = new Car
                {
                    Model = addCarViewModel.Model,
                    Transmission = addCarViewModel.Transmission,
                    Seats = addCarViewModel.Seats,
                    Color = addCarViewModel.Color,
                    AirCondition = addCarViewModel.AirCondition,
                    Description = addCarViewModel.Description,
                    FactoryYear = addCarViewModel.FactoryYear,
                    WithDriver = addCarViewModel.WithDriver,
                    Fuel = addCarViewModel.Fuel,
                    Mileage = addCarViewModel.Mileage,
                    Address = addCarViewModel.Address,
                    IsAvailable = false,
                    UserId = userId
                };

                await _carService.AddAsync(car);

                // Handle car document
                if (addCarViewModel.LicenseUrl != null && addCarViewModel.LicenseUrl.Length > 0)
                {
                    var uploadPath = Path.Combine("wwwroot", "uploads", "documents");
                    Directory.CreateDirectory(uploadPath);

                    var extension = Path.GetExtension(addCarViewModel.LicenseUrl.FileName);
                    var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadPath, uniqueFileName);
                    await addCarViewModel.LicenseUrl.CopyToAsync(new FileStream(filePath, FileMode.Create));

                    var carDocument = new CarDocument
                    {
                        LicenseUrl = $"uploads/documents/{uniqueFileName}",
                        LicenseNumber = addCarViewModel.LicenseNumber,
                        CarId = car.ID,
                        UserId = userId
                    };

                    await _carDocumentService.AddAsync(carDocument);
                    car.CarDocumentId = carDocument.ID;
                    await _carService.UpdateAsync(car);
                }


                // Handle car images
                if (addCarViewModel.CarImages != null && addCarViewModel.CarImages.Count > 0)
                {
                    var imageUploadPath = Path.Combine("wwwroot", "uploads", "cars");
                    Directory.CreateDirectory(imageUploadPath);

                    foreach (var image in addCarViewModel.CarImages)
                    {
                        var extension = Path.GetExtension(image.FileName);
                        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                        var filePath = Path.Combine(imageUploadPath, uniqueFileName);
                        await image.CopyToAsync(new FileStream(filePath, FileMode.Create));

                        var carImage = new CarImage
                        {
                            ImageUrl = $"uploads/cars/{uniqueFileName}",
                            CarId = car.ID
                        };

                        await _carImageService.AddAsync(carImage);
                    }
                }

                TempData["SuccessMessage"] = "Car added successfully!";
                return RedirectToAction("MyCar");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while adding the car. Please try again.";
                return View(addCarViewModel);
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditCar(int id)
        {
            var car = await _carService.GetByIdAsync(id, "CarDocument", "Images");
            if (car == null)
            {
                return NotFound();
            }

            var editCarViewModel = new EditCarViewModel
            {
                ID = car.ID,
                Model = car.Model,
                Transmission = car.Transmission,
                Seats = car.Seats,
                Color = car.Color,
                AirCondition = car.AirCondition,
                Description = car.Description,
                FactoryYear = car.FactoryYear,
                WithDriver = car.WithDriver,
                Fuel = car.Fuel,
                Mileage = car.Mileage,
                Address = car.Address,
                IsAvailable = car.IsAvailable,
                LicenseNumber = car.CarDocument?.LicenseNumber,
                ExistingLicenseUrl = car.CarDocument?.LicenseUrl,
                ExistingCarImages = car.Images?.Select(i => i.ImageUrl).ToList() ?? new List<string>()
            };

            return View(editCarViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditCar(EditCarViewModel editCarViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editCarViewModel);
            }

            try
            {
                var existingCar = await _carService.GetByIdAsync(editCarViewModel.ID, "CarDocument", "Images");
                if (existingCar == null)
                {
                    return NotFound();
                }

                // Update car properties
                existingCar.Model = editCarViewModel.Model;
                existingCar.Transmission = editCarViewModel.Transmission;
                existingCar.Seats = editCarViewModel.Seats;
                existingCar.Color = editCarViewModel.Color;
                existingCar.AirCondition = editCarViewModel.AirCondition;
                existingCar.Description = editCarViewModel.Description;
                existingCar.FactoryYear = editCarViewModel.FactoryYear;
                existingCar.WithDriver = editCarViewModel.WithDriver;
                existingCar.Fuel = editCarViewModel.Fuel;
                existingCar.Mileage = editCarViewModel.Mileage;
                existingCar.Address = editCarViewModel.Address;
                existingCar.IsAvailable = editCarViewModel.IsAvailable;

                // Handle car document update
                if (editCarViewModel.LicenseUrl != null && editCarViewModel.LicenseUrl.Length > 0)
                {
                    var uploadPath = Path.Combine("wwwroot", "uploads", "documents");
                    Directory.CreateDirectory(uploadPath);
                    var fileName = editCarViewModel.LicenseUrl.FileName;
                    var filePath = Path.Combine(uploadPath, fileName);
                    await editCarViewModel.LicenseUrl.CopyToAsync(new FileStream(filePath, FileMode.Create));

                    var existingDocument = await _carDocumentService.GetByIdAsync(existingCar.CarDocumentId);
                    if (existingDocument != null)
                    {
                        existingDocument.LicenseUrl = $"uploads/documents/{fileName}";
                        existingDocument.LicenseNumber = editCarViewModel.LicenseNumber;
                        await _carDocumentService.UpdateAsync(existingDocument);
                    }
                    else
                    {
                        var newDocument = new CarDocument
                        {
                            LicenseUrl = $"uploads/documents/{fileName}",
                            LicenseNumber = editCarViewModel.LicenseNumber,
                            CarId = existingCar.ID,
                            UserId = existingCar.UserId
                        };
                        await _carDocumentService.AddAsync(newDocument);
                        existingCar.CarDocumentId = newDocument.ID;
                    }
                }
                else
                {
                    var existingDocument = await _carDocumentService.GetByIdAsync(existingCar.CarDocumentId);
                    if (existingDocument != null)
                    {
                        var duplicateDocument = await _carDocumentService.GetAllAsync(d => d.LicenseNumber == editCarViewModel.LicenseNumber && d.ID != existingDocument.ID);
                        if (duplicateDocument != null && duplicateDocument.Any(s => s.ID != existingDocument.ID))
                        {
                            ModelState.AddModelError("LicenseNumber", "رقم الرخصة مستخدم بالفعل.");
                            return View(editCarViewModel);
                        }
                        existingDocument.LicenseNumber = editCarViewModel.LicenseNumber;
                        await _carDocumentService.UpdateAsync(existingDocument);
                    }
                }

                // Handle car images update
                if (editCarViewModel.CarImages != null && editCarViewModel.CarImages.Count > 0)
                {
                    // First, delete all existing images
                    if (existingCar.Images != null && existingCar.Images.Any())
                    {
                        foreach (var existingImage in existingCar.Images.ToList())
                        {
                            // Optionally delete the physical file
                            var physicalPath = Path.Combine("wwwroot", existingImage.ImageUrl);
                            if (System.IO.File.Exists(physicalPath))
                            {
                                System.IO.File.Delete(physicalPath);
                            }

                            // Remove from database
                            await _carImageService.DeleteAsync(existingImage);
                        }
                    }

                    // Then add the new images
                    var imageUploadPath = Path.Combine("wwwroot", "uploads", "cars");
                    Directory.CreateDirectory(imageUploadPath);

                    foreach (var image in editCarViewModel.CarImages)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName); // Generate unique filename
                        var filePath = Path.Combine(imageUploadPath, fileName);
                        await image.CopyToAsync(new FileStream(filePath, FileMode.Create));

                        var carImage = new CarImage
                        {
                            ImageUrl = $"uploads/cars/{fileName}",
                            CarId = existingCar.ID
                        };

                        await _carImageService.AddAsync(carImage);
                    }
                }

                await _carService.UpdateAsync(existingCar);
                TempData["SuccessMessage"] = "Car updated successfully!";
                return RedirectToAction("MyCar");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the car. Please try again.");
                return View(editCarViewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCar(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var car = await _carService.GetByIdAsync(id, "CarDocument", "Images");

                if (car == null)
                {
                    TempData["ErrorMessage"] = "Car not found";
                    return RedirectToAction("MyCar");
                }

                // Verify that the car belongs to the current user
                if (car.UserId != userId)
                {
                    TempData["ErrorMessage"] = "You don't have permission to delete this car";
                    return RedirectToAction("MyCar");
                }

                // Check if the car has any active requests
                var activeRequests = await _requestService.GetAllAsync(r => 
                    r.CarId == car.ID && 
                    (r.Status == RequestStatus.Pending || r.Status == RequestStatus.Accepted)
                );

                if (activeRequests.Any())
                {
                    TempData["ErrorMessage"] = "Cannot delete car with active or pending requests";
                    return RedirectToAction("MyCar");
                }

                // Delete car document if exists
                if (car.CarDocumentId.HasValue)
                {
                    await _carDocumentService.DeleteAsync(car.CarDocumentId.Value);
                }

                // Delete car images
                if (car.Images != null && car.Images.Any())
                {
                    foreach (var image in car.Images)
                    {
                        await _carImageService.DeleteAsync(image.ID);
                    }
                }

                // Delete the car
                await _carService.DeleteAsync(car.ID);

                TempData["SuccessMessage"] = "Car deleted successfully";
                return RedirectToAction("MyCar");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the car. Please try again.";
                return RedirectToAction("MyCar");
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyRequests()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var cars = await _carService.GetAllAsync(c => c.UserId == userId);
                var carIds = cars.Select(c => c.ID).ToList();
                var requests = await _requestService.GetAllAsync(r => carIds.Contains(r.CarId.Value), "User", "Car");

                // Check and update status for accepted requests that have passed their end date
                foreach (var request in requests.Where(r => r.Status == RequestStatus.Accepted))
                {
                    var endDate = DateTime.ParseExact(request.EndDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    if (DateTime.Now >= endDate)
                    {
                        // Update request status
                        request.Status = RequestStatus.Completed;
                        await _requestService.UpdateAsync(request);

                        // Update car availability
                        var car = await _carService.GetByIdAsync(request.CarId.Value);
                        if (car != null)
                        {
                            car.IsAvailable = true;
                            await _carService.UpdateAsync(car);
                        }
                    }
                }
                requests = await _requestService.GetAllAsync(r => carIds.Contains(r.CarId.Value), "User", "Car");
                return View(requests);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading requests. Please try again.";
                return RedirectToAction("MyCar");
            }
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
                        // Update car availability
                        var car = await _carService.GetByIdAsync(request.CarId.Value);
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
