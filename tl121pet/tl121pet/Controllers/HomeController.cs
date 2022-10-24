using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tl121pet.Data;
using tl121pet.Entities.Models;

namespace tl121pet.Controllers
{
    public class HomeController : Controller
    {
        //private IPeopleService _peopleService;
        private DataContext _dataContext;
        //private IDataRepository _dataRepository;
        public HomeController(DataContext dataContext)
        {

            _dataContext = dataContext;
            //_dataRepository = dataRepository;
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
        public async Task<IActionResult> Edit([FromForm] Person person)
        {
            if (ModelState.IsValid)
            {                
                _dataContext.People.Update(person);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View("PersonEditor", new Person());
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            var personToDelete = _dataContext.People.Find(id);
            _dataContext.People.Remove(personToDelete);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

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
