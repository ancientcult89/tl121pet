using tl121pet.Entities.Models;

namespace tl121pet.Services.Interfaces
{
    public interface IPeopleService
    {
        public void CreatePerson(Person person);
        public void UpdatePerson(Person person);
        public void DeletePerson(long id);
    }
}
