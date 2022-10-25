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

        public List<Person> People => _dataContext.People.ToList();

        public void CreatePerson(Person person)
        {
            //person.ProjectTeamId = default;
            _dataContext.People.Add(person);
            _dataContext.SaveChanges();
        }

        public void UpdatePerson(Person person)
        {
            _dataContext.People.Update(person);
            _dataContext.SaveChanges();
        }

        public void DeletePerson(long id)
        {
            var personToDelete = _dataContext.People.Find(id);
            _dataContext.People.Remove(personToDelete);
            _dataContext.SaveChanges();            
        }
    }
}
