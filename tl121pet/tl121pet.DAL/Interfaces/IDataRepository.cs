using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tl121pet.Entities.Models;

namespace tl121pet.DAL.Interfaces
{
    public interface IDataRepository
    {
        public IQueryable<Person> People { get; }
        public IQueryable<ProjectTeam> ProjectTeams { get; }
        public IQueryable<SkillGroup> SkillGroups { get; }
        public IQueryable<SkillType> SkillTypes { get; }
        public IQueryable<Grade> Grades { get; }
        public IQueryable<Skill> Skills { get; }
    }
}
