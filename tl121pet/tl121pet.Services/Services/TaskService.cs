using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;
using tl121pet.Services.Application;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{
    public class TaskService : ITaskService
    {
        private readonly IPersonService _personService;
        private readonly IMeetingService _meetingService;
        private readonly OneToOneApplication _application;

        public TaskService(IPersonService personService, IMeetingService meetingService, OneToOneApplication application)
        {
            _personService = personService;
            _meetingService = meetingService;
            _application = application;
        }
        public async Task<List<TaskDTO>> GetTaskListAsync(long? personId)
        {
            List<TaskDTO> taskList = new List<TaskDTO>();
            List<Person> people = new List<Person>();

            if (personId != null)
                people.Add(await _personService.GetPersonByIdAsync((long)personId));
            else
                people = await _application.GetPeopleFilteredByProjectsAsync();

            foreach (Person p in people)
            {
                List<MeetingGoal> goals = await _meetingService.GetMeetingGoalsByPersonAsync(p.PersonId);
                foreach (MeetingGoal goal in goals.Where(g => g.IsCompleted == false))
                {
                    taskList.Add(new TaskDTO() { 
                        MeetingGoalId = goal.MeetingGoalId,
                        IsCompleted = goal.IsCompleted,
                        MeetingGoalDescription = goal.MeetingGoalDescription,
                        PersonName = p.LastName + " " + p.FirstName + " " + p.SurName,
                        PersonId = p.PersonId,
                        FactDate = await _meetingService.GetFactMeetingDateByIdAsync(goal.MeetingId)
                    });
                }
            }

            return taskList.OrderByDescending(t=> t.FactDate).ToList();
        }
    }
}
