using Microsoft.EntityFrameworkCore;
using tl121pet.Entities.Models;

namespace tl121pet.DAL.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> opts) : base(opts) {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }

        public DbSet<Person> People => Set<Person>();
        public DbSet<ProjectTeam> ProjectTeams => Set<ProjectTeam>();
        public DbSet<Skill> Skills => Set<Skill>();
        public DbSet<SkillGroup> SkillGroups => Set<SkillGroup>();
        public DbSet<SkillType> SkillTypes => Set<SkillType>();
        public DbSet<Grade> Grades => Set<Grade>();
        public DbSet<Meeting> Meetings => Set<Meeting>();
        public DbSet<MeetingType> MeetingTypes => Set<MeetingType>();
        public DbSet<MeetingGoal> MeetingGoals => Set<MeetingGoal>();
        public DbSet<MeetingNote> MeetingNotes => Set<MeetingNote>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    }
}
