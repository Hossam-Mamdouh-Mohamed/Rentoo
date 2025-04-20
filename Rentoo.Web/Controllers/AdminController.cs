using Microsoft.AspNetCore.Mvc;

namespace Rentoo.Web.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
