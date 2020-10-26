using NewAppChatSS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAppChatSS.BLL.DTO
{
    public class TypeRoomDTO
    {
        public TypeRoomDTO()
        {
            Rooms = new List<Room>();
        }

        public Int32 Id { get; set; }

        /// <summary>
        /// Название типа комнаты
        /// </summary>
        public String TypeName { get; set; }

        /// <summary>
        /// Список комнат
        /// </summary>
        public List<Room> Rooms { get; set; }
    }
}
