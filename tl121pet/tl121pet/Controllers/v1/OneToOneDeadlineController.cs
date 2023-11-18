using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.Aggregate;
using tl121pet.Services.Interfaces;

namespace tl121pet.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OneToOneDeadlineController : ApiController
    {
        private IOneToOneApplication _oneToOneDeadlineService;

        public OneToOneDeadlineController(IOneToOneApplication oneToOneDeadlineService)
        {
            _oneToOneDeadlineService = oneToOneDeadlineService;
        }

        [HttpGet]
        public async Task<ActionResult<List<OneToOneDeadline>>> GetOneToOneDeadlineList()
        {
            return await _oneToOneDeadlineService.GetDeadLinesAsync();
        }
    }
}
