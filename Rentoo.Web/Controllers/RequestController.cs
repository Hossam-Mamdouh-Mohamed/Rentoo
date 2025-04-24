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
                    
                Car car = await _CarServes.GetByIdAsync(3);
                //RateCode rateCode = await _RateCodeServes.GetByIdAsync(car.RateCodeId);
               // List<RateCodeDay> rateCodeDay = (List<RateCodeDay>)await _RateCodeDayServes.GetAllAsync(rcd => rcd.RateCodeId == rateCode.ID);


                for (int i = 0; i < days; i++)
                {
                    
                }

                

                requestViewModel.TotalPrice = TotalPrice;

                


            }
            return View(requestViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> AddRequest(RequestViewModel requestViewModel)
        {
            if (ModelState.IsValid)
            {
                // Here you would typically save the request to the database
                // await _ReqSetves.AddAsync(request);
               
                return RedirectToAction("Index", "Home");
            }
            return View(requestViewModel);
        }
    }
}
