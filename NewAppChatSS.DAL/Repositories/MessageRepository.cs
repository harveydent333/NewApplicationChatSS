using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.DAL.Repositories.Models;

namespace NewAppChatSS.DAL.Repositories
{
    public class MessageRepository : BaseRepository<Message, string, NewAppChatSSContext, MessageModel>, IMessageRepository
    {
        public MessageRepository(NewAppChatSSContext context)
            : base(context)
        {
        }
    }
}