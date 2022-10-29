using Microsoft.AspNetCore.Mvc;
using tl121pet.Services.Interfaces;

namespace tl121pet.Controllers
{
    public class OneToOneDeadlineController : Controller
    {
        private IOneToOneDeadlineService _oneToOneDeadlineService;
        public OneToOneDeadlineController(IOneToOneDeadlineService oneToOneDeadlineService)
        { 
            _oneToOneDeadlineService = oneToOneDeadlineService;
        }

        public IActionResult OneToOneDeadlineList()
        {
            return View("OneToOneDeadlineList", _oneToOneDeadlineService.GetDeadLines());
        }
    }
}
