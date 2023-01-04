using tl121pet.DAL.Data;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.DAL.Repositories
{
    public class PeopleRepository : IPeopleRepository
    {
        private DataContext _dataContext;
        public PeopleRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public List<Person> People => _dataContext.People.ToList();

        public void CreatePerson(Person person)
        {
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

        public List<Person> GetPeople()
        {
            return _dataContext.People.ToList();
        }

        public Person GetPerson(long id)
        {
            return _dataContext.People.Find(id) ?? new Person();
        }

        public List<Person> GetPeopleFilteredByProject(long projectTeam)
        {
            List<Person> filteredPeople = new List<Person>();

            var people = from p in _dataContext.People
                         join up in _dataContext.ProjectMembers on p.PersonId equals up.PersonId
                         where up.ProjectTeamId == projectTeam
                         select p;

            foreach(var p in people)
            {
                filteredPeople.Add(p);
            }

            return filteredPeople;
        }
    }
}
