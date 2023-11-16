using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.Infrastructure.Exceptions;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class GradeController : ApiController
    {
        private IGradeService _gradeService;
        public GradeController(IGradeService gradeService)
        {
            _gradeService = gradeService;
        }
        [HttpGet]
        public async Task<ActionResult<List<Grade>>> GetGradeList()
        {
            return await _gradeService.GetAllGradesAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Grade>> GetGradeById(long id)
        {
            try
            {
                return await _gradeService.GetGradeByIdAsync(id);
            }
            catch (LogicException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (DataFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Grade>> CreateGrade([FromBody] Grade newGrade)
        {
            try
            {
                return Ok(await _gradeService.CreateGradeAsync(newGrade));
            }
            catch (LogicException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (DataFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Grade>> UpdateGrade([FromBody] Grade grade)
        {            
            try
            {
                return await _gradeService.UpdateGradeAsync(grade);
            }
            catch (LogicException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (DataFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGrade(int id)
        {
            try
            {
                await _gradeService.DeleteGradeAsync(id);
                return Ok();
            }
            catch (LogicException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (DataFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
