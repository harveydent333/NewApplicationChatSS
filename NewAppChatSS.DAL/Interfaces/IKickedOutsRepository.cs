using NewAppChatSS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IKickedOutsRepository
    {
        IEnumerable<KickedOut> GetAll();

        void AddKickedUser(string userId, string roomId, DateTime dateUnkick);

        void DeleteKickedUser(string userId, string roomId);

        IEnumerable<KickedOut> GetListKickedRoomForUser(string userId);

        void Save();
    }
}