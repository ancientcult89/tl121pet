using Microsoft.AspNetCore.Mvc;

namespace tl121pet
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {            
            return View();
        }
    }
}
