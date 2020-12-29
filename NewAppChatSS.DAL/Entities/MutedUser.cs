using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewAppChatSS.DAL.Entities
{
    public class MutedUser : EntityBase<int>
    {
        /// <summary>
        /// Замьюченый пользователь
        /// </summary>
        [ForeignKey("UserId")]
        public User User { get; set; }

        /// <summary>
        /// Идентификатор замьюченого пользователя
        /// </summary>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// Комната в которой замьючен пользователь
        /// </summary>
        [ForeignKey("RoomId")]
        public Room Room { get; set; }

        /// <summary>
        /// Идентификатор комнаты в которой замьючен пользователь
        /// </summary>
        [Required]
        public string RoomId { get; set; }

        /// <summary>
        /// Дата окончания ограничения писать сообщения в чат
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime DateUnmute { get; set; }
    }
}
