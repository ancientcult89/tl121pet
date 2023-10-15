using System.Collections.Generic;
using tl121pet.DAL.Data;
using tl121pet.Entities.Models;

namespace tl121pet.Tests.TestData
{
    public class RoleTestData
    {
        public static Role GetSingleRole()
        {
            return new Role
            {
                RoleId = 1,
                RoleName = "Admin"
            };
        }

        public static Role GetTestRole()
        {
            return new Role
            {
                RoleId = 2,
                RoleName = "Test"
            };
        }

        public static Role GetAnotherTestRole()
        {
            return new Role
            {
                RoleId = 3,
                RoleName = "Test2"
            };
        }
        public static Role GetAnotherSingleRole()
        {
            return new Role
            {
                RoleId = 2,
                RoleName = "Admin"
            };
        }

        public static IEnumerable<Role> GetTestRoles()
        {
            return new List<Role>
            {
                new Role
                {
                    RoleId = 1,
                    RoleName = "Admin"
                },
                new Role
                {
                    RoleId = 2,
                    RoleName = "TestRole"
                }
            };
        }

        public static void FillTestDatabaseByGrades(DataContext dataContext)
        {
            IEnumerable<Role> roles = (IEnumerable<Role>)GetTestRoles();
            dataContext.Roles.AddRange(roles);
            dataContext.SaveChanges();
        }
    }
}
