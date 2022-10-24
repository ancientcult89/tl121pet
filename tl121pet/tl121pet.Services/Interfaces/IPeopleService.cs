using tl121pet.Entities.Models;

namespace tl121pet.Services.Interfaces
{
    public interface IPeopleService
    {
        public List<Person> GetAllPeople();

        public void CreatePersonAsync(Person person);
        public Person GetPerson(long id);

        public void UpdatePersonAsync(Person person);
    }
}
