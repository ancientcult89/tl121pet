using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using tl121pet.Entities.Aggregate;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Extensions;
using tl121pet.Entities.Infrastructure;
using tl121pet.Entities.Infrastructure.Exceptions;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{

    public class OneToOneApplication(
        IMeetingService meetingService,
        ITlMailService mailService,
        IPersonService personService,
        IHttpContextAccessor httpContextAccessor,
        IUserMailSettingService userMailSettingService,
        IEncryptionService encryptionService,
        IRoleService roleService,
        IAuthService authService) : IOneToOneApplication
    {
        private IPersonService _personService = personService;
        private IMeetingService _meetingService = meetingService;
        private ITlMailService _mailService = mailService;
        private readonly IAuthService _authService = authService;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IUserMailSettingService _userMailSettingService = userMailSettingService;
        private readonly IEncryptionService _encryptionService = encryptionService;
        private readonly IRoleService _roleService = roleService;

        public async Task<List<OneToOneDeadline>> GetDeadLinesAsync()
        {
            List<OneToOneDeadline> deadLines = new List<OneToOneDeadline>();

            foreach (Person p in await GetPeopleFilteredByProjectsAsync())
            {
                TimeSpan datediff = new TimeSpan();
                Meeting lastMeeting = await _meetingService.GetLastOneToOneByPersonIdAsync(p.PersonId) ?? new Meeting();
                if (lastMeeting.MeetingDate != null)
                {
                    DateTime lastMeetingDate = (DateTime)lastMeeting.MeetingDate;
                    datediff = lastMeetingDate.AddMonths(1).Date - DateTime.Now.Date;
                }

                DateTime deadlineDate = lastMeeting.MeetingDate != null ? ((DateTime)lastMeeting.MeetingDate).AddMonths(1) : DateTime.Now;

                deadLines.Add(new OneToOneDeadline
                {
                    Person = p,
                    LastMeetingOneToOne = lastMeeting,
                    LastOneToOneMeetingDate = lastMeeting.MeetingDate,
                    DeadLine = deadlineDate,
                    DayToDeadline = datediff.Days,
                });
            }
            return deadLines;
        }

        public async Task<string> GenerateFollowUpAsync(Guid meetingId, long personId)
        {
            string result = "";
            Person person = await _personService.GetPersonByIdAsync(personId);
            result = $"{(!String.IsNullOrEmpty(person.ShortName) ? person.ShortName : person.FirstName)}, спасибо за проведённую встречу!\n\n";
            result += await GetMeetingFeedbackRequiredNotesAndGoalByMeetingId(meetingId, personId);
            result += await GetPreviousMeetingUnclosedGoals(meetingId, personId);
            result += "Если что-то упустил - обязательно сообщи мне об этом!";
            return result;
        }

        public async Task<string> GetPreviousMeetingNoteAndGoalsAsync(Guid meetingId, long personId)
        {
            string prevNotes = "";
            Guid? previousMeetingGuid = await _meetingService.GetPreviousMeetingIdAsync(meetingId, personId);
            if (previousMeetingGuid != null)
            {
                prevNotes = await GetMeetingFeedbackRequiredNotesByMeetingId((Guid)previousMeetingGuid);
            }
            return prevNotes;
        }

        //TODO: на вход должен подаваться айдишка встречи и уже готовая почта
        public async Task SendFollowUpAsync(Guid meetingId, long personId)
        {
            MailRequest mail = await GenerateFollowUpMailRequestAsync(meetingId, personId);
            try
            {
                long? userId = GetMyUserId();
                if (userId == null)
                    throw new Exception("User not found");

                UserMailSetting userMailSetting = await _userMailSettingService.GetUserMailSettingsByUserIdAsync((long)userId);
                User user = await _authService.GetUserByIdAsync((long)userId);

                MailSettings mailSettings = GenerateMailSettings(userMailSetting, user.Email);
                mailSettings.Password = _encryptionService.Decrypt(mailSettings.Password);

                await _mailService.SendMailAsync(mail, mailSettings);
                await MarkAsSendedFollowUpAsync(meetingId);
            }
            catch { throw new Exception("e-mail service is unavalable"); }
        }

        public async Task SendGreetingMailAsync(long personId)
        {
            long? userId = GetMyUserId();
            if (userId == null)
                throw new Exception("User not found");

            UserMailSetting userMailSetting = await _userMailSettingService.GetUserMailSettingsByUserIdAsync((long)userId);
            User user = await _authService.GetUserByIdAsync((long)userId);

            MailSettings mailSettings = GenerateMailSettings(userMailSetting, user.Email);
            mailSettings.Password = _encryptionService.Decrypt(mailSettings.Password);

            MailRequest mail = await GeneratGreetingMailRequest(personId);
            try
            {
                await _mailService.SendMailAsync(mail, mailSettings);
            }
            catch { throw new Exception("e-mail service is unavalable"); }
        }

        public async Task<List<Person>> GetPeopleFilteredByProjectsAsync()
        {
            List<Person> people = new List<Person>();
            long? userId = GetMyUserId();
            List<ProjectTeam> projects = new List<ProjectTeam>();
            if (userId != null)
            {
                projects = await _authService.GetUserProjectsAsync((long)userId);
                people = await _personService.GetPeopleFilteredByProjectsAsync(projects);
            }

            return people;
        }

        public async Task ArchivePersonAsync(long personId)
        {
            try
            {
                await _personService.ArchivePersonAsync(personId);
                await _meetingService.CompleteAllPersonGoalsAsync(personId);
            }
            catch { throw new Exception("Failed to archive employee"); }
        }

        public async Task<List<TaskDTO>> GetTaskListAsync(long? personId, Guid? currentMeetingId)
        {
            List<TaskDTO> taskList = new List<TaskDTO>();
            long? userId = GetMyUserId();

            if (userId == null)
                return taskList;

            taskList = await _meetingService.GetTasksByUserIdAsync((long)userId, personId, currentMeetingId);

            return taskList;
        }

        public async Task<MeetingPagedResponseDTO> GetPagedMeetingsAsync(MeetingPagedRequestDTO request)
        {
            MeetingPagedResponseDTO response = new MeetingPagedResponseDTO();
            long? userId = GetMyUserId();
            if (userId != null)
            {
                response = await _meetingService.GetMeetingsByUserIdAsync(request, (long)userId);
            }

            return response;
        }

        public async Task ChangeLocaleAsync(int localeId)
        {
            await _authService.ChangeLocaleByUserIdAsync(GetMyUserId(), (Locale)localeId);
        }

        public async Task<Meeting> CreateMeetingAsync(MeetingDTO meetingDto)
        {
            Meeting newMeeting = meetingDto.ToEntity();
            newMeeting.UserId = GetMyUserId();
            return await _meetingService.CreateMeetingAsync(newMeeting);
        }

        public async Task<Meeting> UpdateMeetingAsync(MeetingDTO meetingDto)
        {
            Meeting newMeeting = meetingDto.ToEntity();

            newMeeting.UserId = GetMyUserId();
            return await _meetingService.UpdateMeetingAsync(newMeeting);
        }

        public async Task<Meeting> CreateMeetingByPersonIdAsync(long personId)
        {
            return await _meetingService.CreateCurrentMeetingByPersonIdAsync(GetMyUserId(), personId);
        }

        private async Task<string> GetMeetingFeedbackRequiredNotesAndGoalByMeetingId(Guid meetingId, long personId)
        {
            string result = "";
            result += await GetMeetingFeedbackRequiredNotesByMeetingId(meetingId);
            result += await GetMeetingGoalsByMeetingId(meetingId);

            return result;
        }

        private async Task<string> GetMeetingFeedbackRequiredNotesByMeetingId(Guid meetingId)
        {
            string meetingNotes = "";
            List<MeetingNote> notes = await _meetingService.GetMeetingFeedbackRequiredNotesAsync(meetingId);
            if (notes.Count() > 0)
            {
                meetingNotes += "На встрече обсудили:\n";
                foreach (MeetingNote mn in notes)
                {
                    meetingNotes += $"\t- {mn.MeetingNoteContent};\n";
                }
                meetingNotes += "\n\n";
            }

            return meetingNotes;
        }

        private async Task<string> GetMeetingGoalsByMeetingId(Guid meetingId)
        {
            string meetingGoals = "";
            List<MeetingGoal> goals = await _meetingService.GetMeetingGoalsAsync(meetingId);
            if (goals.Count() > 0)
            {
                meetingGoals += "К следующему 1-2-1 договорились:\n";
                foreach (MeetingGoal mg in goals)
                {
                    meetingGoals += $"\t- {mg.MeetingGoalDescription};\n";
                }
                meetingGoals += "\n\n";
            }

            return meetingGoals;
        }

        private async Task<string> GetPreviousMeetingUnclosedGoals(Guid meetingId, long personId)
        {
            string meetingGoals = "";
            List<MeetingGoal> goals = await _meetingService.GetPrevoiusUnclosedMeetingGoalsAsync(meetingId, personId);
            if (goals.Count() > 0)
            {
                meetingGoals += "С прошлой встречи остались цели:\n";
                foreach (MeetingGoal mg in goals)
                {
                    meetingGoals += $"\t- {mg.MeetingGoalDescription};\n";
                }
                meetingGoals += "\n\n";
            }

            return meetingGoals;
        }

        private async Task MarkAsSendedFollowUpAsync(Guid meetingId) => await _meetingService.MarkAsSendedFollowUpAndFillActualDateAsync(meetingId, DateTime.Now);

        private async Task<MailRequest> GenerateFollowUpMailRequestAsync(Guid meetingId, long personId)
        {
            MailRequest mail = new MailRequest();
            Person destinationPerson = await _personService.GetPersonByIdAsync(personId);
            mail.ToEmail = destinationPerson.Email;
            mail.Body = await GenerateFollowUpAsync(meetingId, personId);
            mail.Subject = "1-2-1 Follow-up";

            return mail;
        }

        private async Task<MailRequest> GeneratPasswordRecoveryMailAsync(string newPassword, string email)
        {
            MailRequest mail = new MailRequest();
            mail.ToEmail = email;
            mail.Body = $"Ваш новый пароль: {newPassword}";
            mail.Subject = "Восстановление пароля";

            return mail;
        }

        private async Task<string> GenerateGreetingMessageAsync(Person targetPerson)
        {
            string result = "";
            result = $"{(!String.IsNullOrEmpty(targetPerson.ShortName) ? targetPerson.ShortName : targetPerson.FirstName)}, привет!\n\n";
            result += $"Это приветственное письмо, которое необходимо, что бы убедиться, что именно ты ({targetPerson.LastName + " " + targetPerson.FirstName}) получишь follow-up!\n";
            result += $"При получении этого письма дай мне знать, что оно получено\n";
            return result;
        }

        private async Task<MailRequest> GeneratGreetingMailRequest(long personId)
        {
            MailRequest mail = new MailRequest();
            Person destinationPerson = await _personService.GetPersonByIdAsync(personId);
            mail.ToEmail = destinationPerson.Email;
            mail.Body = await GenerateGreetingMessageAsync(destinationPerson);
            mail.Subject = "Greeting message";

            return mail;
        }

        public long GetMyUserId()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                return Convert.ToInt64(result);
            }
            else
                throw new DataFoundException("User not found");
        }

        private MailSettings GenerateMailSettings(UserMailSetting userMailSetting, string senderMail)
        {
            MailSettings mailSettings = new MailSettings()
            {
                DisplayName = userMailSetting.DisplayName,
                Host = userMailSetting.EmailHostAddress,
                Mail = senderMail,
                Password = userMailSetting.EmailPassword,
                Port = userMailSetting.EmailPort,
            };
            return mailSettings;
        }

        public async Task RecoverPasswordAsync(RecoverPasswordRequestDTO recoverPasswordRequest)
        {
            string newPassword = await _authService.RecoverPasswordAsync(recoverPasswordRequest.Email);
            MailRequest mail = await GeneratPasswordRecoveryMailAsync(newPassword, recoverPasswordRequest.Email);
            try
            {
                await _mailService.SendInfrastructureMailAsync(mail);
            }
            catch { throw new Exception("e-mail service is unavalable"); }
        }

        public async Task<UserMailSetting> GetUserMailSettingsByUserIdAsync(long userId)
        {
            UserMailSetting userMailSetting = await _userMailSettingService.GetUserMailSettingsByUserIdAsync(userId);
            if(userMailSetting?.EmailPassword != null)
                userMailSetting.EmailPassword = _encryptionService.Decrypt(userMailSetting.EmailPassword);

            return userMailSetting;
        }

        public async Task<UserMailSettingsDTO> SetUserMailSettingsAsync(UserMailSettingsDTO userMailSetting)
        {
            userMailSetting.EmailPassword = _encryptionService.Encrypt(userMailSetting.EmailPassword);
            return await _userMailSettingService.SetUserMailSettingsAsync(userMailSetting);
        }

        public async Task<UserDTO> UpdateUserCommonSettingsAsync(UserDTO user)
        {
            long? currentSessionUser = GetMyUserId();
            if (currentSessionUser == null)
                throw new Exception("Not exists session, please login");

            User updatedUser = await _authService.GetUserByIdAsync(user.Id);

            if (updatedUser == null)
                throw new Exception("User is not exists");

            if (updatedUser.RoleId != user.RoleId)
            {
                if (!(await _roleService.IsUserAdmin((long)currentSessionUser)))
                    throw new Exception("Only Admin can change User Role");
            }

            return await _authService.UpdateUserCommonSettingsAsync(user);
        }
    }
}
