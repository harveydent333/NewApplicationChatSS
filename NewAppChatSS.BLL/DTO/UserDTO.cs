using System;
using System.Collections.Generic;
using System.Text;

namespace NewAppChatSS.BLL.DTO
{
    public class UserDTO
    {
        public String id { get; set; }

        /// <summary>
        /// Логин пользователя
        /// </summary>
        public String Login { get; set; }
        
        public String UserName { get; set; }

        /// <summary>
        /// E-mail адрес пользователя
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        public String Password { get; set; }

        /// <summary>
        /// Id роли пользователя
        /// </summary>
        public Int32 RoleId { get; set; }

        /// <summary>
        /// Флаг заблокирован ли пользователь
        /// </summary>
        public Boolean Loked { get; set; } = false;
        
        public DateTime? DateUnblock { get; set; }
    }
}
