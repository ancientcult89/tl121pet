using tl121pet.Entities.DTO;
using tl121pet.Entities.Models;

namespace tl121pet.Entities.Extensions
{
    public static class UserMailSettingsDtoExtension
    {
        public static UserMailSetting ToEntity(this UserMailSettingsDTO userMailSettingsDTO)
        {
            return new UserMailSetting()
            {
                UserId = userMailSettingsDTO.UserId,
                EmailPort = userMailSettingsDTO.EmailPort,
                DisplayName = userMailSettingsDTO.DisplayName,
                EmailHostAddress = userMailSettingsDTO.EmailHostAddress,
                EmailPassword = userMailSettingsDTO.EmailPassword,
                UserMailSettingId = userMailSettingsDTO.UserMailSettingId,
            };
        }
    }
}
