using Microsoft.AspNetCore.Mvc;
using Rentoo.Application.Interfaces;
using Rentoo.Domain.Entities;
using Rentoo.Web.ViewModels;

namespace Rentoo.Web.Controllers
{
    public class CarController : Controller
    {
        private readonly IService<Car> _carService;

        public CarController(IService<Car> carService)
        {
            _carService = carService;
        }

        public async Task<IActionResult> Details(int id)
        {
            var cars = await _carService.GetAllAsync(
                c => c.ID == id,
                "Images", "User"
            );

            var car = cars.FirstOrDefault();
            if (car == null)
                return NotFound();

            var viewModel = new CarDetailsViewModel
            {
                Car = car,
                CarImages = car.Images.ToList()
            };

            return View(viewModel);
        }
    }
}
