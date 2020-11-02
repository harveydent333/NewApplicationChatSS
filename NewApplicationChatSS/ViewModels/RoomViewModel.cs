using NewAppChatSS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewApplicationChatSS.ViewModels
{
    public class RoomViewModel
    {
        public string Id { get; set; }

        /// <summary>
        /// Название комнаты
        /// </summary>
        public string RoomName { get; set; }

        /// <summary>
        /// ID владельца пользователя
        /// </summary>
        public string OwnerId { get; set; }

        /// <summary>
        /// Владелец пользователя
        /// </summary>
        public virtual User Owner { get; set; }

        /// <summary>
        /// Id типа комнаты
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// Тип комнаты
        /// </summary>
        public virtual TypeRoom TypeRoom { get; set; }

        /// <summary>
        /// Id последнего сообщения в комнате
        /// </summary>
        public string LastMessageId { get; set; }

        /// <summary>
        /// Последнее сообщение в комнате
        /// </summary>
        public virtual Message LastMessage { get; set; }
    }
}
