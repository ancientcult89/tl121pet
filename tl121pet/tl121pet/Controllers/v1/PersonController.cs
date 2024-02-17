using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PersonController : ApiController
    {
        private IPersonService _personService;
        private readonly IOneToOneApplication _application;
        public PersonController(IPersonService personService, IOneToOneApplication application)
        {
            _personService = personService;
            _application = application;
        }
        [HttpGet]
        public async Task<ActionResult<List<Person>>> GetPersonList()
        {
            return await _personService.GetPeopleWithGradeAsync();
        }

        [HttpGet("filtered")]
        public async Task<ActionResult<List<Person>>> GetFilteredByProjectsPersonList()
        {
            return await _application.GetPeopleFilteredByProjectsAsync();
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
        public async Task<ActionResult> DeletePerson(long id)
        {
            await _personService.DeletePersonAsync(id);
            return Ok();
        }

        [HttpPut("archive/{id}")]
        public async Task<ActionResult> ArchivePerson(long id)
        {
            await _application.ArchivePersonAsync(id);
            return Ok();
        }

        [HttpPut("testmail/{personId}")]
        public async Task<ActionResult> SendTestMail(long personId)
        {
            await _application.SendGreetingMailAsync(personId);
            return Ok();
        }
    }
}
