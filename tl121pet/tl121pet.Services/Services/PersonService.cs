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
            var filteredPeople = (
                from p in _dataContext.People
                join up in _dataContext.ProjectMembers on p.PersonId equals up.PersonId
                join gr in _dataContext.Grades on p.GradeId equals gr.GradeId
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
                    p.IsArchive,
                    gr.GradeName
                } into g
                select new Person
                {
                    PersonId = g.Key.PersonId,
                    FirstName = g.Key.FirstName,
                    LastName = g.Key.LastName,
                    SurName = g.Key.SurName,
                    Email = g.Key.Email,
                    ShortName = g.Key.ShortName,
                    GradeId = g.Key.GradeId,
                    IsArchive = g.Key.IsArchive,
                    Grade = new Grade { 
                        GradeId = g.Key.GradeId,
                        GradeName = g.Key.GradeName,
                    }
                }
            ).ToListAsync();

            return await filteredPeople;
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
