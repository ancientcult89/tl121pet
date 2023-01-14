using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tl121pet.DAL.Data;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

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
            return View("Login", new UserLoginRequestDTO());
        }

        [HttpPost]
        public IActionResult SignUp([FromForm] UserLoginRequestDTO loginRequest)
        {
            string token = LoginApi(loginRequest);
            if (token != "")
            {
                HttpContext.Session.SetString("Token", token);
                return RedirectToRoute(new { controller = "OneToOneDeadline", action = "OneToOneDeadlineList" });
            }
            return View("Login", loginRequest);
        }

        [HttpPost("/api/auth/login")]
        public string LoginApi(UserLoginRequestDTO loginRequest)
        {
            string res = "";
            User? user = _authService.Login(loginRequest);
            if(user != null)
                res = _authService.CreateToken(user);
            return res;
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.SetString("Token", "");
            return RedirectToRoute(new { controller = "OneToOneDeadline", action = "OneToOneDeadlineList" });
        }
    }
}
