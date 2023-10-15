using Microsoft.AspNetCore.Mvc;
using tl121pet.DAL.Data;
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
            return await _roleService.GetRoleByIdAsync(id);
        }

        [HttpPost]
        public async Task<ActionResult<Role>> CreateRole([FromBody] Role newRole)
        {
            return await _roleService.CreateRoleAsync(newRole);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Role>> UpdateRole([FromBody] Role role)
        {
            return await _roleService.UpdateRoleAsync(role);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRole(int id)
        {
            await _roleService.DeleteRoleAsync(id);
            return Ok();
        }
    }
}
