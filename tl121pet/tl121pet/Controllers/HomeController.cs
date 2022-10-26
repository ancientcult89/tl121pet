using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tl121pet.DAL.Data;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;
using tl121pet.Storage;
using tl121pet.ViewModels;

namespace tl121pet.Controllers
{
    public class HomeController : Controller
    {
        public RedirectToRouteResult Index()
        {
            return RedirectToRoute(new { controller = "Person", action="Index" });
        }
    }
}
