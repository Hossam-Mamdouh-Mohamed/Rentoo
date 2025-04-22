using Microsoft.AspNetCore.Mvc;
using Rentoo.Application.Interfaces;
using Rentoo.Domain.Entities;

namespace Rentoo.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IService<User> service;
        private readonly IService<Car> carService;
        public AdminController(IService<User> _service, IService<Car> carService)
        {
            service = _service;
            this.carService = carService;
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
        public IActionResult Cars()
        {
            var cars = carService.GetAllAsync().Result;
            return View();
        }
    }
}
