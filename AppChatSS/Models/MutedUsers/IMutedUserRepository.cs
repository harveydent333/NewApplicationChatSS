using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppChatSS.Models.MutedUsers
{
    public interface IMutedUserRepository
    {
        IQueryable<MutedUser> MutedUsers { get; }

        void AddMutedUser(Int32? userId, String roomId, DateTime dateUnblock);

        void DeleteMutedUser(Int32? userId, String roomId);

        List<MutedUser> GetListMutedRoomForUser(Int32? userId);

        DateTime GetDateTimeUnmutedUser(Int32? userId, String roomId);

        void Save();
    }
}
