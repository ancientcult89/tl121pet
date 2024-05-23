using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.Aggregate;
using tl121pet.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace tl121pet.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OneToOneDeadlineController(IOneToOneApplication appication) : ApiController
    {
        private IOneToOneApplication _application = appication;

        [HttpGet]
        public async Task<ActionResult<List<OneToOneDeadline>>> GetOneToOneDeadlineList()
        {
            return await _application.GetDeadLinesAsync();
        }
    }
}
