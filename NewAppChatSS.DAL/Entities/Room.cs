using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewAppChatSS.DAL.Entities
{
    /// <summary>
    /// Комната чата
    /// </summary>
    public class Room : EntityBase<string>
    {
        /// <summary>
        /// Название комнаты
        /// </summary>
        [Required]
        public string RoomName { get; set; }

        /// <summary>
        /// Идентификатор владельца комнаты
        /// </summary>
        [Required]
        public string OwnerId { get; set; }

        /// <summary>
        /// Владелец комнаты
        /// </summary>
        [ForeignKey("OwnerId")]
        public User Owner { get; set; }

        /// <summary>
        /// Идентификатор типа комнаты
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// Тип комнаты
        /// </summary>
        [ForeignKey("TypeId")]
        public TypeRoom TypeRoom { get; set; }

        /// <summary>
        /// Идентификатор последнего сообщения в комнате
        /// </summary>
        public string LastMessageId { get; set; }

        /// <summary>
        /// Последнее сообщение в комнате
        /// </summary>
        [ForeignKey("LastMessageId")]
        public Message LastMessage { get; set; }
    }
}
