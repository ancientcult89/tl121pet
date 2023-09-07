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
        //TODO: избавиться от зависимости слоя данных в контроллере
        private DataContext _dataContext;
        private readonly IAuthService _authService;

        public RoleController(DataContext dataContext, IAuthService authService)
        {
            _dataContext = dataContext;
            _authService = authService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Role>>> GetRoleList()
        {
            return await _authService.GetRoleListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRoleById(int id)
        {
            return await _authService.GetRoleByIdAsync(id);
        }

        [HttpPost]
        public async Task<ActionResult<Role>> CreateRole([FromBody] Role newRole)
        {
            return await _authService.CreateRoleAsync(newRole);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Role>> UpdateRole([FromBody] Role role)
        {
            return await _authService.UpdateRoleAsync(role);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRole(int id)
        {
            await _authService.DeleteRoleAsync(id);
            return Ok();
        }
    }
}
