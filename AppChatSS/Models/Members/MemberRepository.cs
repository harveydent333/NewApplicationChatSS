using System;
using System.Collections.Generic;
using System.Linq;

namespace AppChatSS.Models.Members
{
    public class MemberRepository : IMemberRepository
    {
        private ApplicationDbContext memberContext;

        public MemberRepository(ApplicationDbContext context)
        {
            memberContext = context;
        }

        /// <summary>
        /// Возвращает коллекцию записей состава пользователей и комнат
        /// </summary>
        public IQueryable<Member> Members => memberContext.Members;

        public void AddMember(Int32? userId, String roomId)
        {
            memberContext.Members.Add(new Member
            {
                UserId = userId,
                RoomId = roomId,
            });
            Save();
        }

        /// <summary>
        /// Метод удаляет информацию о членстве пользователя в комнате
        /// </summary>
        public void DeleteMember(Int32? userId, String roomId)
        {
            var x = memberContext.Members.ToList();

            Member member = memberContext.Members
                .Where(m => m.UserId == userId && m.RoomId == roomId)
                .FirstOrDefault();

            memberContext.Remove(memberContext.Members
                .Where(m => m.Id == member.Id)
                .FirstOrDefault());

            Save();
        }

        /// <summary>
        /// Метод получаеть участников комнаты
        /// </summary>
        public List<String> GetMembers(String roomId)
        {
            return memberContext.Members
                .Where(m => m.RoomId == roomId)
                .Select(m => m.UserId.ToString())
                .ToList();
        }

        /// <summary>
        /// Метод получает комнаты пользователя
        /// </summary>
        public List<String> GetRooms(Int32? userId)
        {
            return memberContext.Members
                .Where(m => m.UserId == userId)
                .Select(m => m.RoomId)
                .ToList();
        }

        /// <summary>
        /// Метод сохраняет изменения состояния в базе данных
        /// </summary>
        public void Save()
        {
            memberContext.SaveChanges();
        }
    }
}