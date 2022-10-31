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

        public List<PersonInitials> GetInitials()
        {
            List<PersonInitials> personInitials = new List<PersonInitials>();
            personInitials = (List<PersonInitials>)_dataContext.People.Select(p => 
                new PersonInitials { 
                    PersonId = p.PersonId, 
                    Initials = p.FirstName + " " + p.LastName + " " + p.SurName
            }).ToList();
            return personInitials;
        }

        public Person GetPerson(long id)
        {
            return _dataContext.People.Find(id) ?? new Person();
        }
    }
}
