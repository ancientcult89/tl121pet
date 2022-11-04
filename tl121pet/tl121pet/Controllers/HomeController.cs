using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace tl121pet.Controllers
{
    public class HomeController : Controller
    {
        public RedirectToRouteResult Index()
        {
            return RedirectToRoute(new { controller = "OneToOneDeadline", action= "OneToOneDeadlineList" });
        }

        [HttpPost]
        public RedirectToRouteResult SetLanguage(string lang)
        {
            if (lang != null)
            {
                HttpContext.Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang)));
            }
            return RedirectToRoute(new { controller = "OneToOneDeadline", action = "OneToOneDeadlineList" });
        }
    }
}
