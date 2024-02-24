using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class GradeController(IGradeService gradeService) : ApiController
    {
        private IGradeService _gradeService = gradeService;

        [HttpGet]
        public async Task<ActionResult<List<Grade>>> GetGradeList()
        {
            return await _gradeService.GetAllGradesAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Grade>> GetGradeById(long id)
        {
            return await _gradeService.GetGradeByIdAsync(id);
        }

        [HttpPost]
        public async Task<ActionResult<Grade>> CreateGrade([FromBody] Grade newGrade)
        {
            return Ok(await _gradeService.CreateGradeAsync(newGrade));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Grade>> UpdateGrade([FromBody] Grade grade)
        {            
            return await _gradeService.UpdateGradeAsync(grade);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGrade(int id)
        {
            await _gradeService.DeleteGradeAsync(id);
            return Ok();
        }
    }
}
