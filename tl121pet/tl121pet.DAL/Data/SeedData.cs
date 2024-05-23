using Microsoft.EntityFrameworkCore;
using tl121pet.Entities.Models;
using tl121pet.Entities.Infrastructure;

namespace tl121pet.DAL.Data
{
    public static class SeedData
    { 
        public static void SeedDatabase(DataContext dataContext, bool useInMemory, CreatePasswordDelegate createPassword)
        {
            if (!useInMemory)
            {
                dataContext.Database.Migrate();                
            }

            ProjectTeam pt1 = new ProjectTeam() { ProjectTeamName = "САБПЭК" };
            ProjectTeam pt2 = new ProjectTeam() { ProjectTeamName = "УРВП" };
            ProjectTeam pt3 = new ProjectTeam() { ProjectTeamName = "ЦИАС" };
            ProjectTeam pt4 = new ProjectTeam() { ProjectTeamName = "СИДЭ" };
            ProjectTeam pt5 = new ProjectTeam() { ProjectTeamName = "СТЕК" };
            if (dataContext.ProjectTeams.Count() == 0)
            {
                dataContext.ProjectTeams.AddRange(pt1, pt2, pt3, pt4, pt5);
                dataContext.SaveChanges();
            }

            Grade g1 = new Grade() { GradeName = "Junior entry" };
            Grade g2 = new Grade() { GradeName = "Junior intermediate" };
            Grade g3 = new Grade() { GradeName = "Junior plus" };
            Grade g4 = new Grade() { GradeName = "Trainee" };
            if (dataContext.Grades.Count() == 0)
            {
                dataContext.Grades.AddRange(g1, g2, g3, g4);
                dataContext.SaveChanges();
            }
            if (dataContext.ProjectTeams.Count() == 0)
            {
                Person p1 = new Person() { FirstName = "John", LastName = "Smith", Grade = g4 };
                dataContext.People.Add(p1);
                dataContext.SaveChanges();
            }

            Role role1 = new Role() { RoleName = "Admin" };
            if (dataContext.Roles.Count() == 0)
            {
                dataContext.Roles.Add(role1);
                dataContext.SaveChanges(true);
            }

            createPassword("admin", out byte[] passwordHash, out byte[] passwordSalt);
            User newUser = new User
            {
                Email = "admin@example.com",
                UserName = "admin",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = role1
            };
            if (dataContext.Users.Count() == 0)
            {
                dataContext.Users.Add(newUser);
                dataContext.SaveChanges();
            }
        }
    }
}
