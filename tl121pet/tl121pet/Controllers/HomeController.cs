using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tl121pet.DAL.Data;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Controllers
{
    public class HomeController : Controller
    {
        private IPeopleService _peopleService;
        private DataContext _dataContext;
        public HomeController(DataContext dataContext, IPeopleService peopleService)
        {

            _dataContext = dataContext;
            _peopleService = peopleService;
        }
        public IActionResult Index()
        {
            return View(_dataContext.People.Include(p => p.Grade).Include(p => p.ProjectTeam).ToList());
        }

        public IActionResult Edit(long id)
        {
            return View("PersonEditor", _dataContext.People.Find(id));
        }

        [HttpPost]
        public IActionResult Edit([FromForm] Person person)
        {
            if (ModelState.IsValid)
            {                
                _peopleService.UpdatePerson(person);
                return RedirectToAction("Index");
            }
            return View("PersonEditor", new Person());
        }

        [HttpPost]
        public IActionResult Delete(long id)
        {
            _peopleService.DeletePerson(id);
            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {
            return View("PersonCreator", new Person());
        }

        [HttpPost]
        public IActionResult Create([FromForm] Person person)
        {
            if (ModelState.IsValid)
            {
                _peopleService.CreatePerson(person);
                return RedirectToAction("Index");
            }
            return View("PersonEditor", new Person());
        }


        //test db endpoint
        [HttpDelete("/api/DeletePerson/{id}")]
        public string DeletePerson(long id)
        { 
            var person = _dataContext.People.Find(id);
            _dataContext.People.Remove(person);
            _dataContext.SaveChanges();
            return "Ok";
        }
    }
}
