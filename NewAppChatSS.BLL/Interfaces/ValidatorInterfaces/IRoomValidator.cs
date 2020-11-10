using NewAppChatSS.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewAppChatSS.BLL.Interfaces.ValidatorInterfaces
{
    public interface IRoomValidator
    {
        bool UniquenessCheckRoom(string roomName);

        Task<bool> CommandAccessCheckAsync(User user, IEnumerable<string> allowedRoles, string nameProcessedRoom);

        bool IsOwnerRoom(string userId, string nameProcessedRoom);
    }
}
