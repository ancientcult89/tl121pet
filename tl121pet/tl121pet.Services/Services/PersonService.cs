using tl121pet.DAL.Data;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{
    public class PersonService : IPersonService
    {
        private readonly DataContext _dataContext;
        private readonly IAuthService _authService;

        public PersonService(DataContext dataContext, IAuthService authService)
        {
            _dataContext = dataContext;
            _authService = authService;
        }

        public List<PersonInitials> GetInitials()
        {
            List<PersonInitials> personInitials = new List<PersonInitials>();
            personInitials = (List<PersonInitials>)GetPeople().Select(p =>
                new PersonInitials
                {
                    PersonId = p.PersonId,
                    Initials = p.FirstName + " " + p.LastName + " " + p.SurName
                }).ToList();
            return personInitials;
        }

        public List<Person> GetPeople()
        {
            List<Person> people = new List<Person>();
            long? userId = _authService.GetMyUserId();

            var persons = from up in _dataContext.UserProjects
                          join pp in _dataContext.ProjectMembers on up.ProjectTeamId equals pp.ProjectTeamId
                          join p in _dataContext.People on pp.PersonId equals p.PersonId
                          where up.UserId == userId
                          select p;

            foreach (var person in persons)
            {
                if(!people.Contains(person))
                    people.Add(person);
            }

            return people;
        }
    }
}
