using Data.Models.Rooms;
using Data.Models.Users;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.MutedUsers
{
    /// <summary>
    /// Пользователи лишенные права отправлять сообщения
    /// </summary>
    public class MutedUser
    {
        [HiddenInput(DisplayValue = false)]
        [Key]
        public Int32 Id { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public Int32? UserId { get; set; }

        [ForeignKey("RoomId")]
        public Room Room { get; set; }

        [Required]
        public String RoomId { get; set; }

        /// <summary>
        /// Дата окончания ограничения писать сообщения в чат
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime DateUnmute { get; set; }
    }
}
