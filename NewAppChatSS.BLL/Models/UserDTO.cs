using System;

namespace NewAppChatSS.BLL.Models
{
    public class UserDTO
    {
        public string Id { get; set; }

        /// <summary>
        /// E-mail адрес пользователя
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Флаг заблокирован ли пользователь
        /// </summary>
        public bool IsLocked { get; set; } = false;

        public bool? RememberMe { get; set; } = false;

        /// <summary>
        /// Дата разблокировки
        /// </summary>
        public DateTime? DateUnblock { get; set; }
    }
}
