using NewAppChatSS.DAL.Entities;

namespace NewApplicationChatSS.ViewModels
{
    public sealed class RoomViewModel
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
        public User Owner { get; set; }

        /// <summary>
        /// Id типа комнаты
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// Тип комнаты
        /// </summary>
        public TypeRoom TypeRoom { get; set; }

        /// <summary>
        /// Id последнего сообщения в комнате
        /// </summary>
        public string LastMessageId { get; set; }

        /// <summary>
        /// Последнее сообщение в комнате
        /// </summary>
        public Message LastMessage { get; set; }
    }
}
