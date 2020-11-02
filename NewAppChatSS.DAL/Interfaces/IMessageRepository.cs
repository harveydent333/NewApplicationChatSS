using NewAppChatSS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
