using System.Collections.Generic;
using System.Threading.Tasks;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Repositories.Models;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IMemberRepository : IBaseRepository<Member, int, ApplicationDbContext, MemberModel>
    {
        List<Member> GetAll();

        Task AddMemberAsync(string userId, string roomId);

        Task DeleteMemberAsync(string userId, string roomId);

        List<string> GetMembersIds(string roomId);

        List<Room> GetRooms(string userId);

        /// <summary>
        /// Метод возвращает Id всех комнат, в которых состоит пользователь
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        List<string> GetRoomsIds(string userId);

        Task SaveAsync();
    }
}
