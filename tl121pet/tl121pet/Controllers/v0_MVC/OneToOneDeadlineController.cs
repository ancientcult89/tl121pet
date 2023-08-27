using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tl121pet.Services.Interfaces;

namespace tl121pet.Controllers.v0_MVC
{
    [Authorize]
    public class OneToOneDeadlineController : Controller
    {
        private IOneToOneService _oneToOneDeadlineService;
        public OneToOneDeadlineController(IOneToOneService oneToOneDeadlineService)
        { 
            _oneToOneDeadlineService = oneToOneDeadlineService;
        }

        public async Task<IActionResult> OneToOneDeadlineList()
        {
            return View("OneToOneDeadlineList", await _oneToOneDeadlineService.GetDeadLinesAsync());
        }
    }
}
