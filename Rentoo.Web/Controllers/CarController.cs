using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rentoo.Application.Interfaces;
using Rentoo.Domain.Entities;
using Rentoo.Infrastructure.Data;
using Rentoo.Web.ViewModels;

public class CarController : Controller
{
    private readonly IService<Car> _carService;
    private readonly RentooDbContext _context;

    public CarController(IService<Car> carService, RentooDbContext context)
    {
        _carService = carService;
        _context = context;
    }

    public async Task<IActionResult> Details(int id)
    {
        var car = await _context.Cars
            .Include(c => c.Images)
            .Include(c => c.Requests.Where(r => r.Status == RequestStatus.Completed && r.Review != null))
                .ThenInclude(r => r.Review)
            .Include(c => c.Requests)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(c => c.ID == id);

        if (car == null)
            return NotFound();

        var reviews = car.Requests
            .Where(r => r.Review != null)
            .Select(r => new CarReviewViewModel
            {
                UserName = r.User?.FirstName ?? "Anonymous",
                Rating = r.Review!.Rating,
                Comment = r.Review.Comment,
                ReviewDate = r.Review.ReviewDate
            })
            .ToList();

        var viewModel = new CarDetailsViewModel
        {
            Car = car,
            CarImages = car.Images.ToList(),
            Reviews = reviews
        };

        return View(viewModel);
    }
}
