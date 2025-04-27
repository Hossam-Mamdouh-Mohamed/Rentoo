using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rentoo.Infrastructure.Data;
using Rentoo.Web.ViewModels;

namespace Rentoo.Web.Controllers
{
    public class CarController : Controller
    {
        private readonly RentooDbContext _context;

        public CarController(RentooDbContext context)
        {
            _context = context;
        }

        // Details page
        public async Task<IActionResult> Details(int id)
        {
            var car = await _context.Cars
        .Include(c => c.Images)
        .Include(c => c.rateCode)
        .Include(c => c.User) // for owner info
        .FirstOrDefaultAsync(c => c.ID == id);

            if (car == null)
                return NotFound();

            var viewModel = new CarDetailsViewModel
            {
                Car = car,
                CarImages = car.Images.ToList(),
                RateCode = car.rateCode
            };

            return View(viewModel);
        }
    }
}
