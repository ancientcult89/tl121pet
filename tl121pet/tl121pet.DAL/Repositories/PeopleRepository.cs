using Microsoft.EntityFrameworkCore;
using tl121pet.DAL.Data;
using tl121pet.DAL.Interfaces;
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

        public async Task CreatePersonAsync(Person person)
        {
            _dataContext.People.Add(person);
            await _dataContext.SaveChangesAsync();
        }

        public async Task UpdatePersonAsync(Person person)
        {
            _dataContext.People.Update(person);
            await _dataContext.SaveChangesAsync();
        }

        public async Task DeletePersonAsync(long id)
        {
            var personToDelete = _dataContext.People.Find(id);
            _dataContext.People.Remove(personToDelete);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<List<Person>> GetAllPeopleAsync()
        {
            return await _dataContext.People.ToListAsync();
        }

        public async Task<Person> GetPersonAsync(long id)
        {
            return await _dataContext.People.FindAsync(id) ?? new Person();
        }

        public async Task<List<Person>> GetPeopleFilteredByProjectAsync(long projectTeam)
        {
            List<Person> filteredPeople = new List<Person>();

            var people = (
                from p in _dataContext.People
                join up in _dataContext.ProjectMembers on p.PersonId equals up.PersonId
                where up.ProjectTeamId == projectTeam
                group p by new { 
                    p.PersonId
                    , p.FirstName 
                    , p.LastName
                    , p.SurName
                    , p.Email
                    , p.ShortName
                    , p.GradeId
                } into g
                select new {
                    PersonId = g.Key.PersonId,
                    FirstName = g.Key.FirstName,
                    LastName = g.Key.LastName,
                    SurName = g.Key.SurName,
                    Email = g.Key.Email,
                    ShortName = g.Key.ShortName,
                    GradeId = g.Key.GradeId
                }
            ).ToListAsync();

            foreach(var p in await people)
            {
                Person person = new Person()
                {
                    PersonId = p.PersonId,
                    Email = p.Email,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    SurName = p.SurName,
                    ShortName = p.ShortName,
                    GradeId = p.GradeId
                };
                filteredPeople.Add(person);
            }

            return filteredPeople;
        }
    }
}
