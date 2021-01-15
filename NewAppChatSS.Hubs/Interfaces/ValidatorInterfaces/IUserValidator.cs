using System.Collections.Generic;
using System.Threading.Tasks;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.Hubs.Interfaces.ValidatorInterfaces
{
    public interface IUserValidator
    {
        Task<bool> IsUserBlockedAsync(User user);

        Task<bool> IsUserMutedByIdAsync(string userId, string roomId);

        Task<bool> IsUserMutedByNameAsync(string userName, string roomId);

        Task<bool> IsUserKickedByIdAsync(string userId, string roomId);

        Task<bool> IsUserKickedByNameAsync(string userName, string roomId);

        Task<bool> IsUserInGroupByIdAsync(string userId, string roomId);

        Task<bool> IsUserInGroupByNameAsync(string userName, string roomId);

        Task<bool> CommandAccessCheckAsync(User user, List<string> allowedRoles, bool checkOnOwner, string processingUserName);
    }
}
