using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewAppChatSS.DAL.Entities
{
    /// <summary>
    /// Сообщение чата
    /// </summary>
    public class Message
    {
        [HiddenInput(DisplayValue = false)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }

        /// <summary>
        /// Содержимое сообщения
        /// </summary>
        [Required]
        public string ContentMessage { get; set; }

        /// <summary>
        /// Дата публикации
        /// </summary>
        [Required]
        public DateTime DatePublication { get; set; }

        /// <summary>
        /// Объект пользователя отправляющего сообщение
        /// </summary>
        [ForeignKey("UserId")]
        public User User { get; set; }

        /// <summary>
        /// ID пользователя отправляющего сообщение
        /// </summary>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// Объект комнаты в которой было отправлено сообщение
        /// </summary>
        [ForeignKey("RoomId")]
        public Room Room { get; set; }

        /// <summary>
        /// Id комнаты в которой было отправлено сообщение
        /// </summary>
        [Required]
        public string RoomId { get; set; }
    }
}
