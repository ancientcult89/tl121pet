using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.Services.Interfaces
{
    public interface IUserMailSettingService
    {
        Task<UserMailSetting> GetUserMailSettingsByUserIdAsync(long userId);
        Task<UserMailSettingsDTO> SetUserMailSettingsAsync(UserMailSettingsDTO userMailSetting);
    }
}
