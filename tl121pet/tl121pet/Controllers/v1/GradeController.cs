using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class GradeController : ApiController
    {
        private IPersonService _personService;
        public GradeController(IPersonService personService)
        {
            _personService = personService;
        }
        [HttpGet]
        public async Task<ActionResult<List<Grade>>> Get()
        {
            return await _personService.GetAllGradesAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Grade>> Get(long id)
        {
            return await _personService.GetGradeByIdAsync(id);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create()
        {
            //return await Mediator.Send(command);
            throw new NotImplementedException();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id)
        {
            //if (id != command.Id)
            //    return BadRequest();

            //await Mediator.Send(command);

            //return NoContent();
            throw new NotImplementedException();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            //await Mediator.Send(new DeleteTourListCommand { Id = id });

            //return NoContent();
            throw new NotImplementedException();
        }
    }
}
