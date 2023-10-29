using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Application
{
    public class OneToOneApplication
    {
        private readonly IAuthService _authService;
        private readonly IPersonService _personService;
        private readonly IProjectService _projectService;
        private readonly IMeetingService _meetingService;

        public OneToOneApplication(IPersonService personService, IAuthService authService, IProjectService projectService, IMeetingService meetingService)
        {
            _authService = authService;
            _personService = personService;
            _projectService = projectService;
            _meetingService = meetingService;
        }

        public async Task<List<Person>> GetPeopleFilteredByProjectsAsync()
        {
            List<Person> people = new List<Person>();
            long? userId = _authService.GetMyUserId();
            List<ProjectTeam> projects = new List<ProjectTeam>();
            if (userId != null)
            {
                projects = await _authService.GetUserProjectsAsync((long)userId);
                people = await _personService.GetPeopleFilteredByProjectsAsync(projects);
            }

            return people;
        }

        public async Task<List<TaskDTO>> GetTaskListAsync(long? personId)
        {
            List<TaskDTO> taskList = new List<TaskDTO>();
            List<Person> people = new List<Person>();

            if (personId != null)
                people.Add(await _personService.GetPersonByIdAsync((long)personId));
            else
                people = await GetPeopleFilteredByProjectsAsync();

            foreach (Person p in people)
            {
                List<MeetingGoal> goals = await _meetingService.GetMeetingGoalsByPersonAsync(p.PersonId);
                foreach (MeetingGoal goal in goals.Where(g => g.IsCompleted == false))
                {
                    taskList.Add(new TaskDTO()
                    {
                        MeetingGoalId = goal.MeetingGoalId,
                        IsCompleted = goal.IsCompleted,
                        MeetingGoalDescription = goal.MeetingGoalDescription,
                        PersonName = p.LastName + " " + p.FirstName + " " + p.SurName,
                        PersonId = p.PersonId,
                        FactDate = await _meetingService.GetFactMeetingDateByIdAsync(goal.MeetingId)
                    });
                }
            }

            return taskList.OrderByDescending(t => t.FactDate).ToList();
        }

        [Obsolete]
        public async Task<List<ProjectTeam>> GetFilteredProjectsAsync()
        {
            long? userId = _authService.GetMyUserId();
            return await _projectService.GetFilteredProjectsAsync(userId);
        }


        [Obsolete]
        public async Task<List<PersonInitials>> GetInitialsAsync()
        {
            List<PersonInitials> personInitials = new List<PersonInitials>();
            List<Person> people = await GetPeopleFilteredByProjectsAsync();
            personInitials = (List<PersonInitials>)people.Select(p =>
                new PersonInitials
                {
                    PersonId = p.PersonId,
                    Initials = p.LastName + " " + p.FirstName + " " + p.SurName
                }).ToList();
            return personInitials;
        }
    }
}
