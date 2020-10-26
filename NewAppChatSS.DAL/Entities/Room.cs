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
        public String Id { get; set; }
        
        /// <summary>
        /// Название комнаты
        /// </summary>
        [Required]
        public String RoomName { get; set; }

        /// <summary>
        /// ID владельца пользователя
        /// </summary>
        [Required]
        public String OwnerId { get; set; }

        /// <summary>
        /// Владелец пользователя
        /// </summary>
        [ForeignKey("OwnerId")]
        public User Owner { get; set; }

        /// <summary>
        /// Id типа комнаты
        /// </summary>
        public Int32 TypeId { get; set; }

        /// <summary>
        /// Тип комнаты
        /// </summary>
        [ForeignKey("TypeId")]
        public TypeRoom TypeRoom { get; set; }

        /// <summary>
        /// Id последнего сообщения в комнате
        /// </summary>
        public String LastMessageId { get; set; }

        /// <summary>
        /// Последнее сообщение в комнате
        /// </summary>
        [ForeignKey("LastMessageId")]
        public Message LastMessage { get; set; }

    }
}
