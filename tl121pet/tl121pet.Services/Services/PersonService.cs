using Microsoft.EntityFrameworkCore;
using tl121pet.DAL.Data;
using tl121pet.DAL.Interfaces;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{
    public class PersonService : IPersonService
    {
        private readonly IAuthService _authService;
        private readonly IAdminRepository _adminRepository;
        private readonly IPeopleRepository _peopleRepository;
        private DataContext _dataContext;

        public PersonService(IAuthService authService, IAdminRepository adminRepository, IPeopleRepository peopleRepository, DataContext dataContext)
        {
            _authService = authService;
            _adminRepository = adminRepository;
            _peopleRepository = peopleRepository;
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
                projects = await _adminRepository.GetUserProjectsAsync((long)userId);
                people = await GetPeopleFilteredByProjectsAsync(projects);
            }

            return people;
        }

        private async Task<List<Person>> GetPeopleFilteredByProjectsAsync(List<ProjectTeam> projects)
        {
            List<Person> peopleFiltered = new List<Person>();
            foreach (ProjectTeam pt in projects)
            {
                peopleFiltered.AddRange(await _peopleRepository.GetPeopleFilteredByProjectAsync(pt.ProjectTeamId));
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

        public async Task CreateGradeAsync(Grade grade)
        {
            _dataContext.Grades.Add(grade);
            await _dataContext.SaveChangesAsync();
        }

        public async Task UpdateGradeAsync(Grade grade)
        {
            _dataContext.Grades.Update(grade);
            await _dataContext.SaveChangesAsync();
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
    }
}
