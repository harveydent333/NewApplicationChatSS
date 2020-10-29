using NewAppChatSS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IMutedUserRepository
    {
        IEnumerable<MutedUser> GetAll();

        void AddMutedUser(string userId, string roomId, DateTime dateUnmute);

        void DeleteMutedUser(string userId, string roomId);

        DateTime GetDateTimeUnmuteUser(string userId, string roomId);

        IEnumerable<MutedUser> GetListMutedRoomForUser(string userId);

        void Save();
    }
}