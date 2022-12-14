using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.DAL.Interfaces
{
    public interface IPeopleRepository
    {
        public void CreatePerson(Person person);
        public void UpdatePerson(Person person);
        public void DeletePerson(long id);
        public List<Person> GetPeople();
        public List<Person> GetPeopleFilteredByProject(long projectTeam);
        public Person GetPerson(long id);
    }
}
