﻿using NewAppChatSS.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IMemberRepository
    {
        IEnumerable<Member> GetAll();

        Task AddMemberAsync(string userId, string roomId);

        Task DeleteMemberAsync(string userId, string roomId);

        IEnumerable<string> GetMembersIds(string roomId);

        IEnumerable<Room> GetRooms(string userId);

        IEnumerable<string> GetRoomsIds(string userId);

        Task SaveAsync();
    }
}
