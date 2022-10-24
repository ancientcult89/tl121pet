using Microsoft.EntityFrameworkCore;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;
using tl121pet.DAL.Data;

namespace tl121pet.Services.Services
{
    public class PeopleService : IPeopleService
    {
        private DataContext _dataContext;
        public PeopleService(DataContext dataContext)
        { 
            _dataContext = dataContext;
        }
        public List<Person> GetAllPeople()
        {
            return _dataContext.People.ToList();
        }

        public async void CreatePersonAsync(Person person)
        {
            //person.ProjectTeamId = default;
            //_dataContext.People.Add(person);
            //await _dataContext.SaveChangesAsync();
        }

        public Person GetPerson(long id)
        {
            return _dataContext.People.Find(id) ?? new Person();
        }

        public async void UpdatePersonAsync(Person person)
        {
            _dataContext.People.Update(person);
            await _dataContext.SaveChangesAsync();
        }
    }
}
