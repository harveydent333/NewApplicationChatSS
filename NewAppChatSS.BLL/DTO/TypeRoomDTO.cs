using NewAppChatSS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAppChatSS.BLL.DTO
{
    public class TypeRoomDTO
    {
        public int Id { get; set; }

        /// <summary>
        /// Название типа комнаты
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Список комнат
        /// </summary>
        public List<Room> Rooms { get; set; } = new List<Room>();
    }
}
