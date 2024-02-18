using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.DTO;
using tl121pet.Services.Interfaces;

namespace tl121pet.Controllers.v2
{
    [ApiController]
    [Route("api/v2/[controller]")]
    public class MeetingController : ApiController
    {
        private readonly IOneToOneApplication _application;

        public MeetingController(IOneToOneApplication application)
        {
            _application = application;
        }

        [HttpGet]
        public async Task<ActionResult<MeetingPagedResponseDTO>> GetMeetingList([FromQuery] MeetingPagedRequestDTO requestDTO)
        {
            return await _application.GetPagedMeetingsAsync(requestDTO);
        }
    }
}
