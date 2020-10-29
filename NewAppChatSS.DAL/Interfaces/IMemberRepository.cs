using NewAppChatSS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IMemberRepository
    {
        IEnumerable<Member> GetAll();

        void AddMember(string userId, string roomId);

        void DeleteMember(string userId, string roomId);

        IEnumerable<string> GetMembers(string roomId);

        IEnumerable<string> GetRooms(string userId);

        void Save();
    }
}
