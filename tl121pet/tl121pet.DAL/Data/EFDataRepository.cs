using tl121pet.DAL.Interfaces;
using tl121pet.Entities.Models;

namespace tl121pet.DAL.Data
{
    public class EFDataRepository : IDataRepository
    {
        private readonly DataContext _dataContext;
        public EFDataRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IQueryable<Person> People => _dataContext.People;

        public IQueryable<ProjectTeam> ProjectTeams => _dataContext.ProjectTeams;

        public IQueryable<SkillGroup> SkillGroups => _dataContext.SkillGroups;

        public IQueryable<SkillType> SkillTypes => _dataContext.SkillTypes;

        public IQueryable<Grade> Grades => _dataContext.Grades;

        public IQueryable<Skill> Skills => _dataContext.Skills;

        public async void DeletePersonAsync(Person person)
        { 
            _dataContext.People.Remove(person);
            await _dataContext.SaveChangesAsync(true);
        }
    }
}
