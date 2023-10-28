using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using tl121pet.DAL.Data;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;
using tl121pet.Services.Services;
using Xunit;

namespace tl121pet.Tests
{
    public class PersonServiceTests : IDisposable
    {
        private readonly DataContext _dataContext;
        private readonly IPersonService _personService;
        public PersonServiceTests()
        {
            var connectionString = "Server=host.docker.internal;Database=TLPersonTest;Port=49153;User Id=postgres;Password=postgrespw";
            var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseNpgsql(connectionString)
                .Options;

            _dataContext = new DataContext(dbContextOptions);
            _dataContext.Database.EnsureDeleted();
            _dataContext.Database.EnsureCreated();
            _personService = new PersonService(_dataContext);
        }
        public void Dispose()
        {
            _dataContext.Database.EnsureDeleted();
        }

        /// <summary>
        /// Проверка, что GetAllPeopleAsync возвращает всех сотрудников
        /// </summary>
        [Fact]
        public async void GetAllPeopleAsync_ShouldReturnAllPersons()
        {
            //Arrange
            Grade testGrade = new Grade
            {
                GradeId = 1,
                GradeName = "Junior"
            };
            _dataContext.Grades.Add(testGrade);
            _dataContext.SaveChanges();

            List<Person> people = new List<Person>();
            people.AddRange(new List<Person> {
                new Person {
                    Email = "1111@test.com",
                    FirstName = "Eric",
                    LastName = "Cripke",
                    GradeId = testGrade.GradeId,
                    PersonId = 1,
                    ShortName = "Rick",
                    SurName = "Rickson",
                    Grade = testGrade
                },
                new Person {
                    Email = "1111@test1.com",
                    FirstName = "John",
                    LastName = "Bon",
                    GradeId = testGrade.GradeId,
                    PersonId = 2,
                    ShortName = "Jovi",
                    SurName = "Jovison",
                    Grade = testGrade
                },
            });
            _dataContext.People.AddRange(people);
            _dataContext.SaveChanges();

            List<Person> peopleExpected = new List<Person>();
            peopleExpected.AddRange(new List<Person> {
                new Person {
                    Email = "1111@test.com",
                    FirstName = "Eric",
                    LastName = "Cripke",
                    GradeId = testGrade.GradeId,
                    PersonId = 1,
                    ShortName = "Rick",
                    SurName = "Rickson",
                    Grade = testGrade
                },
                new Person {
                    Email = "1111@test1.com",
                    FirstName = "John",
                    LastName = "Bon",
                    GradeId = testGrade.GradeId,
                    PersonId = 2,
                    ShortName = "Jovi",
                    SurName = "Jovison",
                    Grade = testGrade
                },
            });

            //Act
            var resultPeople = await _personService.GetAllPeopleAsync();

            //Assert
            resultPeople.Should().BeEquivalentTo(peopleExpected);
        }

        /// <summary>
        /// Проверка, что GetAllPeopleAsync возвращает пустую коллекцию и не вываливается в ошибку, если нет ни одной записи
        /// </summary>
        [Fact]
        public async void GetAllPeopleAsync_ShouldReturnEmptyListIfPersonsNotExists()
        {
            //Arrange
            List<Person> expectPeople = new();

            //Act
            IEnumerable<Person> people = await _personService.GetAllPeopleAsync();

            //Assert
            expectPeople.Should().BeEquivalentTo(people);
        }

        /// <summary>
        /// проверяем, что CreatePersonAsync создаёт корректную запись сотрудника
        /// </summary>
        [Fact]
        public async void CreatePersonAsync_ShouldCreatePerson()
        {
            //Arrange
            Grade testGrade = new Grade
            {
                GradeId = 1,
                GradeName = "Junior"
            };
            _dataContext.Grades.Add(testGrade);
            _dataContext.SaveChanges();

            Person createdPerson = new Person()
            {
                Email = "1111@test.com",
                FirstName = "Eric",
                LastName = "Cripke",
                GradeId = testGrade.GradeId,
                ShortName = "Rick",
                SurName = "Rickson",
                Grade = testGrade
            };

            Person expectedPerson = new Person()
            {
                Email = "1111@test.com",
                FirstName = "Eric",
                LastName = "Cripke",
                GradeId = testGrade.GradeId,
                PersonId = 1,
                ShortName = "Rick",
                SurName = "Rickson",
                Grade = testGrade
            };

            //Act
            _dataContext.People.Add(createdPerson);
            _dataContext.SaveChanges();

            //Assert
            createdPerson.Should().BeEquivalentTo(expectedPerson);
        }

        [Fact]
        public async void CreatePersonAsyncWithExistEmail_ShouldThrowException()
        {
            //Arrange
            Grade testGrade = new Grade
            {
                GradeId = 2,
                GradeName = "Junior plus"
            };
            _dataContext.Grades.Add(testGrade);

            Person existPerson = new Person()
            {
                Email = "11112@test.com",
                FirstName = "Eric",
                LastName = "Cripke",
                GradeId = testGrade.GradeId,
                ShortName = "Rick",
                SurName = "Rickson",
                Grade = testGrade
            };
            _dataContext.People.Add(existPerson);
            _dataContext.SaveChanges();

            Person personWithDuplicateEmail = new Person()
            {
                Email = "11112@test.com",
                FirstName = "John",
                LastName = "Bon",
                GradeId = testGrade.GradeId,
                ShortName = "Jovi",
                SurName = "Jovison",
                Grade = testGrade
            };

            // Act
            var result = async () => await _personService.CreatePersonAsync(personWithDuplicateEmail);

            // Assert
            await result.Should().ThrowAsync<Exception>().WithMessage("A Person with same Email is already exists");
        }

        /// <summary>
        /// Проверяем что DeletePersonAsync действительно его сотрудника
        /// </summary>
        [Fact]
        public async void DeletePersonAsync_ShouldDeleteGrade()
        {
            //Arrange
            Grade testGrade = new Grade
            {
                GradeName = "Junior entry"
            };
            _dataContext.Grades.Add(testGrade);

            Person deletedPerson = new Person()
            {
                Email = "11112@test.com",
                FirstName = "Eric",
                LastName = "Cripke",
                GradeId = testGrade.GradeId,
                ShortName = "Rick",
                SurName = "Rickson",
                Grade = testGrade
            };
            _dataContext.People.Add(deletedPerson);
            _dataContext.SaveChanges();

            //act
            await _personService.DeletePersonAsync(deletedPerson.PersonId);
            Person notExistPerson = _dataContext.People.Find(deletedPerson.PersonId);

            //assert
            notExistPerson.Should().BeNull();
        }

        /// <summary>
        /// Проверяем что при попытке удалить несуществующего сотрудника получим соответсвующую ошибку
        /// </summary>
        [Fact]
        public async void DeleteNotExistsPerson_ShouldThrowException()
        {
            //arrange
            Person notExistPerson = new Person
            {
                Email = "1111@test.com",
                FirstName = "Eric",
                LastName = "Cripke",
                GradeId = 1,
                PersonId = 1,
                ShortName = "Rick",
                SurName = "Rickson",
            };            

            //act
            var result = async () => await _personService.DeletePersonAsync(notExistPerson.PersonId);

            //assert
            await result.Should().ThrowAsync<Exception>().WithMessage("Person not found");
        }

        /// <summary>
        /// проверяем, что UpdatePersonAsync корректно обновляет сотрудника
        /// </summary>
        [Fact]
        public async void UpdatePersonAsync_ShouldUpdatePerson()
        {
            //Arrange
            Grade testGrade = new Grade
            {
                GradeName = "Junior entry"
            };
            _dataContext.Grades.Add(testGrade);

            Person updatedPerson = new Person()
            {
                Email = "1111@test.com",
                FirstName = "Eric",
                LastName = "Cripke",
                GradeId = testGrade.GradeId,
                ShortName = "Rick",
                SurName = "Rickson",
                Grade = testGrade
            };
            _dataContext.People.Add(updatedPerson);
            _dataContext.SaveChanges();

            Person expectedPerson = new Person()
            {
                Email = "1111@test.com",
                FirstName = "John",
                LastName = "Connor",
                GradeId = testGrade.GradeId,
                ShortName = "Rick",
                SurName = "Rickson",
                Grade = testGrade,
                PersonId = updatedPerson.PersonId
            };

            //act
            updatedPerson.FirstName = "John";
            updatedPerson.LastName = "Connor";
            _dataContext.People.Update(updatedPerson);
            _dataContext.SaveChanges();

            //assert
            updatedPerson.Should().BeEquivalentTo(expectedPerson);
        }

        /// <summary>
        /// Проверяем, что при попытке обновить несуществующую запись получим ошибку
        /// </summary>
        [Fact]
        public async void UpdateNotExistPerson_ShouldThrowException()
        {
            //Arrange
            Grade testGrade = new Grade
            {
                GradeName = "Junior entry"
            };
            _dataContext.Grades.Add(testGrade);

            Person updatedPerson = new Person()
            {
                Email = "1111@test.com",
                FirstName = "Eric",
                LastName = "Cripke",
                GradeId = testGrade.GradeId,
                ShortName = "Rick",
                SurName = "Rickson",
                Grade = testGrade
            };
            _dataContext.SaveChanges();

            updatedPerson.FirstName = "John";
            updatedPerson.LastName = "Connor";

            //act
            var result = async () => await _personService.UpdatePersonAsync(updatedPerson);

            //assert
            await result.Should().ThrowAsync<Exception>().WithMessage("Person not found");
        }

        /// <summary>
        /// Проверяем, что при попытке обновить сотрудника если есть сотрудник с таким же email получим ошибку
        /// </summary>
        [Fact]
        public async void UpdatePersonWithDuplicateEmail_ShouldThrowException()
        {
            //Arrange
            Grade testGrade = new Grade
            {
                GradeId = 1,
                GradeName = "Junior"
            };
            _dataContext.Grades.Add(testGrade);
            _dataContext.SaveChanges();

            List<Person> people = new List<Person>();
            people.AddRange(new List<Person> {
                new Person {
                    Email = "1111@test.com",
                    FirstName = "Eric",
                    LastName = "Cripke",
                    GradeId = testGrade.GradeId,
                    PersonId = 1,
                    ShortName = "Rick",
                    SurName = "Rickson",
                    Grade = testGrade
                },
                new Person {
                    Email = "1111@test1.com",
                    FirstName = "John",
                    LastName = "Bon",
                    GradeId = testGrade.GradeId,
                    PersonId = 2,
                    ShortName = "Jovi",
                    SurName = "Jovison",
                    Grade = testGrade
                },
            });
            _dataContext.People.AddRange(people);
            _dataContext.SaveChanges();

            //act
            Person updatedPerson = _dataContext.People.Where(p => p.PersonId == 1).FirstOrDefault();
            updatedPerson.Email = "1111@test1.com";
            var result = async () => await _personService.UpdatePersonAsync(updatedPerson);

            //assert
            await result.Should().ThrowAsync<Exception>().WithMessage("A Person with same Email is already exists");
        }

        /// <summary>
        /// проверяем что GetPersonByIdAsync возвращает корректного сотрудника
        /// </summary>
        [Fact]
        public async void GetPersonByIdAsync_ShouldReturnCorrectPerson()
        {
            //Arrange
            Grade testGrade = new Grade
            {
                GradeName = "Junior entry"
            };
            _dataContext.Grades.Add(testGrade);

            Person person = new Person()
            {
                Email = "1111@test.com",
                FirstName = "Eric",
                LastName = "Cripke",
                GradeId = testGrade.GradeId,
                ShortName = "Rick",
                SurName = "Rickson",
                Grade = testGrade
            };
            _dataContext.People.Add(person);
            _dataContext.SaveChanges();

            var personId = person.PersonId;

            Person expectedPerson = new Person()
            {
                Email = "1111@test.com",
                FirstName = "Eric",
                LastName = "Cripke",
                GradeId = testGrade.GradeId,
                ShortName = "Rick",
                SurName = "Rickson",
                Grade = testGrade,
                PersonId = personId
            };

            //Act
            Person resultPerson = await _personService.GetPersonByIdAsync(personId);

            //Arrange
            resultPerson.Should().BeEquivalentTo(expectedPerson);
        }

        /// <summary>
        /// проверяем что GetPersonByIdAsync возвращает ошибку в случае поиска сотрудника, которого нет в системе
        /// </summary>
        [Fact]
        public async void GetNotExistPerson_ShouldReturnCorrectPerson()
        {
            //Arrange
            var notExistPersonId = 1;

            //Act
            var result = async () => await _personService.GetPersonByIdAsync(notExistPersonId);

            //Assert
            await result.Should().ThrowAsync<Exception>().WithMessage("Person not found");
        }

        /// <summary>
        /// проверяем что GetPeopleWithGradeAsync возвращает всех острудников
        /// </summary>
        [Fact]
        public async void GetPeopleWithGradeAsync_ShouldReturnAllPersons()
        {
            //Arrange
            Grade testGrade = new Grade
            {
                GradeId = 1,
                GradeName = "Junior"
            };
            _dataContext.Grades.Add(testGrade);
            _dataContext.SaveChanges();

            List<Person> people = new List<Person>();
            people.AddRange(new List<Person> {
                new Person {
                    Email = "1111@test.com",
                    FirstName = "Eric",
                    LastName = "Cripke",
                    GradeId = testGrade.GradeId,
                    PersonId = 1,
                    ShortName = "Rick",
                    SurName = "Rickson",
                    Grade = testGrade
                },
                new Person {
                    Email = "1111@test1.com",
                    FirstName = "John",
                    LastName = "Bon",
                    GradeId = testGrade.GradeId,
                    PersonId = 2,
                    ShortName = "Jovi",
                    SurName = "Jovison",
                    Grade = testGrade
                },
            });
            _dataContext.People.AddRange(people);
            _dataContext.SaveChanges();

            List<Person> peopleExpected = new List<Person>();
            peopleExpected.AddRange(new List<Person> {
                new Person {
                    Email = "1111@test.com",
                    FirstName = "Eric",
                    LastName = "Cripke",
                    GradeId = testGrade.GradeId,
                    PersonId = 1,
                    ShortName = "Rick",
                    SurName = "Rickson",
                    Grade = testGrade
                },
                new Person {
                    Email = "1111@test1.com",
                    FirstName = "John",
                    LastName = "Bon",
                    GradeId = testGrade.GradeId,
                    PersonId = 2,
                    ShortName = "Jovi",
                    SurName = "Jovison",
                    Grade = testGrade
                },
            });

            //Act
            var resultPeople = await _personService.GetAllPeopleAsync();

            //Assert
            resultPeople.Should().BeEquivalentTo(peopleExpected);
        }

        /// <summary>
        /// проверяем что GetPeopleWithGradeAsync возвращает грейды
        /// </summary>
        [Fact]
        public async void GetPeopleWithGradeAsync_ShouldReturnCorrectGrade()
        {
            //Arrange
            Grade testGrade = new Grade
            {
                GradeId = 1,
                GradeName = "Junior"
            };
            Grade testGrade2 = new Grade
            {
                GradeId = 2,
                GradeName = "Middle"
            };
            _dataContext.Grades.Add(testGrade);
            _dataContext.Grades.Add(testGrade2);
            _dataContext.SaveChanges();

            List<Person> people = new List<Person>();
            people.AddRange(new List<Person> {
                new Person {
                    Email = "1111@test.com",
                    FirstName = "Eric",
                    LastName = "Cripke",
                    GradeId = testGrade.GradeId,
                    PersonId = 1,
                    ShortName = "Rick",
                    SurName = "Rickson",
                    Grade = testGrade
                },
                new Person {
                    Email = "1111@test1.com",
                    FirstName = "John",
                    LastName = "Bon",
                    GradeId = testGrade2.GradeId,
                    PersonId = 2,
                    ShortName = "Jovi",
                    SurName = "Jovison",
                    Grade = testGrade2
                },
            });
            _dataContext.People.AddRange(people);
            _dataContext.SaveChanges();

            //Act
            var resultPeople = await _personService.GetAllPeopleAsync();
            var resultGrade = resultPeople.Where(p => p.Email == "1111@test1.com").FirstOrDefault().Grade;

            //Assert
            resultGrade.Should().BeEquivalentTo(testGrade2);
        }

        /// <summary>
        /// проверяем что GetPeopleFilteredByProjectAsync возвращает корректных сотрудников
        /// </summary>
        [Fact]
        public async void GetPeopleFilteredByProjectAsync_ShouldReturnCorrectPersons()
        {
            //Arrange
            long testedProjectId = 1;
            Grade testGrade = new Grade
            {
                GradeId = 1,
                GradeName = "Junior"
            };

            Person testPerson = new Person
            {
                Email = "1111@test.com",
                FirstName = "Eric",
                LastName = "Cripke",
                GradeId = testGrade.GradeId,
                PersonId = 1,
                ShortName = "Rick",
                SurName = "Rickson",
                Grade = testGrade
            };

            Person testPerson2 = new Person
            {
                Email = "1111@test1.com",
                FirstName = "John",
                LastName = "Bon",
                GradeId = testGrade.GradeId,
                PersonId = 2,
                ShortName = "Jovi",
                SurName = "Jovison",
                Grade = testGrade
            };

            Person testPerson3 = new Person
            {
                Email = "2222@test.com",
                FirstName = "",
                LastName = "Cripke",
                GradeId = testGrade.GradeId,
                PersonId = 3,
                ShortName = "Rick",
                SurName = "Rickson",
                Grade = testGrade
            };
            
            ProjectTeam testProject = new ProjectTeam { ProjectTeamId = testedProjectId, ProjectTeamName = "SABPEK"};
            ProjectTeam testProject2 = new ProjectTeam { ProjectTeamId = 2, ProjectTeamName = "SIDE" };

            _dataContext.ProjectTeams.Add(testProject);
            _dataContext.ProjectTeams.Add(testProject2);
            _dataContext.Grades.Add(testGrade);
            _dataContext.People.AddRange(testPerson, testPerson2, testPerson3);
            _dataContext.SaveChanges();

            ProjectMember projectMember = new ProjectMember {
                Person = testPerson,
                PersonId = testPerson.PersonId,
                ProjectMemberId = 1,
                ProjectTeam = testProject,
                ProjectTeamId = testProject.ProjectTeamId,
            };

            ProjectMember projectMember2 = new ProjectMember {
                Person = testPerson2,
                PersonId = testPerson2.PersonId,
                ProjectMemberId = 2,
                ProjectTeam = testProject2,
                ProjectTeamId = testProject2.ProjectTeamId,
            };

            ProjectMember projectMember3 = new ProjectMember
            {
                Person = testPerson3,
                PersonId = testPerson3.PersonId,
                ProjectMemberId = 3,
                ProjectTeam = testProject,
                ProjectTeamId = testProject.ProjectTeamId,
            };

            _dataContext.ProjectMembers.AddRange(projectMember, projectMember2, projectMember3);
            _dataContext.SaveChanges();

            List<Person> expectedPersons = new List<Person>() {
                new Person
                {
                    Email = "1111@test.com",
                    FirstName = "Eric",
                    LastName = "Cripke",
                    GradeId = testGrade.GradeId,
                    PersonId = 1,
                    ShortName = "Rick",
                    SurName = "Rickson",
                    Grade = testGrade
                },
                new Person
                {
                    Email = "2222@test.com",
                    FirstName = "",
                    LastName = "Cripke",
                    GradeId = testGrade.GradeId,
                    PersonId = 3,
                    ShortName = "Rick",
                    SurName = "Rickson",
                    Grade = testGrade
                }
            };

            //Act
            var resultPeople = await _personService.GetPeopleFilteredByProjectAsync(testedProjectId);

            //Assert
            resultPeople.Should().BeEquivalentTo(expectedPersons);
        }

        /// <summary>
        /// проверяем что GetPeopleFilteredByProjectsAsync возвращает корректных сотрудников
        /// </summary>
        [Fact]
        public async void GetPeopleFilteredByProjectsAsync_ShouldReturnCorrectPersons()
        {
            //Arrange
            long testedProjectId = 1;
            Grade testGrade = new Grade
            {
                GradeId = 1,
                GradeName = "Junior"
            };

            Person testPerson = new Person
            {
                Email = "1111@test.com",
                FirstName = "Eric",
                LastName = "Cripke",
                GradeId = testGrade.GradeId,
                PersonId = 1,
                ShortName = "Rick",
                SurName = "Rickson",
                Grade = testGrade
            };

            Person testPerson2 = new Person
            {
                Email = "1111@test1.com",
                FirstName = "John",
                LastName = "Bon",
                GradeId = testGrade.GradeId,
                PersonId = 2,
                ShortName = "Jovi",
                SurName = "Jovison",
                Grade = testGrade
            };

            Person testPerson3 = new Person
            {
                Email = "2222@test.com",
                FirstName = "",
                LastName = "Cripke",
                GradeId = testGrade.GradeId,
                PersonId = 3,
                ShortName = "Rick",
                SurName = "Rickson",
                Grade = testGrade
            };

            ProjectTeam testProject = new ProjectTeam { ProjectTeamId = testedProjectId, ProjectTeamName = "SABPEK" };
            ProjectTeam testProject2 = new ProjectTeam { ProjectTeamId = 2, ProjectTeamName = "SIDE" };
            ProjectTeam testProject3 = new ProjectTeam { ProjectTeamId = 3, ProjectTeamName = "STACK" };

            _dataContext.ProjectTeams.Add(testProject);
            _dataContext.ProjectTeams.Add(testProject2);
            _dataContext.Grades.Add(testGrade);
            _dataContext.People.AddRange(testPerson, testPerson2, testPerson3);
            _dataContext.SaveChanges();

            ProjectMember projectMember = new ProjectMember
            {
                Person = testPerson,
                PersonId = testPerson.PersonId,
                ProjectMemberId = 1,
                ProjectTeam = testProject,
                ProjectTeamId = testProject.ProjectTeamId,
            };

            ProjectMember projectMember2 = new ProjectMember
            {
                Person = testPerson2,
                PersonId = testPerson2.PersonId,
                ProjectMemberId = 2,
                ProjectTeam = testProject2,
                ProjectTeamId = testProject2.ProjectTeamId,
            };

            ProjectMember projectMember3 = new ProjectMember
            {
                Person = testPerson3,
                PersonId = testPerson3.PersonId,
                ProjectMemberId = 3,
                ProjectTeam = testProject3,
                ProjectTeamId = testProject3.ProjectTeamId,
            };

            _dataContext.ProjectMembers.AddRange(projectMember, projectMember2, projectMember3);
            _dataContext.SaveChanges();

            List<Person> expectedPersons = new List<Person>() {
                new Person
                {
                    Email = "1111@test.com",
                    FirstName = "Eric",
                    LastName = "Cripke",
                    GradeId = testGrade.GradeId,
                    PersonId = 1,
                    ShortName = "Rick",
                    SurName = "Rickson",
                    Grade = testGrade
                },
                new Person
                {
                    Email = "2222@test.com",
                    FirstName = "",
                    LastName = "Cripke",
                    GradeId = testGrade.GradeId,
                    PersonId = 3,
                    ShortName = "Rick",
                    SurName = "Rickson",
                    Grade = testGrade
                }
            };

            List<ProjectTeam> testedPrjects = new List<ProjectTeam>() { testProject, testProject3 };

            //Act
            var resultPeople = await _personService.GetPeopleFilteredByProjectsAsync(testedPrjects);

            //Assert
            resultPeople.Should().BeEquivalentTo(expectedPersons);
        }
    }
}
