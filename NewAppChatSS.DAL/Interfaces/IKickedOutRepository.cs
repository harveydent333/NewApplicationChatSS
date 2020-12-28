using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Repositories.Models;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IKickedOutRepository : IBaseRepository<KickedOut, int, ApplicationDbContext, KickedOutModel>
    {
        Task AddKickedUserAsync(string userId, string roomId, DateTime dateUnkick);

        Task DeleteKickedUserAsync(string userId, string roomId);

        IEnumerable<KickedOut> GetListKickedRoomForUser(string userId);

        IEnumerable<string> GetListIdsKickedRoomForUser(string userId);

        Task SaveAsync();
    }
}