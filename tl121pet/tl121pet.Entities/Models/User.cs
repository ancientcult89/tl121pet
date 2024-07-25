using tl121pet.Entities.Infrastructure;

namespace tl121pet.Entities.Models
{
    public class User
    {
        /// <summary>
        /// ИД пользователя
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// адрес электронной почты
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// хешированный пароль
        /// </summary>
        public byte[] PasswordHash { get; set; } = new byte[32];
        /// <summary>
        /// соль хеша пароля
        /// </summary>
        public byte[] PasswordSalt { get; set; } = new byte[32];
        /// <summary>
        /// токен верификации
        /// </summary>
        public string? VerificationToken { get; set; }
        /// <summary>
        /// дата верификации
        /// </summary>
        public DateTime? VerifiedAt { get; set; }
        /// <summary>
        /// токен для сброса пароля
        /// </summary>
        public string? PasswordResetToken { get; set; }
        /// <summary>
        /// время жизни токена сброса пароля
        /// </summary>
        public DateTime? ResetTokenExpired { get; set; }
        /// <summary>
        /// идентификатор роли пользователя
        /// </summary>
        public int? RoleId { get; set; }
        /// <summary>
        /// роль пользователя
        /// </summary>
        public Role? Role { get; set; }
        /// <summary>
        /// локализация
        /// </summary>
        public Locale Locale { get; set; }
        /// <summary>
        /// настройки почты пользователя
        /// </summary>
        public UserMailSetting? MailSettings { get; set; }
    }
}
