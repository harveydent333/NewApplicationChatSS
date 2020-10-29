using NewAppChatSS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IMessageRepository
    {
        IEnumerable<Message> GetAll();

        void AddMessage(Message item);

        void DeleteMessage(string id);

        IEnumerable<Message> FindMessagesByRoomId(string roomId);

        void Save();
    }
}
