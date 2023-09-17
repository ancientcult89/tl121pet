using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using tl121pet.Entities.Models;

namespace tl121pet.DAL.Data
{
    public static class SeedData
    { 
        public static void SeedDatabase(DataContext dataContext)
        {
            dataContext.Database.Migrate();
            if (dataContext.People.Count() == 0)
            {
                ProjectTeam pt1 = new ProjectTeam() { ProjectTeamName = "САБПЭК" };
                ProjectTeam pt2 = new ProjectTeam() { ProjectTeamName = "УРВП" };
                ProjectTeam pt3 = new ProjectTeam() { ProjectTeamName = "ЦИАС" };
                ProjectTeam pt4 = new ProjectTeam() { ProjectTeamName = "СИДЭ" };
                ProjectTeam pt5 = new ProjectTeam() { ProjectTeamName = "СТЕК" };
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

                Role role1 = new Role() { RoleName = "Admin" };
                dataContext.Roles.Add(role1);
                dataContext.SaveChanges(true);
            }

            if (dataContext.Users.Count() == 0)
            {
                Role role1 = new Role() { RoleName = "Admin" };
                dataContext.Roles.Add(role1);
                dataContext.SaveChanges(true);

                CreatePasswordHash("admin", out byte[] passwordHash, out byte[] passwordSalt);
                User newUser = new User
                {
                    Email = "admin@example.com",
                    UserName = "admin",
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Role = role1
                };
                dataContext.Users.Add(newUser);
                dataContext.SaveChanges();
            }
        }


        //duplicate from AuthService
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
