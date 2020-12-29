using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewAppChatSS.DAL.Entities
{
    /// <summary>
    /// Ругающийся пользователь
    /// </summary>
    public class SwearingUser : EntityBase<int>
    {
        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// Количество ругательских слов, написанных пользователем
        /// </summary>
        [Required]
        public int CountSwear { get; set; }
    }
}
