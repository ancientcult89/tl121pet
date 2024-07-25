using Microsoft.EntityFrameworkCore;
using tl121pet.DAL.Data;
using tl121pet.Entities.DTO;
using tl121pet.Entities.Extensions;
using tl121pet.Entities.Models;
using tl121pet.Services.Interfaces;

namespace tl121pet.Services.Services
{
    public class UserMailSettingService(DataContext dataContext) : IUserMailSettingService
    {
        private DataContext _dataContext = dataContext;

        public async Task<UserMailSetting> GetUserMailSettingsByUserIdAsync(long userId)
        {
            return await _dataContext.UserMailSettings.Where(ums => ums.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<UserMailSettingsDTO> SetUserMailSettingsAsync(UserMailSettingsDTO userMailSettingChangeRequest)
        {
            var existingUserMailSetting = await GetUserMailSettingsByUserIdAsync(userMailSettingChangeRequest.UserId);
            UserMailSetting userMailSetting = userMailSettingChangeRequest.ToEntity();

            if (existingUserMailSetting == null)
            {
                userMailSetting = await CreateUserMailSettingAsync(userMailSetting);
            }
            else
            {
                userMailSetting = await UpdateExistingUserMailSettingAsync(existingUserMailSetting, userMailSetting);
            }
            return userMailSetting.ToDto();
        }

        private async Task<UserMailSetting> CreateUserMailSettingAsync(UserMailSetting userMailSetting)
        {
            _dataContext.UserMailSettings.Add(userMailSetting);
            await _dataContext.SaveChangesAsync();
            return userMailSetting;
        }

        private async Task<UserMailSetting> UpdateExistingUserMailSettingAsync(UserMailSetting existingUserMailSetting, UserMailSetting newUserMailSetting)
        {
            _dataContext.Entry(existingUserMailSetting).CurrentValues.SetValues(newUserMailSetting);
            await _dataContext.SaveChangesAsync();
            return existingUserMailSetting;
        }
    }
}
