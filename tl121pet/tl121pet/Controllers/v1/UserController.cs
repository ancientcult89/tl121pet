using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.DTO;
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
        public async Task<ActionResult<List<User>>> GetUserList()
        {
            List<User> respond = await _authService.GetUserListAsync();
            return respond;
        }

        [HttpPost("register")]
        public async Task<ActionResult<int>> CreateUser()
        {
            throw new NotImplementedException();
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] UserLoginRequestDTO loginRequest)
        {
            string res = "";
            User? user = await _authService.LoginAsync(loginRequest);
            if (user != null)
            {
                res = await _authService.CreateTokenAsync(user);
                return res;
            }
            else return BadRequest("Wrong password or username");
        }

        [HttpPut("changepassword")]
        public async Task<ActionResult> ChangePassword(int id)
        {
            throw new NotImplementedException();
        }
    }
}
