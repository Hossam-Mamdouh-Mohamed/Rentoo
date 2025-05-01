using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rentoo.Application.Interfaces;
using Rentoo.Domain.Entities;
using Rentoo.Domain.Shared;
using Rentoo.Infrastructure.Data;
using Rentoo.Web.ViewModels;
using X.PagedList.Extensions;


namespace Rentoo.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IService<User> _userService;
    private readonly IService<Car> _carService;
    private readonly RentooDbContext _context;


    public HomeController(ILogger<HomeController> logger, IService<User> userService, IService<Car> carService, RentooDbContext context)
    {
        _context = context;
        _carService = carService;
        _userService = userService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        ViewBag.CarsModel = new SelectList(await _context.Cars.Select(c => c.Model).Distinct().ToListAsync());
        ViewBag.CarsLocation = new SelectList(await _context.Cars.Select(c => c.Address).Distinct().ToListAsync());
        ViewBag.Cars = _context.Cars.Include(c => c.Images).OrderBy(c => c.ID).ToList();
        return View();
    }
    public async Task<IActionResult> Search(CarSearchViewModel search)
    {
        ViewBag.CarsModel = new SelectList( await _context.Cars.Select(c=>c.Model).Distinct().ToListAsync());
        //ViewBag.CarsTransmission = new SelectList( _context.Cars.Select(c => c.Transmission).Distinct().ToList());
        ViewBag.CarsLocation = new SelectList(await _context.Cars.Select(c => c.Address).Distinct().ToListAsync());

        var query = _context.Cars.AsQueryable();

          
        if (!string.IsNullOrEmpty(search.Model))
        {
            query = query.Where(c => c.Model.Contains(search.Model));
        }
        if (search.FactoryYear > 2000)
        {
            query = query.Where(c => c.FactoryYear == search.FactoryYear);
        }
        if (!string.IsNullOrEmpty(search.Transmission))
        {
            query = query.Where(c => c.Transmission.Contains(search.Transmission));
        }
        if (!string.IsNullOrEmpty(search.Address))
        {
            query = query.Where(c => c.Address.Contains(search.Address));
        }

        var cars = query.Include(c=>c.Images).ToList();
        if (cars.Count == 0)
        {
            ViewBag.Message = "No cars found matching your criteria.";
        }
        var carSearchViewModel = new CarSearchViewModel
        {
            Model = search.Model,
            FactoryYear = search.FactoryYear,
            Transmission = search.Transmission,
            Address = search.Address,
            Cars = cars
        };


        return View(carSearchViewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }
    [AllowAnonymous]
    public async Task<IActionResult> ToggleLanguage(string returnUrl)
    {
        string culture = "ar-SA";
        if (CultureConfiguration.IsArabic)
            culture = "en-US";
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );
        return Redirect(returnUrl);
    }


}
