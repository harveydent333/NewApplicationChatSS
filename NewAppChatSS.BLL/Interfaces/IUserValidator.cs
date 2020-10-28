using NewAppChatSS.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewAppChatSS.BLL.Interfaces
{
    public interface IUserValidator
    {
        bool IsUserBlocked(UserDTO userDto);
        bool IsUserMuted(string userId, string roomId);
        bool IsUserKicked(string userId, string roomId);
        bool IsUserInGroup(string userId, string roomId);
        Task<bool> CommandAccessCheckAsync(UserDTO userDto, IEnumerable<string> allowedRoles, bool checkOnOwner = false, String processingUserName = "");
    }
}
