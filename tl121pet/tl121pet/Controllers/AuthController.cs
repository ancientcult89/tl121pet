using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tl121pet.DAL.Data;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;
using tl121pet.Storage;
using tl121pet.ViewModels;

namespace tl121pet.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        public IActionResult Login()
        {
            //dev temp code
            _authService.Register(new UserRegisterRequest
            {
                Email = "ancientcult89@gmail.com",
                ConfirmPassword = "secret",
                Password = "secret",
                UserName = "ancientcult"
            });


            return View("Login", new UserLoginRequest());
        }

        [HttpPost]
        public IActionResult SignUp([FromForm] UserLoginRequest loginRequest)
        {
            string token = Login(loginRequest);
            if (token != "")
            { 
                return RedirectToRoute(new { controller = "OneToOneDeadline", action = "OneToOneDeadlineList" });
            }
            return View("Login", loginRequest);
        }

        [HttpPost("/api/auth/login")]
        public string Login(UserLoginRequest loginRequest)
        {
            string res = "";
            User? user = _authService.Login(loginRequest);
            if(user != null)
                res = _authService.CreateToken(user);
            return res;
        }
    }
}
