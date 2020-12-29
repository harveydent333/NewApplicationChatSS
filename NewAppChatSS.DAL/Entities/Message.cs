using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewAppChatSS.DAL.Entities
{
    /// <summary>
    /// Сообщение чата
    /// </summary>
    public class Message : EntityBase<string>
    {
        /// <summary>
        /// Содержимое сообщения
        /// </summary>
        [Required]
        public string ContentMessage { get; set; }

        /// <summary>
        /// Пользователь отправивший сообщение
        /// </summary>
        [ForeignKey("UserId")]
        public User User { get; set; }

        /// <summary>
        /// Идентификатор пользователя отправившего сообщение
        /// </summary>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// Комната в которой было отправлено сообщение
        /// </summary>
        [ForeignKey("RoomId")]
        public Room Room { get; set; }

        /// <summary>
        /// Идентификатор комнаты в которой было отправлено сообщение
        /// </summary>
        [Required]
        public string RoomId { get; set; }

        /// <summary>
        /// Дата публикации
        /// </summary>
        [Required]
        public DateTime DatePublication { get; set; }
    }
}
