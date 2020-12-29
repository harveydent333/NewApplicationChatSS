using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewAppChatSS.DAL.Entities
{
    /// <summary>
    /// Модель изгнанных пользователей из комнат
    /// </summary>
    public class KickedOut : EntityBase<int>
    {
        /// <summary>
        /// Объект пользователя исключенного из комнаты
        /// </summary>
        [ForeignKey("UserId")]
        public User User { get; set; }

        /// <summary>
        /// ID пользователя исключенного из комнаты
        /// </summary>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// Объект комнаты из которого исключают пользователя
        /// </summary>
        [ForeignKey("RoomId")]
        public Room Room { get; set; }

        /// <summary>
        /// ID комнаты из которого исключают пользователя
        /// </summary>
        [Required]
        public string RoomId { get; set; }

        /// <summary>
        /// Дата окончания изгнания из комнаты
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime DateUnkick { get; set; }
    }
}
