using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{
    public class TaskService : ITaskService
    {
        private readonly IPersonService _personService;
        private readonly IMeetingService _meetingService;

        public TaskService(IPersonService personService, IMeetingService meetingService)
        {
            _personService = personService;
            _meetingService = meetingService;
        }
        public async Task<List<TaskDTO>> GetTaskListAsync(long? personId)
        {
            List<TaskDTO> taskList = new List<TaskDTO>();
            List<Person> people = new List<Person>();

            if (personId != null)
                people.Add(await _personService.GetPersonAsync((long)personId));
            else
                people = await _personService.GetPeopleAsync();

            foreach (Person p in people)
            {
                List<MeetingGoal> goals = await _meetingService.GetMeetingGoalsByPersonAsync(p.PersonId);
                foreach (MeetingGoal goal in goals)
                {
                    taskList.Add(new TaskDTO() { 
                        MeetingGoalId = goal.MeetingGoalId,
                        CompleteDescription = goal.CompleteDescription,
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
