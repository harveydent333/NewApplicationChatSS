using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewAppChatSS.DAL.Entities
{
    public class KickedOut : EntityBase<int>
    {
        /// <summary>
        /// Пользователь который исключен из комнаты
        /// </summary>
        [ForeignKey("UserId")]
        public User User { get; set; }

        /// <summary>
        /// Идентификатор пользователя исключенного из комнаты
        /// </summary>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// Комната из которой исключен пользователь
        /// </summary>
        [ForeignKey("RoomId")]
        public Room Room { get; set; }

        /// <summary>
        /// Идентификатор комнаты из которой исключен пользователь
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
