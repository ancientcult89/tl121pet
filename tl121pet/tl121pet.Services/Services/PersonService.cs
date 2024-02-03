using Microsoft.EntityFrameworkCore;
using tl121pet.DAL.Data;
using tl121pet.Entities.Infrastructure.Exceptions;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{
    public class PersonService : IPersonService
    {
        private DataContext _dataContext;

        public PersonService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Person>> GetPeopleFilteredByProjectsAsync(List<ProjectTeam> projects)
        {
            List<Person> peopleFiltered = new List<Person>();
            foreach (ProjectTeam pt in projects)
            {
                peopleFiltered.AddRange(await GetPeopleFilteredByProjectAsync(pt.ProjectTeamId));
            }

            peopleFiltered = peopleFiltered.Distinct(new PersonComparer()).ToList();

            return peopleFiltered;
        }

        public async Task<Person> CreatePersonAsync(Person person)
        {
            await CheckPersonExistsByEmail(person);

            _dataContext.People.Add(person);
            await _dataContext.SaveChangesAsync();
            return person;
        }

        public async Task<Person> UpdatePersonAsync(Person person)
        {
            var modifiedPerson = await GetPersonByIdAsync(person.PersonId);
            await CheckPersonExistsByEmail(person);

            _dataContext.Entry(modifiedPerson).CurrentValues.SetValues(person);
            await _dataContext.SaveChangesAsync();
            return person;
        }

        public async Task DeletePersonAsync(long id)
        {
            await GetPersonByIdAsync(id);
            var personToDelete = _dataContext.People.Find(id);
            _dataContext.People.Remove(personToDelete);
            await _dataContext.SaveChangesAsync();
        }

        public async Task ArchivePersonAsync(long id)
        {
            Person archivedPerson = await GetPersonByIdAsync(id);
            archivedPerson.IsArchive = true;
            await _dataContext.SaveChangesAsync();
        }

        public async Task<Person> GetPersonByIdAsync(long id)
        {
            return await _dataContext.People.FindAsync(id) ?? throw new DataFoundException("Person not found");
        }

        public async Task<List<Person>> GetPeopleFilteredByProjectAsync(long projectTeam)
        {
            List<Person> filteredPeople = new List<Person>();

            var people = (
                from p in _dataContext.People
                join up in _dataContext.ProjectMembers on p.PersonId equals up.PersonId
                where up.ProjectTeamId == projectTeam && p.IsArchive == false
                group p by new
                {
                    p.PersonId,
                    p.FirstName,
                    p.LastName,
                    p.SurName,
                    p.Email,
                    p.ShortName,
                    p.GradeId,
                } into g
                select new
                {
                    PersonId = g.Key.PersonId,
                    FirstName = g.Key.FirstName,
                    LastName = g.Key.LastName,
                    SurName = g.Key.SurName,
                    Email = g.Key.Email,
                    ShortName = g.Key.ShortName,
                    GradeId = g.Key.GradeId,
                }
            ).ToListAsync();

            //TODO: не очень оптимальная штука, можно выше получить грейд
            foreach (var p in await people)
            {
                Person person = new Person()
                {
                    PersonId = p.PersonId,
                    Email = p.Email,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    SurName = p.SurName,
                    ShortName = p.ShortName,
                    GradeId = p.GradeId,
                    Grade = _dataContext.Grades.Where(g => g.GradeId == p.GradeId).FirstOrDefault()
                };
                filteredPeople.Add(person);
            }

            return filteredPeople;
        }

        public async Task<List<Person>> GetPeopleWithGradeAsync()
        {
            return await _dataContext.People.Include(p => p.Grade).Where(p => p.IsArchive == false).ToListAsync();
        }

        private async Task CheckPersonExistsByEmail(Person person)
        {
            var examPerson = await _dataContext.People
                .AsNoTracking()
                .Where(r => r.Email == person.Email && r.PersonId != person.PersonId)
                .FirstOrDefaultAsync();

            if (examPerson != null)
                throw new LogicException("A Person with same Email is already exists");
        }
    }
}
