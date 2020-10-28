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
        /// <summary>
        /// Логин пользователя
        /// </summary>
        [Required]
        [RegularExpression(@"[a-zA-Z0-9]+")]
        [StringLength(35, MinimumLength = 4)]
        public string Login { get; set; }

        [Required]
        public bool IsLocked { get; set; } = false;

        /// <summary>
        /// Дата окончания блокировки
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime? DateUnblock { get; set; }
    }
}
