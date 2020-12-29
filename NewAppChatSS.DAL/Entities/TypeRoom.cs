using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewAppChatSS.DAL.Entities
{
    /// <summary>
    /// Тип комнаты
    /// </summary>
    public class TypeRoom : EntityBase<int>
    {
        /// <summary>
        /// Название типа комнаты
        /// </summary>
        [Required]
        [Display(Name = "Тип комнанты")]
        public string TypeName { get; set; }

        /// <summary>
        /// Список комнат
        /// </summary>
        public List<Room> Rooms { get; set; } = new List<Room>();
    }
}