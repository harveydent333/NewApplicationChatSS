using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewAppChatSS.DAL.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly ApplicationDbContext _context;

        public MemberRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Member> GetAll()
        {
            return _context.Members.ToList();
        }

        public void AddMember(string userId, string roomId)
        {
            _context.Members.Add(
                new Member
                {
                    UserId = userId,
                    RoomId = roomId,
                });
            Save();
        }

        public void DeleteMember(string userId, string roomId)
        {
            Member member = _context.Members
                .FirstOrDefault(m => m.UserId == userId && m.RoomId == roomId);

            _context.Remove(_context.Members
                .FirstOrDefault(m => m.Id == member.Id));

            Save();
        }

        public IEnumerable<string> GetMembers(string roomId)
        {
            return _context.Members
                .Where(m => m.RoomId == roomId)
                .Select(m => m.UserId.ToString())
                .ToList();
        }

        public IEnumerable<string> GetRooms(string userId)
        {
            return _context.Members
                .Where(m => m.UserId == userId)
                .Select(m => m.RoomId)
                .ToList();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}