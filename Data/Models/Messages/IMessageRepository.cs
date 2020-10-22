using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppChatSS.Models.Messages
{
    public interface IMessageRepository
    {
        IQueryable<Message> Messages { get; }

        void AddMessage(Message message);

        void DeleteMessage(String Id);

        List<Message> FindMessagesByRoomId(String roomId);

        void Save();
    }
}
