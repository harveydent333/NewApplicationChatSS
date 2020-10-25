using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace NewAppChatSS.DAL.Entities
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class User : IdentityUser
    {
     //   [Required]
        public Boolean Loked { get; set; } = false;

        /// <summary>
        /// Дата окончания блокировки
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime? DateUnblock { get; set; }

        /// <summary>
        /// Роль пользователя
        /// </summary>
        [Required]
        public Role Role { get; set; }

        /// <summary>
        /// Id роли пользователя
        /// </summary>
        [Required]
        public Int32 RoleId { get; set; }
    }
}
