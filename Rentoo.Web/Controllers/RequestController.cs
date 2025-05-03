﻿using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rentoo.Application.Interfaces;
using Rentoo.Domain.Entities;
using Rentoo.Web.ViewModels;

namespace Rentoo.Web.Controllers
{
    public class RequestController : Controller
    {
        private readonly IService<Request> _ReqServes;
        private readonly IService<Car> _CarServes;
        private readonly IService<RateCode> _RateCodeServes;
        private readonly IService<RateCodeDay> _RateCodeDayServes;
        private readonly IService<UserDocument> _userdocumentserves;

        public RequestController(IService<Request> ReqServes,IService<Car> CarServes
                                ,IService<RateCode> RCServes, IService<RateCodeDay> RCDServes, IService<UserDocument> userdocumentserves)
        {
            _ReqServes = ReqServes;
            _CarServes = CarServes;
            _RateCodeServes = RCServes;
            _RateCodeDayServes = RCDServes;
            _userdocumentserves = userdocumentserves;

        }

        [HttpGet]
        [Route("Request/AddRequest/{carId}")]
        public async Task<IActionResult> AddRequestAsync(int carId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction( "Login", "Account");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingRequest = await _ReqServes.GetAllAsync(r =>
                r.UserID == userId &&
                r.CarId == carId &&
                r.Status == RequestStatus.Pending);

            if (existingRequest.Any())
            {
                TempData["HasReq"] = "You Has Pending Request";
                return RedirectToAction("Details","Car",new { id =  carId });
            }

            ViewBag.CarId = carId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CompleteRequest(RequestViewModel requestViewModel)
        {
            if (ModelState.IsValid)
            {
                var ReservationReauest = await _ReqServes.GetAllAsync(r =>
                    (r.StartDate.CompareTo(requestViewModel.EndDate.ToString("yyyy-MM-dd")) < 0 &&
                     requestViewModel.StartDate.ToString("yyyy-MM-dd").CompareTo(r.EndDate) < 0)
                    && r.Status == RequestStatus.Accepted
                    && r.CarId == requestViewModel.CarId);

                // Fix: Access the EndDate property of individual requests in the collection
                if (ReservationReauest.Any())
                {
                    var firstConflictingRequest = ReservationReauest.First();
                    TempData["ErrorMessage"] = $"This car is not available until {firstConflictingRequest.EndDate:yyyy-MM-dd}";
                    return RedirectToAction("AddRequest", "Request", new { id = requestViewModel.CarId });
                }

                // Save the request to the database
                var request = new Request
                {
                    StartDate = requestViewModel.StartDate.ToString("yyyy-MM-dd"),
                    EndDate = requestViewModel.EndDate.ToString("yyyy-MM-dd"),
                    TotalPrice = requestViewModel.TotalPrice,
                    Status = RequestStatus.Pending,
                    DeliveryAddress = requestViewModel.DeliveryAddress,
                    pickupAddress = requestViewModel.pickupAddress,
                    WithDriver = requestViewModel.WithDriver,
                    CarId = requestViewModel.CarId,
                    UserID = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };
                // Check if the WithDriver==true and user has user document with status == Accepted
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userdocument = await _userdocumentserves.GetAllAsync(ud => ud.UserId == userId && ud.Status == UserDocumentStatus.Accepted);
                if (request.WithDriver==false && !userdocument.Any())
                {
                    TempData["ErrorMessage"] = "You must upload your documents to request a car with a driver.";
                    return RedirectToAction("AddRequest", "Request", new { id = requestViewModel.CarId });
                }
                await _ReqServes.AddAsync(request);
            }
            ViewBag.CarId = requestViewModel.CarId;
            TempData["SuccessMessage"] = "Request has been sent successfully";
            return RedirectToAction("Index", "Home");
        }


       
        private async Task<float> CalculateTotalPrice(DateTime startDate, DateTime endDate, int carId, bool WithDriver)
        {
            float totalPrice = 0;
            var car = await _CarServes.GetByIdAsync(carId);
            if (car == null || !car.RateCodeId.HasValue)
            {
                throw new Exception("Car not found or no rate code assigned");
            }
            var rateCode = await _RateCodeServes.GetByIdAsync(car.RateCodeId.Value);
            var rateCodeDays = await _RateCodeDayServes.GetAllAsync(rcd => rcd.RateCodeId == rateCode.ID);
            int totalDays = (int)(endDate - startDate).TotalDays;
            for (int i = 0; i < totalDays; i++)
            {
                DateTime currentDate = startDate.AddDays(i);
                int dayOfWeek = (int)currentDate.DayOfWeek+1; // هنا هيجبلي الايام في الفتره دي 

                // Find the rate for this day
                var rateForDay = rateCodeDays.FirstOrDefault(rcd => rcd.DayId == dayOfWeek);
                if (rateForDay != null)
                {
                    totalPrice += rateForDay.Price;
                }
                else
                {
                    var defaultRate = rateCodeDays.FirstOrDefault();
                    if (defaultRate == null)
                    {
                        throw new Exception("No rate defined for this car");
                    }
                    totalPrice += defaultRate.Price;
                }
            }
            // Add driver cost if applicable
            if (WithDriver)
            {
                totalPrice += totalDays * 25;  
            }

            return totalPrice;
        }
        [HttpGet]
        public async Task<IActionResult> GetTotalPrice(DateTime startDate, DateTime endDate, int carId, bool WithDriver)
        {
            try
            {
                var totalPrice = await CalculateTotalPrice(startDate, endDate, carId, WithDriver);
                return Json(new { success = true, totalPrice });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
