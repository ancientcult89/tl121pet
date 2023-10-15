using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Extensions;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : Controller
    {
        private readonly IAuthService _authService;
        public UserController(IAuthService authService)
        {
            _authService = authService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<UserResponseDTO>>> GetUserList()
        {
            List<User> respond = await _authService.GetUserListAsync();
            return respond.Select(u => u.ToResponseDto()).ToList();
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser([FromBody] UserRegisterRequestDTO request)
        {
            try
            {
                await _authService.RegisterAsync(request);
                return Ok();
            }
            catch (Exception ex)
            { 
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] UserLoginRequestDTO loginRequest)
        {
            try
            {
                return await _authService.LoginAsync(loginRequest);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPut("{id}/changepassword")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangeUserPasswordRequestDTO changeUserPasswordRequest)
        {
            try
            { 
                await _authService.ChangePasswordAsync(changeUserPasswordRequest);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUserById(long id)
        {
            User? user = await _authService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            else 
                return user.ToDto();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDTO>> UpdateUser([FromBody] UserDTO user)
        {
            return await _authService.UpdateUserAsync(user);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(long id)
        {
            await _authService.DeleteUserAsync(id);
            return Ok();
        }
    }
}
