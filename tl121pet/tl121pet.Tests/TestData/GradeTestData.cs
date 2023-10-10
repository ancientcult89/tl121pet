using System.Collections.Generic;
using tl121pet.DAL.Data;
using tl121pet.Entities.Models;

namespace tl121pet.Tests.TestData
{
    public class GradeTestData
    {
        public static Grade GetSingleGrade()
        {
            return new Grade
                {
                    GradeId = 1,
                    GradeName = "Junior"
                };
        }
        public static Grade GetAnotherSingleGrade()
        {
            return new Grade
            {
                GradeId = 2,
                GradeName = "Junior"
            };
        }

        public static IEnumerable<Grade> GetTestGrades()
        {
            return new List<Grade>
            {
                new Grade
                {
                    GradeId = 1,
                    GradeName = "Junior entry"
                },
                new Grade
                {
                    GradeId = 2,
                    GradeName = "Junior intermediate"
                },
                new Grade
                {
                    GradeId = 3,
                    GradeName = "Junior plus"
                },
                new Grade
                {
                    GradeId = 4,
                    GradeName = "Trainee"
                },
            };
        }

        public static void FillTestDatabaseByGrades(DataContext dataContext)
        {
            IEnumerable<Grade> grades = (IEnumerable<Grade>)GetTestGrades();
            dataContext.Grades.AddRange(grades);
            dataContext.SaveChanges();
        }
    }
}
