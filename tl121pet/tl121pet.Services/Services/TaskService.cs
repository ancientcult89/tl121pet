using tl121pet.DAL.Interfaces;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{
    public class TaskService : ITaskService
    {
        private readonly IPersonService _personService;
        private readonly IMeetingRepository _meetingRepository;
        private readonly IPeopleRepository _peopleRepository;
        public TaskService(IPersonService personService, IMeetingRepository meetingRepository, IPeopleRepository peopleRepository)
        {
            _personService = personService;
            _meetingRepository = meetingRepository;
            _peopleRepository = peopleRepository;
        }
        public async Task<List<TaskDTO>> GetTaskListAsync(long? personId)
        {
            List<TaskDTO> taskList = new List<TaskDTO>();
            List<Person> people = new List<Person>();

            if (personId != null)
                people.Add(_peopleRepository.GetPerson((long)personId));
            else
                people = await _personService.GetPeopleAsync();

            foreach (Person p in people)
            {
                List<MeetingGoal> goals = _meetingRepository.GetMeetingGoalsByPerson(p.PersonId);
                foreach (MeetingGoal goal in goals)
                {
                    taskList.Add(new TaskDTO() { 
                        MeetingGoalId = goal.MeetingGoalId,
                        CompleteDescription = goal.CompleteDescription,
                        IsCompleted = goal.IsCompleted,
                        MeetingGoalDescription = goal.MeetingGoalDescription,
                        PersonName = p.LastName + " " + p.FirstName + " " + p.SurName,
                        PersonId = p.PersonId,
                        FactDate = _meetingRepository.GetFactMeetingDateById(goal.MeetingId)
                    });
                }
            }

            return taskList.OrderByDescending(t=> t.FactDate).ToList();
        }
    }
}
