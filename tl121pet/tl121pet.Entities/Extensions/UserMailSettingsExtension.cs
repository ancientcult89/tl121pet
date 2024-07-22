using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.Entities.Extensions
{
    public static class UserMailSettingsExtension
    {
        public static UserMailSettingsDTO ToDto(this UserMailSetting userMailSetting)
        {
            return new UserMailSettingsDTO()
            {
                UserMailSettingId = userMailSetting.UserMailSettingId,
                UserId = userMailSetting.UserId,
                DisplayName = userMailSetting.DisplayName,
                EmailHostAddress = userMailSetting.EmailHostAddress,
                EmailPort = userMailSetting.EmailPort,
                EmailPassword = userMailSetting.EmailPassword,
            };
        }
    }
}
