using tl121pet.DAL.Interfaces;
using tl121pet.Entities.Models;

namespace tl121pet.DAL.Data
{
    internal class EFDataRepository : IDataRepository
    {
        private readonly DataContext _dataContext;
        public EFDataRepository(DataContext dataContext) => _dataContext = dataContext;

        public IQueryable<Person> People => _dataContext.People;

        public IQueryable<ProjectTeam> ProjectTeams => _dataContext.ProjectTeams;

        public IQueryable<SkillGroup> SkillGroups => _dataContext.SkillGroups;

        public IQueryable<SkillType> SkillTypes => _dataContext.SkillTypes;

        public IQueryable<Grade> Grades => _dataContext.Grades;
    }
}
