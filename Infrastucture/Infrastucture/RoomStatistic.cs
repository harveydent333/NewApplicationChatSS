using System;

namespace AppChatSS.Infrastucture
{
    /// <summary>
    /// Статистика сообщений в комнатах
    /// </summary>
    public class RoomStatistic
    {
        /// <summary>
        /// Название комнаты
        /// </summary>
        public String RoomName { get; set; }

        /// <summary>
        /// Количество сообщений
        /// </summary>
        public Int32 CountMessage { get; set; }
    }
}


