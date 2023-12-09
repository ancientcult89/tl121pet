using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace tl121pet.Controllers
{
    [Authorize]
    [ApiController]
    public class ApiController : ControllerBase
    {
    }
}
