using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Rentoo.Application.Interfaces;
using Rentoo.Application.Services;
using Rentoo.Domain.Entities;
using Rentoo.Infrastructure.Data;

namespace Rentoo.Web.Controllers
{
    public class UserDashboardController : Controller
    {
        //private readonly string  stringuserId= "ff237ee5-a469-4677-86ab-0d3c06c311a6";
        IService<User> _userService;
        public UserDashboardController(IService<User> _userService)
        {
            this._userService = _userService;


        }
        public async Task<IActionResult> UserProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            var currentUser = await _userService.GetByIdAsync(userId);
            return View(currentUser);
        }
    }
}
