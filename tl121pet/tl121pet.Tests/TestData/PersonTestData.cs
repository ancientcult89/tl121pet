using System.Collections.Generic;
using tl121pet.DAL.Data;
using tl121pet.Entities.Models;

namespace tl121pet.Tests.TestData
{
    public class PersonTestData
    {
        public static IEnumerable<object[]> GetSinglePerson()
        {
            yield return new object[]
            {
                new Person
                {
                    PersonId = 1,
                    FirstName = "Vladislav",
                    LastName = "Vladislavov",
                    SurName = "Vladislavovich",
                    ShortName = "Vlad",
                    GradeId = 1,
                },
            };
        }

        public static IEnumerable<Person> GetTestPersons()
        {
            return new List<Person>
            {
                new Person
                {
                    PersonId = 1,
                    FirstName = "Vladislav",
                    LastName = "Vladislavov",
                    SurName = "Vladislavovich",
                    ShortName = "Vlad",
                    GradeId = 1,
                },
                new Person
                {
                    PersonId = 2,
                    FirstName = "Johan",
                    LastName = "Johanson",
                    SurName = "Jack",
                    ShortName = "Johan",
                    GradeId = 1,
                },
            };
        }

        public static void FillTestDatabaseByPersons(DataContext dataContext)
        {
            IEnumerable<Person> persons = (IEnumerable<Person>)GetTestPersons();
            dataContext.People.AddRange(persons);
            dataContext.SaveChanges();
        }
    }
}
