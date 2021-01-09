using System.Collections.Generic;
using System.Threading.Tasks;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.Hubs.Interfaces.ValidatorInterfaces
{
    public interface IUserValidator
    {
        Task<bool> IsUserBlocked(User user);

        Task<bool> IsUserMutedById(string userId, string roomId);

        Task<bool> IsUserMutedByNameAsync(string userName, string roomId);

        Task<bool> IsUserKickedById(string userId, string roomId);

        Task<bool> IsUserKickedByNameAsync(string userName, string roomId);

        Task<bool> IsUserInGroupById(string userId, string roomId);

        Task<bool> IsUserInGroupByNameAsync(string userName, string roomId);

        Task<bool> CommandAccessCheckAsync(User user, List<string> allowedRoles, bool checkOnOwner, string processingUserName);
    }
}
