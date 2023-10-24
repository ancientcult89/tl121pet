﻿using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.Models;
using tl121pet.Services.Application;
using tl121pet.Services.Interfaces;

namespace tl121pet.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PersonController : ApiController
    {
        private IPersonService _personService;
        private readonly OneToOneApplication _application;
        public PersonController(IPersonService personService, OneToOneApplication application)
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
        public async Task<ActionResult> DeletePerson(int id)
        {
            await _personService.DeletePersonAsync(id);
            return Ok();
        }
    }
}
