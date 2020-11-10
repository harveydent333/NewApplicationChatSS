using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.BLL.DTO
{
    public class RoomDTO
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
