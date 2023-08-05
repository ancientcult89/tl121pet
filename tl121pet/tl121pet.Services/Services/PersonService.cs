using Microsoft.EntityFrameworkCore;
using tl121pet.DAL.Data;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{
    public class PersonService : IPersonService
    {
        private readonly IAuthService _authService;
        private DataContext _dataContext;

        public PersonService(IAuthService authService, DataContext dataContext)
        {
            _authService = authService;
            _dataContext = dataContext;
        }

        public async Task<List<PersonInitials>> GetInitialsAsync()
        {
            List<PersonInitials> personInitials = new List<PersonInitials>();
            List<Person> people = await GetPeopleAsync();
            personInitials = (List<PersonInitials>)people.Select(p =>
                new PersonInitials
                {
                    PersonId = p.PersonId,
                    Initials = p.LastName + " " + p.FirstName + " " + p.SurName
                }).ToList();
            return personInitials;
        }

        public async Task<List<Person>> GetPeopleAsync()
        {
            List<Person> people = new List<Person>();
            long? userId = _authService.GetMyUserId();
            List<ProjectTeam> projects = new List<ProjectTeam>();
            if (userId != null)
            {
                projects = await _authService.GetUserProjectsAsync((long)userId);
                people = await GetPeopleFilteredByProjectsAsync(projects);
            }

            return people;
        }

        private async Task<List<Person>> GetPeopleFilteredByProjectsAsync(List<ProjectTeam> projects)
        {
            List<Person> peopleFiltered = new List<Person>();
            foreach (ProjectTeam pt in projects)
            {
                peopleFiltered.AddRange(await GetPeopleFilteredByProjectAsync(pt.ProjectTeamId));
            }

            peopleFiltered = peopleFiltered.Distinct(new PersonComparer()).ToList();

            return peopleFiltered;
        }

        public async Task<List<Grade>> GetAllGradesAsync()
        {
            return await _dataContext.Grades.ToListAsync();
        }

        public async Task<string> GetGradeNameAsync(long id)
        {
            Grade selectedGrade = await _dataContext.Grades.FindAsync(id);
            return selectedGrade.GradeName ?? "not found";
        }

        public async Task<Grade> CreateGradeAsync(Grade grade)
        {
            _dataContext.Grades.Add(grade);
            await _dataContext.SaveChangesAsync();
            return grade;
        }

        public async Task<Grade> UpdateGradeAsync(Grade grade)
        {
            _dataContext.Grades.Update(grade);
            await _dataContext.SaveChangesAsync();
            return grade;
        }

        public async Task DeleteGradeAsync(long id)
        {
            var gradeToDelete = _dataContext.Grades.Find(id);
            _dataContext.Grades.Remove(gradeToDelete);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<Grade> GetGradeByIdAsync(long id)
        {
            return await _dataContext.Grades.FindAsync(id) ?? new Grade();
        }

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
                group p by new
                {
                    p.PersonId,
                    p.FirstName,
                    p.LastName,
                    p.SurName,
                    p.Email,
                    p.ShortName,
                    p.GradeId
                } into g
                select new
                {
                    PersonId = g.Key.PersonId,
                    FirstName = g.Key.FirstName,
                    LastName = g.Key.LastName,
                    SurName = g.Key.SurName,
                    Email = g.Key.Email,
                    ShortName = g.Key.ShortName,
                    GradeId = g.Key.GradeId
                }
            ).ToListAsync();

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
                    GradeId = p.GradeId
                };
                filteredPeople.Add(person);
            }

            return filteredPeople;
        }

        public async Task<List<Person>> GetPeopleWithGradeAsync()
        {
            return await _dataContext.People.Include(p => p.Grade).ToListAsync();
        }
    }
}
