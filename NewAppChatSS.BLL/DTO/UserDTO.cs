using System;

namespace NewAppChatSS.BLL.DTO
{
    public class UserDTO
    {
        public String Id { get; set; }

        /// <summary>
        /// Логин пользователя
        /// </summary>
        public String Login { get; set; }

        /// <summary>
        /// E-mail адрес пользователя
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        public String PasswordHash { get; set; }

        /// <summary>
        /// Id роли пользователя
        /// </summary>
        public String RoleId { get; set; }

        /// <summary>
        /// Флаг заблокирован ли пользователь
        /// </summary>
        public Boolean Loked { get; set; } = false;
        
        /// <summary>
        /// Дата разблокировки
        /// </summary>
        public DateTime? DateUnblock { get; set; }
    }
}
