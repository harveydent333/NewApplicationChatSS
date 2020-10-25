using Data.Models.Users;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.SwearingUsers
{
    /// <summary>
    /// Ругающийся пользователь
    /// </summary>
    public class SwearingUser
    {
        [HiddenInput(DisplayValue = false)]
        [Key]
        public Int32 Id { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public Int32? UserId { get; set; }

        /// <summary>
        /// Количество ругающихся слов, написанных пользователем
        /// </summary>
        [Required]
        public Int32 CountSwear { get; set; }
    }
}
