﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Extensions;
using tl121pet.Entities.Infrastructure;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;
using tl121pet.Services.Services;

namespace tl121pet.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController(IAuthService authService, IOneToOneApplication oneToOneApplication) : Controller
    {
        private readonly IAuthService _authService = authService;
        private readonly IOneToOneApplication _oneToOneApplication = oneToOneApplication;

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
        public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] UserLoginRequestDTO loginRequest)
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

        [HttpPost("recoverypassword")]
        public async Task<ActionResult> RecoverPassword([FromBody] RecoverPasswordRequestDTO recoverPasswordRequest)
        {
            try
            {
                await _oneToOneApplication.RecoverPasswordAsync(recoverPasswordRequest);
                return Ok();
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

        [Authorize]
        [HttpPost("changelocale")]
        public async Task<ActionResult> ChangeLocale([FromBody] ChangeLocaleRequestDTO request)
        {
            await _oneToOneApplication.ChangeLocaleAsync(request.Locale);
            return Ok();
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
