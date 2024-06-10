namespace tl121pet.Entities.Models
{
    public class UserMailSetting
    {
        public long UserMailSettingId { get; set; }
        /// <summary>
        /// Выводимое имя в письме (адресат)
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Пароль от электронной почты
        /// </summary>
        public string EMailPassword { get; set; }
        /// <summary>
        /// Адрес почтового сервера
        /// </summary>
        public string EmailHostAddress { get; set; }
        /// <summary>
        /// Порт для подключения к почтовому серверу
        /// </summary>
        public int EmailPort { get; set; }
    }
}
