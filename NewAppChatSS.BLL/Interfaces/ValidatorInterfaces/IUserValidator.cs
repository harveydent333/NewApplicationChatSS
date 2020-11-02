using NewAppChatSS.BLL.DTO;
using NewAppChatSS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewAppChatSS.BLL.Interfaces.ValidatorInterfaces
{
    public interface IUserValidator
    {
        bool IsUserBlocked(User user);

        bool IsUserMutedById(string userId, string roomId);

        Task<bool> IsUserMutedByNameAsync(string userName, string roomId);

        bool IsUserKickedById(string userId, string roomId);

        Task<bool> IsUserKickedByNameAsync(string userName, string roomId);

        bool IsUserInGroupById(string userId, string roomId);

        Task<bool> IsUserInGroupByNameAsync(string userName, string roomId);

        Task<bool> CommandAccessCheckAsync(User user, IEnumerable<string> allowedRoles, bool checkOnOwner = false, string processingUserName = "");
    }
}
