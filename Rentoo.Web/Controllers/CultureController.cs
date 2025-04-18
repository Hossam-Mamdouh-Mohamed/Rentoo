using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Rentoo.Web.Controllers;

public class CultureController : Controller
{
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );

        if (string.IsNullOrEmpty(returnUrl))
        {
            returnUrl = Url.Content("~/"); // Redirect to the home page if returnUrl is null
        }

        return LocalRedirect(returnUrl);
    }
}