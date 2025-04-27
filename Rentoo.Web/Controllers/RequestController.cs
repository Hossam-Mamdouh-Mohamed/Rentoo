using Microsoft.AspNetCore.Mvc;
using Rentoo.Application.Interfaces;
using Rentoo.Domain.Entities;
using Rentoo.Web.ViewModels;

namespace Rentoo.Web.Controllers
{
    public class RequestController : Controller
    {
        int CarId { get; set; }
        private readonly IService<Request> _ReqServes;
        private readonly IService<Car> _CarServes;
        private readonly IService<RateCode> _RateCodeServes;
        private readonly IService<RateCodeDay> _RateCodeDayServes;

        public RequestController(IService<Request> ReqServes,IService<Car> CarServes
                                ,IService<RateCode> RCServes, IService<RateCodeDay> RCDServes)
        {
            _ReqServes = ReqServes;
            _CarServes = CarServes;
            _RateCodeServes = RCServes;
            _RateCodeDayServes = RCDServes;

        }

        [HttpGet]
        public IActionResult AddRequest(int carId)
        {
            CarId = carId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CompleteRequest(RequestViewModel requestViewModel)
        {
            if (ModelState.IsValid)
            {
                int days = (int)(requestViewModel.EndDate - requestViewModel.StartDate).TotalDays;
                int TotalPrice = 0;

                Car car = await _CarServes.GetByIdAsync(1);

                if (car.RateCodeId.HasValue) // Check if RateCodeId is not null
                {
                    RateCode rateCode = await _RateCodeServes.GetByIdAsync(car.RateCodeId.Value); // Use .Value to access the int value
                    List<RateCodeDay> rateCodeDay = (List<RateCodeDay>)await _RateCodeDayServes.GetAllAsync(rcd => rcd.RateCodeId == rateCode.ID);

                    for (int i = 0; i < days; i++)
                    {
                        // Logic for calculating TotalPrice
                    }

                    requestViewModel.TotalPrice = TotalPrice;
                }
                else
                {
                    ModelState.AddModelError("", "RateCodeId is null for the selected car.");
                }
            }
            return View("AddRequest", requestViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> SaveRequest(RequestViewModel requestViewModel)
        {
            if (ModelState.IsValid)
            {
                // Here you would typically save the request to the database
                // await _ReqSetves.AddAsync(request);
               
                return RedirectToAction("Index", "Home");
            }
            return View(requestViewModel);
        }

        private async Task<float> CalculateTotalPrice(DateTime startDate, DateTime endDate, int carId)
        {
            float totalPrice = 0;
            var car = await _CarServes.GetByIdAsync(1);
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
                int dayOfWeek = (int)currentDate.DayOfWeek; // هنا هيجبلي الايام في الفتره دي 

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

            return totalPrice;
        }
        [HttpGet]
        public async Task<IActionResult> GetTotalPrice(DateTime startDate, DateTime endDate, int carId)
        {
            try
            {
                var totalPrice = await CalculateTotalPrice(startDate, endDate, carId);
                return Json(new { success = true, totalPrice });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
