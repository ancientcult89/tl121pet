using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.Infrastructure.Exceptions;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RoleController : ApiController
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Role>>> GetRoleList()
        {
            return await _roleService.GetRoleListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRoleById(int id)
        {            
            try
            {
                return await _roleService.GetRoleByIdAsync(id);
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
        public async Task<ActionResult<Role>> CreateRole([FromBody] Role newRole)
        {            
            try
            {
                return await _roleService.CreateRoleAsync(newRole);
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
        public async Task<ActionResult<Role>> UpdateRole([FromBody] Role role)
        {
            try
            {
                return await _roleService.UpdateRoleAsync(role);
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
        public async Task<ActionResult> DeleteRole(int id)
        {
            try
            {
                await _roleService.DeleteRoleAsync(id);
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
