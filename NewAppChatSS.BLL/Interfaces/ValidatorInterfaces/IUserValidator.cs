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

        bool IsUserMuted(string userId, string roomId);

        bool IsUserKicked(string userId, string roomId);

        bool IsUserInGroup(string userId, string roomId);

        Task<bool> CommandAccessCheckAsync(User user, IEnumerable<string> allowedRoles, bool checkOnOwner = false, string processingUserName = "");
    }
}
