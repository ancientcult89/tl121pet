using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tl121pet.DAL.Data;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.Models;
using tl121pet.Storage;
using tl121pet.ViewModels;

namespace tl121pet.Controllers
{
    public class PersonController : Controller
    {
        private IPeopleRepository _peopleRepository;
        private DataContext _dataContext;
        public PersonController(DataContext dataContext, IPeopleRepository peopleRepository)
        {

            _dataContext = dataContext;
            _peopleRepository = peopleRepository;
        }
        public IActionResult Index()
        {
            return View("PeopleList", _dataContext.People.Include(p => p.Grade).ToList());
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
                _peopleRepository.UpdatePerson(personVM.SelectedPerson);
                return RedirectToAction("Index");
            }
            return View("PersonEditor", personVM);
        }

        [HttpPost]
        public IActionResult Delete(long id)
        {
            _peopleRepository.DeletePerson(id);
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
                _peopleRepository.CreatePerson(personVM.SelectedPerson);
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
