using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewAppChatSS.DAL.Entities
{
    /// <summary>
    /// Комната чата
    /// </summary>
    public class Room
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        
        /// <summary>
        /// Название комнаты
        /// </summary>
        [Required]
        public string RoomName { get; set; }

        /// <summary>
        /// ID владельца пользователя
        /// </summary>
        [Required]
        public string OwnerId { get; set; }

        /// <summary>
        /// Владелец пользователя
        /// </summary>
        [ForeignKey("OwnerId")]
        public User Owner { get; set; }

        /// <summary>
        /// Id типа комнаты
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// Тип комнаты
        /// </summary>
        [ForeignKey("TypeId")]
        public TypeRoom TypeRoom { get; set; }

        /// <summary>
        /// Id последнего сообщения в комнате
        /// </summary>
        public string LastMessageId { get; set; }

        /// <summary>
        /// Последнее сообщение в комнате
        /// </summary>
        [ForeignKey("LastMessageId")]
        public Message LastMessage { get; set; }
    }
}
