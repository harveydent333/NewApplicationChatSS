using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppChatSS.Models.Rooms;

namespace AppChatSS.Models.TypeRooms
{
    /// <summary>
    /// Тип комнаты
    /// </summary>
    public class TypeRoom
    {
        public TypeRoom()
        {
            Rooms = new List<Room>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int32 Id { get; set; }

        /// <summary>
        /// Название типа комнаты
        /// </summary>
        [Required]
        [Display(Name = "Тип комнанты")]
        public String TypeName { get; set; }

        /// <summary>
        /// Список комнат
        /// </summary>
        public List<Room> Rooms { get; set; }
    }
}


