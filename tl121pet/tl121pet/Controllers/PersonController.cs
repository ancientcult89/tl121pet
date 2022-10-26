using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tl121pet.DAL.Data;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;
using tl121pet.Storage;
using tl121pet.ViewModels;

namespace tl121pet.Controllers
{
    public class PersonController : Controller
    {
        private IPeopleService _peopleService;
        private DataContext _dataContext;
        public PersonController(DataContext dataContext, IPeopleService peopleService)
        {

            _dataContext = dataContext;
            _peopleService = peopleService;
        }
        public IActionResult Index()
        {
            return View(_dataContext.People.Include(p => p.Grade).Include(p => p.ProjectTeam).ToList());
        }

        public IActionResult Details(long id)
        {
            return View("PersonEditor", new PersonVM() { SelectedPerson = _dataContext.People.Find(id) ?? new Person(), Mode = FormMode.Details });
        }

        public IActionResult Edit(long id)
        {
            return View("PersonEditor", new PersonVM() { SelectedPerson = _dataContext.People.Find(id) ?? new Person(), Mode = FormMode.Edit });
        }

        [HttpPost]
        public IActionResult Edit([FromForm] PersonVM personVM)
        {
            if (ModelState.IsValid)
            {
                _peopleService.UpdatePerson(personVM.SelectedPerson);
                return RedirectToAction("Index");
            }
            return View("PersonEditor", personVM);
        }

        [HttpPost]
        public IActionResult Delete(long id)
        {
            _peopleService.DeletePerson(id);
            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {
            return View("PersonEditor", new PersonVM() { SelectedPerson = new Person(), Mode = FormMode.Create });
        }

        [HttpPost]
        public IActionResult Create([FromForm] PersonVM personVM)
        {
            if (ModelState.IsValid)
            {
                _peopleService.CreatePerson(personVM.SelectedPerson);
                return RedirectToAction("Index");
            }
            return View("PersonEditor", new PersonVM() { SelectedPerson = new Person(), Mode = FormMode.Create });
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
