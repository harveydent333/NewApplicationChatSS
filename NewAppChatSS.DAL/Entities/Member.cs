using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewAppChatSS.DAL.Entities
{
    /// <summary>
    /// Состав пользователя в комнате
    /// </summary>
    public class Member
    {
        [HiddenInput(DisplayValue = false)]
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Объект пользователя состоящего в комнате
        /// </summary>
        [ForeignKey("UserId")]
        public User User { get; set; }

        /// <summary>
        /// ID пользователя состоящего в комнате
        /// </summary>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// Объект комнаты в которой состоит пользователь
        /// </summary>
        [ForeignKey("RoomId")]
        public Room Room { get; set; }

        /// <summary>
        /// ID комнаты в которой состоит пользователь
        /// </summary>
        [Required]
        public string RoomId { get; set; }
    }
}