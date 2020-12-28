using System;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.BLL.Models
{
    public class MessageDTO
    {
        public string Id { get; set; }

        /// <summary>
        /// Содержимое сообщения
        /// </summary>
        public string ContentMessage { get; set; }

        /// <summary>
        /// Дата публикации
        /// </summary>
        public DateTime DatePublication { get; set; }

        /// <summary>
        /// Объект пользователя отправляющего сообщение
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// ID пользователя отправляющего сообщение
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Объект комнаты в которой было отправлено сообщение
        /// </summary>
        public Room Room { get; set; }

        /// <summary>
        /// Id комнаты в которой было отправлено сообщение
        /// </summary>
        public string RoomId { get; set; }
    }
}
