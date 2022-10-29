using Microsoft.AspNetCore.Mvc;

namespace tl121pet.Controllers
{
    public class HomeController : Controller
    {
        public RedirectToRouteResult Index()
        {
            return RedirectToRoute(new { controller = "OneToOneDeadline", action= "OneToOneDeadlineList" });
        }
    }
}
