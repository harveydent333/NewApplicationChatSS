using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.DAL.Repositories.Models;

namespace NewAppChatSS.DAL.Repositories
{
    public class MemberRepository : BaseRepository<Member, int, ApplicationDbContext, MemberModel>, IMemberRepository
    {
        private readonly ApplicationDbContext context;

        public MemberRepository(ApplicationDbContext context, IUnitOfWork uow)
            : base(context)
        {
            this.context = context;
            Database = uow;
        }

        public IUnitOfWork Database { get; set; }

        public List<Member> GetAll()
        {
            return context.Members.ToList();
        }

        public async Task AddMemberAsync(string userId, string roomId)
        {
            context.Members.Add(
                new Member
                {
                    UserId = userId,
                    RoomId = roomId,
                });
            await SaveAsync();
        }

        public async Task DeleteMemberAsync(string userId, string roomId)
        {
            Member member = context.Members
                .FirstOrDefault(m => m.UserId == userId && m.RoomId == roomId);

            context.Remove(context.Members
                .FirstOrDefault(m => m.Id == member.Id));

            await SaveAsync();
        }

        public List<string> GetMembersIds(string roomId)
        {
            return context.Members
                .Where(m => m.RoomId == roomId)
                .Select(m => m.UserId)
                .ToList();
        }

        public List<Room> GetRooms(string userId)
        {
            List<string> roomIds = context.Members
                .Where(m => m.UserId == userId)
                .Select(m => m.RoomId)
                .ToList();

            return Database.Rooms.GetAll().Where(r => roomIds.Contains(r.Id)).ToList();
        }

        /// <summary>
        /// Метод возвращает Id всех комнат, в которых состоит пользователь
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        public List<string> GetRoomsIds(string userId)
        {
            return context.Members
                .Where(m => m.UserId == userId)
                .Select(m => m.RoomId)
                .ToList();
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}