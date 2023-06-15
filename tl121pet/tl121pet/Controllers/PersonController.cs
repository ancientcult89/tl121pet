using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tl121pet.DAL.Data;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.Models;
using tl121pet.Storage;
using tl121pet.ViewModels;

namespace tl121pet.Controllers
{
    [Authorize]
    public class PersonController : Controller
    {
        private IPeopleRepository _peopleRepository;
        private DataContext _dataContext;
        public PersonController(DataContext dataContext, IPeopleRepository peopleRepository)
        {

            _dataContext = dataContext;
            _peopleRepository = peopleRepository;
        }
        public IActionResult PeopleList()
        {
            return View("PeopleList", GetPeopleList());
        }

        public IActionResult Details(long id)
        {
            return View("PersonEditor", new SimpleEditFormVM<Person>() { 
                SelectedItem = _dataContext.People.Find(id) ?? new Person(),
                Mode = FormMode.Details });
        }

        public IActionResult Edit(long id)
        {
            return View("PersonEditor", new SimpleEditFormVM<Person>() { 
                SelectedItem = _dataContext.People.Find(id) ?? new Person(),
                Mode = FormMode.Edit });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] SimpleEditFormVM<Person> personVM)
        {
            if (ModelState.IsValid)
            {
                await _peopleRepository.UpdatePersonAsync(personVM.SelectedItem);                
                return View("PersonEditor", personVM);
            }
            return View("PersonEditor", personVM);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            await _peopleRepository.DeletePersonAsync(id);
            return RedirectToAction("PeopleList");
        }

        public IActionResult Create()
        {
            return View("PersonEditor", new SimpleEditFormVM<Person>() { 
                SelectedItem = new Person(),
                Mode = FormMode.Create });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] SimpleEditFormVM<Person> personVM)
        {
            if (ModelState.IsValid)
            {
                await _peopleRepository.CreatePersonAsync(personVM.SelectedItem);
                personVM.Mode = FormMode.Edit;
                return View("PersonEditor", personVM);
            }
            personVM.Mode = FormMode.Create;
            return View("PersonEditor", personVM);
        }


        //test db endpoint
        [HttpDelete("/api/person/deleteperson/{id}")]
        public string DeletePerson(long id)
        {
            var person = _dataContext.People.Find(id);
            _dataContext.People.Remove(person);
            _dataContext.SaveChanges();
            return "Ok";
        }

        [Authorize]
        [HttpGet("/api/person/peoplelist")]
        public List<Person> GetPeopleList()
        {
            return _dataContext.People.Include(p => p.Grade).ToList();
        }
    }
}
