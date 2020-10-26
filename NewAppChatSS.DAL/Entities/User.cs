﻿using Microsoft.AspNetCore.Identity;
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
        public String Login { get; set; }

        [Required]
        public Boolean Loked { get; set; } = false;

        /// <summary>
        /// Дата окончания блокировки
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime? DateUnblock { get; set; }

        /// <summary>
        /// ID роль пользователя
        /// </summary>
        [Required]
        public String RoleId { get; set; }
    }
}
