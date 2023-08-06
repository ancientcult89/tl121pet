using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PersonController : ControllerBase
    {
        private IPersonService _personService;
        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }
        [HttpGet]
        public async Task<ActionResult<List<Person>>> Get()
        {
            return await _personService.GetPeopleWithGradeAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> Get(long id)
        {
            return await _personService.GetPersonByIdAsync(id);
        }

        [HttpPost]
        public async Task<ActionResult<Person>> Create([FromBody] Person newPerson)
        {
            return await _personService.CreatePersonAsync(newPerson);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Person>> Update([FromBody] Person person)
        {
            return await _personService.UpdatePersonAsync(person);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _personService.DeletePersonAsync(id);
            return Ok();
        }
    }
}
