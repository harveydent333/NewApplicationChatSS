using Data.Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Roles
{
    /// <summary>
    /// Роль пользователя
    /// </summary>
    public class Role
    {
        public Role()
        {
            Users = new List<User>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int32 Id { get; set; }

        /// <summary>
        /// Наименование роли пользователя
        /// </summary>
        [Required]
        [Display(Name = "Роль")]
        public String RoleName { get; set; }

        /// <summary>
        /// Список пользователей
        /// </summary>
        public List<User> Users { get; set; }
    }
}
