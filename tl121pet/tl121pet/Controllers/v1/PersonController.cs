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
        public async Task<ActionResult<List<Person>>> GetPersonList()
        {
            return await _personService.GetPeopleWithGradeAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPersonById(long id)
        {
            return await _personService.GetPersonByIdAsync(id);
        }

        [HttpPost]
        public async Task<ActionResult<Person>> CreatePerson([FromBody] Person newPerson)
        {
            return await _personService.CreatePersonAsync(newPerson);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Person>> UpdatePerson([FromBody] Person person)
        {
            return await _personService.UpdatePersonAsync(person);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePerson(int id)
        {
            await _personService.DeletePersonAsync(id);
            return Ok();
        }
    }
}
