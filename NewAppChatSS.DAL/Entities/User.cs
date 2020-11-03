using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;

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

        //[ForeignKey("RoleId")]
        //public IdentityRole Role { get; set; }

        //public string RoleId { get; set; }
    }
}
