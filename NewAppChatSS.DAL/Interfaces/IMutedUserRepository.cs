using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Repositories.Models;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IMutedUserRepository : IBaseRepository<MutedUser, int, ApplicationDbContext, MutedUserModel>
    {
        IEnumerable<MutedUser> GetAll();

        Task AddMutedUserAsync(string userId, string roomId, DateTime dateUnmute);

        Task DeleteMutedUserAsync(string userId, string roomId);

        DateTime GetDateTimeUnmuteUser(string userId, string roomId);

        IEnumerable<MutedUser> GetListMutedRoomForUser(string userId);

        IEnumerable<string> GetListIdsMutedRoomForUser(string userId);

        Task SaveAsync();
    }
}