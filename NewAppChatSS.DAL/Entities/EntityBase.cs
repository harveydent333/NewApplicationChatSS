using System.ComponentModel.DataAnnotations;

namespace NewAppChatSS.DAL.Entities
{
    public abstract class EntityBase<TKey> : IEntityBase<TKey>
    {
        [Key]
        public TKey Id { get; set; }
    }
}
