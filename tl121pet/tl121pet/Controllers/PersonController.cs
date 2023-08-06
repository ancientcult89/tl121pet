using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;
using tl121pet.Storage;
using tl121pet.ViewModels;

namespace tl121pet.Controllers
{
    [Authorize]
    public class PersonController : Controller
    {
        private IPersonService _personService;
        public PersonController(IPersonService _peopleService)
        {
            _personService = _peopleService;
        }
        public IActionResult PeopleList()
        {
            return View("PeopleList", _personService.GetPeopleWithGradeAsync());
        }

        public async Task<IActionResult> Details(long id)
        {
            return View("PersonEditor", new SimpleEditFormVM<Person>()
            {
                SelectedItem = await _personService.GetPersonByIdAsync(id),
                Mode = FormMode.Details
            });
        }

        public async Task<IActionResult> Edit(long id)
        {
            return View("PersonEditor", new SimpleEditFormVM<Person>()
            {
                SelectedItem = await _personService.GetPersonByIdAsync(id),
                Mode = FormMode.Edit
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] SimpleEditFormVM<Person> personVM)
        {
            if (ModelState.IsValid)
            {
                await _personService.UpdatePersonAsync(personVM.SelectedItem);
                return View("PersonEditor", personVM);
            }
            return View("PersonEditor", personVM);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            await _personService.DeletePersonAsync(id);
            return RedirectToAction("PeopleList");
        }

        public IActionResult Create()
        {
            return View("PersonEditor", new SimpleEditFormVM<Person>()
            {
                SelectedItem = new Person(),
                Mode = FormMode.Create
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] SimpleEditFormVM<Person> personVM)
        {
            if (ModelState.IsValid)
            {
                await _personService.CreatePersonAsync(personVM.SelectedItem);
                personVM.Mode = FormMode.Edit;
                return View("PersonEditor", personVM);
            }
            personVM.Mode = FormMode.Create;
            return View("PersonEditor", personVM);
        }
    }
}
