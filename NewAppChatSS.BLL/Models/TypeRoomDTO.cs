using System;
using System.Collections.Generic;
using System.Text;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.BLL.Models
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
