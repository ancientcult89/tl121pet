using tl121pet.DAL.Data;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{
    public class MeetingService : IMeetingService
    {
        private readonly DataContext _dataContext;
        private readonly IAuthService _authService;

        public MeetingService(DataContext dataContext, IAuthService authService)
        {
           _dataContext = dataContext;
           _authService = authService;
        }
        public List<Meeting> GetMeetings(long? personId)
        {
            List<Meeting> meetingsRes = new List<Meeting>();
            long? userId = _authService.GetMyUserId();

            var meetings = from up in _dataContext.UserProjects
                          join pp in _dataContext.ProjectMembers on up.ProjectTeamId equals pp.ProjectTeamId
                          join p in _dataContext.People on pp.PersonId equals p.PersonId
                          join m in _dataContext.Meetings on p.PersonId equals m.PersonId
                          join mt in _dataContext.MeetingTypes on m.MeetingTypeId equals mt.MeetingTypeId
                          where (personId == null || p.PersonId == personId) && (up.UserId == userId)
                          select new { m, mt, p };
            foreach (var m in meetings)
            {
                Meeting meeting = m.m;
                meeting.MeetingType = m.mt;
                meeting.Person = m.p;
                meetingsRes.Add(meeting);
            }
            return meetingsRes;
        }
    }
}
