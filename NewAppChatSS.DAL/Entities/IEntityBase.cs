namespace NewAppChatSS.DAL.Entities
{
    public interface IEntityBase<TKey>
    {
        TKey Id { get; set; }
    }
}
