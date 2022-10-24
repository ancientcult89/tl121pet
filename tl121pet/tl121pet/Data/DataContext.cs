using Microsoft.EntityFrameworkCore;
using tl121pet.Entities.Models;

namespace tl121pet.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> opts) : base(opts) { }
        public DbSet<Person> People { get; set; }
        public DbSet<ProjectTeam> ProjectTeams { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<SkillGroup> SkillGroups { get; set; }
        public DbSet<SkillType> SkillTypes { get; set; }
        public DbSet<Grade> Grades { get; set; }
    }
}
