using Microsoft.EntityFrameworkCore;
using tl121pet.Entities.Models;

namespace tl121pet.DAL.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> opts) : base(opts) { }

        public DbSet<Person> People => Set<Person>();
        public DbSet<ProjectTeam> ProjectTeams => Set<ProjectTeam>();
        public DbSet<SkillGroup> SkillGroups => Set<SkillGroup>();
        public DbSet<SkillType> SkillTypes => Set<SkillType>();
        public DbSet<Grade> Grades => Set<Grade>();
    }
}
