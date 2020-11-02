﻿using NewAppChatSS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IMemberRepository
    {
        IEnumerable<Member> GetAll();

        Task AddMember(string userId, string roomId);

        Task DeleteMember(string userId, string roomId);

        IEnumerable<User> GetMembers(string roomId);

        IEnumerable<string> GetMembersIds(string roomId);

        IEnumerable<Room> GetRooms(string userId);

        IEnumerable<string> GetRoomsIds(string userId);

        Task Save();
    }
}
