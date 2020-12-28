using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace NewAppChatSS.DAL.Entities
{
    /// <summary>
    /// Пользователи лишенные права отправлять сообщения
    /// </summary>
    public class MutedUser : EntityBase<int>
    {
        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("RoomId")]
        public Room Room { get; set; }

        [Required]
        public string RoomId { get; set; }

        /// <summary>
        /// Дата окончания ограничения писать сообщения в чат
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime DateUnmute { get; set; }
    }
}
