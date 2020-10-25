using System;
using System.Collections.Generic;
using System.Linq;

namespace Data.Models.Members
{
    public interface IMemberRepository
    {
        IQueryable<Member> Members { get; }

        void AddMember(Int32? userId, String roomID);

        void DeleteMember(Int32? userId, String roomId);

        List<String> GetRooms(Int32? userId);

        List<String> GetMembers(String roomId);

        void Save();
    }
}