using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewAppChatSS.DAL.Entities
{
    public class Member : EntityBase<int>
    {
        /// <summary>
        /// Пользователь состоящий в комнате
        /// </summary>
        [ForeignKey("UserId")]
        public User User { get; set; }

        /// <summary>
        /// Идентификатор пользователя состоящего в комнате
        /// </summary>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// Комната в которой состоит пользователь
        /// </summary>
        [ForeignKey("RoomId")]
        public Room Room { get; set; }

        /// <summary>
        /// Идентификатор комнаты в которой состоит пользователь
        /// </summary>
        [Required]
        public string RoomId { get; set; }
    }
}