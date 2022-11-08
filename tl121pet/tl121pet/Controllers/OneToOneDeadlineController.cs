using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tl121pet.Services.Interfaces;

namespace tl121pet.Controllers
{
    [Authorize]
    public class OneToOneDeadlineController : Controller
    {
        private IOneToOneService _oneToOneDeadlineService;
        public OneToOneDeadlineController(IOneToOneService oneToOneDeadlineService)
        { 
            _oneToOneDeadlineService = oneToOneDeadlineService;
        }

        public IActionResult OneToOneDeadlineList()
        {
            return View("OneToOneDeadlineList", _oneToOneDeadlineService.GetDeadLines());
        }
    }
}
