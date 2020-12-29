using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.DAL.Repositories.Models;

namespace NewAppChatSS.DAL.Repositories
{
    public class MessageRepository : BaseRepository<Message, string, ApplicationDbContext, MessageModel>, IMessageRepository
    {
        public MessageRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}