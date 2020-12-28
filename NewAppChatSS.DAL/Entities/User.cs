using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace NewAppChatSS.DAL.Entities
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class User : IdentityUser
    {
        [Required]
        public bool IsLocked { get; set; } = false;

        /// <summary>
        /// Дата окончания блокировки
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime? DateUnblock { get; set; }
    }
}
