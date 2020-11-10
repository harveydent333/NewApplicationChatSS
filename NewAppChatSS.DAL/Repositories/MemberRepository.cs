using Microsoft.EntityFrameworkCore;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewAppChatSS.DAL.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly ApplicationDbContext _context;
        public IUnitOfWork Database { get; set; }

        public MemberRepository(ApplicationDbContext context, IUnitOfWork uow)
        {
            _context = context;
            Database = uow;
        }

        public IEnumerable<Member> GetAll()
        {
            return _context.Members.ToList();
        }

        public async Task AddMemberAsync(string userId, string roomId)
        {
            _context.Members.Add(
                new Member
                {
                    UserId = userId,
                    RoomId = roomId,
                });
            await SaveAsync();
        }

        public async Task DeleteMemberAsync(string userId, string roomId)
        {
            Member member = _context.Members
                .FirstOrDefault(m => m.UserId == userId && m.RoomId == roomId);

            _context.Remove(_context.Members
                .FirstOrDefault(m => m.Id == member.Id));

            await SaveAsync();
        }

        public IEnumerable<string> GetMembersIds(string roomId)
        {
            return _context.Members
                .Where(m => m.RoomId == roomId)
                .Select(m => m.UserId)
                .ToList();
        }

        public IEnumerable<Room> GetRooms(string userId)
        {
            List<string> roomIds = _context.Members
                .Where(m => m.UserId == userId)
                .Select(m => m.RoomId)
                .ToList();

            return Database.Rooms.GetAll().Where(r => roomIds.Contains(r.Id));
        }

        public IEnumerable<string> GetRoomsIds(string userId)
        {
            return _context.Members
                .Where(m => m.UserId == userId)
                .Select(m => m.RoomId)
                .ToList();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}