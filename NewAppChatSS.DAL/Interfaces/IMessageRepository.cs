using System.Collections.Generic;
using System.Threading.Tasks;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IMessageRepository
    {
        IEnumerable<Message> GetAll();

        Task AddMessage(Message item);

        Task DeleteMessageAsync(string id);

        IEnumerable<Message> FindMessagesByRoomId(string roomId);

        IEnumerable<Message> GetRoomMessages(string roomId);

        Task SaveAsync();
    }
}
