using tl121pet.DAL.Data;

namespace tl121pet.Tests.TestData
{
    public static class DataBaseTestContextCreator
    {
        public static void CreateTestContext(DataContext dataContext)
        {
            GradeTestData.FillTestDatabaseByGrades(dataContext);
            PersonTestData.FillTestDatabaseByPersons(dataContext);
        }
    }
}
