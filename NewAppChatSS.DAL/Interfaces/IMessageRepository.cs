using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Repositories.Models;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IMessageRepository : IBaseRepository<Message, string, ApplicationDbContext, MessageModel>
    {
    }
}
