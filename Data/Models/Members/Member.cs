using AppChatSS.Models.Rooms;
using AppChatSS.Models.Users;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppChatSS.Models.Members
{
    /// <summary>
    /// Состав пользователя в комнате
    /// </summary>
    public class Member
    {
        [HiddenInput(DisplayValue = false)]
        [Key]
        public Int32 Id { get; set; }

        /// <summary>
        /// Объект пользователя состоящего в комнате
        /// </summary>
        [ForeignKey("UserId")]
        public User User { get; set; }

        /// <summary>
        /// ID пользователя состоящего в комнате
        /// </summary>
        [Required]
        public Int32? UserId { get; set; }

        /// <summary>
        /// Объект комнаты в которой состоит пользователь
        /// </summary>
        [ForeignKey("RoomId")]
        public Room Room { get; set; }

        /// <summary>
        /// ID комнаты в которой состоит пользователь
        /// </summary>
        [Required]
        public String RoomId { get; set; }
    }
}