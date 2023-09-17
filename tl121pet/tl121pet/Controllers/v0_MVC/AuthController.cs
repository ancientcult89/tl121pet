using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Controllers.v0_MVC
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
        public async Task<IActionResult> SignUp([FromForm] UserLoginRequestDTO loginRequest)
        {
            string token = await LoginApi(loginRequest);
            if (token != "")
            {
                HttpContext.Session.SetString("Token", token);
                return RedirectToRoute(new { controller = "OneToOneDeadline", action = "OneToOneDeadlineList" });
            }
            return View("Login", loginRequest);
        }

        private async Task<string> LoginApi(UserLoginRequestDTO loginRequest)
        {
            string res = "";
            User? user = await _authService.LoginAsync(loginRequest);
            if(user != null)
                res = await _authService.CreateTokenAsync(user);
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
