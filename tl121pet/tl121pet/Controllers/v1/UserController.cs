﻿using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("register")]
        public async Task<ActionResult<int>> Create()
        {
            //return await Mediator.Send(command);
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
            //if (id != command.Id)
            //    return BadRequest();

            //await Mediator.Send(command);

            //return NoContent();
            throw new NotImplementedException();
        }
    }
}
