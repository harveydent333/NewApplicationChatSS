using Data.Models.Rooms;
using Data.Models.Users;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.KickedOuts
{
    /// <summary>
    /// Модель изгнанных пользователей из комнат
    /// </summary>
    public class KickedOut
    {
        [HiddenInput(DisplayValue = false)]
        [Key]
        public Int32 Id { get; set; }

        /// <summary>
        /// Объект пользователя исключенного из комнаты
        /// </summary>
        [ForeignKey("UserId")]
        public User User { get; set; }

        /// <summary>
        /// ID пользователя исключенного из комнаты
        /// </summary>
        [Required]
        public Int32? UserId { get; set; }

        /// <summary>
        /// Объект комнаты из которого исключают пользователя
        /// </summary>
        [ForeignKey("RoomId")]
        public Room Room { get; set; }

        /// <summary>
        /// ID комнаты из которого исключают пользователя
        /// </summary>
        [Required]
        public String RoomId { get; set; }

        /// <summary>
        /// Дата окончания изгнания из комнаты
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime DateUnkick { get; set; }
    }
}
