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
        public async Task<ActionResult<List<Grade>>> GetGradeList()
        {
            return await _personService.GetAllGradesAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Grade>> GetGradeById(long id)
        {
            return await _personService.GetGradeByIdAsync(id);
        }

        [HttpPost]
        public async Task<ActionResult<Grade>> CreateGrade([FromBody] Grade newGrade)
        {
            return await _personService.CreateGradeAsync(newGrade);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Grade>> UpdateGrade([FromBody] Grade grade)
        {
            return await _personService.UpdateGradeAsync(grade);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGrade(int id)
        {
            await _personService.DeleteGradeAsync(id);
            return Ok();
        }
    }
}
