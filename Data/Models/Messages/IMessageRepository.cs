using System;
using System.Collections.Generic;
using System.Linq;

namespace Data.Models.Messages
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
